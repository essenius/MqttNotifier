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

using System.Windows.Forms;
using MqttNotifier;

namespace MqttNotifierTest
{
    internal class MockMessageHandler : MessageHandler
    {
        public MockMessageHandler(Context context) : base(context)
        {
        }

        public ToolTipIcon Icon { get; private set; }
        public string Message { get; private set; }
        public int Timeout { get; private set; }
        public string Title { get; private set; }

        protected override void TrayMessage(int timeout, string title, string message, ToolTipIcon icon)
        {
            Timeout = timeout;
            Title = title;
            Message = message;
            Icon = icon;
        }
    }
}