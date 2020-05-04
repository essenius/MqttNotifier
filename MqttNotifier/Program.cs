using System;
namespace MqttNotifier
{
    public class Program
    {
        public static void Main()
        {
            var context = new Context();
            var credential = new CredentialFactory(context).Create();
            var mqttClient = new MqttClientFactory(context).Create();
            var messageHandler = new MessageHandler(context);
            var listener = new Listener(mqttClient, credential, messageHandler, context);
            if (
                listener.Listen())
            {
                Console.WriteLine($"Subscribed to topic '{context.Topic}' at MQTT broker '{context.MqttBroker}:{context.MqttPort}'. Press Enter to quit.");
                Console.ReadLine();
                listener.StopListening();
            }
            else
            {
                Console.WriteLine($"Could not connect to MQTT broker '{context.MqttBroker}:{context.MqttPort}'");
            }
        }
    }
}
