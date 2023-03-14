using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JP.DataHub.Api.Core.HttpHeaderConfig
{
    public class HttpHeaderSplit
    {
        public bool IsAllHeader;

        public string[] HeaderKeys;

        public HttpHeaderSplit(string config)
        {
            if (string.IsNullOrEmpty(config) || config == "*")
            {
                IsAllHeader = true;
            }
            else
            {
                var spltedConfig = config.Split(',');
                if (spltedConfig.Contains("*"))
                {
                    IsAllHeader = true;
                }
                else
                {
                    HeaderKeys = spltedConfig.Select(x => x.ToLower().Trim()).ToArray();
                }
            }
        }

        public Dictionary<string, List<string>> FilterHeader(Dictionary<string, List<string>> headers)
        {
            var result = new Dictionary<string, List<string>>();
            headers.ToList().ForEach(x =>
            {
                if (IsAllHeader || HeaderKeys.Contains(x.Key.ToLower()))
                {
                    result.Add(x.Key, x.Value.ToList());
                }
            });
            return result;
        }

        public Dictionary<string, List<string>> FilterHeader(IHeaderDictionary headers)
        {
            var result = new Dictionary<string, List<string>>();
            headers.ToList().ForEach(x =>
            {
                if (IsAllHeader || HeaderKeys.Contains(x.Key.ToLower()))
                {
                    result.Add(x.Key, x.Value.ToList());
                }
            });
            return result;
        }

        public Dictionary<string, List<string>> FilterHeader(HttpHeaders httpHeaders)
        {
            var ret = new Dictionary<string, List<string>>();

            if (IsAllHeader)
            {
                foreach (var header in httpHeaders)
                {
                    ret.Add(header.Key, header.Value.ToList());
                }
            }
            else
            {
                foreach (var header in httpHeaders)
                {
                    if (HeaderKeys.Contains(header.Key.ToLower()))
                    {
                        ret.Add(header.Key, header.Value.ToList());
                    }
                }
            }
            return ret;
        }
    }
}