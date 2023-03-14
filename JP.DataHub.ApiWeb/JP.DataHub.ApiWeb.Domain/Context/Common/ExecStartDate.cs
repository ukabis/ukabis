using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record ExecStartDate : IValueObject
    {
        public DateTime Value { get; }

        public ExecStartDate(DateTime value)
        {
            this.Value = value;
        }

        public static bool operator ==(ExecStartDate me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ExecStartDate me, object other) => !me?.Equals(other) == true;
    }
}