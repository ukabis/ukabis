using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class StreamContentExtensions
    {
        public static StreamContent ToStreamContent(this string data, Encoding encoding = null) => new StreamContent(data.ToStream(encoding));
    }
}
