using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    // .NET6
    internal interface IRdbmsSqlParameterList : IDictionary<string, RdbmsSqlParameter>
    {
        object AsParameterObject { get; }

        void Add(string key, object value, object type = null, bool autoParameterName = true);
        void AddRange(IDictionary<string, object> parameters, bool autoParameterName = true);
    }

    internal class RdbmsSqlParameter
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public string SqlParameter { get; set; }
        public object ParameterMappingType { get; set; }

        public RdbmsSqlParameter(string parameterName, object parameterValue, string sqlParameter, object parameterMappingType)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
            SqlParameter = sqlParameter;
            ParameterMappingType = parameterMappingType;
        }
    }
}