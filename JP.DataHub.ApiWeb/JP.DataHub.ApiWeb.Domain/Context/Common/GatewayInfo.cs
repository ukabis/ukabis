using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record GatewayInfo : IValueObject
    {
        public string Url { get; }

        public string CredentialUsername { get; }

        public string CredentialPassword { get; }

        public bool IsCredential { get { return !string.IsNullOrEmpty(CredentialUsername); } }

        public string GatewayRelayHeader { get; }

        public GatewayInfo(string url, string credential_username, string credential_password, string gateway_relay_header)
        {
            Url = url;
            CredentialUsername = credential_username;
            CredentialPassword = credential_password;
            GatewayRelayHeader = gateway_relay_header;
        }

        public static bool operator ==(GatewayInfo me, object other) => me?.Equals(other) == true;

        public static bool operator !=(GatewayInfo me, object other) => !me?.Equals(other) == true;
    }
}
