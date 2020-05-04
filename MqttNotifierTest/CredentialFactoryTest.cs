using System;
using System.Net;
using System.Security;
using AdysTech.CredentialManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MqttNotifier;

namespace MqttNotifierTest
{
    [TestClass]
    public class CredentialFactoryTest
    {
        [TestMethod, TestCategory("Fast")]
        public void CredentialFactoryNoCredentialTest()
        {
            var context = new MockContext();
            var factory = new CredentialFactory(context);
            Assert.IsNull(factory.Create());
        }

        [TestMethod, TestCategory("Fast"), ExpectedException(typeof(SecurityException))]
        public void CredentialFactoryWrongCredentialTest()
        {
            var context = new MockContext();
            context.Settings.Add("CredentialsTarget", "bogus");
            var factory = new CredentialFactory(context);
            Assert.IsNull(factory.Create());
        }

        [TestMethod, TestCategory("Fast")]
        public void CredentialFactoryRightCredentialTest()
        {
            const string target = "MqttNotifierTest@1";
            var credential = new NetworkCredential("user", "password");
            CredentialManager.SaveCredentials(target, credential);
            var context = new MockContext();
            context.Settings.Add("CredentialsTarget", target);
            var factory = new CredentialFactory(context);
            var credentialLoaded = factory.Create();
            Assert.AreEqual("user", credentialLoaded.UserName);
            Assert.AreEqual("password", credentialLoaded.Password);
            CredentialManager.RemoveCredentials(target);
        }
    }
}
