using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RelativeUri : IValueObject
    {
        public string Value { get; }

        public RelativeUri(string value)
        {
            Value = value;
            ValidatorEx.ExceptionValidateObject(this);
        }

        /// <summary>
        /// Valueの最後の文字が「/」なら、その最後の「/」を削除した文字列を返す
        /// </summary>
        /// <returns></returns>
        public string NormalizeUrlRelative()
        {
            string result = Value;
            if (result[result.Length - 1] == '/')
            {
                return result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static bool operator ==(RelativeUri me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RelativeUri me, object other) => !me?.Equals(other) == true;
    }

    internal static class RelativeUriExtension
    {
        public static RelativeUri ToRelativeUri(this string str) => str == null ? null : new RelativeUri(str);
    }
}