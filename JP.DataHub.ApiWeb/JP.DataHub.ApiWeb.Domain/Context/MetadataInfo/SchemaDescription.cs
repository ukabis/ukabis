using MessagePack;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    [Serializable]
    [MessagePackObject]
    public class SchemaDescription : IValueObject
    {
        /// <summary>スキーマID</summary>
        [Key(0)]
        public string SchemaId { get; set; }

        /// <summary>スキーマ名</summary>
        [Key(1)]
        public string SchemaName { get; set; }

        /// <summary>JSONスキーマ</summary>
        [Key(2)]
        public string JsonSchema { get; set; }

        /// <summary>説明</summary>
        [Key(3)]
        public string Description { get; set; }

        [Key(4)]
        public bool IsCollection { get; set; }

        /// <summary>JSONスキーマ内のパス</summary>
        [Key(5)]
        public string Path { get; set; }


        /// <summary>JSchema</summary>
        [IgnoreMember]
        public JSchema JSchema
        {
            get
            {
                if (jSchema != null)
                {
                    return jSchema;
                }

                if (string.IsNullOrEmpty(JsonSchema))
                {
                    return null;
                }

                try
                {
                    jSchema = JSchema.Parse(JsonSchema);
                }
                catch
                {
                    JsonSchema = null;
                }

                return jSchema;
            }
            set
            {
                jSchema = value;
            }
        }
        [IgnoreMember]
        private JSchema jSchema;


        /// <summary>
        /// 指定したパスのスキーマ情報を返します。
        /// </summary>
        /// <param name="path">パス</param>
        /// <returns>スキーマ情報</returns>
        public SchemaDescription SelectPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            JSchema schema = JSchema;
            if (schema == null) return null;

            string modelName = null;
            string[] pathItems = path.Split('.');
            // パスの要素ごとに繰り返し
            foreach (var item in pathItems)
            {
                modelName = null;
                if (schema.OneOf.Any())
                    schema = schema.OneOf[int.Parse(item)];
                else if (schema.AnyOf.Any())
                    schema = schema.AnyOf[int.Parse(item)];
                else if (schema.AllOf.Any())
                    schema = schema.AllOf[int.Parse(item)];
                else
                {
                    if (!schema.Properties.ContainsKey(item)) return null;

                    schema = schema.Properties[item];
                    modelName = item;
                }

                if (schema.Type.HasValue && schema.Type.Value.HasFlag(JSchemaType.Array)) schema = schema.Items[0];
            }

            // スキーマ情報を返却
            return new SchemaDescription
            {
                SchemaId = SchemaId,
                SchemaName = modelName ?? schema.Title,
                Description = schema.Description,
                JSchema = schema,
                Path = path
            };
        }

        public string GetXmlType()
        {
            if (jSchema.Type.Value.HasFlag(JSchemaType.String))
            {
                return "Edm.String";
            }
            else if (jSchema.Type.Value.HasFlag(JSchemaType.Number))
            {
                return "Edm.Decimal";
            }
            else if (jSchema.Type.Value.HasFlag(JSchemaType.Integer))
            {
                return "Edm.Int64";
            }
            else if (jSchema.Type.Value.HasFlag(JSchemaType.Boolean))
            {
                return "Edm.Boolean";
            }
            else
            {
                return jSchema.Type.ToString();
            }
        }

        public string GetForeignKeyUrl()
        {
            // FK制約
            var formatSplits = jSchema.Format.Split(';');
            var formatFK = formatSplits.FirstOrDefault(y => y.StartsWith("ForeignKey"));
            var url = "";
            if (!string.IsNullOrEmpty(formatFK))
            {
                // URLを切り出す
                url = formatFK.Replace("ForeignKey", "");
                if (url.StartsWith("(") && url.EndsWith(")"))
                {
                    url = url.TrimStart('(').TrimEnd(')');
                }
                else
                {
                    url = url.Trim();
                }

                if (url.Contains("?"))
                {
                    url = url.Substring(0, url.IndexOf("?"));
                    url = url.Substring(0, url.LastIndexOf("/"));
                }
                else if (url.Contains("{"))
                {
                    url = url.Substring(0, url.IndexOf("{"));
                    url = url.Substring(0, url.LastIndexOf("/"));
                }
            }
            return url;
        }
    }
}
