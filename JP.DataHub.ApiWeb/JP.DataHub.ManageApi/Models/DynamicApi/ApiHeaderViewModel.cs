using System;
using System.Collections.Generic;
using JP.DataHub.ManageApi.Models.DynamicApi;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ApiHeaderViewModel
    {
        /// <summary>
        /// ベンダー名システム名リスト
        /// </summary>
        public IEnumerable<VendorNameSystemNameViewModel> VendorNameSystemNameList { get; set; }

        /// <summary>
        /// データスキーマリスト
        /// </summary>
        public IEnumerable<DataSchemaInformationViewModel> DataSchemaList { get; set; }

        /// <summary>
        /// コントローラカテゴリーリスト
        /// </summary>
        public IEnumerable<ControllerCategoryInfomationViewModel> ControllerCategoryInfomationList { get; set; }

        /// <summary>
        /// コントローラコモンIPフィルタグループリスト
        /// </summary>
        public IEnumerable<ControllerCommonIpFilterGroupViewModel> ControllerCommonIpFilterGroupList { get; set; }

        /// <summary>
        /// コントローラ
        /// </summary>
        public ControllerInformationViewModel Controller { get; set; }

        /// <summary>
        /// タグリスト
        /// </summary>
        public IEnumerable<TagInfoViewModel> TagInfoList { get; set; }

        /// <summary>
        /// コントローラタグリスト
        /// </summary>
        public IEnumerable<ControllerTagInfoViewModel> ControllerTagInfoList { get; set; }

        /// <summary>
        /// フィールドリスト
        /// </summary>
        public IEnumerable<FieldQueryViewModel> FieldInfoList { get; set; }

        /// <summary>
        /// コントローラフィールドリスト
        /// </summary>
        public IEnumerable<ControllerFieldInfoViewModel> ControllerFieldInfoList { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public IEnumerable<ControllerOpenIdCaViewModel> OpenIdCaList { get; set; }

        /// <summary>
        /// リポジトリグループリスト
        /// </summary>
        public IEnumerable<DynamicApiRepositoryGroupFullViewModel> RepositoryGroupList { get; set; }

        /// <summary>
        /// 添付ファイル設定
        /// </summary>
        public AttachFileSettingsViewModel AttachFileSettings { get; set; }

        /// <summary>
        /// 履歴ドキュメント設定
        /// </summary>
        public DocumentHistorySettingsViewModel DocumentHistorySettings { get; set; }
    }
}
