using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Data;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class CosmonsDBKeyManagement : IKeyManagement
    {
        private const string SEPARATOR = "~";
        public RepositoryInfo RepositoryInfo { get; set; }

        public string GetCacheKey(QueryParam param, IPerRequestDataContainer PerRequestDataContainer, IResourceVersionRepository resourceVersionRepository)
        {
            var key = new StringBuilder("");
            if (param.IsVendor.Value == true)
            {
                key.Append($"{param.VendorId.Value}~{param.SystemId.Value}~");
            }
            if (param.IsPerson.Value == true)
            {
                key.Append($"{PerRequestDataContainer.OpenId}~");
            }

            key.Append(PerRequestDataContainer.XgetInternalAllField ? "1~" : "0~");
            if (PerRequestDataContainer.XResourceSharingWith != null)
            {
                PerRequestDataContainer.XResourceSharingWith.ToList().ForEach(x => key.Append($"{x.Value}~"));
            }

            var conditionString = new StringBuilder("");

            var keys = GetGenerateKey(param, conditionString, resourceVersionRepository, string.IsNullOrEmpty(param.CacheInfo.CacheKey) ? param.RepositoryKey : new RepositoryKey(param.CacheInfo.CacheKey));
            if (param.QueryString != null)
            {
                foreach (var item in param.QueryString.Dic)
                {
                    if (!keys.ContainsKey(item.Key.Value))
                    {
                        keys.Add(item.Key.Value, item.Value.Value);
                    }
                }
            }

            foreach (var x in keys.Values)
            {
                key.Append(x + "~");
            }
            return key.ToString();
        }

        public DocumentDbId CreateDocumentDbId(QueryParam param)
        {
            return DocumentDbId.Create(param.RepositoryKey, param.IsVendor, param.VendorId, param.SystemId, param.IsPerson, param.OpenId, param.IsAutomaticId, param.ResourceVersion, null);
        }


        /// <summary>
        /// GETメソッド用のキーをリポジトリKEYから作成
        /// </summary>
        /// <returns>BlobFileShardingのキー</returns>
        public Dictionary<string, object> GetGenerateKey(QueryParam param, StringBuilder conditionString, IResourceVersionRepository resourceVersionRepository, RepositoryKey repositoryKey = null)
        {
            var retdic = new Dictionary<string, object>();

            // リポジトリキー、レスポンスモデル、コントローラの代表的なモデルから作成
            if (repositoryKey == null)
            {
                repositoryKey = param.RepositoryKey;
            }

            List<JSchema> schemas = new List<JSchema>() { param.UriSchema?.ToJSchema(), param.ResponseSchema?.ToJSchema() };
            schemas = schemas.Where(x => x != null).ToList();
            if (repositoryKey != null)
            {
                foreach (var item in repositoryKey.LogicalKeys)
                {
                    var queryKey = new QueryStringKey(item);
                    var urlParamKey = new UrlParameterKey(item);

                    if (param.QueryString != null && param.QueryString.Dic.ContainsKey(queryKey) == true)
                    {
                        // UrlSchemaによるパースを優先する
                        if (conditionString.ToString().Contains($"{{{item}}}"))
                        {
                            continue;
                        }

                        if (schemas.Count() > 0)
                        {
                            retdic.Add(item, Convert.ChangeType(param.QueryString.Dic[queryKey].Value, schemas.ToType(item)));
                        }
                        else
                        {
                            retdic.Add(item, param.QueryString.Dic[queryKey].Value);
                        }
                    }
                    else if (param.KeyValue != null && param.KeyValue.Dic.ContainsKey(urlParamKey) == true)
                    {
                        if (schemas.Count() > 0)
                        {
                            retdic.Add(item, Convert.ChangeType(param.KeyValue.Dic[urlParamKey].Value, schemas.ToType(item)));
                        }
                        else
                        {
                            retdic.Add(item, param.KeyValue.Dic[urlParamKey].Value);
                        }
                    }
                }
            }

            if (param.ResponseSchema?.ToJSchema() != null)
            {
                foreach (var prop in param.ResponseSchema.ToJSchema().Properties)
                {
                    var item = prop.Key;
                    var queryKey = new QueryStringKey(item);
                    var urlParamKey = new UrlParameterKey(item);
                    if (!retdic.ContainsKey(item) && param.QueryString != null && param.QueryString.Dic.ContainsKey(queryKey) == true)
                    {
                        // UrlSchemaによるパースを優先する
                        if (conditionString.ToString().Contains($"{{{item}}}"))
                        {
                            continue;
                        }
                        retdic.Add(item, param.QueryString.Dic[queryKey].Value);
                    }
                    else if (!retdic.ContainsKey(item) && param.KeyValue != null && param.KeyValue.Dic.ContainsKey(urlParamKey) == true)
                    {
                        retdic.Add(item, param.KeyValue.Dic[urlParamKey].Value);
                    }
                }
            }

            if (param.ControllerSchema?.ToJSchema() != null)
            {
                foreach (var prop in param.ControllerSchema.ToJSchema().Properties)
                {
                    var item = prop.Key;
                    var queryKey = new QueryStringKey(item);
                    var urlParamKey = new UrlParameterKey(item);
                    if (!retdic.ContainsKey(item) && param.QueryString != null && param.QueryString.Dic.ContainsKey(queryKey) == true)
                    {
                        // UrlSchemaによるパースを優先する
                        if (conditionString.ToString().Contains($"{{{item}}}"))
                        {
                            continue;
                        }
                        retdic.Add(item, param.QueryString.Dic[queryKey].Value);
                    }
                    else if (!retdic.ContainsKey(item) && param.KeyValue != null && param.KeyValue.Dic.ContainsKey(urlParamKey) == true)
                    {
                        retdic.Add(item, param.KeyValue.Dic[urlParamKey].Value);
                    }
                }
            }
            if (param.RepositoryKey != null)
            {
                retdic.Add(JsonPropertyConst.TYPE, param.RepositoryKey.Type);
            }
            if (param.XVersion != null)
            {
                int max = resourceVersionRepository.GetMaxVersion(repositoryKey).Value;
                if (param.XVersion.Value > max)
                {
                    throw new XVersionNotFoundException("Invalid X-Version");
                }
            }
            var version = param.XVersion != null ? param.XVersion?.Value : param.RepositoryKey != null ? resourceVersionRepository.GetCurrentVersion(param.RepositoryKey)?.Value : null;
            if (version != null)
            {
                retdic.Add(JsonPropertyConst.VERSION_COLNAME, version);
            }

            return retdic;
        }

        public bool IsIdValid(JToken idProperty, RegisterParam registerParam, IResourceVersionRepository resourceVersionRepository, out DocumentDataId id)
        {
            id = null;
            var idval = idProperty?.ToString();
            var version = resourceVersionRepository.GetRegisterVersion(registerParam.RepositoryKey, registerParam.XVersion);
            var path = DocumentDbId.CreatePath(registerParam.RepositoryKey, registerParam.IsVendor, registerParam.VendorId, registerParam.SystemId, registerParam.IsPerson, registerParam.OpenId, version);

            if (idval?.StartsWith($"{path}{SEPARATOR}") == true)
            {
                id = new DocumentDataId(idval, path, idval.Substring(path.Length + 1));
                return true;
            }
            return false;
        }

        public DocumentDataId GetId(RegisterParam registerParam, IResourceVersionRepository resourceVersionRepository, IPerRequestDataContainer perRequestDataContainer)
        {
            var version = resourceVersionRepository.GetRegisterVersion(registerParam.RepositoryKey, registerParam.XVersion);
            var id = DocumentDbId.Create(registerParam.RepositoryKey, registerParam.IsVendor, registerParam.VendorId, registerParam.SystemId, registerParam.IsPerson, registerParam.OpenId, registerParam.IsAutomaticId, version, registerParam.Json);
            // 個人共有のケース
            if (!string.IsNullOrEmpty(perRequestDataContainer.XResourceSharingPerson) && registerParam.ResourceSharingPersonRules != null && registerParam.ResourceSharingPersonRules.Any())
            {
                id = DocumentDbId.Create(registerParam.RepositoryKey, registerParam.IsVendor, registerParam.VendorId, registerParam.SystemId, registerParam.IsPerson, new OpenId(perRequestDataContainer.XResourceSharingPerson), registerParam.IsAutomaticId, version, registerParam.Json);
            }
            return new DocumentDataId(id.PhysicalId, id.Path, id.LogicalId);
        }

        /// <summary>
        /// パラメータの型を取得（推論）します。
        /// </summary>
        /// <param name="uriSchema">URIスキーマ</param>
        /// <param name="requestSchema">リクエストスキーマ</param>
        /// <param name="responseSchema">レスポンススキーマ</param>
        /// <param name="key">評価対象のパラメータ名</param>
        /// <returns>keyパラメータの型（該当す型がない場合はstring型）</returns>
        /// <remarks>
        /// パラメータ（key）の型を、DynamicAPI定義で設定された以下のスキーマに対して名称でマッチングして取得します。
        /// 　1.URIスキーマ
        /// 　2.リクエストスキーマ
        /// 　3.レスポンススキーマ
        /// ※上記1→2→3の順番で評価し、名称がマッチする定義が見つかった時点で、対象の型として判断します。
        /// </remarks>
        private Type GetJsonSchemaType(JSchema uriSchema, JSchema requestSchema, JSchema responseSchema, string key)
        {
            JSchema[] schemas = new JSchema[] { uriSchema, requestSchema, responseSchema };
            foreach (JSchema schema in schemas)
            {
                var prop = schema?.Properties.SingleOrDefault(s => s.Key == key);
                if (prop != null && prop.HasValue && prop.Value.Value != null)
                {
                    return JSchemaTypeToType(prop.Value.Value.Type);
                }
            }
            return typeof(string);
        }

        private Type JSchemaTypeToType(JSchemaType? type)
        {
            if (type == null)
            {
                return typeof(string);
            }
            switch (type.Value)
            {
                case JSchemaType.None:
                case JSchemaType.Array:
                case JSchemaType.Null:
                case JSchemaType.String:
                    return typeof(string);
                case JSchemaType.Number:
                    return typeof(double);
                case JSchemaType.Boolean:
                    return typeof(bool);
                case JSchemaType.Integer:
                    return typeof(int);
                default:
                    return typeof(string);
            }
        }
    }
}
