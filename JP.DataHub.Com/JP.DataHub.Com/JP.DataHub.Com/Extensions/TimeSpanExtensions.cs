using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan ToTimeSpan(this long time)
            => new TimeSpan(time);
    }
}
