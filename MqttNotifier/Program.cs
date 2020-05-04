using System;
using System.Diagnostics;
namespace MqttNotifier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var context = new Context();
            if (!new Listener(context).Listen())
            {
                Console.WriteLine($"Could not connect to MQTT broker '{context.MqttBroker}:{context.MqttPort}'");
            }
        }
    }
}
