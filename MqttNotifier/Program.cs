using System;
using MqttNotifier.Properties;

namespace MqttNotifier
{
    public static class Program
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
                Console.WriteLine(Resources.Subscribed, context.Topic, context.MqttBroker, context.MqttPort);
                Console.ReadLine();
                listener.StopListening();
            }
            else
            {
                Console.WriteLine(Resources.CouldNotConnect, context.MqttBroker, context.MqttPort);
            }
            messageHandler.Dispose();
        }
    }
}