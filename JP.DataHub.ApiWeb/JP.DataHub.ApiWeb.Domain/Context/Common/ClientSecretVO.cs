using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Cryptography;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record ClientSecretVO : IValueObject
    {
        public string Value { get; }

        public bool IsCryptography { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCryptography"></param>
        public ClientSecretVO(string value, bool isCryptography = true)
        {
            Value = value;
            IsCryptography = isCryptography;
            ValidatorEx.ExceptionValidateObject(this);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public ClientSecretVO(string clientId, string clientSecret)
        {
            Value = PasswordHashArgorithm.GetHashedPassword(clientSecret, clientId.ToString().ToLower());
            IsCryptography = true;
        }

        public static bool operator ==(ClientSecretVO me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ClientSecretVO me, object other) => !me?.Equals(other) == true;
    }
}
