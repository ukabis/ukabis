using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class ApiQueryModel
    {
        public ApiQueryModel()
        {
            SecondaryRepositoryMapList = new List<SecondaryRepositoryMapQueryModel>();
            SampleCodeList = new List<SampleCodeModel>();
            ApiLinkList = new List<ApiLinkModel>();
            ApiAccessVendorList = new List<ApiAccessVendorModel>();
            ApiOpenIdCAList = new List<OpenIdCaModel>();
        }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string ApiDescription { get; set; }

        /// <summary>
        /// ControllerId
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// メソッドタイプ
        /// </summary>
        public string MethodType { get; set; }

        /// <summary>
        /// ApiUrl
        /// </summary>
        public string ApiUrl { get; set; }

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
        public bool IsClientCertAuthentication { get; set; } = false;

        /// <summary>
        /// パーティションを超えて検索するか
        /// </summary>
        public bool IsOverPartition { get; set; }

        /// <summary>
        /// GatewayのUrl
        /// </summary>
        public string GatewayUrl { get; set; }

        /// <summary>
        /// GatewayのBASIN認証(ユーザー名)
        /// </summary>
        public string GatewayCredentialUserName { get; set; }

        /// <summary>
        /// GatewayのBASIN認証(パスワード)
        /// </summary>
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
        public string ActionTypeCd { get; set; }

        /// <summary>
        /// スクリプトタイプコード
        /// </summary>
        public string ScriptTypeCd { get; set; }

        /// <summary>
        /// キャッシュするか
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// キャッシュ時間(分)
        /// </summary>
        public int CacheMinute { get; set; }

        /// <summary>
        /// キャッシュキー
        /// </summary>
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
        /// 透過APIか
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
        /// 内部呼び出し時のキーワード
        /// </summary>
        public string InternalOnlyKeyword { get; set; }

        /// <summary>
        /// 呼び出し時にJsonバリデーションをスキップする
        /// </summary>
        public bool IsSkipJsonSchemaValidation { get; set; }

        /// <summary>
        /// RDBリポジトリの結合許可
        /// </summary>
        public bool IsOtherResourceSqlAccess { get; set; }

        /// <summary>
        /// セカンダリリポジトリグループリスト
        /// </summary>
        public IEnumerable<SecondaryRepositoryMapQueryModel> SecondaryRepositoryMapList { get; set; }

        /// <summary>
        /// サンプルコードリスト
        /// </summary>
        public IEnumerable<SampleCodeModel> SampleCodeList { get; set; }

        /// <summary>
        /// Apiリンクリスト
        /// </summary>
        public IEnumerable<ApiLinkModel> ApiLinkList { get; set; }

        /// <summary>
        /// アクセスベンダーリスト
        /// </summary>
        public IEnumerable<ApiAccessVendorModel> ApiAccessVendorList { get; set; }

        /// <summary>
        /// OpenId認証局リスト
        /// </summary>
        public IEnumerable<OpenIdCaModel> ApiOpenIdCAList { get; set; }
    }
}
