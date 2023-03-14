using JP.DataHub.Com.DataAnnotations;

namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    public class AsyncApiResultModel : AbstractArgumentModel
    {
        /// <summary>
        /// 要求Id
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 実行状態
        /// Wait
        /// Start
        /// End
        /// Error のいずれか
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 要求開始日
        /// </summary>
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// 終了日
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
