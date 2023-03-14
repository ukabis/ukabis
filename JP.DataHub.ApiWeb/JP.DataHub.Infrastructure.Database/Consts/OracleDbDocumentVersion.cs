using JP.DataHub.Com.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Database.Consts
{
    public static class OracleDbDocumentVersion
    {
        public const string DocumentVersionTableName = "DocumentVersion";
        public const string DocumentVersionColumnName = "documentversions";
        public const string AttachFileDocumentIdColumnName = "DocumentIdForAttachFile";

        public static readonly JSchema DocumentVersionTableSchema = JSchema.Parse($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""{JsonPropertyConst.ID}"": {{ ""type"": ""string"" }},
        ""{DocumentVersionColumnName}"": {{ ""type"": ""object"" }},
        ""{AttachFileDocumentIdColumnName}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.TYPE}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.OPENID}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.REGDATE}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.UPDUSERID}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.UPDDATE}"": {{ ""type"": ""string"" }},
        ""{JsonPropertyConst.ETAG}"": {{ ""type"": ""string"" }}
    }}
}}");
    }
}
