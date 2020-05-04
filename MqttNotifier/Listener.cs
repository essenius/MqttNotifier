using System;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Windows.Forms;
using AdysTech.CredentialManager;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttNotifier
{
    internal class Listener
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private readonly Context _context;

        public Listener(Context context) => _context = context;

        private NetworkCredential Credential()
        {
            var useCredentials = !string.IsNullOrEmpty(_context.CredentialsTarget);
            if (!useCredentials) return null;
            NetworkCredential credential;
            try
            {
                credential = CredentialManager.GetCredentials(_context.CredentialsTarget);
            }
            catch (NullReferenceException)
            {
                throw new SecurityException(
                    Message("Could not find target '{0}' in Generic section of Credential Manager", _context.CredentialsTarget));
            }
            return credential;
        }

        public bool Listen()
        {
            MqttClient client;
            try
            {
                client = new MqttClient(_context.MqttBroker, _context.MqttPort, _context.UseSsl, _context.CaCertificate, null,
                    MqttSslProtocols.TLSv1_2);
            }
            catch (SocketException)
            {
                return false;
            }
            client.MqttMsgPublishReceived += MessageReceived;
            var credential = Credential();
            if (credential != null) { 
                if (client.Connect(_context.ClientId, credential.UserName, credential.Password) != MqttMsgConnack.CONN_ACCEPTED) return false;
            }
            else
            {
                try
                {
                    if (client.Connect(_context.ClientId) != MqttMsgConnack.CONN_ACCEPTED) return false;
                }
                catch (MqttCommunicationException)
                {
                    return false;
                }
            }
            client.Subscribe(new[] {_context.Topic}, new[] {_context.QoS});
            Console.WriteLine("Listening...");
            return true;
        }

        public static string Message(string messageTemplate, params object[] args) => string.Format(Culture, messageTemplate, args);

        private void MessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Received message");
            var title = _context.DefaultTitle;
            var messageType = _context.DefaultMessageType;
            var topicSections = e.Topic.Split(new[] {'/'}, 3);
            if (topicSections.Length > 1)
            {
                messageType = topicSections[1];
            }
            if (topicSections.Length > 2)
            {
                title = topicSections[2];
            }
            TrayMessage(Encoding.UTF8.GetString(e.Message), title, messageType);
        }

        public void TrayMessage(string message, string title, string messageType)
        {
            ToolTipIcon icon;
            switch (messageType.ToUpperInvariant())
            {
                case "INFO":
                    icon = ToolTipIcon.Info;
                    break;
                case "ERROR":
                    icon = ToolTipIcon.Error;
                    break;
                case "WARNING":
                    icon = ToolTipIcon.Warning;
                    break;
                default:
                    icon = ToolTipIcon.None;
                    if (!string.IsNullOrWhiteSpace(messageType))
                    {
                        message = $"{messageType}: {message}";
                    }
                    break;
            }
            var tray = new NotifyIcon
            {
                Icon = new Icon(_context.IconFile),
                BalloonTipIcon = icon,
                BalloonTipText = message,
                BalloonTipTitle = title,
                Visible = true
            };
            tray.ShowBalloonTip(_context.AlertDisplayTime);
        }
    }
}