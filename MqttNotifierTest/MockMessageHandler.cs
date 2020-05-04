using MqttNotifier;
using System.Windows.Forms;

namespace MqttNotifierTest
{
    internal class MockMessageHandler: MessageHandler
    {
        public NotifyIcon Icon { get; private set; }
        public int Timeout { get; private set; }

        protected override void TrayMessage(NotifyIcon notifyIcon, int timeout)
        {
            Icon = notifyIcon;
            Timeout = timeout;
        }

        public MockMessageHandler(Context context) : base(context)
        {
        }
    }
}
