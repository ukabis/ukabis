using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public static class QueryStringExtension
    {
        public static QueryString ToQueryString(this string str, IDictionary<string, string> addtional = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new QueryString(new Dictionary<string, string>());
            }

            var parsedStr = HttpUtility.ParseQueryString(str);
            if (parsedStr.Count < 1)
            {
                return new QueryString(new Dictionary<string, string>());
            }

            var dic = parsedStr.ToDictionary();
            addtional?.Keys.ToList().ForEach(x => dic.Add(x, addtional[x]));
            return new QueryString(dic, str);
        }
    }
}
