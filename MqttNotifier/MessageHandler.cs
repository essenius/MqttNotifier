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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MqttNotifier
{
    internal class MessageHandler : IMessageHandler
    {
        private readonly Context _context;

        public MessageHandler(Context context) => _context = context;

        private int TopicDepth(string topic)
        {
            return topic.Split('/').Length - 1;
        }

        public void HandleMessage(string message, string topic)
        {
            var title = _context.DefaultTitle;
            var messageType = _context.DefaultMessageType;
            var prefixDepth = TopicDepth(_context.Topic);
            var topicSections = topic.Split(new[] {'/'}, prefixDepth + 2).Skip(prefixDepth).ToArray();
            if (topicSections.Length > 0 && !string.IsNullOrEmpty(topicSections[0]))
            {
                messageType = topicSections[0];
            }
            if (topicSections.Length > 1 && !string.IsNullOrEmpty(topicSections[1]))
            {
                title = topicSections[1];
            }
            ShowMessage(message, title, messageType);
        }

        protected static ToolTipIcon IconFor(string messageType)
        {
            ToolTipIcon icon;
            switch (messageType.ToUpperInvariant())
            {
                case "INFO":
                    icon = ToolTipIcon.Info;
                    break;
                case "ERROR":
                    icon = ToolTipIcon.Error;
                    break;
                case "WARNING":
                    icon = ToolTipIcon.Warning;
                    break;
                default:
                    icon = ToolTipIcon.None;
                    break;
            }
            return icon;
        }

        protected void ShowMessage(string message, string title, string messageType)
        {
            var messageTypeIcon = IconFor(messageType);
            if (messageTypeIcon == ToolTipIcon.None && !string.IsNullOrWhiteSpace(messageType))
            {
                message = $"{messageType}: {message}";
            }

            var notifyIcon = new NotifyIcon
            {
                Icon = new Icon(_context.IconFile),
                BalloonTipIcon = messageTypeIcon,
                BalloonTipText = message,
                BalloonTipTitle = title,
            };

            TrayMessage(notifyIcon, _context.AlertDisplayTime);
        }

        protected virtual void TrayMessage(NotifyIcon notifyIcon, int timeout)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(timeout);
        }

    }
}
