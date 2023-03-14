using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ControllerHeaderModel
    {
        public ControllerHeaderModel()
        {
            VendorNameSystemNameList = new List<VendorNameSystemNameModel>();
            DataSchemaList = new List<DataSchemaInformationModel>();
            ControllerCategoryInfomationList = new List<ControllerCategoryInfomationModel>();
            ControllerCommonIpFilterGroupList = new List<ControllerCommonIpFilterGroupModel>();
            Controller = new ControllerInformationModel();
            TagInfoList = new List<TagInfoModel>();
            ControllerTagInfoList = new List<ControllerTagInfoModel>();
            FieldInfoList = new List<FieldQueryModel>();
            ControllerFieldInfoList = new List<ControllerFieldInfoModel>();
            OpenIdCaList = new List<OpenIdCaModel>();
            RepositoryGroupList = new List<RepositoryGroupModel>();
            AttachFileSettings = new AttachFileSettingsModel();
            DocumentHistorySettings = new DocumentHistorySettingsModel();
        }

        /// <summary>
        /// ベンダー名システム名リスト
        /// </summary>
        public IEnumerable<VendorNameSystemNameModel> VendorNameSystemNameList { get; set; }

        /// <summary>
        /// データスキーマリスト
        /// </summary>
        public IEnumerable<DataSchemaInformationModel> DataSchemaList { get; set; }

        /// <summary>
        /// コントローラカテゴリーリスト
        /// </summary>
        public IEnumerable<ControllerCategoryInfomationModel> ControllerCategoryInfomationList { get; set; }

        /// <summary>
        /// コントローラコモンIPフィルタグループリスト
        /// </summary>
        public IEnumerable<ControllerCommonIpFilterGroupModel> ControllerCommonIpFilterGroupList { get; set; }

        /// <summary>
        /// コントローラ
        /// </summary>
        public ControllerInformationModel Controller { get; set; }

        /// <summary>
        /// タグリスト
        /// </summary>
        public IEnumerable<TagInfoModel> TagInfoList { get; set; }

        /// <summary>
        /// コントローラタグリスト
        /// </summary>
        public IEnumerable<ControllerTagInfoModel> ControllerTagInfoList { get; set; }

        /// <summary>
        /// フィールドリスト
        /// </summary>
        public IEnumerable<FieldQueryModel> FieldInfoList { get; set; }

        /// <summary>
        /// コントローラフィールドリスト
        /// </summary>
        public IEnumerable<ControllerFieldInfoModel> ControllerFieldInfoList { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public IEnumerable<OpenIdCaModel> OpenIdCaList { get; set; }

        /// <summary>
        /// リポジトリグループリスト
        /// </summary>
        public IEnumerable<RepositoryGroupModel> RepositoryGroupList { get; set; }

        /// <summary>
        /// 添付ファイル設定
        /// </summary>
        public AttachFileSettingsModel AttachFileSettings { get; set; }

        /// <summary>
        /// 履歴ドキュメント設定
        /// </summary>
        public DocumentHistorySettingsModel DocumentHistorySettings { get; set; }
    }
}
