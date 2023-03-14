using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;

namespace JP.DataHub.Infrastructure.Database.Consts
{
    // .NET6
    public static class OracleDbVersionInfo
    {
        public const string VersionTableName = "VersionInfo";
        public const string VersionInfoColumnName = "versioninfo";

        public static readonly JSchema VersionTableSchema = JSchema.Parse($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""{JsonPropertyConst.ID}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.CURRENT_VERSION}"": {{ ""type"": ""string"" }},
        ""{OracleDbVersionInfo.VersionInfoColumnName}"": {{ ""type"": ""object"" }},
        ""{JsonPropertyConst.OPENID}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.REGDATE}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.UPDUSERID}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.UPDDATE}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.ETAG}"": {{ ""type"": ""string"" }}
    }}
}}");
    }
}
