using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Core.DataContainer
{
    internal class RequestHeaderSync<T>
    {
        private IDictionary<string, List<string>> headers;
        private string headerName;

        public RequestHeaderSync(IDictionary<string, List<string>> headers, string headerName)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentException("リクエストヘッダー名にnullまたは空文字を指定することはできません。");
            }
            this.headerName = headerName;
            this.headers = headers;
        }

        public T GetValue()
        {
            if (this.headers?.ContainsKey(this.headerName) == true)
            {
                return this.headers[headerName].FirstOrDefault().To<T>();
            }
            else
            {
                return default(T);
            }
        }

        public void SetValue(T val)
        {
            if (val == null)
            {
                this.headers.Remove(this.headerName);
            }
            else
            {
                this.headers[this.headerName] = new List<string>() { val.ToString() };
            }
        }
    }
}
