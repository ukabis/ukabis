using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiResourceInformationModel
    {
        public string ControllerId { get; set; }
        //Register時のプロパティ名違い補完
        public string ApiId { get => this.ControllerId; }

        [Required(ErrorMessage = "URLは必須項目です。")]
        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "URLは{0}文字以内で入力して下さい。")]
        [ValidateApiResource(ApiItems.Url)]
        public string Url { get; set; }

        public string ControllerDescription { get; set; }
        //Register時のプロパティ名違い補完
        public string ApiDescriptiveText { get => this.ControllerDescription; }

        [Required(ErrorMessage = "ベンダーを設定してください。")]
        public string VendorId { get; set; }

        [Required(ErrorMessage = "システムを設定してください。")]
        public string SystemId { get; set; }

        public bool IsVendor { get; set; }

        public bool IsPerson { get; set; }

        public bool IsEnable { get; set; }

        public bool IsActive { get; set; }

        public bool IsTopPage { get; set; }

        public bool IsStaticApi { get; set; }

        [ValidateComplexType]
        [ValidateApiResource(ApiItems.ApiIpFilterList)]
        public List<ApiResourceIpFilterModel> ControllerIpFilterList { get; set; } = new();
        //Register時のプロパティ名違い補完
        public List<ApiResourceIpFilterModel> ApiIpFilterList { get => this.ControllerIpFilterList; }

        public string ControllerSchemaId { get; set; }

        public string ModelId { get => this.ControllerSchemaId; }

        public IList<ControllerCategoryInfomationModel> CategoryList { get; set; } = new List<ControllerCategoryInfomationModel>();

        [MaxLengthEx(MaxLength = 4000, ErrorMessageFormat = "リポジトリキーは{0}文字以内で入力して下さい。")]
        [ValidateApiResource(ApiItems.RepositoryKey)]
        public string RepositoryKey { get; set; }

        [MaxLengthEx(MaxLength = 4000, ErrorMessageFormat = "パーティションキーは{0}文字以内で入力して下さい。")]
        [ValidateApiResource(ApiItems.PartitionKey)]
        public string PartitionKey { get; set; }

        public List<ApiResourceCommonIpFilterGroupModel> ControllerCommonIpFilterGroupList { get; set; } = new();
        //Register時のプロパティ名違い補完
        public List<ApiResourceCommonIpFilterGroupModel> ApiCommonIpFilterGroupList { get => this.ControllerCommonIpFilterGroupList; }

        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "API名は{0}文字以内で入力して下さい。")]
        public string ControllerName { get; set; }
        //Register時のプロパティ名違い補完
        public string ApiName { get => this.ControllerName; }

        public bool IsData { get; set; }

        public bool IsBusinessLogic { get; set; }

        public bool IsPay { get; set; }

        public string FeeDescription { get; set; }

        [MaxLengthEx(MaxLength = 260, ErrorMessageFormat = "作成者は{0}文字以内で入力して下さい。")]
        public string ResourceCreateUser { get; set; }

        [MaxLengthEx(MaxLength = 260, ErrorMessageFormat = "メンテナーは{0}文字以内で入力して下さい。")]
        public string ResourceMaintainer { get; set; }

        [ValidateApiResource(ApiItems.ResourceCreateDate)]
        public string ResourceCreateDate { get; set; }

        [ValidateApiResource(ApiItems.ResourceLatestDate)]
        public string ResourceLatestDate { get; set; }

        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "更新頻度は{0}文字以内で入力して下さい。")]
        public string UpdateFrequency { get; set; }

        public bool IsContract { get; set; }

        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "連絡先は{0}文字以内で入力して下さい。")]
        public string ContactInformation { get; set; }

        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "バージョンは{0}文字以内で入力して下さい。")]
        public string Version { get; set; }

        public string AgreeDescription { get; set; }

        public bool IsVisibleAgreement { get; set; }

        public List<ApiResourceTagInfoModel> ControllerTagInfoList { get; set; } = new();
        //Register時のプロパティ名違い補完
        public List<ApiResourceTagInfoModel> ApiTagInfoList { get => this.ControllerTagInfoList; }

        public List<ApiResourceFieldInfoModel> ControllerFieldInfoList { get; set; } = new();
        //Register時のプロパティ名違い補完
        public List<ApiResourceFieldInfoModel> ApiFieldInfoList { get => this.ControllerFieldInfoList; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public List<OpenIdCaViewModel> OpenIdCaList { get; set; } = new();

        public bool IsEnableIpFilter { get; set; }

        public bool IsEnableBlockchain { get; set; }

        public bool IsOptimisticConcurrency { get; set; }

        public bool IsContainerDynamicSeparation { get; set; }

        /// <summary>
        /// Blobキャッシュを使用する
        /// </summary>
        public bool IsUseBlobCache { get; set; }

        [ValidateComplexType]
        public ApiResourceAttachFileSettingsModel AttachFileSettings { get; set; } = new(false, null, null);

        [ValidateComplexType]
        public DocumentHistorySettingsModel DocumentHistorySettings { get; set; } = new(false, null);

        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public bool IsEnableResourceVersion { get; set; } = true;

        /// <summary>
        /// 依存をEnumで返却
        /// </summary>
        public DependenctFlags Dependency
        {
            get
            {
                if (!IsPerson && !IsVendor) return DependenctFlags.NonDependency;
                if (IsPerson && IsVendor) return DependenctFlags.Double;
                return IsVendor ? DependenctFlags.IsVendor : DependenctFlags.IsPerson;
            }
        }

        public enum DependenctFlags
        {
            NonDependency = 1,
            IsVendor = 2,
            IsPerson = 4,
            Double = 8
        }

    }
}
