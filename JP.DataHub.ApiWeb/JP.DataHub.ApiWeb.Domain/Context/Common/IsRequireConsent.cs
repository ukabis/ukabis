using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsRequireConsent : IValueObject
    {
        public bool Value { get; }

        public IsRequireConsent(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsRequireConsent me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsRequireConsent me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsRequireRonsentExtension
    {
        public static IsRequireConsent ToIsRequireRonsent(this bool val) => new IsRequireConsent(val);
    }
}