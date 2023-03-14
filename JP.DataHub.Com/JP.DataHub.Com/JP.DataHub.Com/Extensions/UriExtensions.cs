using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class UriExtensions
    {
        public static Uri UriCombine(this string str1, string str2)
             => new Uri(str1.UriCombineToString(str2));

        public static string UriCombineToString(this string str1, string str2)
        {
            if ((str1 != null && str2 == null) || (str1 == null && str2 != null))
            {
                return str1 ?? str2;
            }
            string separator = str1.LastOrDefault() != '/' && str2.FirstOrDefault() != '/' ? "/" : null;
            return $"{str1}{separator}{str2}";
        }
        public static string QueryStringCombine(this string str1, string str2)
        {
            if ((str1 != null && str2 == null) || (str1 == null && str2 != null))
            {
                return str1 ?? str2;
            }
            string separator = str1.LastOrDefault() != '?' && str2.FirstOrDefault() != '?' ? "?" : null;
            if (str1?.IndexOf("?") != -1)
            {
                separator = "&";
            }
            return $"{str1}{separator}{str2}";
        }
    }
}
