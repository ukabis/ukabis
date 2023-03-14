using System.Text.RegularExpressions;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal partial class DynamicApiService : AbstractService, IDynamicApiService
    {
        private IDynamicApiRepository DynamicApiRepository => _lazyDynamicApiRepository.Value;
        private Lazy<IDynamicApiRepository> _lazyDynamicApiRepository = new(() => UnityCore.Resolve<IDynamicApiRepository>());

        private IRepositoryGroupRepository RepositoryGroupRepository => _lazyRepositoryGroupRepository.Value;
        private Lazy<IRepositoryGroupRepository> _lazyRepositoryGroupRepository = new(() => UnityCore.Resolve<IRepositoryGroupRepository>());

        private IDataSchemaRepository DataSchemaRepository => _lazyDataSchemaRepository.Value;
        private Lazy<IDataSchemaRepository> _lazyDataSchemaRepository = new(() => UnityCore.Resolve<IDataSchemaRepository>());


        /// <summary>
        /// ManageAPIによる依存設定の変更を許可するか
        /// </summary>
        private static bool AllowResourceDependencyChangeByManageApi => AppConfig.GetValue<bool>("AllowResourceDependencyChangeByManageApi", false);

        /// <summary>
        /// OpenId必須か
        /// </summary>
        private static bool OpenIdConnectMustBeUsed => AppConfig.GetValue<bool>("OpenIdConnectMustBeUsed", false);

        #region マスター取得系
        /// <summary>
        /// DynamicAPIのカテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのカテゴリーの一覧</returns>
        public IEnumerable<CategoryQueryModel> GetCategories()
            => DynamicApiRepository.GetCategories();

        /// <summary>
        /// 分野の一覧を取得します。
        /// </summary>
        /// <returns>分野の一覧</returns>
        public IEnumerable<FieldQueryModel> GetFields()
            => new FieldTree(DynamicApiRepository.GetFields()).TreeList;

        /// <summary>
        /// タグの一覧を取得します。
        /// </summary>
        /// <returns>タグの一覧</returns>
        public IEnumerable<TagQueryModel> GetTags()
            => new TagTree(DynamicApiRepository.GetTags()).TreeList;

        /// <summary>
        /// RepositoryGroupの一覧を取得します。
        /// </summary>
        /// <returns>RepositoryGroupの一覧</returns>
        public IEnumerable<RepositoryGroupModel> GetRepositoryGroups()
            => DynamicApiRepository.GetRepositoryGroups();

        /// <summary>
        /// DynamicAPIの指定の添付ファイルストレージの一覧を取得します。
        /// </summary>
        /// <param name="repositoryTypeCd">リポジトリタイプコード</param>
        /// <returns>DynamicAPIの添付ファイルストレージの一覧</returns>
        public IEnumerable<RepositoryGroupModel> GetAttachFileStorage(IEnumerable<string> repositoryTypeCd)
            => DynamicApiRepository.GetAttachFileStorage(repositoryTypeCd);

        /// <summary>
        /// ActionTypeの一覧を取得します。
        /// </summary>
        /// <returns>ActionTypeの一覧</returns>
        public IEnumerable<ActionTypeModel> GetActionTypes()
            => DynamicApiRepository.GetActionTypes();

        /// <summary>
        /// HttpMethodTypeの一覧を取得します。
        /// </summary>
        /// <returns>HttpMethodTypeの一覧</returns>
        public IEnumerable<HttpMethodTypeModel> GetHttpMethodTypes()
            => DynamicApiRepository.GetHttpMethodTypes();

        /// <summary>
        /// Languageリストを取得します。
        /// </summary>
        /// <returns>Languageリスト</returns>
        public IEnumerable<LanguageModel> GetLanguageList()
            => DynamicApiRepository.GetLanguageList();

        /// <summary>
        /// ScriptTypeリストを取得します。
        /// </summary>
        /// <returns>ScriptTypeリスト</returns>
        public IEnumerable<ScriptTypeModel> GetScriptTypeList()
            => DynamicApiRepository.GetScriptTypeList();

        /// <summary>
        /// QueryTypeリストを取得します。
        /// </summary>
        /// <returns>QueryTypeリスト</returns>
        public IEnumerable<QueryTypeModel> GetQueryTypeList()
            => DynamicApiRepository.GetQueryTypeList();

        /// <summary>
        /// ControllerIdに紐づくControllerCommonIpFilterGroupリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>ControllerCommonIpFilterGroupリスト</returns>
        public IEnumerable<ControllerCommonIpFilterGroupModel> GetControllerCommonIpFilterGroup(string controllerId)
        {
            var result = DynamicApiRepository.GetControllerCommonIpFilterGroupListContainControllerIdNull(controllerId);
            if (result == null || !result.Any())
            {
                throw new NotFoundException();
            }

            return result;
        }

        #endregion

        #region Schema
        /// <summary>
        /// ベンダーIDに紐づくDynamicAPIのスキーマの一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID。</param>
        /// <returns>
        /// DynamicAPIのスキーマの一覧。
        /// <paramref name="vendorId"/>がnullの場合、運用管理ベンダーであれば全ベンダーのスキーマ一覧、それ以外のベンダーであれば自ベンダーのスキーマ一覧。
        /// </returns>
        public IEnumerable<SchemaModel> GetSchemas(string vendorId)
        {
            if (!PerRequestDataContainer.IsOperatingVendorUser)
            {
                if (vendorId == null)
                {
                    // 運用管理ベンダー以外でベンダーIDがnullの場合は自ベンダーのスキーマを返す
                    vendorId = PerRequestDataContainer.VendorId;
                }
                else if (vendorId != PerRequestDataContainer.VendorId)
                {
                    // 運用管理ベンダー以外で他ベンダーのIDを指定された場合はエラー
                    throw new AccessDeniedException();
                }
            }

            return DataSchemaRepository.GetSchemas(vendorId);
        }

        /// <summary>
        /// 指定されたスキーマIDに紐づくDynamicAPIのスキーマの一件を取得します。
        /// </summary>
        /// <param name="schemaId">スキーマID</param>
        /// <returns>DynamicAPIのスキーマ</returns>
        public SchemaModel GetSchemaById(string schemaId)
            => DataSchemaRepository.GetDataSchemaById(schemaId);

        /// <summary>
        /// スキーマを登録します。(upsert)
        /// </summary>
        /// <param name="model">スキーマの情報</param>
        /// <param name="isUriOrResponse">URIモデルまたはResponseモデルか？</param>
        /// <returns>SchemaのID</returns>
        public DataSchemaInformationModel RegistOrUpdateSchema(DataSchemaInformationModel model, bool isUriOrResponse)
        {
            model.VendorId = string.IsNullOrEmpty(model.VendorId) ? PerRequestDataContainer.VendorId : model.VendorId;

            // ベンダーID指定でスキーマ登録を行うためベンダーの存在チェックを行う
            if (!DynamicApiRepository.ExistsVendor(model.VendorId))
            {
                throw new ForeignKeyException("Vendor doesn't exist.");
            }
            // 運用管理ベンダー以外は他ベンダーのスキーマの更新不可
            if (model.VendorId.ToUpper() != PerRequestDataContainer.VendorId.ToUpper() && !PerRequestDataContainer.IsOperatingVendorUser)
            {
                throw new AccessDeniedException("Data Schema in other vendors cannot be updated.");
            }

            // JsonSchemaの形式チェック
            JObject.Parse(model.DataSchema);

            // 同じスキーマ名のスキーマがある場合は更新、なければ新規登録
            var sameIdModel = DataSchemaRepository.GetDataSchemaById(model.DataSchemaId);
            var sameNameModel = DataSchemaRepository.GetDataSchemaByName(model.SchemaName);
            SchemaModel currentModel = null;
            if (sameIdModel != null && sameNameModel != null)
            {
                if (sameIdModel.SchemaId == sameNameModel.SchemaId)
                {
                    // 既存モデルをID指定で更新(名前変更なし)
                    currentModel = sameIdModel;
                }
                else
                {
                    // 既存モデルをID指定で更新(名前変更あり、コンフリクトあり)
                    throw new ConflictException("A schema with the same name already exists.");
                }
            }
            else if (sameIdModel != null)
            {
                // 既存モデルをID指定で更新(名前変更あり、コンフリクトなし)
                currentModel = sameIdModel;
            }
            else if (sameNameModel != null)
            {
                if (string.IsNullOrEmpty(model.DataSchemaId))
                {
                    // 既存モデルを名前指定で更新
                    currentModel = sameNameModel;
                    model.DataSchemaId = currentModel.SchemaId.ToString();
                }
                else
                {
                    // 新規モデルの登録(コンフリクトあり)
                    // ※管理画面から既存のスキーマと同じ名前で新規作成する場合
                    throw new ConflictException("A schema with the same name already exists.");
                }
            }
            else
            {
                // 新規モデルの登録(コンフリクトなし)
                model.DataSchemaId ??= Guid.NewGuid().ToString();
            }

            // ベンダー変更は運用管理ベンダーのみ可(使用中のAPIがある場合を除く)
            if (currentModel != null && currentModel.VendorId.ToUpper() != model.VendorId.ToUpper())
            {
                if (PerRequestDataContainer.IsOperatingVendorUser)
                {
                    if (DataSchemaRepository.IsUsedFromApi(model.DataSchemaId))
                    {
                        throw new InUseException("指定されたモデルはApiメソッドで使用されている為ベンダーを変更できません。");
                    }
                    if (DataSchemaRepository.IsUsedFromController(model.DataSchemaId))
                    {
                        throw new InUseException("指定されたモデルはApiで使用されている為ベンダーを変更できません。");
                    }
                }
                else
                {
                    throw new AccessDeniedException("Data Schema in other vendors cannot be updated.");
                }
            }

            // 定義済みキーワードチェック
            if (!isUriOrResponse && HasDefinedKeyword(new DataSchema(model.DataSchema), out var errorKeywords))
            {
                throw new AlreadyExistsException($"既に定義済みのキーワードです。Keywords={string.Join(",", errorKeywords)}");
            }

            return DataSchemaRepository.UpsertSchema(model);
        }

        /// <summary>
        /// スキーマのベンダーを変更しようとしているか
        /// </summary>
        /// <param name="model">スキーマー情報</param>
        /// <returns>true:変更する/false:しない</returns>
        private bool DataSchemaVendorChanged(DataSchemaInformationModel model)
        {
            var info = DataSchemaRepository.GetDataSchemaImformation(model.DataSchemaId);
            if (info == null) return false;
            else return info.VendorId != model.VendorId.ToLower();
        }

        private static readonly Regex AllowedKeywordRegex = new("_etag", RegexOptions.None);
        private static readonly Regex DefinedKeywordRegex = new("(^id$|^_.*$)", RegexOptions.None);

        /// <summary>
        /// スキーマのrootプロパティに定義済みキーワードが含まれているかチェックする
        /// </summary>
        /// <param name="dataSchema">データスキーマ</param>
        /// <param name="errorKeywords">返却用エラーキーワード</param>
        /// <returns>true:含まれている/false:含まれていない</returns>
        private static bool HasDefinedKeyword(DataSchema dataSchema, out List<string> errorKeywords)
        {
            errorKeywords = new List<string>();

            var jSchema = dataSchema?.GetJSchema();
            if (jSchema == null)
            {
                return false;
            }

            // root検索
            SearchDefinedKeyword(jSchema, errorKeywords);

            // rootのoneOf検索
            foreach (var schema in jSchema.OneOf)
            {
                SearchDefinedKeyword(schema, errorKeywords);
            }

            // rootのanyOf検索
            foreach (var schema in jSchema.AnyOf)
            {
                SearchDefinedKeyword(schema, errorKeywords);
            }

            // rootのallOf検索
            foreach (var schema in jSchema.AllOf)
            {
                SearchDefinedKeyword(schema, errorKeywords);
            }

            return errorKeywords.Any();

            void SearchDefinedKeyword(JSchema targetSchema, List<string> errorKeywordList)
            {
                foreach (var jSchemaProperty in targetSchema.Properties)
                {
                    if (AllowedKeywordRegex.IsMatch(jSchemaProperty.Key))
                    {
                        continue;
                    }

                    if (DefinedKeywordRegex.IsMatch(jSchemaProperty.Key))
                    {
                        errorKeywordList.Add(jSchemaProperty.Key);
                    }
                }
            }
        }

        /// <summary>
        /// スキーマを削除します。
        /// </summary>
        /// <param name="dataSchemaId">スキーマID</param>
        public void DeleteDataSchema(string dataSchemaId)
        {
            // 削除しようとしているDataSchemeを使用しているApiが存在するか
            if (DataSchemaRepository.IsUsedFromApi(dataSchemaId))
            {
                throw new InUseException("指定されたモデルは使用されているため削除できません。");
            }

            // 削除しようとしているDataSchemeを使用しているControllerが存在するか
            if (DataSchemaRepository.IsUsedFromController(dataSchemaId))
            {
                throw new InUseException("指定されたモデルは使用されているため削除できません。");
            }

            var model = DataSchemaRepository.GetDataSchemaImformation(dataSchemaId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }

            DataSchemaRepository.DeleteDataSchema(dataSchemaId);
        }

        /// <summary>
        /// DataSchema名重複チェック
        /// </summary>
        /// <param name="schemaName">スキーマ名</param>
        /// <returns>true:登録済/false:未登録</returns>
        public bool ExistsSameSchemaName(string schemaName)
            => DataSchemaRepository.ExistsSameSchemaName(schemaName);
        #endregion

        #region OpenIdCa
        /// <summary>
        /// ApiAccessOpenIdを登録する
        /// </summary>
        /// <param name="apiId">メソッドID</param>
        /// <param name="openId">オープンID</param>
        /// <returns>apiAccessOpenIdInfo</returns>
        public ApiAccessOpenIdInfoModel RegisterApiAccessOpenId(string apiId, string openId)
        {
            if (string.IsNullOrEmpty(apiId) || !Guid.TryParse(apiId, out _))
            {
                throw new ArgumentException("Can not get methodId");
            }

            if (string.IsNullOrEmpty(openId) || !Guid.TryParse(openId, out _))
            {
                throw new ArgumentException("Can not get openId");
            }

            // 対象APIが有効か
            if (!DynamicApiRepository.ExistsEnableApi(apiId))
            {
                throw new NotFoundException();
            }

            // 更新時はApiAccessOpenIdに更新をかけないので
            // 登録用にApiAccessOpenIdを新規作成
            ApiAccessOpenIdInfoModel model = new()
            {
                ApiAccessOpenId = Guid.NewGuid().ToString(),
                ApiId = apiId,
                OpenId = openId
            };

            return DynamicApiRepository.RegisterApiAccessOpenId(model);
        }

        /// <summary>
        /// コントローラに紐づくOpenID認証局リストを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>OpenID認証局リスト</returns>
        public IEnumerable<OpenIdCaModel> GetControllerOpenIdCaList(string controllerId)
            => DynamicApiRepository.GetControllerOpenIdCaList(controllerId);


        /// <summary>
        /// ApiAccessOpenIdを削除します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="openId">openId</param>
        public void DeleteApiAccessOpenId(string apiId, string openId)
        {
            if (string.IsNullOrEmpty(apiId) || !Guid.TryParse(apiId, out _))
            {
                throw new ArgumentException("Can not get methodId");
            }

            if (string.IsNullOrEmpty(openId) || !Guid.TryParse(openId, out _))
            {
                throw new ArgumentException("Can not get openId");
            }

            var model = DynamicApiRepository.GetApi(apiId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }

            DynamicApiRepository.DeleteApiAccessOpenId(apiId, openId);
        }

        /// <summary>
        /// ApiのOpenIdアクセスコントロールの有効/無効を設定します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="isEnable">有効か</param>
        public void UseApiAccessOpenId(string apiId, bool isEnable)
        {
            if (string.IsNullOrEmpty(apiId) || !Guid.TryParse(apiId, out _))
            {
                throw new ArgumentException("Can not get methodId");
            }

            // 対象APIが有効か
            if (!DynamicApiRepository.ExistsEnableApi(apiId))
            {
                throw new NotFoundException();
            }

            DynamicApiRepository.UseApiAccessOpenId(apiId, isEnable);
        }

        #endregion
    }
}
