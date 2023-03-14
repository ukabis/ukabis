namespace JP.DataHub.Batch.AsyncDynamicApi.Models
{
    public class AsyncDynamicApiStatusModel
    {
        public string RequestId { get; set; }

        public string Status { get; set; }

        public DateTime? RequestDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan? ExecutionTime { get; set; }

        public string ResultPath { get; set; }

        public string OpenId { get; set; }

        public string VendorId { get; set; }

        public string SystemId { get; set; }

        public string Url { get; set; }

        public string QueryString { get; set; }

        public string RequestBody { get; set; }

        public string MethodType { get; set; }

        public string Accept { get; set; }

        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// DynamicAPIの処理開始済かどうかを表す
        /// 処理開始時に既にtrueであればリトライ
        /// </summary>
        public bool InProcess { get; set; } = false;
    }
}
