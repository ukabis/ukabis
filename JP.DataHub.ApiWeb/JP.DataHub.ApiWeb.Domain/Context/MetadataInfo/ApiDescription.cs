using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    ///  API情報
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class ApiDescription
    {
        /// <summary>API ID</summary>
        [Key(0)]
        public Guid ApiId { get; set; }

        /// <summary>ベンダーID</summary>
        [Key(1)]
        public Guid VendorId { get; set; }

        /// <summary>ベンダー名</summary>
        [Key(2)]
        public string VendorName { get; set; }

        /// <summary>ベンダーUrl</summary>
        [Key(3)]
        public string VendorUrl { get; set; }

        /// <summary>システムID</summary>
        [Key(4)]
        public Guid SystemId { get; set; }

        /// <summary>システム名</summary>
        [Key(5)]
        public string SystemName { get; set; }

        /// <summary>システムUrl</summary>
        [Key(6)]
        public string SystemUrl { get; set; }

        /// <summary>API名</summary>
        [Key(7)]
        public string ApiName
        {
            get
            {
                if (!string.IsNullOrEmpty(apiName)) return apiName;

                if (!string.IsNullOrEmpty(RelativePath))
                {
                    int idx = RelativePath.LastIndexOf('/');
                    if (idx >= 0 && RelativePath.Length > idx + 1)
                        return RelativePath.Substring(idx + 1);
                }

                return RelativePath;
            }
            set
            {
                apiName = value;
            }
        }
        [IgnoreMember]
        private string apiName;

        /// <summary>相対パス</summary>
        [Key(8)]
        public string RelativePath { get; set; }

        /// <summary>説明</summary>
        [Key(9)]
        public string Description { get; set; }

        /// <summary>StaticAPIフラグ</summary>
        [Key(10)]
        public bool IsStaticApi { get; set; }

        /// <summary>ベンダー依存フラグ</summary>
        [Key(11)]
        public bool PartitionByVendor { get; set; }

        /// <summary>個人依存フラグ</summary>
        [Key(12)]
        public bool PartitionByPerson { get; set; }

        /// <summary>データフラグ</summary>
        [Key(13)]
        public bool IsData { get; set; }

        /// <summary>ビジネスロジックフラグ</summary>
        [Key(14)]
        public bool IsBusinesslogic { get; set; }

        /// <summary>有料フラグ</summary>
        [Key(15)]
        public bool IsPay { get; set; }

        /// <summary>使用料に関数する説明</summary>
        [Key(16)]
        public string FeeDescription { get; set; }

        /// <summary>リソース作成者</summary>
        [Key(17)]
        public string ResourceCreateUser { get; set; }

        /// <summary>リソースメンテナー</summary>
        [Key(18)]
        public string ResourceMaintainer { get; set; }

        /// <summary>リソース作成日</summary>
        [Key(19)]
        public DateTime? ResourceCreateDate { get; set; }

        /// <summary>リソース最終更新日</summary>
        [Key(20)]
        public DateTime? ResourceLatestDate { get; set; }

        /// <summary>更新頻度の説明</summary>
        [Key(21)]
        public string UpdateFrequency { get; set; }

        /// <summary>契約必要フラグ</summary>
        [Key(22)]
        public bool NeedContract { get; set; }

        /// <summary>契約に関する情報</summary>
        [Key(23)]
        public string ContactInformation { get; set; }

        /// <summary>バージョン</summary>
        [Key(24)]
        public string Version { get; set; }

        /// <summary>利用規約</summary>
        [Key(25)]
        public string AgreeDescription { get; set; }

        /// <summary>規約同意要否</summary>
        [Key(26)]
        public bool IsVisibleAgreement { get; set; }

        /// <summary>カテゴリー</summary>
        [Key(27)]
        public IEnumerable<Category> Categories { get; set; }

        /// <summary>分野</summary>
        [Key(28)]
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>タグ</summary>
        [Key(29)]
        public IEnumerable<Tag> Tags { get; set; }

        /// <summary>メソッド情報</summary>
        [Key(30)]
        public List<MethodDescription> Methods { get; set; }

        /// <summary>有効フラグ</summary>
        [Key(31)]
        public bool IsEnable { get; set; }

        /// <summary>論理削除フラグ</summary>
        [Key(32)]
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        [Key(33)]
        public DateTime UpdDate { get; set; }

        /// <summary>スキーマID</summary>
        [Key(34)]
        public Guid ApiSchemaId { get; set; }

        /// <summary>リポジトリキー</summary>
        [Key(35)]
        public string RepositoryKey { get; set; }


        /// <summary>
        /// コンストラクタ(デフォルト)
        /// </summary>
        public ApiDescription()
        {
        }

        /// <summary>
        /// コンストラクタ(全項目)
        /// </summary>
        public ApiDescription(Guid apiId, Guid vendorId, string vendorName, string vendorUrl, Guid systemId, string systemName, string systemUrl, string apiName,
            string relativePath, string description, bool isStaticApi, bool partitionByVendor, bool partitionByPerson, bool isData, bool isBusinesslogic,
            bool isPay, string feeDescription, string resourceCreateUser, string resourceMaintainer, DateTime? resourceCreateDate, DateTime? resourceLatestDate,
            string updateFrequency, bool needContract, string contactInformation, string version, string agreeDescription, bool isVisibleAgreement,
            IEnumerable<Category> categories, IEnumerable<Field> fields, IEnumerable<Tag> tags, List<MethodDescription> methods, bool isEnable,
            bool isActive, DateTime updDate, Guid apiSchemaId, string repositoryKey)
        {
            ApiId = apiId;
            VendorId = vendorId;
            VendorName = vendorName;
            VendorUrl = vendorUrl;
            SystemId = systemId;
            SystemName = systemName;
            SystemUrl = systemUrl;
            ApiName = apiName;
            RelativePath = relativePath;
            Description = description;
            IsStaticApi = isStaticApi;
            PartitionByVendor = partitionByVendor;
            PartitionByPerson = partitionByPerson;
            IsData = isData;
            IsBusinesslogic = isBusinesslogic;
            IsPay = isPay;
            FeeDescription = feeDescription;
            ResourceCreateUser = resourceCreateUser;
            ResourceMaintainer = resourceMaintainer;
            ResourceCreateDate = resourceCreateDate;
            ResourceLatestDate = resourceLatestDate;
            UpdateFrequency = updateFrequency;
            NeedContract = needContract;
            ContactInformation = contactInformation;
            Version = version;
            AgreeDescription = agreeDescription;
            IsVisibleAgreement = isVisibleAgreement;
            Categories = categories;
            Fields = fields;
            Tags = tags;
            Methods = methods;
            IsEnable = isEnable;
            IsActive = isActive;
            UpdDate = updDate;
            ApiSchemaId = apiSchemaId;
            RepositoryKey = repositoryKey;
        }

        /// <summary>
        /// コンストラクタ(MetadataInfoRepository向け)
        /// </summary>
        public ApiDescription(Guid apiId, Guid vendorId, string vendorName, Guid systemId, string systemName, string apiName, string relativePath, 
            string description, bool partitionByVendor, bool partitionByPerson, bool isStaticApi, bool isData, bool isBusinesslogic,
            bool isPay, string feeDescription, string resourceCreateUser, string resourceMaintainer, DateTime? resourceCreateDate, DateTime? resourceLatestDate,
            string updateFrequency, bool needContract, string contactInformation, string version, string agreeDescription, bool isVisibleAgreement,
            bool isEnable, bool isActive, Guid apiSchemaId, string repositoryKey, DateTime updDate, string vendorUrl, string systemUrl)
        {
            ApiId = apiId;
            VendorId = vendorId;
            VendorName = vendorName;
            SystemId = systemId;
            SystemName = systemName;
            ApiName = apiName;
            RelativePath = relativePath;
            Description = description;
            PartitionByVendor = partitionByVendor;
            PartitionByPerson = partitionByPerson;
            IsStaticApi = isStaticApi;
            IsData = isData;
            IsBusinesslogic = isBusinesslogic;
            IsPay = isPay;
            FeeDescription = feeDescription;
            ResourceCreateUser = resourceCreateUser;
            ResourceMaintainer = resourceMaintainer;
            ResourceCreateDate = resourceCreateDate;
            ResourceLatestDate = resourceLatestDate;
            UpdateFrequency = updateFrequency;
            NeedContract = needContract;
            ContactInformation = contactInformation;
            Version = version;
            AgreeDescription = agreeDescription;
            IsVisibleAgreement = isVisibleAgreement;
            IsEnable = isEnable;
            IsActive = isActive;
            UpdDate = updDate;
            ApiSchemaId = apiSchemaId;
            RepositoryKey = repositoryKey;
            VendorUrl = vendorUrl;
            SystemUrl = systemUrl;
        }
    }
}
