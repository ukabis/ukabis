using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class RegisterApiRequestViewModel
    {
        #region 基本設定
        /// <summary>
        /// ベンダーID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public string SystemId { get; set; }

        [ValidateGuid]
        public Guid? ApiId { get; set; }

        /// <summary>
        /// API名
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "controller_name")]
        public string ApiName { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [RegularExpression(@"^/[-/_a-zA-Z0-9]+[a-zA-Z0-9]$", ErrorMessage = "正しいURLではありません。")]
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "url")]
        public string Url { get; set; }

        /// <summary>
        /// Api説明
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "controller_description")]
        public string ApiDescriptiveText { get; set; }

        /// <summary>
        /// ベンダー依存かどうか
        /// </summary>
        public bool IsVendor { get; set; }

        /// <summary>
        /// 個人依存かどうか
        /// </summary>
        public bool IsPerson { get; set; }

        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 代表的なモデルID
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// リポジトリキー
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "controller_repository_key")]
        [ValidateApi(ApiItems.RepositoryKey)]
        public string RepositoryKey { get; set; }

        /// <summary>
        /// パーティションキー
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "controller_partition_key")]
        [ValidateApi(ApiItems.PartitionKey)]
        public string PartitionKey { get; set; }

        /// <summary>
        /// StaticAPIかどうか
        /// </summary>
        /// <remarks>管理画面だとStaticApiじゃないと画面にでてこない</remarks>
        public bool IsStaticApi { get; set; }

        #endregion

        #region Data Dictionary

        /// <summary>
        /// タグ
        /// </summary>
        public List<RegisterApiTagInfoViewModel> ApiTagInfoList { get; set; }

        /// <summary>
        /// カテゴリー
        /// </summary>
        public List<RegisterApiCategoryViewModel> CategoryList { get; set; }

        /// <summary>
        /// 分野
        /// </summary>
        public List<RegisterApiFieldInfoViewModel> ApiFieldInfoList { get; set; }

        /// <summary>
        /// 区分がデータか
        /// </summary>
        public bool IsData { get; set; }

        /// <summary>
        /// 区分がビジネスロジックか
        /// </summary>
        public bool IsBusinessLogic { get; set; }

        /// <summary>
        /// 使用料　有料か無料か
        /// </summary>
        public bool IsPay { get; set; }

        /// <summary>
        /// 使用料 有料時の料金
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "fee_description")]
        public string FeeDescription { get; set; }

        /// <summary>
        /// 作成者
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "resource_create_user")]
        public string ResourceCreateUser { get; set; }

        /// <summary>
        /// メンテナー
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "resource_maintainer")]
        public string ResourceMaintainer { get; set; }

        /// <summary>
        /// 作成日
        /// </summary>
        [ValidateApi(ApiItems.ResourceCreateDate)]
        public string ResourceCreateDate { get; set; }

        /// <summary>
        /// 最終更新日
        /// </summary>
        [ValidateApi(ApiItems.ResourceLatestDate)]
        public string ResourceLatestDate { get; set; }

        /// <summary>
        /// 更新頻度
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "update_frequency")]
        public string UpdateFrequency { get; set; }

        /// <summary>
        /// 個別手続きの有無 
        /// </summary>
        public bool IsContract { get; set; }

        /// <summary>
        /// 連絡先 
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "contact_information")]
        public string ContactInformation { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "version")]
        public string Version { get; set; }

        /// <summary>
        /// データ利用規約 
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "Controller", "agree_description")]
        public string AgreeDescription { get; set; }

        /// <summary>
        /// 規約に同意ボタンを表示するか
        /// </summary>
        public bool IsVisibleAgreement { get; set; }

        #endregion

        #region 添付ファイル

        /// <summary>
        /// DynamicAPIの添付ファイル設定を有効にするかどうか
        /// </summary>
        [ValidateApi(ApiItems.AttachFileSettings)]
        public DynamicApiAttachFileSettingsViewModel AttachFileSettings { get; set; }

        #endregion

        #region 履歴機能

        /// <summary>
        /// DynamicAPIとして管理するデータの履歴を残すかどうか
        /// </summary>
        [ValidateApi(ApiItems.HistorySettings)]
        public DocumentHistorySettingsViewModel DocumentHistorySettings { get; set; }

        #endregion

        #region その他設定
        /// <summary>
        /// ブロックチェーンを使用するかどうか
        /// </summary>
        public bool IsEnableBlockchain { get; set; }

        /// <summary>
        /// 楽観排他を使用するかどうか
        /// </summary>
        public bool IsOptimisticConcurrency { get; set; }

        /// <summary>
        /// IPフィルタを設定するかどうか
        /// </summary>
        public bool IsEnableIpFilter { get; set; }

        /// <summary>
        /// 共通IPフィルタリスト
        /// </summary>
        public List<RegisterApiCommonIpFilterGroupViewModel> ApiCommonIpFilterGroupList { get; set; }

        /// <summary>
        /// IPフィルタ
        /// </summary>
        [ValidateApi(ApiItems.ApiIpFilterList)]
        public List<RegisterApiIpFilterViewModel> ApiIpFilterList { get; set; }

        /// <summary>
        /// OpenId認証局
        /// </summary>
        public List<RegisterResourceOpenIdCaViewModel> OpenIdCaList { get; set; }

        #endregion

        /// <summary>
        /// blobのキャッシュを使用するかどうか
        /// </summary>
        public bool IsUseBlobCache { get; set; }

        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public bool IsEnableResourceVersion { get; set; } = true;

        /// <summary>
        /// コンテナー分離をどうするか
        /// </summary>
        public bool IsContainerDynamicSeparation { get; set; } = false;
    }
}
