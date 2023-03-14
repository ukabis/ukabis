using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Attributes;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    internal interface IDynamicApiService
    {
        #region マスター取得系
        /// <summary>
        /// DynamicAPIのカテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのカテゴリーの一覧</returns>
        IEnumerable<CategoryQueryModel> GetCategories();

        /// <summary>
        /// 分野の一覧を取得します。
        /// </summary>
        /// <returns>分野の一覧</returns>
        IEnumerable<FieldQueryModel> GetFields();

        /// <summary>
        /// タグの一覧を取得します。
        /// </summary>
        /// <returns>タグの一覧</returns>
        IEnumerable<TagQueryModel> GetTags();

        /// <summary>
        /// RepositoryGroupの一覧を取得します。
        /// </summary>
        /// <returns>RepositoryGroupの一覧</returns>
        IEnumerable<RepositoryGroupModel> GetRepositoryGroups();

        /// <summary>
        /// DynamicAPIの指定の添付ファイルストレージの一覧を取得します。
        /// </summary>
        /// <param name="repositoryTypeCd">リポジトリタイプコード</param>
        /// <returns>DynamicAPIの添付ファイルストレージの一覧</returns>
        IEnumerable<RepositoryGroupModel> GetAttachFileStorage(IEnumerable<string> repositoryTypeCd);

        /// <summary>
        /// ActionTypeの一覧を取得します。
        /// </summary>
        /// <returns>ActionTypeの一覧</returns>
        IEnumerable<ActionTypeModel> GetActionTypes();

        /// <summary>
        /// HttpMethodTypeの一覧を取得します。
        /// </summary>
        /// <returns>HttpMethodTypeの一覧</returns>
        IEnumerable<HttpMethodTypeModel> GetHttpMethodTypes();

        /// <summary>
        /// Languageリストを取得します。
        /// </summary>
        /// <returns>Languageリスト</returns>
        IEnumerable<LanguageModel> GetLanguageList();

        /// <summary>
        /// ScriptTypeリストを取得します。
        /// </summary>
        /// <returns>ScriptTypeリスト</returns>
        IEnumerable<ScriptTypeModel> GetScriptTypeList();

        /// <summary>
        /// QueryTypeリストを取得します。
        /// </summary>
        /// <returns>QueryTypeリスト</returns>
        IEnumerable<QueryTypeModel> GetQueryTypeList();

        /// <summary>
        /// ControllerIdに紐づくControllerCommonIpFilterGroupリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>ControllerCommonIpFilterGroupリスト</returns>
        IEnumerable<ControllerCommonIpFilterGroupModel> GetControllerCommonIpFilterGroup(string controllerId);
        #endregion

        #region Controller
        /// <summary>
        /// ベンダーに紐づくのコントローラとAPI情報のリストを取得します。
        /// </summary>
        /// <param name="isAll">全てのベンダーのリソースを取得するか</param>
        /// <returns>リソースのリスト</returns>
        IEnumerable<ControllerQueryModel> GetControllerApiList(bool isAll);

        /// <summary>
        /// ベンダーに紐づくのコントローラとAPI情報のリストを取得します。（内容はシンプルなもの、管理画面のツリーを表示するための項目のみ）
        /// </summary>
        /// <param name="vendorId">ベンダーID。</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>
        /// ベンダーに紐づくリソースのリスト。
        /// <paramref name="vendorId"/>がnullの場合、運用管理ベンダーであれば全ベンダーのリソースのリスト、それ以外のベンダーは自ベンダーのリソースのリスト。
        /// </returns>
        IEnumerable<ControllerSimpleQueryModel> GetControllerApiSimpleList(string vendorId = null, bool isTransparent = true);

        /// <summary>
        /// ControllerIdに紐づくコントローラ情報を取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        ControllerQueryModel GetControllerResourceFromControllerId(string controllerId, bool isTransparent);

        /// <summary>
        /// ControllerIdに紐づくコントローラ情報を取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>リソース</returns>
        ControllerLightQueryModel GetControllerResourceLight(string controllerId);

        /// <summary>
        /// URLに紐づくコントローラ情報を取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        ControllerQueryModel GetControllerResourceFromUrl(string url, bool isTransparent);

        /// <summary>
        /// コントローラを登録または更新します。
        /// </summary>
        /// <param name="model">コントローラモデル</param>
        /// <returns>コントローラモデル</returns>
        ControllerInformationModel RegistOrUpdateController(ControllerInformationModel model);

        /// <summary>
        /// コントローラを削除します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        void DeleteController(string controllerId);

        /// <summary>
        /// URLに紐づくコントローラを削除します。
        /// </summary>
        /// <param name="url">URL</param>
        void DeleteControllerFromUrl(string url);

        /// <summary>
        /// コントローラが重複しているか
        /// </summary>
        /// <param name="controller">コントローラ</param>
        /// <returns>true:存在する/false:存在しない</returns>
        bool IsDuplicateController(ControllerInformationModel controller, out string controllerId);

        /// <summary>
        /// Controller設定情報取得
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="controllerId"></param>
        /// <returns></returns>
        ControllerHeaderModel GetControllerHeader(string vendorId, string controllerId);
        #endregion

        #region API
        /// <summary>
        /// apiIdに紐づくAPIを取得します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        /// <returns>API</returns>
        ApiQueryModel GetApi(string apiId);

        /// <summary>
        /// URLに紐づくAPIを取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>API</returns>
        ApiQueryModel GetApiFromUrl(string url);

        /// <summary>
        /// APIを登録または更新します。
        /// </summary>
        /// <param name="model">APIモデル</param>
        /// <param name="IsTransparentApi">透過APIか</param>
        /// <returns>APIモデル</returns>
        ApiInformationModel RegistOrUpdateApi(ApiInformationModel model, bool IsTransparentApi = false);

        /// <summary>
        /// APIを削除します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        void DeleteApi(string apiId);

        /// <summary>
        /// URLに紐づくAPIを削除します。
        /// </summary>
        /// <param name="url">URL</param>
        void DeleteApiFromUrl(string url);

        /// <summary>
        /// 指定したAPIが重複しているかを判定する。
        /// </summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="httpMethodType">メソッドタイプ</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="apiUrl">API(メソッド)のURL</param>
        /// <param name="apiId">API(メソッド)のID。指定した場合は指定API以外との重複があるかどうかを判定する。</param>
        /// <returns>重複している場合はtrue、それ以外はfalse。</returns>
        bool IsDuplicateApi(string actionType, string httpMethodType, string controllerId, string apiUrl, string apiId = null);

        /// <summary>
        /// アクションタイプ、メソッドタイプ、リポジトリグループの組み合わせでAPIが実行可能かを判定する。
        /// </summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="methodType">メソッドタイプ</param>
        /// <param name="repositoryGroupId">リポジトリグループID</param>
        /// <returns>APIが実行可能な場合はtrue、それ以外はfalse。</returns>
        bool IsExcuseApiCombinationConstraints(string actionType, string methodType, string repositoryGroupId);

        /// <summary>
        /// スクリプト構文チェックメッセージを取得します。
        /// </summary>
        /// <param name="script">スクリプト</param>
        /// <param name="scriptType">スクリプトタイプ</param>
        /// <returns>メッセージ</returns>
        string GetScriptSyntaxCheckMessage(string script, string scriptType);
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
        IEnumerable<SchemaModel> GetSchemas(string vendorId);

        /// <summary>
        /// 指定されたスキーマIDのDynamicAPIのスキーマの一件を取得します。
        /// </summary>
        /// <param name="schemaId">スキーマID</param>
        /// <returns>DynamicAPIのスキーマ</returns>
        SchemaModel GetSchemaById(string schemaId);

        /// <summary>
        /// スキーマを登録または更新します。
        /// </summary>
        /// <param name="model">スキーマモデル</param>
        /// <param name="isUriOrResponse">URIモデルまたはResponseモデルか？</param>
        /// <returns>スキーマモデル</returns>
        DataSchemaInformationModel RegistOrUpdateSchema(DataSchemaInformationModel model, bool isUriOrResponse);

        /// <summary>
        /// スキーマを削除します。
        /// </summary>
        /// <param name="dataSchemaId">スキーマID</param>
        void DeleteDataSchema(string dataSchemaId);

        /// <summary>
        /// DataSchema名重複チェック
        /// </summary>
        /// <param name="schemaName">スキーマ名</param>
        /// <returns>true:登録済/false:未登録</returns>
        bool ExistsSameSchemaName(string schemaName);
        #endregion

        #region OpenIdCa
        /// <summary>
        /// コントローラに紐づくOpenID認証局リストを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>OpenID認証局リスト</returns>
        IEnumerable<OpenIdCaModel> GetControllerOpenIdCaList(string controllerId);

        /// <summary>
        /// ApiAccessOpenIdを登録する
        /// </summary>
        /// <param name="apiAccessOpenIdInfo">apiAccessOpenIdInfo</param>
        /// <returns>apiAccessOpenIdInfo</returns>
        ApiAccessOpenIdInfoModel RegisterApiAccessOpenId(string methodId, string openId);

        /// <summary>
        /// ApiAccessOpenIdを削除する。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="openId">openId</param>
        void DeleteApiAccessOpenId(string apiId, string openId);

        /// <summary>
        /// ApiのOpenIdアクセスコントロールの有効/無効を設定する。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="isEnable">有効な場合true、無効な場合falseを指定する</param>
        void UseApiAccessOpenId(string apiId, bool isEnable);

        #endregion

    }
}
