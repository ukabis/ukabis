using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    /// <summary>
    /// ODataPatchのリクエストBodyの追加条件
    /// </summary>
    internal class ODataPatchAdditionalCondition
    {
        [JsonProperty(Required = Required.Always)]
        public string ColumnName { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Operator { get; set; }
        [JsonProperty(Required = Required.Always)]
        public List<string> Object { get; set; }
    }
}
