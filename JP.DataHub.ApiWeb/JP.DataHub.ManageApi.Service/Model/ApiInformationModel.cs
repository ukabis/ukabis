using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ApiInformationModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// コントローラURL
        /// </summary>
        public string ControllerUrl { get; set; }

        /// <summary>
        /// APIID
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// APIの説明
        /// </summary>
        public string ApiDescription { get; set; }

        /// <summary>
        /// コントローラID
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// StaticApiか
        /// </summary>
        public bool IsStaticApi { get; set; }

        /// <summary>
        /// メソッドタイプ
        /// </summary>
        public string MethodType { get; set; }

        /// <summary>
        /// APIURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// リポジトリキー
        /// </summary>
        public string RepositoryKey { get; set; }

        /// <summary>
        /// パーティションキー
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// リクエストスキーマID
        /// </summary>
        public string RequestSchemaId { get; set; }

        /// <summary>
        /// レスポンススキーマID
        /// </summary>
        public string ResponseSchemaId { get; set; }

        /// <summary>
        /// URLスキーマID
        /// </summary>
        public string UrlSchemaId { get; set; }

        /// <summary>
        /// ポストデータタイプ
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
        /// セカンダリリポジトリグループIDリスト
        /// </summary>
        public List<string> SecondaryRepositoryGroupIds { get; set; } = new();

        /// <summary>
        /// 有効か
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// ベンダーシステム認証するか
        /// </summary>
        public bool IsHeaderAuthentication { get; set; }

        /// <summary>
        /// ベンダーシステム認証で省略を許容するか
        /// </summary>
        public bool IsVendorSystemAuthenticationAllowNull { get; set; }

        /// <summary>
        /// OpenId認証するか
        /// </summary>
        public bool IsOpenIdAuthentication { get; set; }

        /// <summary>
        /// Admin認証するか
        /// </summary>
        public bool IsAdminAuthentication { get; set; }

        /// <summary>
        /// パーティションを超えて検索するか
        /// </summary>
        public bool IsOverPartition { get; set; }

        /// <summary>
        /// GetwayのURL
        /// </summary>
        public string GatewayUrl { get; set; }

        /// <summary>
        /// ゲートウェイ先認証情報：ユーザー名
        /// </summary>
        public string GatewayCredentialUserName { get; set; }

        /// <summary>
        /// ゲートウェイ先認証情報：パスワード
        /// </summary>
        public string GatewayCredentialPassword { get; set; }

        /// <summary>
        /// ゲートウェイ中継ヘッダー
        /// </summary>
        public string GatewayRelayHeader { get; set; }

        /// <summary>
        /// Helpページ非表示
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Helpページにはサインインユーザーにのみ表示
        /// </summary>
        public bool IsVisibleSigninuserOnly { get; set; }

        /// <summary>
        /// アクティブか
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// スクリプト
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// スクリプトタイプ
        /// </summary>
        public string ScriptType { get; set; }

        /// <summary>
        /// アクションタイプ
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// キャッシュ有無
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// キャッシュ時間(分)
        /// </summary>
        public int? CacheMinute { get; set; }

        /// <summary>
        /// キャッシュキー
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// アクセスコントロール設定有無
        /// </summary>
        public bool IsAccessKey { get; set; }

        /// <summary>
        /// ID自動割り振り
        /// </summary>
        public bool IsAutomaticId { get; set; }

        /// <summary>
        /// 透過APIかどうか
        /// </summary>
        public bool IsTransparentApi { get; set; }

        /// <summary>
        /// Queryタイプ
        /// </summary>
        public string QueryType { get; set; }

        /// <summary>
        /// 内部呼び出しのみか
        /// </summary>
        public bool IsInternalOnly { get; set; }

        /// <summary>
        /// アクセス許可キー（内部から呼び出す際のキー）
        /// </summary>
        public string InternalOnlyKeyword { get; set; }

        /// <summary>
        /// 登録時のデータチェックをするか
        /// </summary>
        public bool IsSkipJsonSchemaValidation { get; set; }

        /// <summary>
        /// RDBリポジトリの結合許可
        /// </summary>
        public bool IsOtherResourceSqlAccess { get; set; }

        /// <summary>
        /// クライアント証明書認証をするかどうか
        /// </summary>
        public bool IsClientCertAuthentication { get; set; }

        /// <summary>
        /// APIアクセスベンダーリスト
        /// </summary>
        public List<ApiAccessVendorModel> ApiAccessVendorList { get; set; } = new();

        /// <summary>
        /// セカンダリリポジトリグループリスト
        /// </summary>
        public IList<SecondaryRepositoryMapModel> SecondaryRepositoryMapList { get; set; } = new List<SecondaryRepositoryMapModel>();

        /// <summary>
        /// APIリンクリスト
        /// </summary>
        public List<ApiLinkModel> ApiLinkList { get; set; } = new();

        /// <summary>
        /// サンプルコードリスト
        /// </summary>
        public List<SampleCodeModel> SampleCodeList { get; set; } = new();

        /// <summary>
        /// 返却メッセージ
        /// </summary>
        public (string ReturnMessageType, string Message) ReturnMessage { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public IEnumerable<OpenIdCaModel> OpenIdCaList { get; set; } = new List<OpenIdCaModel>();

        /// <summary>
        /// 添付ファイル
        /// </summary>
        public AttachFileSettingsModel Attachfile { get; set; } = new();

        /// <summary>
        /// マルチランゲージ対応リソース
        /// </summary>
        public List<ApiMultiLanguageModel> ApiMultiLanguageList { get; set; } = new();
    }
}
