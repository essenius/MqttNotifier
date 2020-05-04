using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttNotifier
{
    internal class Context
    {
        public int AlertDisplayTime => Setting("AlertDisplayTime", 5000);

        public X509Certificate CaCertificate
        {
            get
            {
                var certFile = Setting<string>("CaCertificateFile", null);
                return string.IsNullOrEmpty(certFile) ? null : new X509Certificate(certFile);
            }
        }

        public string ClientId => Setting("ClientId", Guid.NewGuid().ToString());
        public string CredentialsTarget => Setting<string>("CredentialsTarget", null);
        public string DefaultMessageType => Setting("DefaultMessageType", "warning");
        public string DefaultTitle => Setting("DefaultTitle", "MQTT");
        public string IconFile => Setting("IconFile", "default.ico");
        public string MqttBroker => Setting("MqttBroker", "mqtt");
        public int MqttPort => Setting("Port", UseSsl ? MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT : MqttSettings.MQTT_BROKER_DEFAULT_PORT);
        public byte QoS => Setting("QoS", MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);
        public string Topic => Setting("Topic", "alert/#");
        public bool UseSsl => CaCertificate != null;

        private static T Setting<T>(string setting, T defaultValue)
        {
            var settingValue = ConfigurationManager.AppSettings.Get(setting);
            if (string.IsNullOrEmpty(settingValue))
            {
                return defaultValue;
            }
            return (T) Convert.ChangeType(settingValue, typeof(T));
        }
    }
}