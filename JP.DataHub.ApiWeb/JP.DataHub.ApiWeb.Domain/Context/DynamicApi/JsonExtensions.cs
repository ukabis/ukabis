using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    internal static class JsonExtensions
    {
        private static List<string> s_removeField = new List<string>(new string[] { "_Type", "_Vendor_Id", "_System_Id", "_Reguser_Id", "_Reg_date", "_Upduser_Id", "_Upd_date", "_Version", "_partitionkey", "_Owner_Id", "id" });

        private static Dictionary<string, JToken> PickupAdminFields(JToken json)
        {
            var result = new Dictionary<string, JToken>();
            if (json != null && json.Count() > 0)
            {
                for (JToken token = json.First(); token != null; token = token.Next)
                {
                    if (token != null && token.Path != null && s_removeField.Contains(token.Path) == true)
                    {
                        result.Add(token.Path, json[token.Path]);
                    }
                }
                if (!result.Keys.Contains("id"))
                {
                    result.Add("id", null);
                }
            }
            return result;
        }

        public static JToken RemoveAdminFields(this JToken token, out Dictionary<string, JToken> removed)
        {
            removed = PickupAdminFields(token);
            return token.RemoveFields(removed.Keys.ToArray());
        }

        public static JToken RemoveFields(this JToken token, params string[] fields)
        {
            if (fields != null && fields.Count() > 0)
            {
                foreach (var field in fields)
                {
                    var result = token.SelectToken(field);
                    if (result != null)
                    {
                        result.Parent.Remove();
                    }
                }
            }
            return token;
        }
    }
}
