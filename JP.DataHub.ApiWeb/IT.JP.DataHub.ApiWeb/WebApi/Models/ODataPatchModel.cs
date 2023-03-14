using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ODataPatchModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string STR_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string STR_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? INT_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? DBL_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? NUM_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public ODataPatchObject OBJ_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public ODataPatchObject OBJ_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<string> ARY_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<string> ARY_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public bool? BOL_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public bool? BOL_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string DAT_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string DAT_NULL { get; set; }
    }

    /// <summary>
    /// 全てnullプロパティなし(更新用)
    /// </summary>
    public class ODataPatchModelForUpd : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string STR_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string STR_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? INT_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? DBL_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? NUM_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ODataPatchObject OBJ_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ODataPatchObject OBJ_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ARY_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ARY_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? BOL_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? BOL_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DAT_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DAT_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HOGE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public new ODataPatchWhereEx _Where { get; set; }
    }

    /// <summary>
    /// null更新する項目のみnullプロパティあり(更新用)
    /// </summary>
    public class ODataPatchModelForUpdEx : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string STR_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string STR_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? INT_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? DBL_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? NUM_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ODataPatchObject OBJ_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public ODataPatchObject OBJ_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ARY_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<string> ARY_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? BOL_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public bool? BOL_NULL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DAT_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string DAT_NULL { get; set; }
    }

    /// <summary>
    /// 型違い(更新用)
    /// </summary>
    public class ODataPatchModelForUpdEx2 : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string INT_VALUE { get; set; }
    }


    public class ODataPatchObject
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key4 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key5_1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key5_2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key6_1 { get; set; }
    }

    public class ODataPatchWhereEx
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ColumnName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Operator { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Object { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HOGE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FUGA { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> PIYO { get; set; }
    }
}
