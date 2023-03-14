using JP.DataHub.ManageApi.Service.DymamicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ControllerInformationModel
    {
        public string ControllerId { get; set; }

        public string Url { get; set; }

        public string ControllerDescription { get; set; }

        public string VendorId { get; set; }

        public string SystemId { get; set; }

        public bool IsVendor { get; set; }

        public bool IsPerson { get; set; }

        public bool IsEnable { get; set; }

        public bool IsActive { get; set; }
        public bool IsTopPage { get; set; }

        public bool IsStaticApi { get; set; }

        public List<ApiInformationModel> ApiList { get; set; } = new();

        public List<ControllerIpFilterModel> ControllerIpFilterList { get; set; } = new();

        public string ControllerSchemaId { get; set; }

        public IList<ControllerCategoryInfomationModel> CategoryList { get; set; } = new List<ControllerCategoryInfomationModel>();

        public string RepositoryKey { get; set; }

        public string PartitionKey { get; set; }

        public List<ControllerCommonIpFilterGroupModel> ControllerCommonIpFilterGroupList { get; set; } = new();

        public string ControllerName { get; set; }

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

        public bool IsVisibleAgreement { get; set; }

        public List<ControllerTagInfoModel> ControllerTagInfoList { get; set; } = new();

        public List<ControllerFieldInfoModel> ControllerFieldInfoList { get; set; } = new();

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public List<OpenIdCaModel> OpenIdCaList { get; set; } = new();

        public bool IsEnableIpFilter { get; set; }

        public bool IsEnableBlockchain { get; set; }

        public bool IsOptimisticConcurrency { get; set; }

        public bool IsContainerDynamicSeparation { get; set; }

        /// <summary>
        /// Blobキャッシュを使用する
        /// </summary>
        public bool IsUseBlobCache { get; set; }

        public AttachFileSettingsModel AttachFileSettings { get; set; }

        public DocumentHistorySettingsModel DocumentHistorySettings { get; set; }

        /// <summary>
        /// マルチランゲージ対応リソース
        /// </summary>
        public List<ControllerMultiLanguageModel> ControllerMultiLanguageList { get; set; } = new();

        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public bool IsEnableResourceVersion { get; set; } = true;
    }
}
