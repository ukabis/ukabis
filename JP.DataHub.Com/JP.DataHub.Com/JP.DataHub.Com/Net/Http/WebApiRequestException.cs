using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http
{
    public class WebApiRequestException : Exception
    {
        public WebApiResponseResult WebApiResponseResult { get; set; }

        public WebApiRequestException()
        {
        }

        public WebApiRequestException(string message = null, Exception innnerException = null)
            : base(message, innnerException)
        {
        }

        public WebApiRequestException(WebApiResponseResult responseResult, string message = null, Exception innnerException = null)
            : base(message, innnerException)
        {
            WebApiResponseResult = responseResult;
        }
    }

    public class WebApiRequestException<T> : WebApiRequestException where T : new()
    {
        public WebApiResponseResult<T> WebApiResponseResult_T { get; set; }

        public WebApiRequestException()
        {
        }

        public WebApiRequestException(WebApiResponseResult<T> responseResult, string message = null, Exception innnerException = null)
            : base(message, innnerException)
        {
            WebApiResponseResult_T = responseResult;
        }
    }

    public class WebApiRequestSimpleException : Exception
    {
        public WebApiResponseResultSimple WebApiResponseResult { get; set; }

        public WebApiRequestSimpleException()
        {
        }

        public WebApiRequestSimpleException(string message = null, Exception innnerException = null)
            : base(message, innnerException)
        {
        }

        public WebApiRequestSimpleException(WebApiResponseResultSimple responseResult, string message = null, Exception innnerException = null)
            : base(message, innnerException)
        {
            WebApiResponseResult = responseResult;
        }
    }

    public class WebApiRequestSimpleException<T> : WebApiRequestException where T : new()
    {
        public WebApiResponseResultSimple<T> WebApiResponseResult_T { get; set; }

        public WebApiRequestSimpleException()
        {
        }

        public WebApiRequestSimpleException(WebApiResponseResultSimple<T> responseResult, string message = null, Exception innnerException = null)
            : base(message, innnerException)
        {
            WebApiResponseResult_T = responseResult;
        }
    }
}
