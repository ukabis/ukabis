using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class PublicDate : IValueObject
    {
        public DateTime? Start { get; }
        public DateTime? End { get; }

        public PublicDate(DateTime? start, DateTime? end)
        {
            Start = start;
            End = end;
        }

        public bool IsPublicNow(DateTime now)
        {
            if (Start == null || End == null)
            {
                return true;
            }
            if (now < Start.Value)
            {
                return false;
            }
            if (now > End.Value)
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(PublicDate me, object other) => me?.Equals(other) == true;

        public static bool operator !=(PublicDate me, object other) => !me?.Equals(other) == true;
    }
}