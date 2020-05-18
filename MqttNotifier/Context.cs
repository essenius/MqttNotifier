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

using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using MqttNotifier.Properties;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttNotifier
{
    internal class Context
    {
        public int AlertDisplayTime => Setting("AlertDisplayTime", 5000);
        public X509Certificate CaCertificate => CertificateFor(CaCertificateFile);
        protected string CaCertificateFile => Setting<string>("CaCertificateFile", null);
        public X509Certificate ClientCertificate => CertificateFor(Setting<string>("ClientCertificateFile", null));
        public string ClientId => Setting("ClientId", Guid.NewGuid().ToString());
        public string CredentialsTarget => Setting<string>("CredentialsTarget", null);
        public string DefaultMessageType => Setting("DefaultMessageType", "warning");
        public string DefaultTitle => Setting("DefaultTitle", "MQTT");
        public string IconFile => Setting("IconFile", "default.ico");
        public string MqttBroker => Setting("MqttBroker", "mqtt");
        public int MqttPort => Setting("MqttPort", UseSsl ? MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT : MqttSettings.MQTT_BROKER_DEFAULT_PORT);
        public byte QoS => Setting("QoS", MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);

        public MqttSslProtocols SslProtocol
        {
            get
            {
                var protocol = Setting("SslProtocol", UseSsl ? "TLSV1_2" : "None");
                if (Enum.TryParse(protocol, true, out MqttSslProtocols result))
                {
                    return result;
                }
                throw new InvalidEnumArgumentException(Resources.SslProtocolOptions);
            }
        }

        public string Topic
        {
            get
            {
                var topic = Setting("Topic", "alert/#");
                if (!topic.EndsWith("/#", StringComparison.Ordinal)) topic += "/#";
                return topic;
            }
        }

        public bool UseSsl => !string.IsNullOrEmpty(CaCertificateFile);

        protected virtual string AppSetting(string setting) => ConfigurationManager.AppSettings.Get(setting);

        protected static X509Certificate CertificateFor(string certificateFile) =>
            string.IsNullOrEmpty(certificateFile) ? null : new X509Certificate(certificateFile);

        protected T Setting<T>(string setting, T defaultValue)
        {
            var settingValue = AppSetting(setting);
            if (string.IsNullOrEmpty(settingValue))
            {
                return defaultValue;
            }
            return (T) Convert.ChangeType(settingValue, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}