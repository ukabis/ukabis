using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IDynamicApiRepository
    {
        IMethod FindApi(HttpMethodType httpMethodType, RequestRelativeUri requestRelativeUri, GetQuery getQuery = null);

        IMethod FindApiForGetExecuteApiInfo(ControllerId controllerId, ApiId apiId, DataId dataId, Contents contents, QueryStringVO queryString, UrlParameter keyValue);

        IMethod FindEnumerableApi(HttpMethodType httpMethodType, RequestRelativeUri requestRelativeUri, GetQuery getQuery = null);

        IEnumerable<IpAddress> GetIpFilter(VendorId vendorId, SystemId systemId, ControllerId controllerId, ApiId apiId);

        ApiAccessVendor GetApiAccessVendor(VendorId vendorId, SystemId systemId, ControllerId controllerId, ApiId apiId, VendorId targetVendorId, SystemId targetSystemId);


        /// <summary>
        /// OpenId認証を許可するアプリケーションの一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="apiId">アプリケーションID</param>
        /// <returns>OpenId認証を許可するアプリケーションの一覧</returns>
        IEnumerable<OpenIdAllowedApplication> GetOpenIdAllowedApplications(VendorId vendorId, ControllerId controllerId, ApiId apiId);

        /// <summary>
        /// 指定されたAPI、ベンダーにメールテンプレートが設定されているか返します。
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>メールテンプレートが設定されているかどうか</returns>
        bool HasMailTemplate(ControllerId controllerId, VendorId vendorId);

        /// <summary>
        /// 指定されたAPI、ベンダーにWebhookが設定されているか返します。
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>Webhookが設定されているかどうか</returns>
        bool HasWebhook(ControllerId controllerId, VendorId vendorId);

        /// <summary>
        /// 指定されたAPIにopenidで指定した人のアクセスが許可されているか返します
        /// </summary>
        /// <param name="apiId">APIのID</param>
        /// <param name="openid">人のopenid</param>
        /// <returns></returns>
        bool HasApiAccessOpenid(ApiId apiId, OpenId openid);

        /// <summary>
        /// 指定された時間のデータが存在するブロックチェーンノードを取得する
        /// 引数がない場合は最新のブロックチェーンノードを取得する
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        RepositoryInfo GetBlockchainNodeRepositoryInfoByTimeStamp(DateTime? timestamp = null);

        /// <summary>
        /// ベンダー情報の取得
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>ベンダー情報</returns>
        VendorVO GetVendor(VendorId vendorId);

        /// <summary>
        /// 呼び出しベンダー、システムが、同意して承認を得られているか？
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="controllerId"></param>
        /// <returns></returns>
        bool IsApprovedAgreement(VendorId vendorId, ControllerId controllerId);

        /// <summary>
        /// 指定されたリソースの物理リポジトリを取得する。
        /// </summary>
        PhysicalRepositoryId GetPhysicalRepositoryIdByControllerId(ControllerId controllerId, RepositoryType repositoryType);

        /// <summary>
        /// リソースURLからモデル情報を取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns>データモデル</returns>
        DataSchema GetControllerSchemaByUrl(ControllerUrl url);

        /// <summary>
        /// データモデルIDからデータモデルを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataSchema GetSchemaModelById(DataSchemaId id);

        /// <summary>
        /// データモデル名からデータモデルを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        DataSchema GetSchemaModelByName(DataSchemaName name);

        Task RefreshStaticCache(string time);

        void CheckStaticCacheTime();

        /// <summary>
        /// 規約に同意しているか
        /// </summary>
        /// <param name="termsGroupCode"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        bool IsAgreeToTerms(TermsGroupCode termsGroupCode, OpenId openid);

        /// <summary>
        /// 自身に対して共有されているOpenIdを取得
        /// </summary>
        /// <param name="resourceGroupId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        IEnumerable<string> GetResourceSharedOpenId(ResourceGroupId resourceGroupId, OpenId openid);
    }
}