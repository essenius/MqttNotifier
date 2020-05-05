// Copyright 2020 Rik Essenius
//
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
//   except in compliance with the License. You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License 
//   is distributed on an "AS IS" BASIS WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and limitations under the License.

using System.Net;
using System.Text;
using System.Threading;
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
            try
            {
                client.Publish("MqttNotifier/alert/info/test", Encoding.UTF8.GetBytes("Test Message"));
                var waitCount = 0;
                while (string.IsNullOrEmpty(messageHandler.Title))
                {
                    Assert.IsTrue(waitCount++ < 5, "Acceptable wait");
                    Thread.Sleep(100);
                }
            }
            finally
            {
                listener.StopListening();
                Assert.AreEqual("Test Message", messageHandler.Message, "Message OK");
                Assert.AreEqual("test", messageHandler.Title, "Title OK");
            }
            messageHandler.Dispose();
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
            messageHandler.Dispose();
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
            messageHandler.Dispose();
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
            messageHandler.Dispose();
        }
    }
}
