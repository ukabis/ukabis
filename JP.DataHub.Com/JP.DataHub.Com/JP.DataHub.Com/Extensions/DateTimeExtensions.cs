using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this string str)
            => DateTime.Parse(str);
    }
}
