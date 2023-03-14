namespace JP.DataHub.ApiWeb.Domain.Service.Model
{
    public class ResourceSharingPersonRuleModel
    {
        /// <summary>
        /// 個人リソースシェアリングルールID
        /// </summary>
        public string ResourceSharingPersonRuleId { get; set; }

        /// <summary>
        /// ルール名
        /// </summary>
        public string ResourceSharingRuleName { get; set; }

        /// <summary>
        /// 共有対象のリリースパス
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// 共有元ユーザーID
        /// </summary>
        public string SharingFromUserId { get; set; }

        /// <summary>
        /// 共有元メールアドレス
        /// </summary>
        public string SharingFromMailAddress { get; set; }

        /// <summary>
        /// 共有先ユーザーID
        /// </summary>
        public string SharingToUserId { get; set; }

        /// <summary>
        /// 共有先メールアドレス
        /// </summary>
        public string SharingToMailAddress { get; set; }

        /// <summary>
        /// 共有先ベンダーID
        /// </summary>
        public Guid? SharingToVendorId { get; set; }

        /// <summary>
        /// 共有先システムID
        /// </summary>
        public Guid? SharingToSystemId { get; set; }

        /// <summary>
        /// 共有条件のクエリー
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// 共有条件を生成するスクリプト
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
