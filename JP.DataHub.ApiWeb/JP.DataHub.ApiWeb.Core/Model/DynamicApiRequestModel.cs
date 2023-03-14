using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class DynamicApiRequestModel
    {
        public string HttpMethod { get; set; }

        public string Contents { get; set; }

        public Stream ContentsStream { get; set; }

        public string ContentType { get; set; }

        public long? ContentLength { get; set; }

        public HttpHeaderModel Header { get; set; }

        public string MediaType { get; set; }

        public string RelativeUri { get; set; }

        public string QueryString { get; set; }

        public string Accept { get; set; }

        public string ContentRange { get; set; }

        public Stream GetContentsStream()
        {
            if (string.IsNullOrEmpty(Contents))
            {
                return ContentsStream;
            }
            else
            {
                return Contents.ToStream();
            }
        }
    }
}
