
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    ///  メソッド情報
    /// </summary>
    public class MethodDescriptionModel
    {
        /// <summary>メソッドID</summary>
        public Guid MethodId { get; set; }

        /// <summary>HTTPメソッド</summary>
        public string HttpMethod { get; set; }

        /// <summary>相対パス</summary>
        public string RelativePath { get; set; }

        /// <summary>アクションタイプ</summary>
        public string ActionType { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }

        /// <summary>ベンダー/システム認証フラグ</summary>
        public bool AuthVendorSystem { get; set; }

        /// <summary>OpenId認証フラグ</summary>
        public bool AuthOpenId { get; set; }

        /// <summary>管理者認証フラグ</summary>
        public bool AuthAdmin { get; set; }

        /// <summary>ベンダー/システム認証なし許可フラグ</summary>
        public bool IsVendorSystemAuthenticationAllowNull { get; set; }

        /// <summary>データ登録時に配列型を許すか</summary>
        public bool PostArray { get; set; }

        /// <summary>UrlスキーマID</summary>
        public Guid? UrlSchemaId { get; set; }

        /// <summary>リクエストスキーマID</summary>
        public Guid? RequestSchemaId { get; set; }

        /// <summary>レスポンススキーマID</summary>
        public Guid? ResponseSchemaId { get; set; }

        /// <summary>メソッドリンク情報</summary>
        public IEnumerable<MethodLinkModel> MethodLinks { get; set; }

        /// <summary>サンプルコード情報</summary>
        public IEnumerable<SampleCodeModel> SampleCodes { get; set; }

        /// <summary>親のAPIの相対パス</summary>
        public string ApiRelativePath { get; set; }

        /// <summary>サインインユーザーのみ可視フラグ</summary>
        public bool IsVisibleSigninuserOnly { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsEnable { get; set; }

        /// <summary>非表示フラグ</summary>
        public bool IsHidden { get; set; }

        /// <summary>論理削除フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }
    }
}
