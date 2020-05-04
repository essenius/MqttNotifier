using System.CodeDom;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MqttNotifier;
using uPLibrary.Networking.M2Mqtt;

namespace MqttNotifierTest
{
    [TestClass]
    public class ListenerTest
    {
        [TestMethod, TestCategory("Fast")] 
        public void ListenerNoCredentialsSucceedsTest()
        {
            var context = new MockContext();
            context.Settings.Add("Topic", "MqttNotifier/alert/#");
            var client = new MqttClient("test.mosquitto.org");
            var messageHandler = new MockMessageHandler(context);
            var listener = new Listener(client, null, messageHandler, context);
            Assert.IsTrue(listener.Listen(), "Listen succeeds");
            NotifyIcon icon = null;
            try
            {
                client.Publish("MqttNotifier/alert/info/test", Encoding.UTF8.GetBytes("Test Message"));
                var waitCount = 0;
                while ((icon = messageHandler.Icon) == null)
                {
                    Assert.IsTrue(waitCount++ < 5, "Acceptable wait");
                    Thread.Sleep(100);
                }
            }
            finally
            {
                listener.StopListening();
                Assert.AreEqual("Test Message", icon?.BalloonTipText, "Message OK");
                Assert.AreEqual("test", icon?.BalloonTipTitle, "Title OK");
            }
        }

        [TestMethod, TestCategory("Fast")]
        public void ListenerCredentialsFailsTest()
        {
            var context = new MockContext();
            context.Settings.Add("MqttBroker", "test.mosquitto.org");
            var client = new MqttClientFactory(context).Create();
            var messageHandler = new MockMessageHandler(context);
            var credential = new NetworkCredential("user", "password");
            var listener = new Listener(client, credential, messageHandler, context);
            Assert.IsFalse(listener.Listen(), "Listen fails");
        }

        [TestMethod, TestCategory("Slow")]
        public void ListenerServerFailsTest()
        {
            var context = new MockContext();
            context.Settings.Add("MqttBroker", "bogus");
            // this is the slow step.
            var client = new MqttClientFactory(context).Create();
            var messageHandler = new MockMessageHandler(context);
            var listener = new Listener(client, null, messageHandler, context);
            Assert.IsFalse(listener.Listen(), "Listen fails");
        }

        [TestMethod, TestCategory("Slow")]
        public void ListenerPortFailsTest()
        {
            var context = new MockContext();
            context.Settings.Add("MqttBroker", "test.mosquitto.org");
            context.Settings.Add("MqttPort", "8889");
            var client = new MqttClientFactory(context).Create();
            var messageHandler = new MockMessageHandler(context);
            var listener = new Listener(client, null, messageHandler, context);
            // this is the slow step.
            Assert.IsFalse(listener.Listen(), "Listen fails");
        }
    }
}
