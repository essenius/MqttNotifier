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