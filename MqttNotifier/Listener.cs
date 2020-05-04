using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttNotifier
{
    internal class Listener
    {
        private readonly MqttClient _client;
        private readonly Context _context;
        private readonly NetworkCredential _credential;
        private readonly IMessageHandler _messageHandler;

        public Listener(MqttClient client, NetworkCredential credential, IMessageHandler messageHandler, Context context)
        {
            _client = client;
            _credential = credential;
            _messageHandler = messageHandler;
            _context = context;
        }

        public bool Listen()
        {
            if (_client == null) return false;
            _client.MqttMsgPublishReceived += MessageReceived;

            try
            {
                var returnValue = _credential == null
                    ? _client.Connect(_context.ClientId)
                    : _client.Connect(_context.ClientId, _credential.UserName, _credential.Password);
                if (returnValue != MqttMsgConnack.CONN_ACCEPTED) return false;
            }
            catch (MqttConnectionException)
            {
                return false;
            }

            _client.Subscribe(new[] {_context.Topic}, new[] {_context.QoS});
            return true;
        }

        public void StopListening()
        {
            _client.Unsubscribe(new[] { _context.Topic });
            _client.Disconnect();
        }

        private void MessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            _messageHandler.HandleMessage(Encoding.UTF8.GetString(e.Message), e.Topic);
        }
    }
}