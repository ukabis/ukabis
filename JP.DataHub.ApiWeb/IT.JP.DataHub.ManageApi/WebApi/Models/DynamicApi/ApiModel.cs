using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class RegisterApiResponseModel
    {
        public string ApiId { get; set; }
    }
    public class RegisterApiRequestModel
    {
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string ApiName { get; set; }
        public string Url { get; set; }
        public string ApiDescriptiveText { get; set; }
        public bool IsVendor { get; set; }
        public bool IsPerson { get; set; }
        public bool IsEnable { get; set; }
        public string ModelId { get; set; }
        public string RepositoryKey { get; set; }
        public string PartitionKey { get; set; }
        public bool IsStaticApi { get; set; }
        public List<RegisterApiTagModel> ApiTagInfoList { get; set; }
        public List<RegisterApiCategoryModel> CategoryList { get; set; }
        public List<RegisterApiFieldModel> ApiFieldInfoList { get; set; }

        public bool IsData { get; set; }
        public bool IsBusinessLogic { get; set; }
        public bool IsPay { get; set; }
        public string FeeDescription { get; set; }
        public string ResourceCreateUser { get; set; }
        public string ResourceMaintainer { get; set; }
        public string ResourceCreateDate { get; set; }
        public string ResourceLatestDate { get; set; }
        public string UpdateFrequency { get; set; }
        public bool IsContract { get; set; }
        public string ContactInformation { get; set; }
        public string Version { get; set; }
        public string AgreeDescription { get; set; }
        public DynamicApiAttachFileSettingsModel AttachFileSettings { get; set; }
        public DocumentHistorySettingsModel DocumentHistorySettings { get; set; }
        public bool IsEnableBlockchain { get; set; }
        public bool IsOptimisticConcurrency { get; set; }
        public bool IsEnableIpFilter { get; set; }
        public List<RegisterApiCommonIpFilterGroupModel> ApiCommonIpFilterGroupList { get; set; }
        public List<RegisterApiIpFilterModel> ApiIpFilterList { get; set; }
        public List<RegisterResourceOpenIdCaModel> OpenIdCaList { get; set; }
        public bool IsUseBlobCache { get; set; }
        public bool IsVisibleAgreement { get; set; }
        public bool IsEnableResourceVersion { get; set; } = true;
    }
    public class RegisterApiTagModel
    {
        public string TagId { get; set; }
    }
    public class RegisterApiCategoryModel
    {
        public string CategoryId { get; set; }
    }
    public class RegisterApiFieldModel
    {
        public string FieldId { get; set; }
    }
    public class RegisterApiIpFilterModel
    {
        public string IpAddress { get; set; }
    }
    public class RegisterResourceOpenIdCaModel
    {
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public bool IsActive { get; set; }
        public string AccessControl { get; set; }
    }
    public class DynamicApiAttachFileSettingsModel
    {
        public bool IsEnable { get; set; } = false;
        public string MetaRepositoryId { get; set; }
        public string BlobRepositoryId { get; set; }
    }
    public class ApiModel
    {        /// <summary>
             /// ApiId
             /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Apiの名前
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Apiの相対パス
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Apiの説明
        /// </summary>
        public string ApiDescription { get; set; }

        /// <summary>
        /// このAPIを管理するベンダー
        /// </summary>
        public bool IsVendor { get; set; }

        /// <summary>
        /// 個人依存か
        /// </summary>
        public bool IsPerson { get; set; }

        /// <summary>
        /// StaticApiか
        /// </summary>
        public bool IsStaticApi { get; set; }

        /// <summary>
        /// Topページに表示させるか
        /// </summary>
        public bool IsTopPage { get; set; }

        /// <summary>
        /// ApiのスキーマーID
        /// </summary>
        public string ApiSchemaId { get; set; }

        /// <summary>
        /// Apiのスキーマー名
        /// </summary>
        public string ApiSchemaName { get; set; }

        /// <summary>
        /// レポジトリーキー
        /// </summary>
        public string RepositoryKey { get; set; }

        /// <summary>
        /// パーティションキー
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// データか
        /// </summary>
        public bool IsData { get; set; }

        /// <summary>
        /// ビジネスロジックか
        /// </summary>
        public bool IsBusinessLogic { get; set; }

        /// <summary>
        /// 有料か
        /// </summary>
        public bool IsPay { get; set; }

        /// <summary>
        /// 使用料
        /// </summary>
        public string FeeDescription { get; set; }

        /// <summary>
        /// リソース作成者
        /// </summary>
        public string ResourceCreateUser { get; set; }

        /// <summary>
        /// メンテナー
        /// </summary>
        public string ResourceMaintainer { get; set; }

        /// <summary>
        /// リソース作成日
        /// </summary>
        public DateTime ResourceCreateDate { get; set; }

        /// <summary>
        /// リソース最終更新日
        /// </summary>
        public DateTime ResourceLatestDate { get; set; }

        /// <summary>
        /// 更新頻度
        /// </summary>
        public string UpdateFrequency { get; set; }

        /// <summary>
        /// 契約が必要か
        /// </summary>
        public bool IsContract { get; set; }

        /// <summary>
        /// 連絡先
        /// </summary>
        public string ContactInfomation { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 利用規約
        /// </summary>
        public string AgreeDescription { get; set; }

        /// <summary>
        /// 規約に同意ボタンを表示するか
        /// </summary>
        public bool IsVisibleAgreement { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 論理削除されているか
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 登録者
        /// </summary>
        public string RegUserName { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdUserName { get; set; }

        /// <summary>
        /// 登録日付
        /// </summary>
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 更新日付
        /// </summary>
        public DateTime UpdDate { get; set; }

        /// <summary>
        /// タグリスト
        /// </summary>
        public IEnumerable<ApiTagModel> TagList { get; set; }

        /// <summary>
        /// カテゴリーリスト
        /// </summary>
        public IEnumerable<ApiCategoryModel> CategoryList { get; set; }

        /// <summary>
        /// 分野リスト
        /// </summary>
        public IEnumerable<ApiFieldModel> FieldList { get; set; }

        /// <summary>
        /// 楽観排他を使用するかどうか
        /// </summary>
        public bool IsOptimisticConcurrency { get; set; }

        /// <summary>
        /// IPフィルタグループリスト
        /// </summary>
        public IEnumerable<ApiCommonIpFilterGroupModel> CommonIpFilterGroupList { get; set; }

        /// <summary>
        /// IPフィルタリスト
        /// </summary>
        public IEnumerable<ApiIpFilterModel> IpFilterList { get; set; }

        /// <summary>
        /// OpenId認証局リスト
        /// </summary>
        public IEnumerable<ApiOpenIdCAModel> OpenIdCAList { get; set; }

        /// <summary>
        /// Methodリスト
        /// </summary>
        public IEnumerable<MethodModel> MethodList { get; set; }

        /// <summary>
        /// DynamicAPIの添付ファイル設定を有効にするかどうか
        /// </summary>
        public bool IsEnableAttachFile { get; set; }

        /// <summary>
        /// DynamicAPIとして管理するデータの履歴を残すかどうか
        /// </summary>
        public bool IsDocumentHistory { get; set; }

        /// <summary>
        /// ブロックチェーンを使用するかどうか
        /// </summary>
        public bool IsEnableBlockchain { get; set; }
        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public bool IsEnableResourceVersion { get; set; }


    }
    public class ApiTagModel
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
    }
    public class ApiCategoryModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
    public class ApiFieldModel
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
    }
    public class ApiIpFilterModel
    {
        /// <summary>
        /// IPアドレス
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public string IsEnable { get; set; }
    }
    public class ApiOpenIdCAModel
    {
        /// <summary>
        /// ApiOpenIdCaId
        /// </summary>
        public string ApiOpenIdCaId { get; set; }

        /// <summary>
        /// ApplicationId
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Application名
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// アクセスコントロール
        /// </summary>
        public string AccessControl { get; set; }
    }
}
