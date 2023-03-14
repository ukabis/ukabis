using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JP.DataHub.Com.Extensions
{
    public static class HttpHeaderExtensions
    {
        public static void Add(this Dictionary<string, string[]> dic, string key, string value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = new string[] { value };
            }
            else
            {
                dic.Add(key, new string[] { value });
            }
        }

        public static void Add(this Dictionary<string, List<string>> header, string key, string value)
        {
            var list = new List<string>();
            list.Add(value);
            header.Add(key, list);
        }
    }
}
