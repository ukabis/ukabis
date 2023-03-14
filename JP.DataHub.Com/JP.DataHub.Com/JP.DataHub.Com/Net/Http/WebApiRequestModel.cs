using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web;

namespace JP.DataHub.Com.Net.Http
{
    public class WebApiRequestModel
    {
        private string _serverUrl = null;

        public string ServerUrl { get => _serverUrl ?? UnityCore.ResolveDefaultTo<string>("ServerUrl", null); set => _serverUrl = value; }

        public IResource Resource { get; set; }

        public string ResourceUrl { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public string Contents { get; set; }

        public Stream ContentsStream { get; set; }

        public string MediaType { get; set; } = MediaTypeConst.ApplicationJson;

        public ContentRange ContentRange { get; set; }

        public string RequestRelativeUri
        {
            get
            {
                string resourceName = null;
                if (Resource != null)
                {
                    resourceName = Resource.GetType().GetCustomAttribute<WebApiResourceAttribute>()?.ResourceName;
                }
                if (!string.IsNullOrEmpty(ResourceUrl))
                {
                    resourceName = ResourceUrl;
                }
                if (string.IsNullOrEmpty(resourceName))
                {
                    throw new NullReferenceException("API呼出のリソースが指定されていません。");
                }
                string qs = string.IsNullOrEmpty(QueryString) ? null : QueryString.StartsWith("?") ? QueryString : "?" + QueryString;
                return resourceName.UriCombineToString(Action).QueryStringCombine(qs);
            }
        }

        public string Action { get; set; }

        public string QueryString { get; set; }

        public Dictionary<string, string[]> Header { get; set; }

        #region ヘルパーメソッド

        public static WebApiRequestModel Get(string action, string queryString, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel()
            {
                HttpMethod = HttpMethod.Get,
                Action = action,
                QueryString = queryString,
                Header = header
            };
        }

        public static WebApiRequestModel Post(string action, string queryString, string content, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel()
            {
                HttpMethod = HttpMethod.Post,
                Action = action,
                QueryString = queryString,
                Contents = content,
                Header = header
            };
        }

        public static WebApiRequestModel Post(string action, string queryString, Stream contentStream, string mediaType, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel()
            {
                HttpMethod = HttpMethod.Post,
                Action = action,
                QueryString = queryString,
                ContentsStream = contentStream,
                MediaType = mediaType,
                Header = header
            };
        }

        public static WebApiRequestModel Patch(string action, string queryString, string content, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel()
            {
                HttpMethod = new HttpMethod("PATCH"),
                Action = action,
                QueryString = queryString,
                Contents = content,
                Header = header
            };
        }

        public static WebApiRequestModel Patch(string action, string queryString, Stream contentStream, string mediaType, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel()
            {
                HttpMethod = new HttpMethod("PATCH"),
                Action = action,
                QueryString = queryString,
                ContentsStream = contentStream,
                MediaType = mediaType,
                Header = header
            };
        }

        public static WebApiRequestModel Delete(string action, string queryString, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel()
            {
                HttpMethod = HttpMethod.Delete,
                Action = action,
                QueryString = queryString,
                Header = header
            };
        }

        #endregion
    }

    public class WebApiRequestModel<T> : WebApiRequestModel
    {
        public new string RequestRelativeUri
        {
            get
            {
                // リソース名の優先順位(1の方が高い)
                // 1.IResourceのImplクラスのメソッドのWebApiResourceAttribute
                // 2.IResourceから派生したリソースクラスのResultModelのWebApiResourceAttribute
                // 3.ResultModelのWebApiResourceAttribute

                var resourceName = typeof(T).GetCustomAttribute<WebApiResourceAttribute>()?.ResourceName;
                // ResourceがあるならResourceの属性から取得する
                if (Resource != null)
                {
                    resourceName = Resource.GetType().GetCustomAttribute<WebApiResourceAttribute>()?.ResourceName;
                }
                // メソッドのWebApiResource属性の方が強い
                if (!string.IsNullOrEmpty(ResourceUrl))
                {
                    resourceName = ResourceUrl;
                }
                if (string.IsNullOrEmpty(resourceName))
                {
                    throw new NullReferenceException("API呼出のリソースが指定されていません。");
                }
                string qs = string.IsNullOrEmpty(QueryString) ? null : QueryString.StartsWith("?") ? QueryString : "?" + QueryString;
                return resourceName.UriCombineToString(Action).QueryStringCombine(qs);
            }
        }

        #region ヘルパーメソッド

        public new static WebApiRequestModel<T> Get(string action, string queryString, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel<T>()
            {
                HttpMethod = HttpMethod.Get,
                Action = action,
                QueryString = queryString,
                Header = header
            };
        }

        public new static WebApiRequestModel<T> Post(string action, string queryString, string content, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel<T>()
            {
                HttpMethod = HttpMethod.Post,
                Action = action,
                QueryString = queryString,
                Contents = content,
                Header = header
            };
        }

        public new static WebApiRequestModel<T> Post(string action, string queryString, Stream contentStream, string mediaType, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel<T>()
            {
                HttpMethod = HttpMethod.Post,
                Action = action,
                QueryString = queryString,
                ContentsStream = contentStream,
                MediaType = mediaType,
                Header = header
            };
        }

        public new static WebApiRequestModel<T> Patch(string action, string queryString, string content, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel<T>()
            {
                HttpMethod = new HttpMethod("PATCH"),
                Action = action,
                QueryString = queryString,
                Contents = content,
                Header = header
            };
        }

        public new static WebApiRequestModel<T> Patch(string action, string queryString, Stream contentStream, string mediaType, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel<T>()
            {
                HttpMethod = new HttpMethod("PATCH"),
                Action = action,
                QueryString = queryString,
                ContentsStream = contentStream,
                MediaType = mediaType,
                Header = header
            };
        }

        public new static WebApiRequestModel<T> Delete(string action, string queryString, Dictionary<string, string[]> header = null)
        {
            return new WebApiRequestModel<T>()
            {
                HttpMethod = HttpMethod.Delete,
                Action = action,
                QueryString = queryString,
                Header = header
            };
        }

        #endregion
    }

    public class ContentRange
    {
        public long From { get; set; }
        public long To { get; set; }
        public long Length { get; set; }
    }
}
