using System;
using System.ComponentModel;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
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
                throw new InvalidEnumArgumentException("SslProtocol should be any of [none, SSLv3, TLSv1_0, TLSv1_1, TLSv1_2]");
            }
        }

        public string Topic
        {
            get
            {
                var topic = Setting("Topic", "alert/#");
                if (!topic.EndsWith("/#")) topic += "/#";
                return topic;
            }
        }

        public bool UseSsl => !string.IsNullOrEmpty(CaCertificateFile);

        protected static X509Certificate CertificateFor(string certificateFile) =>
            string.IsNullOrEmpty(certificateFile) ? null : new X509Certificate(certificateFile);

        protected T Setting<T>(string setting, T defaultValue)
        {
            var settingValue = AppSetting(setting);
            if (string.IsNullOrEmpty(settingValue))
            {
                return defaultValue;
            }
            return (T) Convert.ChangeType(settingValue, typeof(T));
        }

        protected virtual string AppSetting(string setting) => ConfigurationManager.AppSettings.Get(setting);
    }
}