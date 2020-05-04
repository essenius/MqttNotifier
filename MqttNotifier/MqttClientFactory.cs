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

using System.Net.Sockets;
using uPLibrary.Networking.M2Mqtt;

namespace MqttNotifier
{
    internal class MqttClientFactory
    {
        private readonly Context _context;

        public MqttClientFactory(Context context) => _context = context;

        public MqttClient Create()
        {
            try
            {
                return new MqttClient(_context.MqttBroker, _context.MqttPort,
                    _context.UseSsl, _context.CaCertificate, _context.ClientCertificate, _context.SslProtocol);
            }
            catch (SocketException)
            {
                return null;
            }
        }
    }
}