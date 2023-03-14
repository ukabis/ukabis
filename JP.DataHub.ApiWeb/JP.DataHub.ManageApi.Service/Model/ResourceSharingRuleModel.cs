namespace JP.DataHub.ManageApi.Service.Model
{
    public class ResourceSharingRuleModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ResourceSharingRuleId { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// データ所有ベンダーID
        /// </summary>
        public string SharingFromVendorId { get; set; }

        /// <summary>
        /// データ所有システムID
        /// </summary>
        public string SharingFromSystemId { get; set; }

        /// <summary>
        /// データ公開対象ベンダーID
        /// </summary>
        public string SharingToVendorId { get; set; }


        /// <summary>
        /// データ公開対象システムID
        /// </summary>
        public string SharingToSystemId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ResourceSharingRuleName { get; set; }

        /// <summary>
        /// 条件クエリ
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// 条件スクリプト
        /// </summary>
        public string RoslynScript { get; set; }

        /// <summary>
        /// 有効
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
