using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ControllerUrl : IValueObject
    {
        public string Value { get; }

        public ControllerUrl(string value)
        {
            Value = value;
        }

        public static bool operator ==(ControllerUrl me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ControllerUrl me, object other) => !me?.Equals(other) == true;
    }
}