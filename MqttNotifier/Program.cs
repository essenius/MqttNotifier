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
            messageHandler.Dispose();
        }
    }
}
