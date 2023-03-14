using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class ControllerQueryModel
    {
        public ControllerQueryModel()
        {
            ControllerTagList = new List<ControllerTagInfoModel>();
            ControllerCategoryList = new List<ControllerCategoryInfomationModel>();
            ControllerFieldList = new List<ControllerFieldInfoModel>();
            ControllerCommonIpFilterGroupList = new List<ControllerCommonIpFilterGroupModel>();
            ControllerIpFilterList = new List<ControllerIpFilterModel>();
            ControllerOpenIdCAList = new List<OpenIdCaModel>();
            ApiList = new List<ApiQueryModel>();
        }

        /// <summary>
        /// ControllerのID
        /// </summary>
        public string ControllerId { get; set; }

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
        /// Controllerの名前
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Controllerの相対パス
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Controllerの説明
        /// </summary>
        public string ControllerDescription { get; set; }

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
        /// コントローラーのスキーマーID
        /// </summary>
        public string ControllerSchemaId { get; set; }

        /// <summary>
        /// コントローラーのスキーマー名
        /// </summary>
        public string ControllerSchemaName { get; set; }

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
        public IEnumerable<ControllerTagInfoModel> ControllerTagList { get; set; }

        /// <summary>
        /// カテゴリーリスト
        /// </summary>
        public IEnumerable<ControllerCategoryInfomationModel> ControllerCategoryList { get; set; }

        /// <summary>
        /// 分野リスト
        /// </summary>
        public IEnumerable<ControllerFieldInfoModel> ControllerFieldList { get; set; }

        /// <summary>
        /// IPフィルタグループリスト
        /// </summary>
        public IEnumerable<ControllerCommonIpFilterGroupModel> ControllerCommonIpFilterGroupList { get; set; }

        /// <summary>
        /// IPフィルタリスト
        /// </summary>
        public IEnumerable<ControllerIpFilterModel> ControllerIpFilterList { get; set; }

        /// <summary>
        /// OpenId認証局リスト
        /// </summary>
        public IEnumerable<OpenIdCaModel> ControllerOpenIdCAList { get; set; }

        /// <summary>
        /// Apiリスト
        /// </summary>
        public IEnumerable<ApiQueryModel> ApiList { get; set; }

        /// <summary>
        /// 楽観排他を使用するかどうか
        /// </summary>
        public bool IsOptimisticConcurrency { get; set; }

        /// <summary>
        /// DynamicAPIとして管理するデータの履歴を残すかどうか
        /// </summary>
        public bool IsDocumentHistory { get; set; }

        /// <summary>
        /// ブロックチェーンを使用するかどうか
        /// </summary>
        public bool IsEnableBlockchain { get; set; }

        /// <summary>
        /// 添付ファイルを使用するかどうか
        /// </summary>
        public bool IsEnableAttachFile { get; set; }

        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public bool IsEnableResourceVersion { get; set; }
    }
}
