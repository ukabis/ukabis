using JP.DataHub.Com.Json.Schema;

namespace JP.DataHub.Infrastructure.Database.Consts
{
    // .NET6
    public static class SqlServerDocumentVersion
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
