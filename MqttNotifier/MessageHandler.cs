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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MqttNotifier
{
    internal class MessageHandler : IMessageHandler, IDisposable
    {
        private readonly Context _context;
        private readonly NotifyIcon _notifyIcon;
        private bool _disposed;

        public MessageHandler(Context context)
        {
            _context = context;
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon(_context.IconFile),
                Visible = false
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _notifyIcon.Icon = null;
                _notifyIcon.Dispose();
            }
            _disposed = true;
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
            TrayMessage(_context.AlertDisplayTime, title, message, messageTypeIcon);
        }

        protected static int TopicDepth(string topic) => topic.Split('/').Length - 1;

        // Separated out to be able to override in subclass for testing.
        protected virtual void TrayMessage(int timeout, string title, string message, ToolTipIcon icon)
        {
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(timeout, title, message, icon);
        }
    }
}