using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.MetadataInfo;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    /// <summary>
    /// APIのメタデータを取得します。
    /// </summary>
    public class MetadataInfoApplicationService : IMetadataInfoApplicationService
    {
        private const string DocumentUrl = "http://docs.oasis-open.org/odata/ns/edmx";
        private const string ODataVersion = "4.01";
        private const string SchemaName = "DynamicApi";

        private IMetadataInfoRepository Repository => _lazyRepository.Value;
        private Lazy<IMetadataInfoRepository> _lazyRepository = new Lazy<IMetadataInfoRepository>(() => UnityCore.Resolve<IMetadataInfoRepository>());


        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">削除フラグの検索条件</param>
        /// <param name="isEnableOnly">有効フラグの検索条件</param>
        /// <param name="isNotHiddenOnly">非公開フラグの検索条件</param>
        /// <returns>API情報の一覧</returns>
        public IEnumerable<ApiDescription> GetApiDescription(bool noChildren, string localeCode = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false)
            => Repository.GetApiDescription(noChildren, localeCode, isActiveOnly, isEnableOnly, isNotHiddenOnly);

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <returns>スキーマ情報の一覧</returns>
        public IEnumerable<SchemaDescription> GetSchemaDescription(string localeCode = null)
            => Repository.GetSchemaDescription(localeCode);

        /// <summary>
        /// メタデータを作成します。
        /// </summary>
        /// <param name="apis">API情報</param>
        /// <param name="schemas">スキーマ情報</param>
        /// <param name="urlSchemas">URLスキーマ情報</param>
        /// <returns>メタデータ</returns>
        public string CreateMetadata(List<ApiDescription> apis, List<SchemaDescription> schemas, List<SchemaDescription> urlSchemas)
        {
            //XMLにする
            StringWriter stringWriter = new StringWriterUTF8();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            try
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("edmx:Edmx", "");
                xmlWriter.WriteAttributeString("xmlns:edmx", "", DocumentUrl);
                xmlWriter.WriteAttributeString("Version", "", ODataVersion);
                xmlWriter.WriteStartElement("edmx:DataServices", "");
                // metadataのスキーマ
                xmlWriter.WriteStartElement("Schema", "");
                xmlWriter.WriteAttributeString("Namespace", "", SchemaName);

                // jsonスキーマをXMLにする
                foreach (var schema in schemas)
                {
                    if (schema.JSchema == null) continue;

                    // キー項目はAPIのリポジトリキーから取得する
                    var api = apis.FirstOrDefault(x => x.ApiSchemaId.ToString() == schema.SchemaId);
                    var parentSchemas = new List<SchemaDescription>();
                    CreateXmlEntity(schema, api?.RepositoryKey, ref xmlWriter, ref parentSchemas, apis, schemas);
                }

                // APIメソッドをXMLにする
                foreach (var api in apis)
                {
                    CreateXmlActionFunction(ref xmlWriter, api, urlSchemas, schemas);
                }

                // コンテナの項目を作成する
                CreateXmlContainer(ref xmlWriter, apis, schemas);

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            finally
            {
                xmlWriter.Close();
            }

            return stringWriter.ToString();
        }

        /// <summary>
        /// XMLのEntityTypeを作成する
        /// </summary>
        /// <param name="schema">対象のjsonスキーマ</param>
        /// <param name="repositoryKey">APIのリポジトリキー</param>
        /// <param name="xmlWriter">XmlTextWriter</param>
        /// <param name="parentSchemas">親のjsonスキーマ</param>
        /// <param name="apis">全API</param>
        /// <param name="allSchemas">全jsonスキーマ</param>
        private void CreateXmlEntity(SchemaDescription schema, string repositoryKey, ref XmlTextWriter xmlWriter,
            ref List<SchemaDescription> parentSchemas, IEnumerable<ApiDescription> apis, List<SchemaDescription> allSchemas)
        {
            var children = new List<SchemaDescription>();
            var enumEntity = new Dictionary<string, IList<JToken>>();

            // キー項目はAPIのリポジトリキーから取得する
            var hasKey = false;
            if (!string.IsNullOrEmpty(repositoryKey))
            {
                var repositoryKeys = repositoryKey.Split('/');
                foreach (var key in repositoryKeys)
                {
                    if (key.StartsWith("{") && key.EndsWith("}"))
                    {
                        if (!hasKey)
                        {
                            xmlWriter.WriteStartElement("EntityType", "");
                            xmlWriter.WriteAttributeString("Name", "", schema.SchemaName);
                            xmlWriter.WriteStartElement("Key", "");
                            hasKey = true;
                        }
                        xmlWriter.WriteStartElement("PropertyRef", "");
                        xmlWriter.WriteAttributeString("Name", "", key.TrimStart('{').TrimEnd('}'));
                        xmlWriter.WriteEndElement();
                    }
                }
            }
            if (hasKey)
            {
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteStartElement("ComplexType", "");
                xmlWriter.WriteAttributeString("Name", "", schema.SchemaName);
            }

            // Propertyを作る
            CreateProperty(schema, repositoryKey, ref xmlWriter, ref parentSchemas, apis, allSchemas, true, ref children, ref enumEntity);

            xmlWriter.WriteEndElement();
            parentSchemas.Add(schema);

            // 子要素のEntityを作る
            foreach (var child in children)
            {
                CreateXmlEntity(child, null, ref xmlWriter, ref parentSchemas, apis, allSchemas);
            }

            // Enum定義を作る
            foreach (var enumValue in enumEntity)
            {
                CreateXmlEnum(ref xmlWriter, enumValue.Key, enumValue.Value);
            }
        }

        /// <summary>
        /// Property(Parameter)を作る
        /// </summary>
        /// <param name="schema">対象のjsonスキーマ</param>
        /// <param name="repositoryKey">APIのリポジトリキー</param>
        /// <param name="xmlWriter">XmlTextWriter</param>
        /// <param name="parentSchemas">親のjsonスキーマ</param>
        /// <param name="apis">全API</param>
        /// <param name="allSchemas">全jsonスキーマ</param>
        /// <param name="isProperty">tlue:Property、false:Parameter</param>
        /// <param name="children">子スキーマ</param>
        /// <param name="enumEntity">Enum</param>
        private void CreateProperty(SchemaDescription schema, string repositoryKey, ref XmlTextWriter xmlWriter,
            ref List<SchemaDescription> parentSchemas, IEnumerable<ApiDescription> apis, List<SchemaDescription> allSchemas,
            bool isProperty, ref List<SchemaDescription> children, ref Dictionary<string, IList<JToken>> enumEntity)
        {
            var properties = schema.JSchema.Properties.Any() ? schema.JSchema.Properties :
                schema.JSchema.Items.Any() ? schema.JSchema.Items.First().Properties : new Dictionary<string, JSchema>();
            foreach (var property in properties)
            {
                SchemaDescription fkSchema = null;

                if (isProperty)
                {
                    xmlWriter.WriteStartElement("Property", "");
                }
                else
                {
                    xmlWriter.WriteStartElement("Parameter", "");
                }
                xmlWriter.WriteAttributeString("Name", "", property.Key);

                var parameter = new SchemaDescription()
                {
                    Path = schema.Path == null ? property.Key : schema.Path + "." + property.Key,
                    SchemaName = property.Key,
                    SchemaId = schema.SchemaId,
                    JSchema = property.Value
                };

                if (parameter.JSchema.Type.HasValue)
                {
                    // 型
                    string type = "";
                    if (parameter.JSchema.Type.Value.HasFlag(JSchemaType.Object) ||
                        parameter.JSchema.Type.Value.HasFlag(JSchemaType.Array))
                    {
                        if (parentSchemas != null)
                        {
                            var parentSchema = parentSchemas.Where(y => y.JSchema == property.Value);
                            if (parentSchema.Any())
                            {
                                type = parentSchema.First().SchemaName;
                            }
                            else
                            {
                                type = schema.SchemaName + "_" + parameter.SchemaName;
                            }
                        }
                        else
                        {
                            type = parameter.JSchema.Type.ToString();
                        }
                    }
                    else
                    {
                        type = parameter.GetXmlType();
                    }

                    if (parameter.IsCollection || parameter.JSchema.Type.Value.HasFlag(JSchemaType.Object) || parameter.JSchema.Type.Value.HasFlag(JSchemaType.Array))
                    {
                        // Object、Arrayの場合は別のEntityを参照する
                        xmlWriter.WriteAttributeString("Type", "", "Collection(self." + type + ")");

                        // 子要素は後でまとめて作る
                        if (!parentSchemas.Any(y => y.JSchema == property.Value))
                        {
                            if (property.Value.Items.Any() && property.Value.Items.First().Enum.Any())
                            {
                                enumEntity.Add(type, property.Value.Items.First().Enum);
                            }
                            else
                            {
                                children.Add(new SchemaDescription()
                                {
                                    Path = (schema.Path == null ? schema.SchemaName : schema.Path) + "." + property.Key,
                                    SchemaName = type,
                                    SchemaId = schema.SchemaId,
                                    JSchema = property.Value
                                });
                            }
                        }
                    }
                    else if (parameter.JSchema.Enum.Any())
                    {
                        // Enumの場合も参照する
                        string enumName = (schema.Path == null ? schema.SchemaName : schema.Path) + "_" + property.Key + "_enum";
                        xmlWriter.WriteAttributeString("Type", "", "self." + enumName);
                        xmlWriter.WriteAttributeString("Nullable", "", parameter.JSchema.Type.Value.HasFlag(JSchemaType.Null).ToString());

                        // enum定義は後でまとめて作る
                        enumEntity.Add(enumName, parameter.JSchema.Enum);
                    }
                    else
                    {
                        xmlWriter.WriteAttributeString("Type", "", type);
                        xmlWriter.WriteAttributeString("Nullable", "", parameter.JSchema.Type.Value.HasFlag(JSchemaType.Null).ToString());
                    }

                    // 文字数
                    if (parameter.JSchema.Type.Value.HasFlag(JSchemaType.String))
                    {
                        if (parameter.JSchema.MaximumLength != null)
                        {
                            xmlWriter.WriteAttributeString("maxLength", "", parameter.JSchema.MaximumLength.ToString());
                        }
                        if (parameter.JSchema.MinimumLength != null)
                        {
                            xmlWriter.WriteStartElement("Annotation", "");
                            xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                            xmlWriter.WriteAttributeString("String", "", "MinimumLength " + parameter.JSchema.MinimumLength.ToString());
                            xmlWriter.WriteEndElement();
                        }
                        if (parameter.JSchema.Pattern != null)
                        {
                            xmlWriter.WriteStartElement("Annotation", "");
                            xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                            xmlWriter.WriteAttributeString("String", "", "Pattern '" + parameter.JSchema.Pattern.ToString() + "'");
                            xmlWriter.WriteEndElement();
                        }
                    }
                    // 桁数
                    else if (parameter.JSchema.Type.Value.HasFlag(JSchemaType.Number) || parameter.JSchema.Type.Value.HasFlag(JSchemaType.Integer))
                    {
                        if (!string.IsNullOrEmpty(parameter.JSchema.Format) && parameter.JSchema.Format.Contains("Number"))
                        {
                            var formatSplit = parameter.JSchema.Format.Split(';');
                            var format = formatSplit.FirstOrDefault(y => y.StartsWith("Number"));
                            if (!string.IsNullOrEmpty(format))
                            {
                                var number = format.Replace("Number", "");
                                if (number.StartsWith("(") && number.EndsWith(")"))
                                {
                                    number = number.TrimStart('(').TrimEnd(')');
                                }
                                else
                                {
                                    number = number.Trim();
                                }

                                var numberSplit = number.Split(',');
                                if (int.TryParse(numberSplit[0], out var precision))
                                {
                                    xmlWriter.WriteAttributeString("Precision", "", precision.ToString());
                                }
                                if (numberSplit.Length > 1 && int.TryParse(numberSplit[1], out var scale))
                                {
                                    xmlWriter.WriteAttributeString("Scale", "", scale.ToString());
                                }
                            }
                        }
                    }

                    // ここからAnnotationを作成する
                    // 桁数
                    if (parameter.JSchema.Type.Value.HasFlag(JSchemaType.Number) || parameter.JSchema.Type.Value.HasFlag(JSchemaType.Integer))
                    {
                        if (parameter.JSchema.Maximum != null)
                        {
                            xmlWriter.WriteStartElement("Annotation", "");
                            xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                            xmlWriter.WriteAttributeString("String", "", "Maximum " + parameter.JSchema.Maximum.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Annotation", "");
                            xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                            xmlWriter.WriteAttributeString("String", "", "ExclusiveMaximum " + parameter.JSchema.ExclusiveMaximum);
                            xmlWriter.WriteEndElement();
                        }
                        if (parameter.JSchema.Minimum != null)
                        {
                            xmlWriter.WriteStartElement("Annotation", "");
                            xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                            xmlWriter.WriteAttributeString("String", "", "Maximum " + parameter.JSchema.Minimum.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Annotation", "");
                            xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                            xmlWriter.WriteAttributeString("String", "", "ExclusiveMaximum " + parameter.JSchema.ExclusiveMinimum);
                            xmlWriter.WriteEndElement();
                        }
                    }
                    // Format
                    if (!string.IsNullOrEmpty(parameter.JSchema.Format))
                    {
                        var formats = parameter.JSchema.Format.Split(';');
                        foreach (var format in formats)
                        {
                            if (format.Contains("Number"))
                            {
                                // 別で作るから
                                continue;
                            }

                            if (format.Contains("ForeignKey"))
                            {
                                // FK制約
                                var url = parameter.GetForeignKeyUrl();
                                if (!string.IsNullOrEmpty(url))
                                {
                                    foreach (var api in apis)
                                    {
                                        var method = api.Methods.FirstOrDefault(x => (api.RelativePath.TrimEnd('/') + "/" + x.RelativePath).Contains(url));
                                        if (method == null) continue;

                                        fkSchema = allSchemas.FirstOrDefault(x => x.SchemaId == method.UrlSchemaId.ToString());
                                        break;
                                    }

                                    // jsonスキーマが見つからない場合はAnnotationで作る
                                    if (fkSchema != null)
                                    {
                                        continue;
                                    }
                                }
                            }

                            xmlWriter.WriteStartElement("Annotation", "");
                            xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                            xmlWriter.WriteAttributeString("String", "", format.ToString());
                            xmlWriter.WriteEndElement();
                        }
                    }
                }
                else if (parameter.JSchema.OneOf.Any() || parameter.JSchema.AnyOf.Any() || parameter.JSchema.AllOf.Any())
                {
                    // xxxOfパターン
                    IList<JSchema> ofSchemas = new List<JSchema>();
                    var AnnotationString = "";
                    if (parameter.JSchema.OneOf.Any())
                    {
                        ofSchemas = parameter.JSchema.OneOf;
                        AnnotationString = "OneOf";
                    }
                    else if (parameter.JSchema.AnyOf.Any())
                    {
                        ofSchemas = parameter.JSchema.AnyOf;
                        AnnotationString = "AnyOf";
                    }
                    else if (parameter.JSchema.AllOf.Any())
                    {
                        ofSchemas = parameter.JSchema.AllOf;
                        AnnotationString = "AllOf";
                    }

                    var type = "";
                    if (parentSchemas != null)
                    {
                        var parentSchema = parentSchemas.Where(y => y.JSchema == property.Value);
                        if (parentSchema.Any())
                        {
                            type = parentSchema.First().SchemaName;
                        }
                        else
                        {
                            type = schema.SchemaName + "_" + parameter.SchemaName;
                        }
                    }
                    else
                    {
                        type = schema.JSchema.Type.ToString();
                    }

                    for (var i = 0; i < ofSchemas.Count(); i++)
                    {
                        var ofSchemaName = type + (i + 1).ToString();

                        if (i > 0)
                        {
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("Property", "");
                            xmlWriter.WriteAttributeString("Name", "", property.Key);
                        }
                        // 別のEntityを参照する
                        if (parameter.JSchema.OneOf.Any())
                        {
                            xmlWriter.WriteAttributeString("Type", "", "self." + ofSchemaName);
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString("Type", "", "Collection(self." + ofSchemaName + ")");
                        }

                        xmlWriter.WriteStartElement("Annotation", "");
                        xmlWriter.WriteAttributeString("Term", "", "Core.Description");
                        xmlWriter.WriteAttributeString("String", "", AnnotationString);
                        xmlWriter.WriteEndElement();


                        // 子要素は後でまとめて作る
                        children.Add(new SchemaDescription()
                        {
                            Path = (schema.Path == null ? schema.SchemaName : schema.Path) + "." + property.Key,
                            SchemaName = ofSchemaName,
                            SchemaId = schema.SchemaId,
                            JSchema = ofSchemas[i]
                        });
                    }
                }

                xmlWriter.WriteEndElement();

                // FK制約
                if (fkSchema != null)
                {
                    xmlWriter.WriteStartElement("NavigationProperty", "");
                    xmlWriter.WriteAttributeString("Name", "", fkSchema.SchemaName);
                    xmlWriter.WriteAttributeString("Type", "", "self." + fkSchema.SchemaName);
                    xmlWriter.WriteAttributeString("Nullable", "", "false");

                    var fkProperties = fkSchema.JSchema.Properties.Any() ? fkSchema.JSchema.Properties :
                        fkSchema.JSchema.Items.Any() ? fkSchema.JSchema.Items.First().Properties : new Dictionary<string, JSchema>();
                    if (fkProperties != null)
                    {
                        xmlWriter.WriteStartElement("ReferentialConstraint", "");
                        xmlWriter.WriteAttributeString("Property", "", property.Key);
                        xmlWriter.WriteAttributeString("ReferencedProperty", "", fkProperties.Keys.First());
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// XMLのEnumTypeを作成する
        /// </summary>
        /// <param name="xmlWriter">XmlTextWriter</param>
        /// <param name="name">EnumTypeの名前</param>
        /// <param name="enumValue">Enumデータ</param>
        private void CreateXmlEnum(ref XmlTextWriter xmlWriter, string name, IList<JToken> enumValue)
        {
            xmlWriter.WriteStartElement("EnumType", "");
            xmlWriter.WriteAttributeString("Name", "", name);
            foreach (var value in enumValue)
            {
                xmlWriter.WriteStartElement("Member", "");
                xmlWriter.WriteAttributeString("Name", "", value.ToString());
                xmlWriter.WriteAttributeString("Value", "", value.ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XMLのActionとFunctionを作成する
        /// </summary>
        /// <param name="xmlWriter">XmlTextWriter</param>
        /// <param name="api">API</param>
        /// <param name="urlSchemas">URLスキーマ</param>
        /// <param name="allSchemas">他のスキーマ</param>
        private void CreateXmlActionFunction(ref XmlTextWriter xmlWriter, ApiDescription api, List<SchemaDescription> urlSchemas, List<SchemaDescription> allSchemas)
        {
            foreach (var method in api.Methods)
            {
                if (method.HttpMethod.ToLower() == "get")
                {
                    xmlWriter.WriteStartElement("Function", "");
                }
                else
                {
                    xmlWriter.WriteStartElement("Action", "");
                }

                // Nameを一意にするためにURLを入れる
                xmlWriter.WriteAttributeString("Name", "", api.RelativePath.TrimEnd('/') + "/" + method.RelativePath);

                // URLスキーマを検索する
                if (method.UrlSchemaId != null)
                {
                    var urlSchema = urlSchemas.FirstOrDefault(x => x.SchemaId == method.UrlSchemaId.ToString());
                    if (urlSchema != null)
                    {
                        // Propertyを作る
                        var dummySchemas = new List<SchemaDescription>();
                        var dummyApis = new List<ApiDescription>();
                        var dummyEnum = new Dictionary<string, IList<JToken>>();
                        CreateProperty(urlSchema, "", ref xmlWriter, ref dummySchemas, dummyApis, allSchemas, false, ref dummySchemas, ref dummyEnum);
                    }
                }
                else if (method.RelativePath.Contains("{"))
                {
                    var urlSplit = new List<string>();
                    // URLスキーマがない場合はURLから作る
                    if (method.RelativePath.Contains("?"))
                    {
                        var param = method.RelativePath.Substring(method.RelativePath.IndexOf("?") + 1);
                        urlSplit = param.Split('&').ToList();
                    }
                    else
                    {
                        urlSplit = method.RelativePath.Split('/').ToList();
                    }
                    foreach (var item in urlSplit)
                    {
                        if (item.Contains("{") && item.Contains("}"))
                        {
                            var paramName = item.Substring(item.IndexOf("{") + 1);
                            paramName = paramName.Substring(0, paramName.IndexOf("}"));

                            xmlWriter.WriteStartElement("Parameter", "");
                            xmlWriter.WriteAttributeString("Name", "", paramName);
                            xmlWriter.WriteAttributeString("Type", "", "Edm.String");
                            xmlWriter.WriteEndElement();
                        }
                    }
                }

                // レスポンススキーマを検索する
                if (method.ResponseSchemaId != null)
                {
                    var responseSchema = allSchemas.FirstOrDefault(x => x.SchemaId == method.ResponseSchemaId.ToString());
                    if (responseSchema != null)
                    {
                        xmlWriter.WriteStartElement("ReturnType", "");
                        xmlWriter.WriteAttributeString("Type", "", "self." + responseSchema.SchemaName);
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// XMLのコンテナを作成する
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="apis"></param>
        /// <param name="schemas"></param>
        private void CreateXmlContainer(ref XmlTextWriter xmlWriter, IEnumerable<ApiDescription> apis, List<SchemaDescription> schemas)
        {
            xmlWriter.WriteStartElement("EntityContainer", "");
            xmlWriter.WriteAttributeString("Name", "", "Container");
            foreach (var api in apis)
            {
                var usedSchemaID = new List<Guid>();
                xmlWriter.WriteStartElement("EntitySet", "");
                xmlWriter.WriteAttributeString("Name", "", api.RelativePath);
                var apiSchema = schemas.FirstOrDefault(x => x.SchemaId == api.ApiSchemaId.ToString());
                usedSchemaID.Add(api.ApiSchemaId);
                if (apiSchema != null)
                {
                    xmlWriter.WriteAttributeString("EntityType", "", "self." + apiSchema.SchemaName);
                }

                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }



        /// <summary>
        /// XMLをUTF-8で作りたいので、StringWriterのエンコードをUTF-8にする
        /// </summary>
        private class StringWriterUTF8 : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}
