using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MqttNotifier;

namespace MqttNotifierTest
{
    [TestClass]
    public class MessageHandlerTest
    {
        [TestMethod, TestCategory("Fast")]
        public void MessageHandlerDefaultContextTest()
        {
            var context = new MockContext();
            var handler = new MockMessageHandler(context);
            handler.HandleMessage("message1", "alert/error/title");
            Assert.AreEqual("message1", handler.Icon.BalloonTipText, "Message OK");
            Assert.AreEqual("title", handler.Icon.BalloonTipTitle, "Title OK");
            Assert.AreEqual(ToolTipIcon.Error, handler.Icon.BalloonTipIcon, "Message Type Icon Error");
            Assert.AreEqual(5000, handler.Timeout, "Default timeout");

            handler.HandleMessage("message2", "alert/info");
            Assert.AreEqual("message2", handler.Icon.BalloonTipText, "Message 2 OK");
            Assert.AreEqual("MQTT", handler.Icon.BalloonTipTitle, "default title used if not specified in topic (2)");
            Assert.AreEqual(ToolTipIcon.Info, handler.Icon.BalloonTipIcon, "Message Type Icon Info");

            handler.HandleMessage("message3", "alert/");
            Assert.AreEqual("message3", handler.Icon.BalloonTipText, "Message 3 OK");
            Assert.AreEqual("MQTT", handler.Icon.BalloonTipTitle, "default title used if not specified in topic (3)");
            Assert.AreEqual(ToolTipIcon.Warning, handler.Icon.BalloonTipIcon, "Message Type Icon Warning if not specified");

            handler.HandleMessage("message4", "alert");
            Assert.AreEqual("message4", handler.Icon.BalloonTipText, "Message 4 OK");
            Assert.AreEqual("MQTT", handler.Icon.BalloonTipTitle, "default title used if not specified in topic (4)");
            Assert.AreEqual(ToolTipIcon.Warning, handler.Icon.BalloonTipIcon, "Message Type Icon Warning if not specified (4)");

            handler.HandleMessage("message5", "alert/bogus/title5");
            Assert.AreEqual("bogus: message5", handler.Icon.BalloonTipText, "Message contains message type if not indentified as icon");
            Assert.AreEqual("title5", handler.Icon.BalloonTipTitle, "Title OK (5)");
            Assert.AreEqual(ToolTipIcon.None, handler.Icon.BalloonTipIcon, "Message Type Icon None if not specified");
        }

        [TestMethod, TestCategory("Fast")]
        public void MessageHandlerMultiLevelTopicTest()
        {
            var context = new MockContext();
            context.Settings.Add("Topic", "this/is/a/long/topic/for/an/alert");
            var handler = new MockMessageHandler(context);
            handler.HandleMessage("message1", "this/is/a/long/topic/for/an/alert/error/title");
            Assert.AreEqual("message1", handler.Icon.BalloonTipText, "Message OK");
            Assert.AreEqual("title", handler.Icon.BalloonTipTitle, "Title OK");
            Assert.AreEqual(ToolTipIcon.Error, handler.Icon.BalloonTipIcon, "Message Type Icon Error");

            handler.HandleMessage("message2", "this/is/a/long/topic/for/an/alert");
            Assert.AreEqual("message2", handler.Icon.BalloonTipText, "Message OK");
            Assert.AreEqual("MQTT", handler.Icon.BalloonTipTitle, "Title OK");
            Assert.AreEqual(ToolTipIcon.Warning, handler.Icon.BalloonTipIcon, "Message Type Icon Warning");

        }
    }
}
