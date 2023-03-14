using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace JP.DataHub.Com.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsFail(this HttpStatusCode code)
            => code != HttpStatusCode.OK && code != HttpStatusCode.NotFound;
    }
}
