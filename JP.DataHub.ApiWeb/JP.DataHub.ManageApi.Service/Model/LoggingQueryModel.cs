namespace JP.DataHub.ManageApi.Service.Model
{
    public class LoggingQueryModel
    {
        /// <summary>
        /// ログID
        /// </summary>
        public Guid LogId { get; set; }

        /// <summary>
        /// コントローラーID
        /// </summary>
        public Guid ControllerId { get; set; }

        /// <summary>
        /// ApiID
        /// </summary>
        public Guid ApiId { get; set; }

        /// <summary>
        /// リクエスト日付
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// 実行時間
        /// </summary>
        public long ExecuteTime { get; set; }

        /// <summary>
        /// ステータスコード
        /// </summary>
        public string HttpStatusCode { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public Guid VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// オープンID
        /// </summary>
        public Guid OpenId { get; set; }

        /// <summary>
        /// 提供元ベンダーID
        /// </summary>
        public Guid ProviderVendorId { get; set; }

        /// <summary>
        /// 提供元システムID
        /// </summary>
        public Guid ProviderSystemId { get; set; }

        /// <summary>
        /// IpAddress
        /// </summary>
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// コントローラー名
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// アクション名
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// メソッドタイプ
        /// </summary>
        public string HttpMethodType { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// クエリストリング
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// リクエストのHeader
        /// </summary>
        public string RequestHeaders { get; set; }

        /// <summary>
        /// リクエストのContentType
        /// </summary>
        public string RequestContentType { get; set; }

        /// <summary>
        /// リクエストのContentLength
        /// </summary>
        public string RequestContentLength { get; set; }

        /// <summary>
        /// リクエストのBody
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// レスポンスのHeader
        /// </summary>
        public string ResponseHeaders { get; set; }

        /// <summary>
        /// レスポンスのContentType
        /// </summary>
        public string ResponseContentType { get; set; }

        /// <summary>
        /// レスポンスのContentLength
        /// </summary>
        public string ResponseLength { get; set; }

        /// <summary>
        /// レスポンスのBody
        /// </summary>
        public string ResponseBody { get; set; }

        /// <summary>
        /// リクエスト日付(Ymd)
        /// </summary>
        public DateTime RequestDateYmd { get; set; }

        /// <summary>
        /// リクエスト日付(YmdH)
        /// </summary>
        public DateTime RequestDateYmdH { get; set; }

        /// <summary>
        /// リクエスト日付(YmdHm)
        /// </summary>
        public DateTime RequestDateYmdHm { get; set; }
    }
}
