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
using System.Globalization;
using System.Net;
using System.Security;
using AdysTech.CredentialManager;

namespace MqttNotifier
{
    internal class CredentialFactory
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private readonly Context _context;

        public CredentialFactory(Context context) => _context = context;

        public NetworkCredential Create()
        {
            var useCredentials = !string.IsNullOrEmpty(_context.CredentialsTarget);
            if (!useCredentials) return null;
            NetworkCredential credential;
            try
            {
                credential = CredentialManager.GetCredentials(_context.CredentialsTarget);
            }
            catch (NullReferenceException)
            {
                throw new SecurityException(
                    string.Format(Culture, "Could not find target '{0}' in Generic section of Credential Manager", _context.CredentialsTarget));
            }
            return credential;
        }
    }
}