using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace JP.DataHub.Com.Net.Http
{
    internal class HttpClientFactory : IHttpClientFactory
    {
        public static IDictionary<string, HttpClient> _dic = new ConcurrentDictionary<string, HttpClient>();
        public static object obj = new object();

        private static IHttpClientFactory _instance;
        private static HttpMessageHandler _httpMessageHandler;

        public static IHttpClientFactory CreateInstance(HttpMessageHandler httpMessageHandler = null)
        {
            _httpMessageHandler = httpMessageHandler;
            if (_instance == null)
            {
                _instance = new HttpClientFactory();
            }
            return _instance;
        }

        public HttpClient CreateClient(string name)
        {
            lock (obj)
            {
                if (_dic.ContainsKey(name))
                {
                    return _dic[name];
                }
                else
                {
                    var instance = _httpMessageHandler == null ? new HttpClient() : new HttpClient(_httpMessageHandler);
                    _dic.Add(name, instance);
                    return instance;
                }
            }
        }
    }
}
