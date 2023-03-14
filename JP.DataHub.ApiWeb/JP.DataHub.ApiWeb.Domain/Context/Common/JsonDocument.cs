using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class JsonDocument : IValueObject
    {
        public JToken Value { get; }

        public JsonDocument(JToken value)
        {
            Value = value;
        }
    }

    internal static class JsonDocumentEx
    {
        private static string TYPE = "_Type";
        private static string VENDORID = "_Vendor_Id";
        private static string SYSTEMID = "_System_Id";
        private static string REGUSERID = "_Reguser_Id";
        private static string REGDATE = "_Regdate";
        private static string UPDUSERID = "_Upduser_Id";
        private static string UPDDATE = "_Upddate";
        private static string VERSION_COLNAME = "_Version";
        private static string PARTITIONKEY = "_partitionkey";
        private static string ETAG = "_etag";
        private static string OWNERID = "_Owner_Id";

        private static readonly string[] RemoveXGetInnerAllField = new string[] { "_rid", "_self", "_attachments", "_ts", ETAG };
        private static readonly string[] RemoveAll = new string[] { TYPE, "_rid", "_self", "_attachments", "_ts", REGUSERID, REGDATE, UPDUSERID, UPDDATE, VENDORID, SYSTEMID, PARTITIONKEY, VERSION_COLNAME, ETAG };

        public static JToken RemoveTokenToJson(this JToken json, bool XGetInnerAllField, string[] removeIgnoreFields = null)
        {
            if (json == null)
            {
                return null;
            }
            else if (json is JValue jvalue)
            {
                return jvalue.ConvertToString();
            }
            else
            {
                return CloneToken(json, XGetInnerAllField, removeIgnoreFields).ToString(Formatting.None);
            }
        }

        public static string RemoveTokenToJson(this JsonDocument json, bool XGetInnerAllField, string[] removeIgnoreFields = null) => json == null ? null : json.Value.RemoveTokenToJson(XGetInnerAllField, removeIgnoreFields)?.ToString();

        public static string GetEtag(this JsonDocument json)
        {
            return json.Value.Value<string>(ETAG) ?? " ";
        }

        public static IEnumerable<string> RemoveTokenToJson(this IEnumerable<JsonDocument> jsons, bool XGetInnerAllField, string[] removeIgnoreFields = null)
        {
            foreach (var json in jsons)
            {
                yield return json.RemoveTokenToJson(XGetInnerAllField, removeIgnoreFields);
            }
        }

        private static JToken CloneToken(JToken token, bool XGetInnerAllField, string[] removeIgnoreFields = null)
        {
            if (XGetInnerAllField)
            {
                return RemoveFields(token, RemoveXGetInnerAllField, removeIgnoreFields);
            }
            else
            {
                return RemoveFields(token, RemoveAll, removeIgnoreFields);
            }
        }

        private static JToken RemoveFields(JToken token, string[] fields, string[] removeIgnoreFields = null)
        {
            foreach (var field in fields)
            {
                if (removeIgnoreFields != null && removeIgnoreFields.Contains(field))
                {
                    continue;
                }
                var result = token.SelectToken(field);
                if (result != null)
                {
                    result.Parent.Remove();
                }
            }

            return token;
        }
    }
}

