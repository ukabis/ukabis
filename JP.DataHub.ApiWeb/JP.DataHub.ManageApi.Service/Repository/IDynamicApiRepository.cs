using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.DymamicApi.Actions.AllApi;
using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IDynamicApiRepository
    {
        #region DynamicApiRepository
        /// <summary>
        /// DynamicAPIのカテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのカテゴリーの一覧</returns>
        IEnumerable<CategoryQueryModel> GetCategories();

        /// <summary>
        /// 分野の一覧を取得します。
        /// </summary>
        /// <returns>分野の一覧</returns>
        IList<FieldQueryModel> GetFields();

        /// <summary>
        /// タグの一覧を取得します。
        /// </summary>
        /// <returns>タグの一覧</returns>
        IEnumerable<TagQueryModel> GetTags();

        /// <summary>
        /// タグ情報リストを取得します。
        /// </summary>
        /// <returns>タグ情報リスト</returns>
        IList<TagInfoModel> GetTagInfoList();

        /// <summary>
        /// 指定されたベンダーIDのDynamicAPIのスキーマの一覧を取得します。
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
        /// <returns></returns>
        IList<LanguageModel> GetLanguageList();

        /// <summary>
        /// ScriptTypeリストを取得します。
        /// </summary>
        /// <returns></returns>
        IList<ScriptTypeModel> GetScriptTypeList();

        /// <summary>
        /// QueryTypeリストを取得します。
        /// </summary>
        /// <returns></returns>
        IList<QueryTypeModel> GetQueryTypeList();

        /// <summary>
        /// RepositoryGroupの一覧を取得します。
        /// </summary>
        /// <returns>RepositoryGroupの一覧</returns>
        IEnumerable<RepositoryGroupModel> GetRepositoryGroups();

        /// <summary>
        /// DynamicAPIの指定の添付ファイルストレージの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIの添付ファイルストレージの一覧</returns>
        IList<RepositoryGroupModel> GetAttachFileStorage(IEnumerable<string> repositoryTypeCd);

        /// <summary>
        /// コントローラに紐づくメソッドのプライマリまたはセカンダリのリポジトリグループに
        /// 「DataLakeStore」がある場合、かつ、「代表的なモデル」が未入力の場合はエラーとする
        /// </summary>
        /// <param name="controllerId">controllerId</param>
        /// <param name="dataSchemaId">dataSchemaId</param>
        /// <returns></returns>
        bool CheckDataLakeStoreMethod(string controllerId, string dataSchemaId);

        /// <summary>
        /// 対象のリポジトリグループIDのリポジトリタイプがDataLakeStoreと一致するか
        /// </summary>
        /// <param name="repositoryGroupIdList">リポジトリグループIDリスト</param>
        /// <returns>true:一致する/false:一致しない</returns>
        bool MatchDataLakeStoreId(IEnumerable<string> repositoryGroupIdList);

        /// <summary>
        /// 有効なベンダー/システムの一覧を取得します。
        /// ベンダーIDが指定された場合は、指定されたベンダーのみ取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>有効なベンダー/システムの一覧</returns>
        IEnumerable<VendorNameSystemNameModel> GetVendorList(string? vendorId = null);

        /// <summary>
        /// ベンダーが存在するか
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>true:存在する/false:存在しない</returns>
        bool ExistsVendor(string vendorId);

        /// <summary>
        /// APIで使用中のVendorSystemの組み合わせが正しいかどうか
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="systemId">システムID</param>
        /// <returns>true:正しい/false:正しくない</returns>
        bool CheckVendorSystemCombination(string vendorId, string systemId);

        /// <summary>
        /// ベンダーに紐づくOpenID認証局リストを取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>OpenID認証局リスト</returns>
        IList<OpenIdCaModel> GetVendorOpenIdCaList(string vendorId);

        /// <summary>
        /// コントローラーに紐づくOpenID認証局リストを取得します。（OUTER JOIN）
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>OpenID認証局リスト</returns>
        IList<OpenIdCaModel> GetControllerOpenIdCaList(string controllerId);

        /// <summary>
        /// ControllerIdに紐づくOpenID認証局リストを取得します。（INNER JOIN）
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>OpenIdCA</returns>
        IEnumerable<OpenIdCaModel> GetControllerOpenIdCAList(string controllerId);

        /// <summary>
        /// APIに紐づくOpenID認証局リストを取得します。（OUTER JOIN）
        /// </summary>
        /// <param name="apiId">API ID</param>
        /// <returns>OpenID認証局リスト</returns>
        IList<OpenIdCaModel> GetApiOpenIdCaList(string apiId);

        /// <summary>
        /// APIに紐づくOpenID認証局リストを取得します。（INNER JOIN）
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>ApiOpenIdCA</returns>
        IEnumerable<OpenIdCaModel> GetApiOpenIdCAList(string apiId);

        /// <summary>
        /// ベンダーに紐づくOpenID認証局リストを更新します。
        /// </summary>
        /// <param name="model">OpenID認証局リスト</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>OpenID認証局リスト</returns>
        OpenIdCaModel UpsertVendorOpenIdCa(OpenIdCaModel model, string vendorId);

        /// <summary>
        /// コントローラーに紐づくOpenID認証局リストを更新します。
        /// </summary>
        /// <param name="model">OpenID認証局リスト</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>OpenID認証局リスト</returns>
        OpenIdCaModel UpsertControllerOpenIdCa(OpenIdCaModel model, string controllerId);

        /// <summary>
        /// Apiに紐づくOpenID認証局リストを更新します。
        /// </summary>
        /// <param name="model">OpenID認証局リスト</param>
        /// <param name="apiId">API ID</param>
        /// <returns>OpenID認証局リスト</returns>
        OpenIdCaModel UpsertApiOpenIdCa(OpenIdCaModel model, string apiId);

        #region APIDescription

        /// <summary>
        /// ベンダーのリンク情報の一覧を取得します。
        /// </summary>
        /// <returns>ベンダーのリンク情報</returns>
        IEnumerable<VendorLinkModel> GetVendorLink();

        /// <summary>
        /// システムのリンク情報の一覧を取得します。
        /// </summary>
        /// <returns>システムのリンク情報</returns>
        IEnumerable<SystemLinkModel> GetSystemLink();

        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">未削除のみ</param>
        /// <param name="isEnableOnly">有効のみ</param>
        /// <param name="isNotHiddenOnly">公開のみ</param>
        /// <returns>API情報の一覧</returns>
        IEnumerable<ApiDescriptionModel> GetApiDescription(bool noChildren, string localeCode = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false);

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <param name="localeCode">ロケール</param>
        /// <returns>スキーマ情報の一覧</returns>
        IEnumerable<SchemaDescriptionModel> GetSchemaDescription(string localeCode = null);

        /// <summary>
        /// カテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>カテゴリーの一覧</returns>
        IEnumerable<CategoryModel> GetCategoryList(string localeCode = null);
        #endregion
        #endregion

        #region DynamicApiRepository_Controller
        /// <summary>
        /// DynamicAPIのリソースのリストを取得する
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="isAll">全てのベンダーのリソースを取得する</param>
        /// <returns>リソースのリスト</returns>
        IEnumerable<ControllerQueryModel> GetControllerApiList(string vendorId, bool isAll);

        /// <summary>
        /// DynamicAPIのリソースのリストを取得する（内容はシンプルなもの、管理画面のツリーを表示するための項目のみ）
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="isAll">全てのベンダーのリソースを取得する</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソースのリスト</returns>
        IEnumerable<ControllerSimpleQueryModel> GetControllerApiSimpleList(string vendorId, bool isAll, bool isTransparent);

        /// <summary>
        /// コントローラ情報を取得します。
        /// </summary>
        /// <param name="controllerId">コントローラId</param>
        /// <param name="staticApiOnly">StaticAPIのみ取得</param>
        /// <returns></returns>
        ControllerInformationModel GetController(string controllerId, bool staticApiOnly = false);

        /// <summary>
        /// ControllerIdに紐づくリソースを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        ControllerQueryModel GetControllerResourceFromControllerId(string controllerId, bool isTransparent);

        /// <summary>
        /// ControllerIdに紐づくリソースを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>リソース</returns>
        ControllerLightQueryModel GetControllerResourceLight(string controllerId);

        /// <summary>
        /// URLに紐づくリソースを取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        ControllerQueryModel GetControllerResourceFromUrl(string url, bool isTransparent);

        /// <summary>
        /// コントローラーのURLを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラーのID</param>
        /// <returns>URL</returns>
        string GetControllerUrl(string controllerId);

        /// <summary>
        /// URLが一致するAPIが存在するか
        /// </summary>
        /// <param name="outApiId">APIのID</param>
        /// <param name="controllerId">ContorollerId</param>
        /// <param name="apiUrl">APIの相対URL</param>
        /// <returns>true:存在する/false:存在しない</returns>
        bool IsDuplicateUrl(out string outApiId, string controllerId, string apiUrl);

        /// <summary>
        /// URL（相対）が一致するコントローラが存在するか
        /// </summary>
        /// <param name="url">コントローラURL</param>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>true:存在する/false:存在しない</returns>
        bool IsDuplicateController(string url, out string controllerId);

        /// <summary>
        /// コントローラの存在確認をします。
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="vendorId"></param>
        /// <returns>bool</returns>
        bool IsExists(string controllerId, string vendorId = null);

        /// <summary>
        /// コントローラを登録または更新します。
        /// </summary>
        /// <param name="model">コントローラモデル</param>
        /// <returns>コントローラモデル</returns>
        ControllerInformationModel UpsertController(ControllerInformationModel model);

        /// <summary>
        /// コントローラ情報を削除します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        void DeleteController(string controllerId);

        /// <summary>
        /// URLをもとにコントローラ情報を削除します。
        /// </summary>
        /// <param name="url">URL</param>
        void DeleteControllerFromUrl(string url, string controllerId);

        /// <summary>
        /// ControllerIdに紐づくContollerCategoryリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>ContollerCategoryリスト</returns>
        IEnumerable<ControllerCategoryInfomationModel> GetContollerCategoryList(string controllerId);

        /// <summary>
        /// ControllerIdに紐づくContollerCategoryリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>ContollerCategoryリスト</returns>
        IList<ControllerCategoryInfomationModel> GetContollerCategoryListContainControllerIdNull(string controllerId);

        /// <summary>
        /// コントローラカテゴリーリストを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="controllerCategoryInfoList">コントローラカテゴリーリスト</param>
        /// <returns>コントローラカテゴリーリストモデル</returns>
        IList<ControllerCategoryInfomationModel> UpsertControllerCategoryList(string controllerId, IList<ControllerCategoryInfomationModel> controllerCategoryInfoLis);

        /// <summary>
        /// ControllerIdに紐づくCommonIpFilterGroupリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>CommonIpFilterGroupリスト</returns>
        IEnumerable<ControllerCommonIpFilterGroupModel> GetControllerCommonIpFilterGroupList(string controllerId);

        /// <summary>
        /// ControllerIdに紐づくCommonIpFilterGroupリストを取得します。
        /// （コントローラIDがNULLのものも含む）
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>CommonIpFilterGroupリスト</returns>
        IEnumerable<ControllerCommonIpFilterGroupModel> GetControllerCommonIpFilterGroupListContainControllerIdNull(string controllerId);

        /// <summary>
        /// ControllerCommonIpFilterGroupを一括登録または更新します。
        /// </summary>
        /// <param name="modelList">ControllerCommonIpFilterGroupModel</param>
        /// <param name="controllerId">コントローラID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        List<ControllerCommonIpFilterGroupModel> UpsertControllerCommonIpFilterGroupList(List<ControllerCommonIpFilterGroupModel> modelList, string controllerId);

        /// <summary>
        /// ControllerFieldリストを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>ControllerFieldリスト</returns>
        IEnumerable<ControllerFieldInfoModel> GetControllerFieldList(string controllerId);

        /// <summary>
        /// ControllerIpFilterを登録または更新します。
        /// </summary>
        /// <param name="modelList">ControllerIpFilterModel</param>
        /// <param name="controllerId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        List<ControllerIpFilterModel> UpsertControllerIpFilter(List<ControllerIpFilterModel> modelList, string controllerId);

        /// <summary>
        /// ControllerTagリストを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>ControllerTagリスト</returns>
        IEnumerable<ControllerTagInfoModel> GetControllerTagList(string controllerId);

        /// <summary>
        /// ControllerTagを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="modelList">ControllerTagInfoModelリスト</param>
        /// <returns>ControllerTagInfoModelリスト</returns>
        List<ControllerTagInfoModel> UpsertControllerTagInfoList(string controllerId, List<ControllerTagInfoModel> modelList);

        /// <summary>
        /// ControllerFieldを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="modelList">ControllerFieldInfoLリスト</param>
        /// <returns>ControllerFieldInfoLリスト</returns>
        List<ControllerFieldInfoModel> UpsertControllerFieldInfoList(string controllerId, List<ControllerFieldInfoModel> modelList);

        /// <summary>
        /// ControllerMultiLanguageを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="modelList"></param>
        /// <returns></returns>
        List<ControllerMultiLanguageModel> RegisterControllerMultiLanguage(string controllerId, List<ControllerMultiLanguageModel> modelList);

        /// <summary>
        /// 添付ファイル設定を取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>添付ファイル設定</returns>
        AttachFileSettingsModel GetAttachFileSettings(string controllerId);

        /// <summary>
        /// 履歴ドキュメント設定を取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>履歴ドキュメント設定</returns>
        DocumentHistorySettingsModel GetDocumentHistorySettings(string controllerId);

        /// <summary>
        /// コントローラーとAPIを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>コントローラモデル</returns>
        List<AllApiModel> GetControllerApi(string controllerId);
        #endregion

        #region DynamicApiRepository_API

        /// <summary>
        /// API情報をすべて取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>API情報</returns>
        ApiInformationModel GetAllFieldApiInformation(string apiId);

        /// <summary>
        /// API情報を取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>APIモデル</returns>
        ApiInformationModel GetApiInformation(string apiId);

        /// <summary>
        /// apiIdに紐づくAPIを取得します。
        /// </summary>
        /// <returns>HttpMethodTypeの一覧</returns>
        ApiQueryModel GetApi(string apiId);

        /// <summary>
        /// URLに紐づくAPIを取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>API</returns>
        ApiQueryModel GetApiFromUrl(string controllerUrl, string apiUrl);

        /// <summary>
        /// メソッドタイプ一致かつコントローラーURL前方一致のAPIを取得します。
        /// </summary>
        /// <param name="httpMethodType">メソッドタイプ</param>
        /// <param name="controllerUrl">コントローラーURL</param>
        /// <returns>重複API有無</returns>
        IEnumerable<ApiUrlIdentifier> GetApiUrlIdentifier(HttpMethodType httpMethodType, string controllerUrl);

        /// <summary>
        /// ApiIdに紐づく有効なAPIが存在するかを返します。
        /// </summary>
        /// <returns>true:存在する/false:存在しない</returns>
        bool ExistsEnableApi(string apiId);

        /// <summary>
        /// APIが実行可能か取得します。
        /// </summary>
        /// <param name="actionTypeCd">アクションタイプコード</param>
        /// <param name="httpMethodTypeCd">メソッドタイプコード</param>
        /// <param name="repositoryGroupId">リポジトリグループID</param>
        /// <returns></returns>
        bool IsExcuseApiCombinationConstraints(ActionType actionTypeCd, HttpMethodType httpMethodTypeCd, string repositoryGroupId = null);

        /// <summary>
        /// APIを登録または更新します。
        /// </summary>
        /// <param name="api">APIモデル</param>
        /// <returns>APIモデル</returns>
        ApiInformationModel UpsertApi(ApiInformationModel api);

        /// <summary>
        /// APIを削除します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        void DeleteApi(string apiId);

        /// <summary>
        /// URL紐づくAPIを削除します。
        /// </summary>
        /// <param name="controllerUrl">コントローラURL</param>
        /// <param name="apiUrl">APIURL</param>
        void DeleteApiFromUrl(string controllerUrl, string apiUrl, string apiId);

        /// <summary>
        /// APIアクセスベンダーを登録または更新します。
        /// </summary>
        /// <param name="modelList">APIアクセスベンダーモデルリスト</param>
        /// <param name="apiId">APIID</param>
        /// <returns>APIアクセスベンダーモデル</returns>
        List<ApiAccessVendorModel> UpsertApiAccessVendor(List<ApiAccessVendorModel> modelList, string apiId);

        /// <summary>
        /// ApiLinkを登録または更新します。
        /// </summary>
        /// <param name="model">APIリンクモデル</param>
        /// <param name="apiId">APIID</param>
        /// <returns>APIリンクモデル</returns>
        /// <exception cref="ArgumentNullException"></exception>
        List<ApiLinkModel> UpsertApiLink(List<ApiLinkModel> modelList, string apiId);

        /// <summary>
        /// サンプルコードを登録または更新します。
        /// </summary>
        /// <param name="model">サンプルコードモデル</param>
        /// <param name="apiId">APIID</param>
        /// <returns>サンプルコードモデル</returns>
        List<SampleCodeModel> UpsertSampleCode(List<SampleCodeModel> modelList, string apiId);

        /// <summary>
        /// セカンダリリポジトリが重複しているか
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="AlreadyExistsException"></exception>
        bool IsDuplicateSecondaryRepositoryMap(IEnumerable<SecondaryRepositoryMapModel> model);

        /// <summary>
        /// ApiIDに紐づくSecondaryRepositoryMapリストを取得します。
        /// </summary>
        /// <param name="apiId">ApiID</param>
        /// <param name="vendorId">VendorID</param>
        /// <returns>SecondaryRepositoryMapリスト</returns>
        IList<SecondaryRepositoryMapModel> GetSecondaryRepositoryMapList(string apiId, string vendorId);

        /// <summary>
        /// コンテナ分離対応されたセカンダリリポジトリを持っているか
        /// </summary>
        /// <param name="apiId">ApiID</param>
        /// <param name="vendorId">VendorID</param>
        /// <returns>SecondaryRepositoryMapリスト</returns>
        bool HasContainerDynamicSeparationSecondaryRepositoryMap(string apiId, string vendorId, string repositoryGroupId);

        /// <summary>
        /// SecondaryRepositoryMapListを登録または更新します。
        /// </summary>
        /// <param name="modelList">セカンダリーリポジトリマップモデル</param>
        /// <param name="apiId">APIID</param>
        /// <returns>セカンダリーリポジトリマップモデル</returns>
        IEnumerable<SecondaryRepositoryMapModel> UpsertSecondaryRepositoryMapList(IEnumerable<SecondaryRepositoryMapModel> modelList, string apiId);

        /// <summary>
        /// ApiIdに紐づくSecondaryRepositoryMapをすべて削除します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        void DeleteSecondaryRepositoryMap(string apiId);

        /// <summary>
        /// StaticApiの一覧を取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns></returns>
        IEnumerable<StaticApiId> GetStaticApiList(string controllerId = null);

        /// <summary>
        /// ApiMultiLanguageを登録または更新します。
        /// </summary>
        /// <param name="model">ApiMultiLanguageModel</param>
        /// <param name="apiId">APIID</param>
        /// <returns>ApiMultiLanguageModelリスト</returns>
        List<ApiMultiLanguageModel> UpsertApiMultiLanguage(List<ApiMultiLanguageModel> model, string apiId);

        /// <summary>
        /// ApiIdとOpenIdに紐づく有効なApiAccessOpenIdを一件取得します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="openId">openId</param>
        /// <param name="isActiveOnly">有効なもののみ取得するかどうか</param>
        /// <returns>ApiAccessOpenId</returns>
        ApiAccessOpenIdInfoModel GetApiAccessOpenId(string apiId, string openId, bool isActiveOnly = true);

        /// <summary>
        /// ApiAccessOpenIdを登録します。
        /// </summary>
        /// <param name="model">ApiAccessOpenIdInfoModel</param>
        /// <returns>ApiAccessOpenIdInfoModel</returns>
        ApiAccessOpenIdInfoModel RegisterApiAccessOpenId(ApiAccessOpenIdInfoModel apiAccessOpenId);

        /// <summary>
        /// ApiAccessOpenIdを削除します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="openId">openId</param>
        void DeleteApiAccessOpenId(string apiId, string openId);

        /// <summary>
        /// ApiのOpenIdアクセスコントロールの有効/無効を設定します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="isEnable">有効か</param>
        void UseApiAccessOpenId(string apiId, bool isEnable);

        /// <summary>
        /// パラメータを取得します。
        /// </summary>
        /// <param name="parameters">パラメータ</param>
        /// <param name="isQuery"></param>
        /// <returns></returns>
        IEnumerable<string> GetParameters(IEnumerable<string> parameters);
        #endregion
    }
}
