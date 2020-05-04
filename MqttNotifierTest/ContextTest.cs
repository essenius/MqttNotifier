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

using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uPLibrary.Networking.M2Mqtt;

namespace MqttNotifierTest
{
    [TestClass]
    public class ContextTest
    {
        [TestMethod, TestCategory("Fast")]
        public void ContextUseSslTest()
        {
            var context = new MockContext();
            Assert.AreEqual(false, context.UseSsl, "Without any configuration, UseSsl is false");
            Assert.AreEqual(1883, context.MqttPort, "without ssl, port is default MQTT http port");
            Assert.AreEqual(MqttSslProtocols.None, context.SslProtocol, "without ssl, SslProtocol is none");
            Assert.IsNull(context.CaCertificate, "By default, CaCertificate is null");
            context.Settings.Add("CaCertificateFile", "ca.crt");
            Assert.AreEqual(true, context.UseSsl, "Use of CaCertificateFile makes UseSsl true");
            Assert.AreEqual(8883, context.MqttPort, "with SSL, default port = 8883");
            Assert.AreEqual(MqttSslProtocols.TLSv1_2, context.SslProtocol, "with ssl, SslProtocol is TLSv1_2");
        }

        [TestMethod, TestCategory("Fast"), ExpectedException(typeof(InvalidEnumArgumentException))]
        public void ContextWrongSslOptionTest()
        {
            var context = new MockContext();
            context.Settings.Add("SslProtocol", "bogus");
            var _ = context.SslProtocol;
        }
    }
}
