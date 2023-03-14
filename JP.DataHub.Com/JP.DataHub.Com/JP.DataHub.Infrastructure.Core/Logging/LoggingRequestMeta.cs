using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Logging
{
    public class LoggingRequestMeta
    {
        public string http_method_type { get; set; }
        public string relative_url { get; set; }
        public string query_string { get; set; }
        public Dictionary<string, List<string>> header { get; set; }
        public string media_type { get; set; }
        public string content_type { get; set; }
        public long? content_length { get; set; }

        public LoggingRequestMeta()
        {
        }

        public LoggingRequestMeta(string http_method_type, string relative_url, string query_string, Dictionary<string, List<string>> header, string media_type, string content_type, long? content_length)
        {
            this.http_method_type = http_method_type;
            this.relative_url = relative_url;
            this.query_string = query_string;
            this.header = header;
            this.media_type = media_type;
            this.content_type = content_type;
            this.content_length = content_length;
        }
    }
}
