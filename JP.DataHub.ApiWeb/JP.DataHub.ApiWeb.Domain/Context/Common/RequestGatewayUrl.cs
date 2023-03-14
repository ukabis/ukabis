using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RequestGatewayUrl : IValueObject
    {
        public string Value { get; }

        public RequestGatewayUrl(string value)
        {
            this.Value = value;
        }
        public RequestGatewayUrl(GatewayUri gatewayUrl, QueryStringVO query, UrlParameter keyValue, ApiUri apiUrl)
        {
            this.Value = CreateGatewayUrl(gatewayUrl, query, keyValue, apiUrl);
        }

        /// <summary>
        /// アクセス用のURLを生成する
        /// </summary>
        private string CreateGatewayUrl(GatewayUri gatewayUrl, QueryStringVO queryString, UrlParameter keyValue, ApiUri apiUrl)
        {
            var gatewayUri = new UriBuilder(gatewayUrl.Value);
            var RequestParameters = GetRequestParameters(queryString, keyValue, apiUrl);
            var requestUrl = new UriBuilder(gatewayUrl.Value);

            //Urlパラメータの置き換え
            requestUrl.Path = CreateGatewayUrlPath(System.Web.HttpUtility.UrlDecode(gatewayUri.Path), gatewayUrl.Value, RequestParameters);

            //QueryStringパラメータの置き換え
            if (gatewayUri.Query != null)
            {
                var requestQueryString = CreateGatewayQueryString(System.Web.HttpUtility.UrlDecode(gatewayUri.Query), RequestParameters);
                if (!string.IsNullOrEmpty(requestQueryString))
                {
                    requestUrl.Query = requestQueryString;
                }
            }

            //GatewayURLにパラメータクエリが指定されていない場合はQueryStringをそのまま中継する
            if (string.IsNullOrEmpty(gatewayUri.Query))
            {
                if (queryString != null)
                {
                    var keys = keyValue.Dic.Select(x => x.Key.Value.ToString()).ToList();
                    var requestQuery = queryString.GetQueryString(keys);
                    if (!string.IsNullOrEmpty(requestQuery))
                    {
                        requestUrl.Query = requestQuery;
                    }
                }
            }

            return requestUrl.Uri.ToString();
        }

        private string CreateGatewayUrlPath(string urlPath, string gatewayUrl, Dictionary<string, string> requestParameters)
        {
            //Urlパラメータの置き換え
            foreach (var replace in requestParameters.Where(x => !string.IsNullOrEmpty(x.Value)))
            {
                string key = $"{{{replace.Key}}}";
                urlPath = urlPath.Replace(key, replace.Value);
            }
            //URLパラメータからパラメータに指定されなかったものを削除
            var pars = urlPath.Split('/');
            var urlKeys = GetUrlKeyList(gatewayUrl);
            if (!urlKeys.Contains(pars[pars.Length - 1]))
            {
                for (int i = pars.Length - 1; i > 0; i--)
                {
                    if (urlKeys.Contains(pars[i]))
                    {
                        throw new Exception($"Parameter is not set : {pars[i]}");
                    }
                }
            }
            foreach (var k in GetUrlKeyList(gatewayUrl))
            {
                urlPath = urlPath.Replace("/" + k, "");
            }
            return urlPath;
        }

        private string CreateGatewayQueryString(string queryString, Dictionary<string, string> requestParameters)
        {
            if (string.IsNullOrEmpty(queryString)) return queryString;
            if (queryString.IndexOf("?") == 0)
            {
                queryString = queryString.Substring(1, queryString.Length - 1);
            }
            foreach (var replace in requestParameters)
            {
                string key = $"{{{replace.Key}}}";
                queryString = queryString.Replace(key, replace.Value);
            }
            //クエリストリングからパラメータに指定されなかったものを削除
            var queryPars = queryString.Split('&');
            foreach (var queryPar in queryPars)
            {
                var keyValue = queryPar.Split('=');
                if (keyValue.Length == 2)
                {
                    if (keyValue[1].IndexOf("{") == 0 && keyValue[1].LastIndexOf("}") == keyValue[1].Length - 1)
                    {
                        queryString = queryString.Replace(queryPar, "");
                    }
                }
            }
            queryString = queryString.Replace("&&", "&");
            if (queryString.Length > 0 && queryString.LastIndexOf("&") == queryString.Length - 1)
            {
                queryString = queryString.Substring(0, queryString.Length - 1);
            }
            return queryString;
        }

        /// <summary>
        /// リクエストのパラメータを取得する
        /// URLパラメータとQueryStringのパラメータのKeyValueをDictionaryにする
        /// </summary>
        private Dictionary<string, string> GetRequestParameters(QueryStringVO query, UrlParameter keyValue, ApiUri apiUrl)
        {
            Dictionary<string, string> RequestKeyValue = new Dictionary<string, string>();
            if (query != null)
            {
                var apiUrlSplits = apiUrl.Value.Split('?');
                if (apiUrlSplits.Length == 2)
                {
                    foreach (var queryPar in apiUrlSplits[1].Split('&'))
                    {
                        var qp = queryPar.Split('=');
                        if (qp.Length == 2)
                        {
                            var q = query.Dic.Where(x => x.Key.Value == qp[0]).FirstOrDefault();
                            if (q.Value != null)
                            {
                                RequestKeyValue.Add(qp[1].Replace("{", "").Replace("}", ""), q.Value.Value);
                            }
                        }
                    }
                }
            }
            if (keyValue != null)
            {
                foreach (var k in keyValue.Dic)
                {
                    RequestKeyValue.Add(k.Key.Value, k.Value.Value);
                }
            }
            return RequestKeyValue;
        }
        private static List<string> GetUrlKeyList(string url)
        {
            url = System.Web.HttpUtility.UrlDecode(url);
            List<string> result = new List<string>();
            var gatewayUrls = url.Split('?');
            var matchedObjects = System.Text.RegularExpressions.Regex.Match(gatewayUrls[0], @"\{.+?\}");
            while (matchedObjects.Success)
            {
                result.Add(matchedObjects.Value);
                matchedObjects = matchedObjects.NextMatch();
            }
            return result;
        }
    }
}
