using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    ///  メソッド情報
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class MethodDescription
    {
        /// <summary>メソッドID</summary>
        [Key(0)]
        public Guid MethodId { get; set; }

        /// <summary>HTTPメソッド</summary>
        [Key(1)]
        public string HttpMethod { get; set; }

        /// <summary>相対パス</summary>
        [Key(2)]
        public string RelativePath { get; set; }

        /// <summary>アクションタイプ</summary>
        [Key(3)]
        public string ActionType { get; set; }

        /// <summary>説明</summary>
        [Key(4)]
        public string Description { get; set; }

        /// <summary>ベンダー/システム認証フラグ</summary>
        [Key(5)]
        public bool AuthVendorSystem { get; set; }

        /// <summary>OpenId認証フラグ</summary>
        [Key(6)]
        public bool AuthOpenId { get; set; }

        /// <summary>管理者認証フラグ</summary>
        [Key(7)]
        public bool AuthAdmin { get; set; }

        /// <summary>ベンダー/システム認証なし許可フラグ</summary>
        [Key(8)]
        public bool IsVendorSystemAuthenticationAllowNull { get; set; }

        /// <summary>データ登録時に配列型を許すか</summary>
        [Key(9)]
        public bool PostArray { get; set; }

        /// <summary>UrlスキーマID</summary>
        [Key(10)]
        public Guid? UrlSchemaId { get; set; }

        /// <summary>リクエストスキーマID</summary>
        [Key(11)]
        public Guid? RequestSchemaId { get; set; }

        /// <summary>レスポンススキーマID</summary>
        [Key(12)]
        public Guid? ResponseSchemaId { get; set; }

        /// <summary>メソッドリンク情報</summary>
        [Key(13)]
        public IEnumerable<MethodLink> MethodLinks { get; set; }

        /// <summary>サンプルコード情報</summary>
        [Key(14)]
        public IEnumerable<SampleCode> SampleCodes { get; set; }

        /// <summary>親のAPIの相対パス</summary>
        [Key(15)]
        public string ApiRelativePath { get; set; }

        /// <summary>サインインユーザーのみ可視フラグ</summary>
        [Key(16)]
        public bool IsVisibleSigninuserOnly { get; set; }

        /// <summary>有効フラグ</summary>
        [Key(17)]
        public bool IsEnable { get; set; }

        /// <summary>非表示フラグ</summary>
        [Key(18)]
        public bool IsHidden { get; set; }

        /// <summary>論理削除フラグ</summary>
        [Key(19)]
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        [Key(20)]
        public DateTime UpdDate { get; set; }


        /// <summary>
        /// コンストラクタ(デフォルト)
        /// </summary>
        public MethodDescription()
        {

        }

        /// <summary>
        /// コンストラクタ(全項目)
        /// </summary>
        public MethodDescription(Guid methodId, string httpMethod, string relativePath, string actionType, string description, bool authVendorSystem,
            bool authOpenId, bool authAdmin, bool isVendorSystemAuthenticationAllowNull, bool postArray, Guid? urlSchemaId, Guid? requestSchemaId,
            Guid? responseSchemaId, IEnumerable<MethodLink> methodLinks, IEnumerable<SampleCode> sampleCodes, string apiRelativePath, bool isVisibleSigninuserOnly,
            bool isEnable, bool isHidden, bool isActive, DateTime updDate)
        {
            MethodId = methodId;
            HttpMethod = httpMethod;
            RelativePath = relativePath;
            ActionType = actionType;
            Description = description;
            AuthVendorSystem = authVendorSystem;
            AuthOpenId = authOpenId;
            AuthAdmin = authAdmin;
            IsVendorSystemAuthenticationAllowNull = isVendorSystemAuthenticationAllowNull;
            PostArray = postArray;
            UrlSchemaId = urlSchemaId;
            RequestSchemaId = requestSchemaId;
            ResponseSchemaId = responseSchemaId;
            MethodLinks = methodLinks;
            SampleCodes = sampleCodes;
            ApiRelativePath = apiRelativePath;
            IsVisibleSigninuserOnly = isVisibleSigninuserOnly;
            IsEnable = isEnable;
            IsHidden = isHidden;
            IsActive = isActive;
            UpdDate = updDate;
        }

        /// <summary>
        /// コンストラクタ(MetadataInfoRepository向け)
        /// </summary>
        public MethodDescription(Guid methodId, string httpMethod, string relativePath, string description, bool authVendorSystem,
            bool authOpenId, bool authAdmin, bool isVendorSystemAuthenticationAllowNull, bool postArray, Guid? urlSchemaId, Guid? requestSchemaId,
            Guid? responseSchemaId, bool isVisibleSigninuserOnly, bool isEnable, bool isHidden, bool isActive, DateTime updDate, string actionType)
        {
            MethodId = methodId;
            HttpMethod = httpMethod;
            RelativePath = relativePath;
            ActionType = actionType;
            Description = description;
            AuthVendorSystem = authVendorSystem;
            AuthOpenId = authOpenId;
            AuthAdmin = authAdmin;
            IsVendorSystemAuthenticationAllowNull = isVendorSystemAuthenticationAllowNull;
            PostArray = postArray;
            UrlSchemaId = urlSchemaId;
            RequestSchemaId = requestSchemaId;
            ResponseSchemaId = responseSchemaId;
            IsVisibleSigninuserOnly = isVisibleSigninuserOnly;
            IsEnable = isEnable;
            IsHidden = isHidden;
            IsActive = isActive;
            UpdDate = updDate;
        }
    }
}
