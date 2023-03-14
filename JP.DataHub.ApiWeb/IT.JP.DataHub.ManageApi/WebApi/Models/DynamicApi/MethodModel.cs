using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class RegisterMethodResponseModel
    {
        public string MethodId { get; set; }
    }
    public class RegisterMethodRequestModel
    {
        public string ApiId { get; set; }
        public string Url { get; set; }
        public string ActionTypeCd { get; set; }
        public string HttpMethodTypeCd { get; set; }
        public string MethodDescriptiveText { get; set; }
        public bool IsPostDataTypeArray { get; set; } = false;
        public bool IsAutomaticId { get; set; } = false;
        public string UrlModelId { get; set; }
        public string RequestModelId { get; set; }
        public string ResponseModelId { get; set; }
        public bool IsEnable { get; set; } = true;
        public bool IsHidden { get; set; } = true;
        public bool IsVisibleSigninuserOnly { get; set; }
        public bool IsTransparentApi { get; set; } = false;
        public bool IsHeaderAuthentication { get; set; } = true;
        public bool IsVendorSystemAuthenticationAllowNull { get; set; } = false;
        public bool IsOpenIdAuthentication { get; set; } = false;
        public bool IsAdminAuthentication { get; set; } = false;
        public bool IsClientCertAuthentication { get; set; } = false;
        public bool IsSkipJsonSchemaValidation { get; set; } = false;
        public bool IsAccessKey { get; set; }
        public List<RegisterAccessVendorModel> ApiAccessVendorList { get; set; }
        public bool IsInternalOnly { get; set; } = false;
        public string InternalOnlyKeyword { get; set; }
        public bool IsOtherResourceSqlAccess { get; set; } = false;
        public bool IsOverPartition { get; set; } = false;
        public bool IsCache { get; set; } = false;
        public string CacheMinute { get; set; }
        public string CacheKey { get; set; }
        public string GatewayUrl { get; set; }
        public string GatewayCredentialUserName { get; set; }
        public string GatewayCredentialPassword { get; set; }
        public string GatewayRelayHeader { get; set; }
        public string RepositoryGroupId { get; set; }
        public List<string> SecondaryRepositoryGroupIds { get; set; }
        public string QueryType { get; set; }
        public string Query { get; set; }
        public string ScriptType { get; set; }
        public string Script { get; set; }
        public List<RegisterSampleCodeModel> SampleCodeList { get; set; }
        public List<RegisterApiLinkModel> ApiLinkList { get; set; }
        public List<RegisterResourceOpenIdCaModel> OpenIdCaList { get; set; }

    }
    public class RegisterAccessVendorModel
    {
        /// <summary>
        /// 許可・除外対象のベンダーID
        /// </summary>
        public string VendorId { get; set; }
        /// <summary>
        /// 許可・除外対象のシステムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// アクセスコントロールの許可設定（true:許可 false:非許可）
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// アクセスコントロールのアクセスキー
        /// </summary>
        public string AccessKey { get; set; }
    }
    public class RegisterSampleCodeModel
    {
        /// <summary>
        /// SampleCodeId
        /// </summary>
        public string SampleCodeId { get; set; }

        /// <summary>
        /// LanguageId
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
    public class RegisterApiLinkModel
    {
        /// <summary>
        /// APIリンク表示名
        /// </summary>
        public string LinkTitle { get; set; }

        /// <summary>
        /// APIリンク詳細
        /// </summary>
        public string LinkDetail { get; set; }

        /// <summary>
        /// APIリンクURL
        /// </summary>
        public string LinkUrl { get; set; }
    }

    public class MethodModel
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
        public string MethodType { get; set; }

        /// <summary>
        /// MethodUrl
        /// </summary>
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
        public string GatewayUrl { get; set; }

        /// <summary>
        /// GatewayのBASIN認証(ユーザー名)
        /// </summary>
        public string GatewayCredentialUserName { get; set; }

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
        public string InternalOnlyKeyword { get; set; }

        /// <summary>
        /// 呼び出し時にJsonバリデーションをスキップする
        /// </summary>
        public bool IsSkipJsonSchemaValidation { get; set; }

        /// <summary>
        /// セカンダリリポジトリグループリスト
        /// </summary>
        public IEnumerable<SecondaryRepositoryModel> SecondaryRepositoryMapList { get; set; }

        /// <summary>
        /// サンプルコードリスト
        /// </summary>
        public IEnumerable<DynamicApiSampleCodeModel> SampleCodeList { get; set; }

        /// <summary>
        /// リンクリスト
        /// </summary>
        public IEnumerable<DynamicApiMethodLinkModel> MethodLinkList { get; set; }

        /// <summary>
        /// アクセスベンダーリスト
        /// </summary>
        public IEnumerable<AccessVendorModel> AccessVendorList { get; set; }

        /// <summary>
        /// OpenId認証局リスト
        /// </summary>
        public IEnumerable<MethodOpenIdCAModel> MethodOpenIdCAList { get; set; }

    }
    public class SecondaryRepositoryModel
    {
        /// <summary>SecondaryRepositoryMapId</summary>
        public string SecondaryRepositoryMapId { get; set; }

        /// <summary>RepositoryGroupId</summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>RepositoryGroupName</summary>
        public string RepositoryGroupName { get; set; }

        /// <summary>IsPrimary</summary>
        public bool IsPrimary { get; set; }
    }
    public class DynamicApiSampleCodeModel
    {
        /// <summary>
        /// SampleCodeId
        /// </summary>
        public string SampleCodeId { get; set; }

        /// <summary>
        /// LanguageId
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
    }
    public class DynamicApiMethodLinkModel
    {
        /// <summary>
        /// MethodLinkId
        /// </summary>
        public string MethodLinkId { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 詳細
        /// </summary>
        public string Detail { get; set; }

        // 表示/非表示
        public bool IsVisible { get; set; }
    }
    public class AccessVendorModel
    {
        /// <summary>
        /// AccessVendorId
        /// </summary>
        public string AccessVendorId { get; set; }

        /// <summary>
        /// VendorId
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// SystemId
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
    public class MethodOpenIdCAModel
    {
        /// <summary>
        /// MethodOpenIdCaId
        /// </summary>
        public string MethodOpenIdCaId { get; set; }

        /// <summary>
        /// ApplicationId
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Application名
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// アクセスコントロール
        /// </summary>
        public string AccessControl { get; set; }
    }

}
