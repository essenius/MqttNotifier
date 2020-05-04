namespace MqttNotifier
{
    internal interface IMessageHandler
    {
        void HandleMessage(string message, string topic);
    }
}
