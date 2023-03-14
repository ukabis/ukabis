using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Cryptography;

namespace JP.DataHub.Api.Core.Authentication
{
    public class ClientSecret
    {
        public string Value { get; }

        public bool IsCryptography { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCryptography"></param>
        public ClientSecret(string value, bool isCryptography = true)
        {
            Value = value;
            IsCryptography = isCryptography;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public ClientSecret(string clientId, string clientSecret)
        {
            Value = PasswordHashArgorithm.GetHashedPassword(clientSecret, clientId.ToString().ToLower());
            IsCryptography = true;
        }
    }
}
