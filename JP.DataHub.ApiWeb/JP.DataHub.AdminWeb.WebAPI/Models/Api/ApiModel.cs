using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiModel
    {
        /// <summary>
        /// MethodId
        /// </summary>
        public string MethodId { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string MethodDescription { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// メソッドタイプ
        /// </summary>
        [Required(ErrorMessage = "HTTPメソッドタイプは必須項目です。")]
        public string MethodType { get; set; }

        /// <summary>
        /// MethodUrl
        /// </summary>
        [Required(ErrorMessage = "URLは必須項目です。")]
        [MaxLengthEx(MaxLength = 4000, ErrorMessageFormat = "URLは{0}文字以内で入力して下さい。")]
        [RegularExpression(@"^[^/|^\d](((?![/]{2,})[-/_.a-zA-Z\d])|(\{[-_a-zA-Z\d]+}))+[\?](((?![=]{2,})[-_=&\[\]\.a-zA-Z\d])|(\{[-_a-zA-Z0-9]+}))+$|^[^/|^\d](((?![/]{2,})[-/_.a-zA-Z\d])|(\{[-_.a-zA-Z\d]+}))+$", ErrorMessage = "正しいURLではありません。")]
        [ValidateMethod(MethodItems.MethodUrl)]
        public string MethodUrl { get; set; }

        /// <summary>
        /// リポジトリキー
        /// </summary>
        public string RepositoryKey { get; set; }

        //リクエストスキーマID
        public string RequestSchemaId { get; set; }

        /// <summary>
        /// リクエストスキーマ名
        /// </summary>
        public string RequestSchemaName { get; set; }

        /// <summary>
        /// レスポンススキーマID
        /// </summary>
        public string ResponseSchemaId { get; set; }

        /// <summary>
        /// レスポンススキーマ名
        /// </summary>
        public string ResponseSchemaName { get; set; }

        /// <summary>
        /// UrlスキーマID
        /// </summary>
        public string UrlSchemaId { get; set; }

        /// <summary>
        /// Urlスキーマ名
        /// </summary>
        public string UrlSchemaName { get; set; }

        /// <summary>
        /// PostDataタイプ
        /// </summary>
        public string PostDataType { get; set; }

        /// <summary>
        /// クエリ
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// リポジトリグループID
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// リポジトリグループ名
        /// </summary>
        public string RepositoryGroupName { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// Vendor、System認証をするか
        /// </summary>
        public bool IsHeaderAuthentication { get; set; }

        /// <summary>
        /// OpenId認証をするか
        /// </summary>
        public bool IsOpenIdAuthentication { get; set; }

        /// <summary>
        /// ADMIN認証をするか
        /// </summary>
        public bool IsAdminAuthentication { get; set; }

        /// <summary>
        /// クライアント証明書認証をするかどうか
        /// </summary>
        public bool IsClientCertAuthentication { get; set; }

        /// <summary>
        /// パーティションを超えて検索するか
        /// </summary>
        public bool IsOverPartition { get; set; }

        /// <summary>
        /// GatewayのUrl
        /// </summary>
        [MaxLengthEx(MaxLength = 4000, ErrorMessageFormat = "ゲートウェイURLは{0}文字以内で入力して下さい。")]
        [ValidateMethod(MethodItems.GatewayUrl)]
        public string GatewayUrl { get; set; }

        /// <summary>
        /// GatewayのBASIN認証(ユーザー名)
        /// </summary>
        [MaxLengthEx(MaxLength = 256, ErrorMessageFormat = "ユーザー名は{0}文字以内で入力して下さい。")]
        public string GatewayCredentialUserName { get; set; }

        /// <summary>
        /// GatewayのBASIN認証(パスワード)
        /// </summary>
        [MaxLengthEx(MaxLength = 256, ErrorMessageFormat = "パスワードは{0}文字以内で入力して下さい。")]
        public string GatewayCredentialPassword { get; set; }

        /// <summary>
        /// 非表示か
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// スクリプト
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// アクションタイプコード
        /// </summary>
        [Required(ErrorMessage = "アクションタイプは必須項目です。")]
        public string ActionTypeCd { get; set; }

        /// <summary>
        /// スクリプトタイプコード
        /// </summary>
        [ValidateMethod(MethodItems.Script)]
        public string ScriptTypeCd { get; set; }

        /// <summary>
        /// キャッシュするか
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// キャッシュ時間(分)
        /// </summary>
        [ValidateMethod(MethodItems.CacheMinute)]
        public int CacheMinute { get; set; }

        /// <summary>
        /// キャッシュキー
        /// </summary>
        [MaxLengthEx(MaxLength = 4000, ErrorMessageFormat = "キャッシュキーは{0}文字以内で入力して下さい。")]
        public string CacheKey { get; set; }

        /// <summary>
        /// アクセスキー
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// ID自動割り振りするか
        /// </summary>
        public string Automatic { get; set; }

        /// <summary>
        /// アクションタイプバージョン
        /// </summary>
        public int ActionTypeVersion { get; set; }

        /// <summary>
        /// パーティションキー
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Gateway中継ヘッダー
        /// </summary>
        [MaxLengthEx(MaxLength = 4000, ErrorMessageFormat = "ゲートウェイ中継ヘッダーは{0}文字以内で入力して下さい。")]
        public string GatewayRelayHeader { get; set; }

        /// <summary>
        /// 登録者
        /// </summary>
        public string RegUserName { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdUserName { get; set; }

        /// <summary>
        /// 登録日付
        /// </summary>
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 更新日付
        /// </summary>
        public DateTime UpdDate { get; set; }

        /// <summary>
        /// 論理削除するか
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 透過Methodか
        /// </summary>
        public bool IsTransparent { get; set; }

        /// <summary>
        /// VendorSystem認証を省略できるか
        /// </summary>
        public bool IsVendorSystemAuthenticationAllowNull { get; set; }

        /// <summary>
        /// ログインユーザーにのみ表示
        /// </summary>
        public bool IsVisibleSigninUserOnly { get; set; }

        /// <summary>
        /// クエリタイプコード
        /// </summary>
        public string QueryTypeCd { get; set; }

        /// <summary>
        /// 内部呼び出し専用
        /// </summary>
        public bool IsInternalOnly { get; set; }

        /// <summary>
        /// RDBリポジトリの結合許可
        /// </summary>
        public bool IsOtherResourceSqlAccess { get; set; }

        /// <summary>
        /// 内部呼び出し時のキーワード
        /// </summary>
        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "アクセス許可キーは{0}文字以内で入力して下さい。")]
        public string InternalOnlyKeyword { get; set; }

        /// <summary>
        /// 呼び出し時にJsonバリデーションをスキップする
        /// </summary>
        public bool IsSkipJsonSchemaValidation { get; set; }

        /// <summary>
        /// セカンダリリポジトリグループリスト
        /// </summary>
        public List<SecondaryRepositoryModel> SecondaryRepositoryMapList { get; set; }

        /// <summary>
        /// サンプルコードリスト
        /// </summary>
        public List<SampleCodeModel> SampleCodeList { get; set; }

        /// <summary>
        /// リンクリスト
        /// </summary>
        public List<MethodLinkModel> MethodLinkList { get; set; }

        /// <summary>
        /// アクセスベンダーリスト
        /// </summary>
        public List<AccessVendorModel> AccessVendorList { get; set; }

        /// <summary>
        /// OpenId認証局リスト
        /// </summary>
        public List<MethodOpenIdCaModel> MethodOpenIdCAList { get; set; }

        public class SecondaryRepositoryModel
        {
            /// <summary>
            /// リポジトリグループID
            /// </summary>
            public string RepositoryGroupId { get; set; }

            /// <summary>
            /// リポジトリグループ名
            /// </summary>
            public string RepositoryGroupName { get; set; }

            /// <summary>
            /// プライマリーかどうか
            /// </summary>
            public bool IsPrimary { get; set; }
        }

        public class SampleCodeModel
        {
            /// <summary>
            /// サンプルコードID
            /// </summary>
            public string SampleCodeId { get; set; }

            /// <summary>
            /// LanguageId
            /// </summary>
            public string LanguageId { get; set; }

            /// <summary>
            /// 言語
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// コード
            /// </summary>
            public string Code { get; set; }
        }

        public class MethodLinkModel
        {
            /// <summary>
            /// MethodLinkId
            /// </summary>
            public string MethodLinkId { get; set; }

            /// <summary>
            /// タイトル
            /// </summary>
            [Required(ErrorMessage = "タイトルは必須項目です。")]
            [MaxLengthEx(MaxLength = 100, ErrorMessageFormat = "タイトルは{0}文字以内で入力して下さい。")]

            public string Title { get; set; }

            /// <summary>
            /// Url
            /// </summary>
            [Required(ErrorMessage = "URLは必須項目です。")]
            [Url(ErrorMessage = "正しいURLではありません")]
            [MaxLengthEx(MaxLength = 512, ErrorMessageFormat = "URLは{0}文字以内で入力して下さい。")]
            public string Url { get; set; }

            /// <summary>
            /// 詳細
            /// </summary>
            [Required(ErrorMessage = "説明は必須項目です。")]
            [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "説明は{0}文字以内で入力して下さい。")]
            public string Detail { get; set; }

            /// <summary>
            /// 表示するかどうか
            /// </summary>
            public bool IsVisivle { get; set; }

            /// <summary>
            /// このインスタンスの簡易コピーを取得する。
            /// </summary>
            /// <returns>このインスタンスの簡易コピー。</returns>
            public MethodLinkModel Clone()
            {
                return (MethodLinkModel)this.MemberwiseClone();
            }
        }

        public class AccessVendorModel
        {
            /// <summary>
            /// AccessVendorId
            /// </summary>
            public string AccessVendorId { get; set; }

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
            public string SystemName { get; set; }

            /// <summary>
            /// 有効/無効
            /// </summary>
            public bool IsEnable { get; set; }

            /// <summary>
            /// アクセスキー
            /// </summary>
            public string AccessKey { get; set; }
        }

        public class MethodOpenIdCaModel
        {
            /// <summary>
            /// MethodOpenIdCaId
            /// </summary>
            public string MethodOpenIdCaId { get; set; }

            /// <summary>
            /// アプリケーションID
            /// </summary>
            public string ApplicationId { get; set; }

            /// <summary>
            /// アプリケーション名
            /// </summary>
            public string ApplicationName { get; set; }

            /// <summary>
            /// アクセスコントロール
            /// </summary>
            public string AccessControl { get; set; }

            /// <summary>
            /// このインスタンスの簡易コピーを取得する。
            /// </summary>
            /// <returns>このインスタンスの簡易コピー。</returns>
            public MethodOpenIdCaModel Clone()
            {
                return (MethodOpenIdCaModel)this.MemberwiseClone();
            }
        }
    }
}
