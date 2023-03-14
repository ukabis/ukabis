using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.Com.Net.Http
{
    public class WebApiResponseResult
    {
        public bool IsSuccessStatusCode { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; }
        public Type ResultType { get; set; }
        public object Result { get; set; }
        public RFC7807ProblemDetail Error { get; set; } = null;
        public HttpResponseHeaders Headers { get; set; }
        public HttpContent RawContent { get; set; }
        public string ContentString { get; set; }
        public HttpRequestMessage RequestMessge { get; set; }

        public WebApiResponseResult()
        {
        }

        public WebApiResponseResult(Exception e)
        {
            IsSuccessStatusCode = false;
            Error = new RFC7807ProblemDetail();
            Error.Title = e.Message;
            Error.Detail = e.StackTrace;
        }

        public WebApiResponseResult(Rfc7807Exception e)
        {
            IsSuccessStatusCode = false;
            Error = e.Rfc7807;
        }

        public WebApiResponseResult(HttpResponseMessage httpResponseMessage, Type type = null)
        {
            IsSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
            StatusCode = httpResponseMessage.StatusCode;
            RequestMessge = httpResponseMessage.RequestMessage;
            ResultType = type;
            Headers = httpResponseMessage.Headers;
            if (httpResponseMessage.Content is null)
            {
                return;
            }
            RawContent = httpResponseMessage.Content;
            ContentString = httpResponseMessage.Content.ReadAsStringAsync().Result;

            try
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (type == null)
                    {
                        Result = ContentString;
                    }
                    else
                    {
                        Result = JsonConvert.DeserializeObject(ContentString, type);
                    }
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                }
                else
                {
                    Error = JsonConvert.DeserializeObject<RFC7807ProblemDetail>(ContentString, RFC7807Extensions.JsonSerializerSetting);
                }
            }
            catch (Exception e)
            {
                IsSuccessStatusCode = false;
                Error = new RFC7807ProblemDetail() { Title = e.Message, Detail = e.StackTrace };
            }
        }

        public WebApiResponseResult Action(Func<WebApiResponseResult, bool> predicate, Action action)
        {
            if (predicate(this) == true)
            {
                action();
            }
            return this;
        }

        public WebApiResponseResult Throw(Func<WebApiResponseResult, bool> predicate, string message)
        {
            if (predicate(this) == true)
            {
                throw new WebApiRequestException(this, message);
            }
            return this;
        }

        public WebApiResponseResult Throw(Func<WebApiResponseResult, bool> predicate, Func<WebApiResponseResult, string> func)
        {
            if (predicate(this) == true && func != null)
            {
                var message = func(this);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new WebApiRequestException(this, message);
                }
            }
            return this;
        }

        public WebApiResponseResult Throw(Func<WebApiResponseResult, bool> predicate, Exception exception)
        {
            if (predicate(this) == true && exception != null)
            {
                throw exception;
            }
            return this;
        }

        public WebApiResponseResult Throw(Func<WebApiResponseResult, bool> predicate, Func<WebApiResponseResult, Exception> func)
        {
            if (predicate(this) == true && func != null)
            {
                var exception = func(this);
                if (exception != null)
                {
                    throw exception;
                }
            }
            return this;
        }

        public WebApiResponseResult ThrowRfc7807()
        {
            if (Error != null)
            {
                throw new Rfc7807Exception(Error);
            }
            return this;
        }

        public WebApiResponseResult ThrowRfc7807(Func<WebApiResponseResult, bool> predicate)
        {
            if (predicate(this) == true && Error != null)
            {
                throw new Rfc7807Exception(Error);
            }
            return this;
        }

        public TConvert Convert<TConvert>(Func<WebApiResponseResult, TConvert> predicate)
        {
            return predicate(this);
        }

        public bool IsFail()
            => IsSuccessStatusCode == false && StatusCode != HttpStatusCode.NotFound;
    }

    public class WebApiResponseResult<T> where T : new()
    {
        public bool IsSuccessStatusCode { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; }
        public T Result { get; set; } = new T();
        public RFC7807ProblemDetail Error { get; set; } = null;
        public HttpResponseHeaders Headers { get; set; }
        public HttpContent RawContent { get; set; }
        public string RawContentString { get; set; }
        public HttpRequestMessage RequestMessge { get; set; }

        public WebApiResponseResult()
        {
        }

        public WebApiResponseResult(Exception e)
        {
            IsSuccessStatusCode = false;
            Error = new RFC7807ProblemDetail();
            Error.Title = e.Message;
            Error.Detail = e.StackTrace;
        }

        public WebApiResponseResult(Rfc7807Exception e)
        {
            IsSuccessStatusCode = false;
            Error = e.Rfc7807;
        }

        public WebApiResponseResult(HttpResponseMessage httpResponseMessage)
        {
            IsSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
            StatusCode = httpResponseMessage.StatusCode;
            RequestMessge = httpResponseMessage.RequestMessage;
            Headers = httpResponseMessage.Headers;
            if (httpResponseMessage.Content is null)
            {
                return;
            }
            RawContent = httpResponseMessage.Content;
            RawContentString = httpResponseMessage.Content.ReadAsStringAsync().Result;

            try
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Result = JsonConvert.DeserializeObject<T>(RawContentString);
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                }
                else
                {
                    Error = JsonConvert.DeserializeObject<RFC7807ProblemDetail>(RawContentString, RFC7807Extensions.JsonSerializerSetting);
                }
            }
            catch (Exception e)
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    IsSuccessStatusCode = false;
                    Error = new RFC7807ProblemDetail() { Title = $"Http通信は成功しましたが、ResponseBodyを型{typeof(T)}に変換できませんでした。", Detail = e.Message, Instance = httpResponseMessage.RequestMessage.RequestUri };
                }
                else
                {
                    IsSuccessStatusCode = false;
                    Error = new RFC7807ProblemDetail() { Title = e.Message, Detail = e.StackTrace };
                }
            }
        }

        //public WebApiResponseResult<T> Action(Func<WebApiResponseResult<T>, bool> predicate, Action action)
        //{
        //    if (predicate(this) == true)
        //    {
        //        action();
        //    }
        //    return this;
        //}

        public WebApiResponseResult<T> Action(Func<WebApiResponseResult<T>, bool> predicate, Action<WebApiResponseResult<T>> action)
        {
            if (predicate(this) == true)
            {
                action(this);
            }
            return this;
        }

        public WebApiResponseResult<T> Throw(Func<WebApiResponseResult<T>, bool> predicate, string message)
        {
            if (predicate(this) == true)
            {
                throw new WebApiRequestException<T>(this, message);
            }
            return this;
        }

        public WebApiResponseResult<T> Throw(Func<WebApiResponseResult<T>, bool> predicate, Func<WebApiResponseResult<T>, string> func)
        {
            if (predicate(this) == true && func != null)
            {
                var message = func(this);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new WebApiRequestException<T>(this, message);
                }
            }
            return this;
        }

        public WebApiResponseResult<T> Throw(Func<WebApiResponseResult<T>, bool> predicate, Exception exception)
        {
            if (predicate(this) == true && exception != null)
            {
                throw exception;
            }
            return this;
        }

        public WebApiResponseResult<T> Throw(Func<WebApiResponseResult<T>, bool> predicate, Func<WebApiResponseResult<T>, Exception> func)
        {
            if (predicate(this) == true && func != null)
            {
                var exception = func(this);
                if (exception != null)
                {
                    throw exception;
                }
            }
            return this;
        }

        public WebApiResponseResult<T> ThrowMessage(Func<WebApiResponseResult<T>, bool> predicate, Func<WebApiResponseResult<T>, HttpResponseMessage> func)
        {
            if (predicate(this) == true && func != null)
            {
                var message = func(this);
                if (message != null)
                {
                    throw new AopResponseException(message);
                }
            }
            return this;
        }

        public WebApiResponseResult<T> ThrowMessage(Func<WebApiResponseResult<T>, bool> predicate, Func<WebApiResponseResult<T>, string> func)
        {
            if (predicate(this) == true && func != null)
            {
                var message = func(this);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new AopResponseException(new HttpResponseMessage(StatusCode) { Content = message.ToStreamContent() });
                }
            }
            return this;
        }

        public WebApiResponseResult<T> ThrowMessage(Func<WebApiResponseResult<T>, bool> predicate, Func<HttpStatusCode> status, Func<WebApiResponseResult<T>, string> func)
        {
            if (predicate(this) == true && func != null)
            {
                var message = func(this);
                if (!string.IsNullOrEmpty(message))
                {
                    var msg = new HttpResponseMessage() { StatusCode = status(), Content = message.ToStreamContent() };
                    throw new AopResponseException(msg);
                }
            }
            return this;
        }

        public WebApiResponseResult<T> ThrowMessage(Func<WebApiResponseResult<T>, bool> predicate, Exception exception)
        {
            if (predicate(this) == true && exception != null)
            {
                throw exception;
            }
            return this;
        }

        public WebApiResponseResult<T> ThrowMessage(Func<WebApiResponseResult<T>, bool> predicate, Func<WebApiResponseResult<T>, Exception> func)
        {
            if (predicate(this) == true && func != null)
            {
                var exception = func(this);
                if (exception != null)
                {
                    throw exception;
                }
            }
            return this;
        }

        public WebApiResponseResult<T> ThrowRfc7807(HttpStatusCode? code = null)
        {
            if (Error != null)
            {
                throw new AopResponseException(new HttpResponseMessage(code ?? StatusCode) { Content = JsonConvert.SerializeObject(Error).ToStreamContent() });
            }
            return this;
        }

        public WebApiResponseResult<T> ThrowRfc7807(Func<WebApiResponseResult<T>, bool> predicate)
        {
            if (predicate(this) == true && Error != null)
            {
                throw new Rfc7807Exception(Error);
            }
            return this;
        }

        public TConvert Convert<TConvert>(Func<WebApiResponseResult<T>, TConvert> predicate)
        {
            return predicate(this);
        }

        public bool IsFail()
            => IsSuccessStatusCode == false && StatusCode != HttpStatusCode.NotFound;

        /// <summary>
        /// Post系の場合
        /// </summary>
        /// <returns></returns>
        public bool IsFailByGet()
            => IsSuccessStatusCode == false && StatusCode != HttpStatusCode.NotFound;

        /// <summary>
        /// Post系の場合
        /// </summary>
        /// <returns></returns>
        public bool IsFailByPost()
            => IsSuccessStatusCode == false;
    }

    public class WebApiResponseResultSimple
    {
        public bool IsSuccessStatusCode { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; }
        public Type ResultType { get; set; }
        public object Result { get; set; }
        public RFC7807ProblemDetail Error { get; set; } = null;
        public string ContentString { get; set; }
        public HttpRequestMessage RequestMessge { get; set; }

        public WebApiResponseResultSimple()
        {
        }

        public WebApiResponseResultSimple(Exception e)
        {
            IsSuccessStatusCode = false;
            Error = new RFC7807ProblemDetail();
            Error.Title = e.Message;
            Error.Detail = e.StackTrace;
        }

        public WebApiResponseResultSimple(Rfc7807Exception e)
        {
            IsSuccessStatusCode = false;
            Error = e.Rfc7807;
        }

        public WebApiResponseResultSimple(HttpResponseMessage httpResponseMessage, Type type = null)
        {
            IsSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
            StatusCode = httpResponseMessage.StatusCode;
            RequestMessge = httpResponseMessage.RequestMessage;
            ResultType = type;
            if (httpResponseMessage.Content is null)
            {
                return;
            }
            ContentString = httpResponseMessage.Content.ReadAsStringAsync().Result;

            try
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (type == null)
                    {
                        Result = ContentString;
                    }
                    else
                    {
                        Result = JsonConvert.DeserializeObject(ContentString, type);
                    }
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                }
                else
                {
                    Error = JsonConvert.DeserializeObject<RFC7807ProblemDetail>(ContentString, RFC7807Extensions.JsonSerializerSetting);
                }
            }
            catch (Exception e)
            {
                IsSuccessStatusCode = false;
                Error = new RFC7807ProblemDetail() { Title = e.Message, Detail = e.StackTrace };
            }
        }

        public WebApiResponseResultSimple Action(Func<WebApiResponseResultSimple, bool> predicate, Action action)
        {
            if (predicate(this) == true)
            {
                action();
            }
            return this;
        }

        public WebApiResponseResultSimple Throw(Func<WebApiResponseResultSimple, bool> predicate, string message)
        {
            if (predicate(this) == true)
            {
                throw new WebApiRequestSimpleException(this, message);
            }
            return this;
        }

        public WebApiResponseResultSimple Throw(Func<WebApiResponseResultSimple, bool> predicate, Func<WebApiResponseResultSimple, string> func)
        {
            if (predicate(this) == true && func != null)
            {
                var message = func(this);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new WebApiRequestSimpleException(this, message);
                }
            }
            return this;
        }

        public WebApiResponseResultSimple Throw(Func<WebApiResponseResultSimple, bool> predicate, Exception exception)
        {
            if (predicate(this) == true && exception != null)
            {
                throw exception;
            }
            return this;
        }

        public WebApiResponseResultSimple Throw(Func<WebApiResponseResultSimple, bool> predicate, Func<WebApiResponseResultSimple, Exception> func)
        {
            if (predicate(this) == true && func != null)
            {
                var exception = func(this);
                if (exception != null)
                {
                    throw exception;
                }
            }
            return this;
        }

        public WebApiResponseResultSimple ThrowRfc7807()
        {
            if (Error != null)
            {
                throw new Rfc7807Exception(Error);
            }
            return this;
        }

        public WebApiResponseResultSimple ThrowRfc7807(Func<WebApiResponseResultSimple, bool> predicate)
        {
            if (predicate(this) == true && Error != null)
            {
                throw new Rfc7807Exception(Error);
            }
            return this;
        }

        public TConvert Convert<TConvert>(Func<WebApiResponseResultSimple, TConvert> predicate)
        {
            return predicate(this);
        }

        public bool IsFail()
            => IsSuccessStatusCode == false && StatusCode != HttpStatusCode.NotFound;
    }

    public class WebApiResponseResultSimple<T> where T : new()
    {
        public bool IsSuccessStatusCode { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; }
        public T Result { get; set; } = new T();
        public RFC7807ProblemDetail Error { get; set; } = null;
        public string ContentString { get; set; }
        public byte[] ContentByteArray { get; set; }
        public HttpRequestMessage RequestMessge { get; set; }

        public WebApiResponseResultSimple()
        {
        }

        public WebApiResponseResultSimple(Exception e)
        {
            IsSuccessStatusCode = false;
            Error = new RFC7807ProblemDetail();
            Error.Title = e.Message;
            Error.Detail = e.StackTrace;
        }

        public WebApiResponseResultSimple(Rfc7807Exception e)
        {
            IsSuccessStatusCode = false;
            Error = e.Rfc7807;
        }

        public WebApiResponseResultSimple(HttpResponseMessage httpResponseMessage)
        {
            IsSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
            StatusCode = httpResponseMessage.StatusCode;
            RequestMessge = httpResponseMessage.RequestMessage;
            if (httpResponseMessage.Content is null)
            {
                return;
            }
            ContentString = httpResponseMessage.Content.ReadAsStringAsync().Result;
            ContentByteArray = httpResponseMessage.Content.ReadAsByteArrayAsync().Result;

            try
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Result = JsonConvert.DeserializeObject<T>(ContentString);
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                }
                else
                {
                    Error = JsonConvert.DeserializeObject<RFC7807ProblemDetail>(ContentString, RFC7807Extensions.JsonSerializerSetting);
                }
            }
            catch (Exception e)
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    IsSuccessStatusCode = false;
                    Error = new RFC7807ProblemDetail() { Title = $"Http通信は成功しましたが、ResponseBodyを型{typeof(T)}に変換できませんでした。", Detail = e.Message, Instance = httpResponseMessage.RequestMessage.RequestUri };
                }
                else
                {
                    IsSuccessStatusCode = false;
                    Error = new RFC7807ProblemDetail() { Title = e.Message, Detail = e.StackTrace };
                }
            }
        }

        public WebApiResponseResultSimple<T> Action(Func<WebApiResponseResultSimple<T>, bool> predicate, Action action)
        {
            if (predicate(this) == true)
            {
                action();
            }
            return this;
        }

        public WebApiResponseResultSimple<T> Throw(Func<WebApiResponseResultSimple<T>, bool> predicate, string message)
        {
            if (predicate(this) == true)
            {
                throw new WebApiRequestSimpleException<T>(this, message);
            }
            return this;
        }

        public WebApiResponseResultSimple<T> Throw(Func<WebApiResponseResultSimple<T>, bool> predicate, Func<WebApiResponseResultSimple<T>, string> func)
        {
            if (predicate(this) == true && func != null)
            {
                var message = func(this);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new WebApiRequestSimpleException<T>(this, message);
                }
            }
            return this;
        }

        public WebApiResponseResultSimple<T> Throw(Func<WebApiResponseResultSimple<T>, bool> predicate, Exception exception)
        {
            if (predicate(this) == true && exception != null)
            {
                throw exception;
            }
            return this;
        }

        public WebApiResponseResultSimple<T> Throw(Func<WebApiResponseResultSimple<T>, bool> predicate, Func<WebApiResponseResultSimple<T>, Exception> func)
        {
            if (predicate(this) == true && func != null)
            {
                var exception = func(this);
                if (exception != null)
                {
                    throw exception;
                }
            }
            return this;
        }

        public WebApiResponseResultSimple<T> ThrowRfc7807()
        {
            if (Error != null)
            {
                throw new Rfc7807Exception(Error);
            }
            return this;
        }

        public WebApiResponseResultSimple<T> ThrowRfc7807(Func<WebApiResponseResultSimple<T>, bool> predicate)
        {
            if (predicate(this) == true && Error != null)
            {
                throw new Rfc7807Exception(Error);
            }
            return this;
        }

        public TConvert Convert<TConvert>(Func<WebApiResponseResultSimple<T>, TConvert> predicate)
        {
            return predicate(this);
        }

        public bool IsFail()
            => IsSuccessStatusCode == false && StatusCode != HttpStatusCode.NotFound;
    }
}
