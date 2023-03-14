using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingSummary.Models
{
    public class SummaryResultModel
    {
        public DateTime requestdate { get; set; }

        public Guid controller_id { get; set; }

        public Guid api_id { get; set; }

        public string url { get; set; }

        public string httpmethodtype { get; set; }

        public Guid system_id { get; set; }

        public Guid vendor_id { get; set; }

        public string system_name { get; set; }

        public string vendor_name { get; set; }

        public int execute_count { get; set; }

        public int successes { get; set; }

        public int failure { get; set; }
        public int running { get; set; }

        public long execute_time { get; set; }

        public long request_contentlength { get; set; }

        public long response_contentlength { get; set; }

        public long ConvertExecuteTime
        {
            get
            {
                return Convert.ToInt64(new TimeSpan(execute_time).TotalMilliseconds);

            }

        }
    }
}