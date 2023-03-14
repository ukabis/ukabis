using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Resources;
using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.Com.Web
{
    public class HttpResponseResult<T>
    {
        public bool IsSuccessStatusCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T Result { get; set; }
        public string Error { get; set; }
        public Rfc7807Detail ErrorDetail { get; set; }

        public HttpResponseResult()
        {
        }

        public HttpResponseResult(HttpResponseMessage response)
        {
            IsSuccessStatusCode = response.IsSuccessStatusCode;
            StatusCode = response.StatusCode;
            if (IsSuccessStatusCode == false)
            {
                Error = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                var msg = response.Content.ReadAsStringAsync().Result;
                Result = JsonConvert.DeserializeObject<T>(msg);
            }
        }

        public HttpResponseResult(Exception e)
        {
            IsSuccessStatusCode = false;
            Error = e.Message;
        }
    }
}
