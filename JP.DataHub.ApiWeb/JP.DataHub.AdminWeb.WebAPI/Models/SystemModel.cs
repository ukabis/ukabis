using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    public class SystemModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        [Required(ErrorMessage = "システム名は必須項目です。")]
        [MaxLengthEx(MaxLength = 100, ErrorMessageFormat = "システム名は{0}文字以内で入力して下さい。")]
        public string SystemName { get; set; }

        /// <summary>
        /// OpenId認証のアプリケーションID
        /// </summary>
        [MaxLengthEx(MaxLength = 128, ErrorMessageFormat = "アプリケーションIDは{0}文字以内で入力して下さい。")]
        public string OpenIdApplicationId { get; set; }

        /// <summary>
        /// OpenId認証のクライアントシークレット
        /// </summary>
        public string OpenIdClientSecret { get; set; }

        /// <summary>
        /// 代表メールアドレス
        /// </summary>
        [EmailAddressEx]
        public string RepresentativeMailAddress { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 認証情報リスト
        /// </summary>
        [ValidateComplexType]
        public List<ClientModel> ClientList { get; set; }

        /// <summary>
        /// システムリンクリスト
        /// </summary>
        [ValidateComplexType]
        [LinkDefaultCount]
        public List<SystemLinkModel> SystemLinkList { get; set; }

        /// <summary>
        /// 管理者認証
        /// </summary>
        [ValidateComplexType]
        public SystemAdminModel SystemAdmin { get; set; }
    }

    public class SystemVendorModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }
        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }
    }

    public class ClientModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// クライアントID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>クライアントシークレット</summary>
        [ClientSecret]
        public string ClientSecret { get; set; }

        /// <summary>
        /// 有効期限
        /// </summary>
        [AccessTokenExpiretion]
        public string AccessTokenExpirationTimeSpan { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public ClientModel Clone()
        {
            return (ClientModel)this.MemberwiseClone();
        }
    }

    public class SystemLinkModel
    {
        /// <summary>
        /// SystemリンクID
        /// </summary>
        public string SystemLinkId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// リンク表示名
        /// </summary>
        [Required(ErrorMessage = "タイトルは必須項目です。")]
        [MaxLengthEx(MaxLength = 100, ErrorMessageFormat = "タイトルは{0}文字以内で入力して下さい。")]
        public string Title { get; set; }

        /// <summary>
        /// リンク詳細
        /// </summary>
        [Required(ErrorMessage = "説明は必須項目です。")]
        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "説明は{0}文字以内で入力して下さい。")]
        public string Detail { get; set; }

        /// <summary>
        /// リンクURL
        /// </summary>
        [Required(ErrorMessage = "URLは必須項目です。")]
        [Url(ErrorMessage = "正しいURLではありません。")]
        [MaxLengthEx(MaxLength = 512, ErrorMessageFormat = "URLは{0}文字以内で入力して下さい。")]
        public string Url { get; set; }

        /// <summary>
        /// APIリンク表示フラグ
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// デフォルトかどうか
        /// </summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public SystemLinkModel Clone()
        {
            return (SystemLinkModel)this.MemberwiseClone();
        }
    }
    public class SystemAdminModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// 管理者認証キー
        /// </summary>
        [MaxLengthEx(MaxLength = 260, ErrorMessageFormat = "管理者認証キーは{0}文字以内で入力して下さい。")]
        public string AdminSecret { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }
}
