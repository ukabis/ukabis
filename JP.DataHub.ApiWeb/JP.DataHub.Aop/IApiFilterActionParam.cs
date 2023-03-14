using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Net;

namespace JP.DataHub.Aop
{
    // .NET6
    /// <summary>
    /// ApiFilterの時に渡すパラメータ
    /// </summary>
    public interface IApiFilterActionParam
    {
        /// <summary>
        /// API実行時のアクション
        /// </summary>
        string Action { get; set; }
        /// <summary>
        /// API定義したベンダーID
        /// </summary>
        string VendorId { get; set; }
        /// <summary>
        /// API定義したシステムID
        /// </summary>
        string SystemId { get; set; }
        /// <summary>
        /// ログインしたOpenIdConnectのOpenID
        /// </summary>
        string OpenId { get; set; }
        /// <summary>
        /// APIのリソースURL
        /// </summary>
        string ResourceUrl { get; set; }
        /// <summary>
        /// APIのAPI-URL
        /// </summary>
        string ApiUrl { get; set; }
        /// <summary>
        /// REQUEST時のメディアタイプ
        /// </summary>
        string MediaType { get; set; }
        /// <summary>
        /// REQUEST時のクエリ文字列
        /// </summary>
        string QueryString { get; set; }
        /// <summary>
        /// QueryStringをkey,valueで分解したディクショナリー
        /// </summary>
        QueryStringDictionary QueryStringDic { get; set; }
        /// <summary>
        /// コンテンツのストリーム
        /// </summary>
        Stream ContentsStream { get; set; }
        /// <summary>
        /// REQUEST時のACCEPTヘッダーの値
        /// </summary>
        string Accept { get; set; }
        /// <summary>
        /// REQUEST時のCONTENT RANGEの値
        /// </summary>
        string ContentRange { get; set; }
        /// <summary>
        /// API呼び出し時のHttpMethod
        /// </summary>
        string HttpMethodType { get; set; }
        /// <summary>
        /// リソースの(json)スキーマ
        /// </summary>
        string ControllerSchema { get; set; }
        /// <summary>
        /// API定義時のURL(json)スキーマ
        /// </summary>
        string UriSchema { get; set; }
        /// <summary>
        /// REQUESTの(json)スキーマ
        /// </summary>
        string RequestSchema { get; set; }
        /// <summary>
        /// RESPONSEの(json)スキーマ
        /// </summary>
        string ResponseSchema { get; set; }
        /// <summary>
        /// API呼び出しのベンダーID
        /// </summary>
        string RequestVendorId { get; set; }
        /// <summary>
        /// API呼び出しのシステムID
        /// </summary>
        string RequestSystemId { get; set; }
        /// <summary>
        /// REQUEST時のヘッダー（これを書き換えると移行の処理のヘッダーも変わる）
        /// </summary>
        Dictionary<string, List<string>> Headers { get; set; }
        /// <summary>
        /// Gatewayの情報
        /// </summary>
        ApiFilterGateway Gateway { get; set; }
        /// <summary>
        /// 透過APIか
        /// </summary>
        bool TransparentApi { get; set; }
        /// <summary>
        /// ActionInjectorを無効化するか
        /// </summary>
        /// <remarks>
        /// APIフィルターのBeforeActionでtrueを設定するとActionInjectorが実行されなくなる。
        /// OpenDataAOPにて透過APIのGateway時に本来のActionInjectorを無効化するために使用。
        /// </remarks>
        bool DisableActionInjector { get; set; }

        IApiHelper ApiHelper { get; set; }
        ITermsHelper TermsHelper { get; set; }
        bool IsOverPartition { get; set; }
        string PostDataType { get; set; }
        CultureInfo LanguageInfo { get; set; }
        /// <summary>
        /// 偽装するベンダーID
        /// </summary>
        string ImpersonateRequestVendorId { get; set; }
        /// <summary>
        /// 偽装するシステムID
        /// </summary>
        string ImpersonateRequestSystemId { get; set; }
        bool IsInternalCall { get; set; }
    }

    /// <summary>
    /// ApiFilterの時に渡すパラメータ
    /// </summary>
    public class ApiFilterActionParam : IApiFilterActionParam
    {
        public string Action { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string OpenId { get; set; }
        public string ResourceUrl { get; set; }
        public string ApiUrl { get; set; }
        public string MediaType { get; set; }
        public string QueryString { get; set; }
        public QueryStringDictionary QueryStringDic { get; set; }
        public Stream ContentsStream { get; set; }
        public string Accept { get; set; }
        public string ContentRange { get; set; }
        public string HttpMethodType { get; set; }
        public string ControllerSchema { get; set; }
        public string UriSchema { get; set; }
        public string RequestSchema { get; set; }
        public string ResponseSchema { get; set; }
        public string RequestVendorId { get; set; }
        public string RequestSystemId { get; set; }
        public Dictionary<string, List<string>> Headers { get; set; }
        public ApiFilterGateway Gateway { get; set; }
        public bool TransparentApi { get; set; }
        public bool DisableActionInjector { get; set; } = false;

        public IApiHelper ApiHelper { get; set; }
        public ITermsHelper TermsHelper { get; set; }
        public bool IsOverPartition { get; set; }
        public string PostDataType { get; set; }
        public CultureInfo LanguageInfo { get; set; }
        public string ImpersonateRequestVendorId { get; set; }
        public string ImpersonateRequestSystemId { get; set; }
        public bool IsInternalCall { get; set; }
    }

    public class ApiFilterGateway
    {
        public string Url { get; set; }
        public string CredentialUsername { get; set; }
        public string CredentialPassword { get; set; }
        public string GatewayRelayHeader { get; set; }
    }
}
