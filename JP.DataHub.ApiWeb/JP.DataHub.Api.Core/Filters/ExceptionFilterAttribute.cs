using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.MVC.Filters;
using JP.DataHub.Api.Core.ErrorCode;

namespace JP.DataHub.Api.Core.Filters
{
    /// <summary>
    /// ControllerクラスのActionメソッド中に発生した例外によって、HttpStatusCodeを返す
    /// コンストラクターにはExceptionの型、HttpStatusCodeを指定する
    /// この組み合わせは複数しているすることもできる
    /// もし発生したExceptionの型が、コンストラクタのException型に合致しない場合は
    /// Exception型としてnullを指定したHttpStatusCodeを返す
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExceptionFilterAttribute : ActionFilterAttribute
    {
        private IDictionary<Type, HttpStatusCode> _map = new Dictionary<Type, HttpStatusCode>();
        private IDictionary<Type, ErrorCodeMessage.Code> _mapRfc7807 = new Dictionary<Type, ErrorCodeMessage.Code>();
        private HttpStatusCode? _allHttpStatusCode;

        public ExceptionFilterAttribute()
        {
        }

        public ExceptionFilterAttribute(IDictionary<Type, HttpStatusCode> dic)
        {
            _map = dic;
        }

        public ExceptionFilterAttribute(Type exceptionType, ErrorCodeMessage.Code errorCode)
        {
            Add(exceptionType, errorCode);
        }

        public ExceptionFilterAttribute(Type exceptionType1, ErrorCodeMessage.Code errorCode1, Type exceptionType2, ErrorCodeMessage.Code errorCode2)
        {
            Add(exceptionType1, errorCode1);
            Add(exceptionType2, errorCode2);
        }

        public ExceptionFilterAttribute(Type exceptionType1, ErrorCodeMessage.Code errorCode1, Type exceptionType2, ErrorCodeMessage.Code errorCode2, Type exceptionType3, ErrorCodeMessage.Code errorCode3)
        {
            Add(exceptionType1, errorCode1);
            Add(exceptionType2, errorCode2);
            Add(exceptionType3, errorCode3);
        }

        public ExceptionFilterAttribute(Type exceptionType1, ErrorCodeMessage.Code errorCode1, Type exceptionType2, ErrorCodeMessage.Code errorCode2, Type exceptionType3, ErrorCodeMessage.Code errorCode3, Type exceptionType4, ErrorCodeMessage.Code errorCode4)
        {
            Add(exceptionType1, errorCode1);
            Add(exceptionType2, errorCode2);
            Add(exceptionType3, errorCode3);
            Add(exceptionType4, errorCode4);
        }

        public ExceptionFilterAttribute(Type exceptionType, HttpStatusCode httpStatusCode)
        {
            Add(exceptionType, httpStatusCode);
        }

        public ExceptionFilterAttribute(Type[] exceptionType, HttpStatusCode httpStatusCode)
        {
            exceptionType.ToList().ForEach(x => Add(x, httpStatusCode));
        }

        public ExceptionFilterAttribute(Type exceptionType1, HttpStatusCode httpStatusCode1, Type exceptionType2, HttpStatusCode httpStatusCode2)
        {
            Add(exceptionType1, httpStatusCode1);
            Add(exceptionType2, httpStatusCode2);
        }

        public ExceptionFilterAttribute(Type[] exceptionType1, HttpStatusCode httpStatusCode1, Type[] exceptionType2, HttpStatusCode httpStatusCode2)
        {
            exceptionType1.ToList().ForEach(x => Add(x, httpStatusCode1));
            exceptionType2.ToList().ForEach(x => Add(x, httpStatusCode2));
        }

        public ExceptionFilterAttribute(Type exceptionType1, HttpStatusCode httpStatusCode1, Type[] exceptionType2, HttpStatusCode httpStatusCode2)
        {
            Add(exceptionType1, httpStatusCode1);
            exceptionType2.ToList().ForEach(x => Add(x, httpStatusCode2));
        }

        public ExceptionFilterAttribute(Type[] exceptionType1, HttpStatusCode httpStatusCode1, Type exceptionType2, HttpStatusCode httpStatusCode2)
        {
            exceptionType1.ToList().ForEach(x => Add(x, httpStatusCode1));
            Add(exceptionType2, httpStatusCode2);
        }

        public ExceptionFilterAttribute(Type exceptionType1, HttpStatusCode httpStatusCode1, Type exceptionType2, HttpStatusCode httpStatusCode2, Type exceptionType3, HttpStatusCode httpStatusCode3)
        {
            Add(exceptionType1, httpStatusCode1);
            Add(exceptionType2, httpStatusCode2);
            Add(exceptionType3, httpStatusCode3);
        }

        public ExceptionFilterAttribute(Type exceptionType1, HttpStatusCode httpStatusCode1, Type exceptionType2, HttpStatusCode httpStatusCode2, Type exceptionType3, HttpStatusCode httpStatusCode3, Type exceptionType4, HttpStatusCode httpStatusCode4)
        {
            Add(exceptionType1, httpStatusCode1);
            Add(exceptionType2, httpStatusCode2);
            Add(exceptionType3, httpStatusCode3);
            Add(exceptionType4, httpStatusCode4);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var type = context.Exception.GetType();
                if (_map.ContainsKey(type))
                {
                    context.Result = CreateResult(context.Exception, _map[type].ToString(), (int)_map[type]);
                    context.ExceptionHandled = true;
                }
                else if (_mapRfc7807.ContainsKey(type))
                {
                    var rfc7807 = ErrorCodeMessage.GetRFC7807(_mapRfc7807[type]);
                    context.Result = new ObjectResult(JsonConvert.SerializeObject(new HttpResponseExceptionFilter.ExceptionResponseBody(rfc7807))) { StatusCode = rfc7807.Status };
                    context.ExceptionHandled = true;
                }
                else if (_allHttpStatusCode != null)
                {
#if (DEBUG)
                    context.Result = CreateResult(context.Exception, _allHttpStatusCode.Value.ToString(), (int)_allHttpStatusCode.Value);
#else
                    context.Result = new StatusCodeResult((int)_allHttpStatusCode.Value);
#endif
                    context.ExceptionHandled = true;
                }
                if (context.ExceptionHandled == false)
                {
#if (DEBUG)
                    context.Result = CreateResult(context.Exception, HttpStatusCode.InternalServerError.ToString(), (int)HttpStatusCode.InternalServerError);
#else
                    context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
#endif
                    context.ExceptionHandled = true;
                }



                context.ExceptionHandled = true;
            }
        }

        private void Add(Type exceptionType, HttpStatusCode httpStatusCode)
        {
            if (exceptionType == null)
            {
                _allHttpStatusCode = httpStatusCode;
            }
            else
            {
                _map.Add(exceptionType, httpStatusCode);
            }
        }

        private void Add(Type exceptionType, ErrorCodeMessage.Code errorCode)
        {
            _mapRfc7807.Add(exceptionType, errorCode);
        }

        public class Rfc7807
        {
            public string error_code { get; set; }
            public int status { get; set; }
            public string title { get; set; }
            public string detail { get; set; }
            public string instance { get; set; }
            public Dictionary<string, IEnumerable<string>> errors { get; set; }
        }

        private Rfc7807 ToRfc7807(string msg)
        {
            try
            {
                var rfc7807 = msg.ToJson<Rfc7807>();
                return rfc7807;
            }
            catch
            {
                return null;
            }
        }

        private ObjectResult CreateResult(Exception exception, string title, int statusCode)
        {
            var httpContext = UnityCore.Resolve<IHttpContextAccessor>().HttpContext;
            var rfc7807 = ToRfc7807(exception.Message);
#if (DEBUG)
            if (rfc7807 == null)
            {
                return new ObjectResult(new { Title = title, Detail = exception.Message, StackTrace = exception.StackTrace, Status = statusCode, Instance = $"{httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value}" })
                {
                    StatusCode = statusCode
                };
            }
            else
            {
                return new ObjectResult(new { Title = title, Detail = rfc7807.detail, ErrorCode = rfc7807.error_code, Errors = rfc7807.errors, StackTrace = exception.StackTrace, Status = statusCode, Instance = $"{httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value}" })
                {
                    StatusCode = statusCode
                };
            }
#else
            if (rfc7807 == null)
            {
                return new ObjectResult(new { Title = title, Detail = exception.Message, Status = statusCode, Instance = $"{httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value}" })
                {
                    StatusCode = statusCode
                };
            }
            else
            {
                return new ObjectResult(new { Title = title, Detail = rfc7807.detail, ErrorCode = rfc7807.error_code, Errors = rfc7807.errors, Status = statusCode, Instance = $"{httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value}" })
                {
                    StatusCode = statusCode
                };
            }
#endif
        }
    }
}
