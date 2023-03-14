using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class RegisterResourceApiModel
    {
        #region 基本設定

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// API名
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Api説明
        /// </summary>
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
        public string RepositoryKey { get; set; }

        /// <summary>
        /// パーティションキー
        /// </summary>
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
        public List<RegisterApiTagInfoModel> ApiTagInfoList { get; set; }

        /// <summary>
        /// カテゴリー
        /// </summary>
        public List<RegisterApiCategoryModel> CategoryList { get; set; }

        /// <summary>
        /// 分野
        /// </summary>
        public List<RegisterApiFieldInfoModel> ApiFieldInfoList { get; set; }

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
        public string FeeDescription { get; set; }

        /// <summary>
        /// 作成者
        /// </summary>
        public string ResourceCreateUser { get; set; }

        /// <summary>
        /// メンテナー
        /// </summary>
        public string ResourceMaintainer { get; set; }

        /// <summary>
        /// 作成日
        /// </summary>
        public string ResourceCreateDate { get; set; }

        /// <summary>
        /// 最終更新日
        /// </summary>
        public string ResourceLatestDate { get; set; }

        /// <summary>
        /// 更新頻度
        /// </summary>
        public string UpdateFrequency { get; set; }

        /// <summary>
        /// 個別手続きの有無 
        /// </summary>
        public bool IsContract { get; set; }

        /// <summary>
        /// 連絡先 
        /// </summary>
        public string ContactInformation { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// データ利用規約 
        /// </summary>
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
        public RegisterDynamicApiAttachFileSettingsModel AttachFileSettings { get; set; }

        #endregion

        #region 履歴機能

        /// <summary>
        /// DynamicAPIとして管理するデータの履歴を残すかどうか
        /// </summary>
        public RegisterDocumentHistorySettingsModel DocumentHistorySettings { get; set; }

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
        /// コンテナー分離
        /// </summary>
        public bool IsContainerDynamicSeparation { get; set; }

        /// <summary>
        /// IPフィルタを設定するかどうか
        /// </summary>
        public bool IsEnableIpFilter { get; set; }

        /// <summary>
        /// 共通IPフィルタリスト
        /// </summary>
        public List<RegisterApiCommonIpFilterGroupModel> ApiCommonIpFilterGroupList { get; set; }

        /// <summary>
        /// IPフィルタ
        /// </summary>
        public List<RegisterApiIpFilterModel> ApiIpFilterList { get; set; }

        /// <summary>
        /// OpenId認証局
        /// </summary>
        public List<RegisterResourceOpenIdCaModel> OpenIdCaList { get; set; }

        #endregion

        /// <summary>
        /// blobのキャッシュを使用するかどうか
        /// </summary>
        public bool IsUseBlobCache { get; set; }

        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public bool IsEnableResourceVersion { get; set; } = true;
    }
}
