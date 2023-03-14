using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    internal class UpdateValue
    {
        public string Url { get; set; }
        public string ControllerUrl { get; set; }
        public string OriginalProperty { get; set; }
        public string Property { get; set; }
        public object Value { get; set; }
        public object RollbackValue { get; set; }
        public HttpMethod TargetHttpMethod { get; set; } = new HttpMethod("Patch");
        public string RollbackUrl { get; set; }
        public HttpMethod RollbackHttpMethod { get; set; } = new HttpMethod("Patch");
    }
}
