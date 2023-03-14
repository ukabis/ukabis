using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ApiViewModel
    {
        /// <summary>
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
        public IEnumerable<ApiTagViewModel> TagList { get; set; }

        /// <summary>
        /// カテゴリーリスト
        /// </summary>
        public IEnumerable<ApiCategoryViewModel> CategoryList { get; set; }

        /// <summary>
        /// 分野リスト
        /// </summary>
        public IEnumerable<ApiFieldViewModel> FieldList { get; set; }

        /// <summary>
        /// 楽観排他を使用するかどうか
        /// </summary>
        public bool IsOptimisticConcurrency { get; set; }

        /// <summary>
        /// IPフィルタグループリスト
        /// </summary>
        public IEnumerable<DynamicApiCommonIpFilterGroupViewModel> CommonIpFilterGroupList { get; set; }

        /// <summary>
        /// IPフィルタリスト
        /// </summary>
        public IEnumerable<ApiIpFilterViewModel> IpFilterList { get; set; }

        /// <summary>
        /// OpenId認証局リスト
        /// </summary>
        public IEnumerable<ApiOpenIdCAViewModel> OpenIdCAList { get; set; }

        /// <summary>
        /// Methodリスト
        /// </summary>
        public IEnumerable<MethodViewModel> MethodList { get; set; }

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
}
