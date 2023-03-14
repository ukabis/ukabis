using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http.Models;

namespace JP.DataHub.Com.Net.Http
{
    public static class WebApiResponseResultExtensions
    {
        private static T ResultConvert<T>(this WebApiResponseResult response) where T : new()
        {
            T result = default(T);

            if (response.IsSuccessStatusCode == true && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                // Resultの型が返す方と一緒の場合
                if (response.Result?.GetType() == typeof(T))
                {
                    result = (T)response.Result;
                }
                // 返す型がImageModelの場合（それ専用に変換）
                else if (typeof(T) == typeof(ImageModel))
                {
                    result = (T)(object)response.ToImage();
                }
                // Resultが配列の場合
                else if (response.Result is IEnumerable enumresult)
                {
                    // 返す型がgenericの場合ということは基本的にListとなる
                    // List<T>に変換する
                    var type = typeof(T);
                    if (type.IsGenericType == true)
                    {
                        var json = response.ContentString.ToJson();
                        var tmp = json.JsonToList(type.GenericTypeArguments[0]);
                        result = (T)tmp.ToList(type.GenericTypeArguments[0]);
                    }
                    // genericでない場合ということは、プリミティブ型な場合は、最初の要素をプリミティブ型に変換する
                    else if (typeof(T).IsPrimitive == true)
                    {
                        var iterator = enumresult.GetEnumerator();
                        if (iterator.MoveNext() && iterator.Current != null)
                        {
                            result = iterator.Current.ToString().To<T>();
                        }
                    }
                }
                // Resultが配列ではなくオブジェクト（か、文字列か、プリミティブ型など）の可能性がある
                else
                {
                    // TResultがプリミティブ型ということは、ContentString（Http通信での戻り文字列）からプリミティブ型に変換する
                    if (typeof(T).IsPrimitive == true)
                    {
                        result = (T)response.ContentString.To<T>();
                    }
                    // プリミティブ型でないということはクラスの可能性が高いので、クラス（オブジェクト）に変換する
                    else
                    {
                        result = response.ContentString.ToObject<T>();
                    }
                }
            }
            return result;
        }

        public static WebApiResponseResult NoneGeneric<TReceiveType>(this WebApiResponseResult response)
            where TReceiveType : new()
        {
            var ret = new WebApiResponseResult();
            ret.IsSuccessStatusCode = response.IsSuccessStatusCode;
            ret.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode == true && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var tmp = response.ResultConvert<TReceiveType>();
                var json = tmp.ToJsonString().ToJson();
                var val = (json.FirstOrDefault() as JProperty)?.Value?.ToString();
                ret.Result = val.To<string>();
            }
            ret.Error = response.Error;
            ret.RawContent = response.RawContent;
            ret.ContentString = response.ContentString;
            ret.RequestMessge = response.RequestMessge;
            return ret;
        }

        public static WebApiResponseResult<TResult> ToGeneric<TResult,TReceiveType>(this WebApiResponseResult response)
            where TReceiveType : new()
            where TResult : new()
        {
            var ret = new WebApiResponseResult<TResult>();
            ret.IsSuccessStatusCode = response.IsSuccessStatusCode;
            ret.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode == true && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var received = response.ResultConvert<TReceiveType>();
                var receivedJson = received.ToJsonString().ToJson();
                var resultType = typeof(TResult);
                if (receivedJson is JArray)
                {
                    var result = new TResult();
                    var primitiveInGeneric = resultType.IsGenericType == false ? null : resultType.GenericTypeArguments[0];
                    foreach (var x in receivedJson)
                    {
                        var val = (x.FirstOrDefault() as JProperty)?.Value?.ToString();
                        result.ExecuteMethod("Add", val.To(primitiveInGeneric));
                    }
                    ret.Result = result;
                }
                else if (receivedJson is JObject)
                {
                    var val = (receivedJson.FirstOrDefault() as JProperty)?.Value?.ToString();
                    ret.Result = val.To<TResult>();
                }
            }
            ret.Error = response.Error;
            ret.RawContent = response.RawContent;
            ret.RawContentString = response.ContentString;
            ret.RequestMessge = response.RequestMessge;
            return ret;
        }

        public static WebApiResponseResult<TResult> ToGeneric<TResult>(this WebApiResponseResult response) where TResult : new()
        {
            var ret = new WebApiResponseResult<TResult>();
            ret.IsSuccessStatusCode = response.IsSuccessStatusCode;
            ret.StatusCode = response.StatusCode;
            ret.Result = response.ResultConvert<TResult>();
            ret.Error = response.Error;
            ret.RawContent = response.RawContent;
            ret.RawContentString = response.ContentString;
            ret.RequestMessge = response.RequestMessge;
            return ret;
        }

        public static WebApiResponseResult<TResult> ToGeneric<TResult>(this WebApiResponseResult response, TResult result) where TResult : new()
        {
            var ret = new WebApiResponseResult<TResult>();
            ret.IsSuccessStatusCode = response.IsSuccessStatusCode;
            ret.StatusCode = response.StatusCode;
            ret.Result = result;
            ret.Error = response.Error;
            ret.RawContent = response.RawContent;
            ret.RawContentString = response.ContentString;
            ret.RequestMessge = response.RequestMessge;
            return ret;
        }

        public static WebApiResponseResult<TResult> ToGeneric<TResult>(this WebApiResponseResult response, Func<object,TResult> func) where TResult : new()
        {
            var ret = new WebApiResponseResult<TResult>();
            ret.IsSuccessStatusCode = response.IsSuccessStatusCode;
            ret.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode == true && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                ret.Result = func(response.Result);
            }
            ret.Error = response.Error;
            ret.RawContent = response.RawContent;
            ret.RawContentString = response.ContentString;
            ret.RequestMessge = response.RequestMessge;
            return ret;
        }

        public static ImageModel ToImage(this WebApiResponseResult response)
        {
            if (response.IsSuccessStatusCode == true)
            {
                return new ImageModel() { image = response.RawContent.ReadAsByteArrayAsync().Result };
            }
            return null;
        }

        public static WebApiResponseResultSimple ToSimple(this WebApiResponseResult response)
        {
            var result = new WebApiResponseResultSimple();
            result.IsSuccessStatusCode = response.IsSuccessStatusCode;
            result.StatusCode = response.StatusCode;
            result.Result = response.Result;
            result.Error = response.Error;
            result.ContentString = response.ContentString;
            result.RequestMessge = response.RequestMessge;
            return result;
        }

        public static WebApiResponseResultSimple<TResult> ToSimple<TResult>(this WebApiResponseResult<TResult> response) where TResult : new()
        {
            var result = new WebApiResponseResultSimple<TResult>();
            result.IsSuccessStatusCode = response.IsSuccessStatusCode;
            result.StatusCode = response.StatusCode;
            result.Result = response.Result;
            result.Error = response.Error;
            result.ContentString = response.RawContentString;
            result.ContentByteArray = response.RawContent.ReadAsByteArrayAsync().Result;
            result.RequestMessge = response.RequestMessge;
            return result;
        }
    }
}
