using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Unity;
using Unity.Lifetime;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.MVC.Unity
{
    public class UnityMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly object ModuleKey = new object();

        public UnityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using var buffer = new MemoryStream();
            var request = httpContext.Request;
            var response = httpContext.Response;

            var stream = response.Body;
            response.Body = buffer;

            OnBeginRequest(httpContext);

            await _next(httpContext);

            OnEndRequest(httpContext);

            //Debug.WriteLine($"Request content type:  {httpContext.Request.Headers["Accept"]} {System.Environment.NewLine} Request path: {request.Path} {System.Environment.NewLine} Response type: {response.ContentType} {System.Environment.NewLine} Response length: {response.ContentLength ?? buffer.Length}");
            buffer.Position = 0;

            await buffer.CopyToAsync(stream);
        }

        private void OnBeginRequest(HttpContext httpContext)
        {
        }

        private void OnEndRequest(HttpContext httpContext)
        {
            var dict = GetDictionary(httpContext);
            if (dict != null)
            {
                foreach (var disposable in dict.Values.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
            }
        }

        internal static object GetValue(object lifetimeManagerKey)
        {
            var dict = GetDictionary(HttpContextAccessor.Current);

            if (dict != null)
            {
                if (dict.TryGetValue(lifetimeManagerKey, out var obj))
                {
                    return obj;
                }
            }

            return LifetimeManager.NoValue;
        }

        internal static void SetValue(object lifetimeManagerKey, object value)
        {
            var dict = GetDictionary(HttpContextAccessor.Current);

            if (dict == null)
            {
                dict = new Dictionary<object, object>();
                HttpContextAccessor.Current.Items[ModuleKey] = dict;
            }

            dict[lifetimeManagerKey] = value;
        }

        public void Dispose()
        {
        }

        private static Dictionary<object, object> GetDictionary(HttpContext context)
        {
            if (context == null)
            {
                //throw new InvalidOperationException("The PerRequestLifetimeManager can only be used in the context of an HTTP request.Possible causes for this error are using the lifetime manager on a non-ASP.NET application, or using it in a thread that is not associated with the appropriate synchronization context.");
                return new Dictionary<object, object>();
            }
            return (Dictionary<object, object>)context.Items[ModuleKey];
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.  
    public static class UnityMiddlewareExtensions
    {
        public static IApplicationBuilder UseUnityMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UnityMiddleware>();
        }
    }
}
