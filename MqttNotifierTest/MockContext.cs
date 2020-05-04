using System.Collections.Generic;
using MqttNotifier;

namespace MqttNotifierTest
{
    class MockContext : Context
    {
        public Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        protected override string AppSetting(string setting) => Settings.ContainsKey(setting) ? Settings[setting] : null;
    }
}
