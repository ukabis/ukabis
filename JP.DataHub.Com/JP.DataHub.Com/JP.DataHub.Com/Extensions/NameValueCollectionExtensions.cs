using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class NameValueCollectionExtension
    {
        public static IDictionary<string, string> ToDictionary(this NameValueCollection name)
        {
            var result = new Dictionary<string, string>();
            foreach (string key in name.Keys)
            {
                if (key != null)
                {
                    result.Add(key, name[key]);
                }
            }
            return result;
        }
    }
}
