using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public static class HttpHeaderModelExtensions
    {
        public static HttpHeaderModel ToDomainHttpHeader(this System.Net.Http.Headers.HttpHeaders httpHeader)
        {
            return new HttpHeaderModel(httpHeader.ToDictionary(x => x.Key, y => y.Value.ToList()));
        }
    }
}
