using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http
{
    public static class HeaderExtensions
    {
        public static void Add(this Dictionary<string, string[]> header, string key, string value)
        {
            if (header.ContainsKey(key) == false)
            {
                header.Add(key, new string[] { value });
            }
        }
        public static void Add(this IDictionary<string, string[]> header, string key, string value)
        {
            if (header.ContainsKey(key) == false)
            {
                header.Add(key, new string[] { value });
            }
        }
    }
}
