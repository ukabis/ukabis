using System.Collections.Generic;
using Newtonsoft.Json;

namespace JP.DataHub.Com.RFC7807
{
    public class RFC7807ProblemDetailExtendErrors : RFC7807ProblemDetail
    {
        [JsonProperty(PropertyName = "errors", NullValueHandling = NullValueHandling.Ignore)]
        public new IDictionary<string, dynamic> Errors { get; set; }

        public RFC7807ProblemDetailExtendErrors()
        {
        }

        public RFC7807ProblemDetailExtendErrors(string errorCode, Dictionary<string, dynamic> errors)
        {
            ErrorCode = errorCode;
            Errors = errors;
        }

        public RFC7807ProblemDetailExtendErrors(Dictionary<string, dynamic> errors)
        {
            Errors = errors;
        }
    }
}
