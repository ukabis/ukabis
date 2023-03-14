using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class RegisterMethodModel
    {
        /// <summary>
        /// APIId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// アクション種別
        /// </summary>
        public string ActionTypeCd { get; set; }

        /// <summary>
        /// メソッド種類
        /// </summary>
        public string HttpMethodTypeCd { get; set; }

        /// <summary>
        /// メソッド説明
        /// </summary>
        public string MethodDescriptiveText { get; set; }

        /// <summary>
        /// 配列データか
        /// </summary>
        public bool IsPostDataTypeArray { get; set; } = false;

        /// <summary>
        /// ID自動割り振り
        /// </summary>
        public bool IsAutomaticId { get; set; } = false;

        #region スキーマ

        /// <summary>
        /// URLのスキーマID
        /// </summary>
        public string UrlModelId { get; set; }

        /// <summary>
        /// リクエストのスキーマID
        /// </summary>
        public string RequestModelId { get; set; }

        /// <summary>
        /// レスポンスのスキーマID
        /// </summary>
        public string ResponseModelId { get; set; }

        #endregion

        #region 状態設定

        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// Helpページ非表示
        /// </summary>
        public bool IsHidden { get; set; } = true;

        /// <summary>
        /// Helpページにはサインインユーザーにのみ表示
        /// </summary>
        public bool IsVisibleSigninuserOnly { get; set; }

        /// <summary>
        /// 透過APIかどうか
        /// </summary>
        public bool IsTransparentApi { get; set; } = false;

        #endregion

        #region 認証設定

        /// <summary>
        /// ベンダーシステム認証をするかどうか
        /// </summary>
        public bool IsHeaderAuthentication { get; set; } = true;

        /// <summary>
        /// ベンダーシステム認証で省略を許容するか
        /// </summary>
        public bool IsVendorSystemAuthenticationAllowNull { get; set; } = false;

        /// <summary>
        /// OpenId認証をするかどうか
        /// </summary>
        public bool IsOpenIdAuthentication { get; set; } = false;

        /// <summary>
        /// Admin認証をするかどうか
        /// </summary>
        public bool IsAdminAuthentication { get; set; } = false;

        /// <summary>
        /// クライアント証明書認証をするかどうか
        /// </summary>
        public bool IsClientCertAuthentication { get; set; } = false;

        #endregion

        #region 登録データ設定

        /// <summary>
        /// 登録時のデータチェックをするか
        /// </summary>
        public bool IsSkipJsonSchemaValidation { get; set; } = false;

        #endregion

        #region アクセス設定
        /// <summary>
        /// アクセスコントロール設定有無
        /// </summary>
        public bool IsAccessKey { get; set; }

        /// <summary>
        /// アクセスコントロール有効時のアクセスベンダー許可、除外リスト
        /// MethodAccessVendor+MethodAccessSelfVendor
        /// </summary>
        public List<RegisterAccessVendorModel> ApiAccessVendorList { get; set; }

        /// <summary>
        /// 内部呼び出しのみか
        /// </summary>
        public bool IsInternalOnly { get; set; } = false;

        /// <summary>
        /// アクセス許可キー（内部から呼び出す際のキー）
        /// </summary>
        public string InternalOnlyKeyword { get; set; }

        /// <summary>
        /// RDBリポジトリの結合許可
        /// </summary>
        public bool IsOtherResourceSqlAccess { get; set; } = false;
        #endregion

        #region 検索設定

        /// <summary>
        /// 領域越えをするかどうか
        /// </summary>
        public bool IsOverPartition { get; set; } = false;

        #endregion

        #region キャッシュ設定

        /// <summary>
        /// キャッシュ有無
        /// </summary>
        public bool IsCache { get; set; } = false;

        /// <summary>
        /// キャッシュ時間(分)
        /// </summary>
        public string CacheMinute { get; set; }

        /// <summary>
        /// キャッシュキー
        /// </summary>
        public string CacheKey { get; set; }

        #endregion

        #region Gateway設定

        /// <summary>
        /// ゲートウェイURL
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

        #endregion

        #region リポジトリグループ

        /// <summary>
        /// リポジトリグループID(プライマリ)
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// セカンダリリポジトリグループリスト
        /// </summary>
        public List<string> SecondaryRepositoryGroupIds { get; set; }

        #endregion

        #region コード類

        /// <summary>
        /// Queryタイプ
        /// </summary>
        public string QueryType { get; set; }

        /// <summary>
        /// クエリー
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// スクリプトタイプ
        /// </summary>
        public string ScriptType { get; set; }

        /// <summary>
        /// スクリプト
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// サンプルコード
        /// </summary>
        public List<RegisterSampleCodeModel> SampleCodeList { get; set; }

        #endregion

        /// <summary>
        /// ApiLink
        /// </summary>
        public List<RegisterApiLinkModel> ApiLinkList { get; set; }

        /// <summary>
        /// OpenId認証局
        /// </summary>
        public List<RegisterResourceOpenIdCaModel> OpenIdCaList { get; set; }
    }
}
