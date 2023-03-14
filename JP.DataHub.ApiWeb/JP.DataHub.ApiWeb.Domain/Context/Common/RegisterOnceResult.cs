using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RegisterOnceResult : IValueObject
    {
        public string Value { get; }

        public Dictionary<string, object> Additional { get; }

        public RegisterOnceResult(string val)
        {
            Value = val;
        }

        public RegisterOnceResult(string val, Dictionary<string, object> dic)
        {
            Value = val;
            Additional = dic;
        }

        public static bool operator ==(RegisterOnceResult me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RegisterOnceResult me, object other) => !me?.Equals(other) == true;
    }
}
