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