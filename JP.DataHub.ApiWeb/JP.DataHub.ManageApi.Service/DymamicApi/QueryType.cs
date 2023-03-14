using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    internal class QueryType
    {
        public QueryTypes Value { get; }

        public QueryType(QueryTypes value)
        {
            this.Value = value;
        }

        public static QueryType Parse(string queryTypeCode)
        {

            foreach (QueryTypes value in Enum.GetValues(typeof(QueryTypes)))
            {
                if (value.GetCode() == queryTypeCode)
                {
                    return new QueryType(value);
                }
            }
            return null;
        }
    }

    internal enum QueryTypes
    {

        [QueryTypeCodeConvert(Code = "cdb")]
        NativeDbQuery,
        [QueryTypeCodeConvert(Code = "odt")]
        ODataQuery,
    }

    public class QueryTypeCodeConvertAttribute : Attribute
    {
        public string Code { get; set; }
    }

    internal static class QueryTypesEx
    {
        private static Dictionary<QueryTypes, string> CodeList;


        internal static string GetCode(this QueryTypes queryTypes)
        {
            return CodeList[queryTypes];
        }

        static QueryTypesEx()
        {
            CodeList = new Dictionary<QueryTypes, string>();

            foreach (QueryTypes value in Enum.GetValues(typeof(QueryTypes)))
            {
                CodeList.Add(
                    value,
                    ((QueryTypeCodeConvertAttribute)value.GetType()?.GetMember(value.ToString())?.FirstOrDefault()?.GetCustomAttributes(typeof(QueryTypeCodeConvertAttribute), false).FirstOrDefault())?.Code ?? ""
                    );
            }
        }
    }
}
