using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record ExecDurationMsec : IValueObject
    {
        public int Value { get; }

        public ExecDurationMsec(int value)
        {
            this.Value = value;
        }

        public static bool operator ==(ExecDurationMsec me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ExecDurationMsec me, object other) => !me?.Equals(other) == true;
    }
}