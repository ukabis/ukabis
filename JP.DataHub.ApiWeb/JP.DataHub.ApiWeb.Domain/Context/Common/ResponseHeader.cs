using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ResponseHeader : IValueObject
    {
        public string Headername { get; }

        public JToken Value { get; }

        public ResponseHeader(string headername, JToken value)
        {
            Headername = headername;
            Value = value;
        }

        public static bool operator ==(ResponseHeader me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ResponseHeader me, object other) => !me?.Equals(other) == true;
    }
}
