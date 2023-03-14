
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    ///  API情報
    /// </summary>
    public class ApiDescriptionModel
    {
        private string apiName;

        /// <summary>API ID</summary>
        public Guid ApiId { get; set; }

        /// <summary>ベンダーID</summary>
        public Guid VendorId { get; set; }

        /// <summary>ベンダー名</summary>
        public string VendorName { get; set; }

        /// <summary>ベンダーUrl</summary>
        public string VendorUrl { get; set; }

        /// <summary>システムID</summary>
        public Guid SystemId { get; set; }

        /// <summary>システム名</summary>
        public string SystemName { get; set; }

        /// <summary>システムUrl</summary>
        public string SystemUrl { get; set; }

        /// <summary>API名</summary>
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

        /// <summary>相対パス</summary>
        public string RelativePath { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }

        /// <summary>StaticAPIフラグ</summary>
        public bool IsStaticApi { get; set; }

        /// <summary>ベンダー依存フラグ</summary>
        public bool PartitionByVendor { get; set; }

        /// <summary>個人依存フラグ</summary>
        public bool PartitionByPerson { get; set; }

        /// <summary>データフラグ</summary>
        public bool IsData { get; set; }

        /// <summary>ビジネスロジックフラグ</summary>
        public bool IsBusinesslogic { get; set; }

        /// <summary>有料フラグ</summary>
        public bool IsPay { get; set; }

        /// <summary>使用料に関数する説明</summary>
        public string FeeDescription { get; set; }

        /// <summary>リソース作成者</summary>
        public string ResourceCreateUser { get; set; }

        /// <summary>リソースメンテナー</summary>
        public string ResourceMaintainer { get; set; }

        /// <summary>リソース作成日</summary>
        public DateTime? ResourceCreateDate { get; set; }

        /// <summary>リソース最終更新日</summary>
        public DateTime? ResourceLatestDate { get; set; }

        /// <summary>更新頻度の説明</summary>
        public string UpdateFrequency { get; set; }

        /// <summary>契約必要フラグ</summary>
        public bool NeedContract { get; set; }

        /// <summary>契約に関する情報</summary>
        public string ContactInformation { get; set; }

        /// <summary>バージョン</summary>
        public string Version { get; set; }

        /// <summary>利用規約</summary>
        public string AgreeDescription { get; set; }

        /// <summary>規約同意要否</summary>
        public bool IsVisibleAgreement { get; set; }

        /// <summary>カテゴリー</summary>
        public IEnumerable<CategoryModel> Categories { get; set; }

        /// <summary>分野</summary>
        public IEnumerable<FieldModel> Fields { get; set; }

        /// <summary>タグ</summary>
        public IEnumerable<TagModel> Tags { get; set; }

        /// <summary>メソッド情報</summary>
        public List<MethodDescriptionModel> Methods { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsEnable { get; set; }

        /// <summary>論理削除フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }

        /// <summary>スキーマID</summary>
        public Guid ApiSchemaId { get; set; }

        /// <summary>リポジトリキー</summary>
        public string RepositoryKey { get; set; }
    }
}
