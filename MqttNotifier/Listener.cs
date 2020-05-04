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