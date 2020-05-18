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

        [TestMethod, TestCategory("Fast"), ExpectedException(typeof(SecurityException))]
        public void CredentialFactoryWrongCredentialTest()
        {
            var context = new MockContext();
            context.Settings.Add("CredentialsTarget", "bogus");
            var factory = new CredentialFactory(context);
            Assert.IsNull(factory.Create());
        }
    }
}