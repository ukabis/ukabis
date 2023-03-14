using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.UnitTest.Com.Extensions
{
    public static class AssertionExtensions
    {
        private const string ERROR_CODE_PROPERTY = "error_code";


        public static WebApiResponseResult Assert(this WebApiResponseResult response, HttpStatusCode expectedStatusCode, Dictionary<string, string[]> expectedHeaders = null, char? headerSeparator = null, Action<string, IEnumerable<string>> actionHeaderCallback = null)
        {
            response.StatusCode.Is(expectedStatusCode);
            if (expectedHeaders?.Any() == true)
            {
                HeaderContains(response.Headers, expectedHeaders, headerSeparator, actionHeaderCallback);
            }
            return response;
        }

        public static WebApiResponseResult Assert(this WebApiResponseResult response, IEnumerable<HttpStatusCode> expectedStatusCodes)
        {
            response.StatusCode.IsContains(expectedStatusCodes);
            return response;
        }

        public static WebApiResponseResult Assert(this WebApiResponseResult response, HttpStatusCode expectedStatusCode, string expectedResult)
        {
            response.StatusCode.Is(expectedStatusCode);
            response.ContentString.ToJson().Is(expectedResult.ToJson());
            return response;
        }

        public static WebApiResponseResult Assert<T>(this WebApiResponseResult response, HttpStatusCode expectedStatusCode, T expectedResult)
        {
            response.StatusCode.Is(expectedStatusCode);
            JsonConvert.DeserializeObject<T>(response.ContentString).IsStructuralEqual(expectedResult);
            return response;
        }

        public static T AssertAndResult<T>(this WebApiResponseResult response, HttpStatusCode expectedStatusCode, T expectedResult)
        {
            response.StatusCode.Is(expectedStatusCode);
            var result = JsonConvert.DeserializeObject<T>(response.ContentString);
            result.IsStructuralEqual(expectedResult);
            return result;
        }

        public static T AssertAndResult<T>(this WebApiResponseResult response, HttpStatusCode expectedStatusCode)
        {
            response.StatusCode.Is(expectedStatusCode);
            var result = JsonConvert.DeserializeObject<T>(response.ContentString);
            return result;
        }

        public static WebApiResponseResult AssertErrorCode(this WebApiResponseResult response, HttpStatusCode expectedStatusCode, string expectedErrorCode)
        {
            response.StatusCode.Is(expectedStatusCode);
            AssertErrorCode(response.ContentString, expectedErrorCode);
            return response;
        }

        public static WebApiResponseResult AssertErrorCode(this WebApiResponseResult response, HttpStatusCode expectedStatusCode, (HttpStatusCode, string) expectedErrorCode)
        {
            response.StatusCode.Is(expectedStatusCode);
            if (response.StatusCode == expectedErrorCode.Item1)
            {
                AssertErrorCode(response.ContentString, expectedErrorCode.Item2);
            }
            return response;
        }

        public static WebApiResponseResult AssertErrorCode(this WebApiResponseResult response, IEnumerable<HttpStatusCode> expectedStatusCodes, string expectedErrorCode)
        {
            response.StatusCode.IsContains(expectedStatusCodes);
            AssertErrorCode(response.ContentString, expectedErrorCode);
            return response;
        }

        public static WebApiResponseResult AssertErrorCode(this WebApiResponseResult response, IEnumerable<HttpStatusCode> expectedStatusCodes, (HttpStatusCode, string) expectedErrorCode)
        {
            response.StatusCode.IsContains(expectedStatusCodes);
            if (response.StatusCode == expectedErrorCode.Item1)
            {
                AssertErrorCode(response.ContentString, expectedErrorCode.Item2);
            }
            return response;
        }



        public static WebApiResponseResult<T> Assert<T>(this WebApiResponseResult<T> response, HttpStatusCode expectedStatusCode) where T : new()
        {
            response.StatusCode.Is(expectedStatusCode);
            return response;
        }

        public static WebApiResponseResult<T> Assert<T>(this WebApiResponseResult<T> response, IEnumerable<HttpStatusCode> expectedStatusCodes) where T : new()
        {
            response.StatusCode.IsContains(expectedStatusCodes);
            return response;
        }

        public static WebApiResponseResult<T> Assert<T>(this WebApiResponseResult<T> response, HttpStatusCode expectedStatusCode, T expectedResult, Dictionary<string, string[]> expectedHeaders = null, char? headerSeparator = null, Action<string, IEnumerable<string>> actionHeaderCallback = null) where T : new()
        {
            response.StatusCode.Is(expectedStatusCode);
            response.Result.IsStructuralEqual(expectedResult);
            if (expectedHeaders?.Any() == true)
            {
                HeaderContains(response.Headers, expectedHeaders, headerSeparator, actionHeaderCallback);
            }
            return response;
        }

        public static WebApiResponseResult<T> Assert<T>(this WebApiResponseResult<T> response, IEnumerable<HttpStatusCode> expectedStatusCodes, T expectedResult) where T : new()
        {
            response.StatusCode.IsContains(expectedStatusCodes);
            response.Result.IsStructuralEqual(expectedResult);
            return response;
        }

        public static WebApiResponseResult<T> AssertErrorCode<T>(this WebApiResponseResult<T> response, HttpStatusCode expectedStatusCode, string expectedErrorCode) where T : new()
        {
            response.StatusCode.Is(expectedStatusCode);
            AssertErrorCode(response.RawContentString, expectedErrorCode);
            return response;
        }

        public static WebApiResponseResult<T> AssertErrorCode<T>(this WebApiResponseResult<T> response, HttpStatusCode expectedStatusCode, (HttpStatusCode, string) expectedErrorCode) where T : new()
        {
            response.StatusCode.Is(expectedStatusCode);
            if (response.StatusCode == expectedErrorCode.Item1)
            {
                AssertErrorCode(response.RawContentString, expectedErrorCode.Item2);
            }
            return response;
        }

        public static WebApiResponseResult<T> AssertErrorCode<T>(this WebApiResponseResult<T> response, IEnumerable<HttpStatusCode> expectedStatusCodes, string expectedErrorCode) where T : new()
        {
            response.StatusCode.IsContains(expectedStatusCodes);
            AssertErrorCode(response.RawContentString, expectedErrorCode);
            return response;
        }

        public static WebApiResponseResult<T> AssertErrorCode<T>(this WebApiResponseResult<T> response, IEnumerable<HttpStatusCode> expectedStatusCodes, (HttpStatusCode, string) expectedErrorCode) where T : new()
        {
            response.StatusCode.IsContains(expectedStatusCodes);
            if (response.StatusCode == expectedErrorCode.Item1)
            {
                AssertErrorCode(response.RawContentString, expectedErrorCode.Item2);
            }
            return response;
        }


        private static void AssertErrorCode(string jsonText, string expectedErrorCode)
        {
            var jToken = jsonText.ToJson();
            expectedErrorCode.Is(jToken[ERROR_CODE_PROPERTY].ToString());
        }

        public static WebApiResponseResult<List<string>> Assert<T>(this WebApiResponseResult<List<string>> response, HttpStatusCode expectedStatusCode, List<T> expectedResult)
        {
            response.StatusCode.Is(expectedStatusCode);
            JsonConvert.DeserializeObject<List<T>>(response.RawContentString).IsStructuralEqual(expectedResult);
            return response;
        }

        public static WebApiResponseResult Assert(this WebApiResponseResult response, HttpStatusCode expectedStatusCode, byte[] expectedResult)
        {
            response.StatusCode.Is(expectedStatusCode);
            response.RawContent.ReadAsByteArrayAsync().Result.Is(expectedResult);
            return response;
        }

        public static WebApiResponseResult<T> Assert<T>(this WebApiResponseResult<T> response, HttpStatusCode expectedStatusCode, byte[] expectedResult) where T : new()
        {
            response.StatusCode.Is(expectedStatusCode);
            response.RawContent.ReadAsByteArrayAsync().Result.Is(expectedResult);
            return response;
        }


        public static void HeaderContains(HttpResponseHeaders httpResponseHeaders, Dictionary<string, string[]> containsHeaders, char? separator = null, Action<string, IEnumerable<string>> actionHeaderCallback = null)
        {
            if (actionHeaderCallback != null)
            {
                foreach (var x in httpResponseHeaders)
                {
                    actionHeaderCallback(x.Key, x.Value);
                }
            }

            foreach (var h in containsHeaders)
            {
                foreach (var hv in h.Value.ToArray())
                {
                    bool hit = false;
                    var headerValues = httpResponseHeaders.Where(x => x.Key == h.Key).Select(x => x.Value).ToList();
                    if (headerValues.Any())
                    {
                        if (separator == null)
                        {
                            if (hit) continue;
                            hit = HeaderContains(headerValues, hv);
                        }
                        else
                        {
                            foreach (var headerValue in headerValues)
                            {
                                foreach (var headerValueInner in headerValue)
                                {
                                    var headerValueInnerSp = headerValueInner.Split((char)separator).ToList();
                                    foreach (var headerValues2 in headerValueInnerSp)
                                    {
                                        if (hit) continue;
                                        hit = HeaderContains(headerValues2, hv);
                                    }
                                }
                            }

                        }
                    }
                    if (!hit)
                    {
                        throw new AssertFailedException($"is not contains header {h.Key}:{hv}");
                    }
                }
            }
        }

        private static bool HeaderContains(List<IEnumerable<string>> responseHeader, string patternHeader)
        {
            if (patternHeader.StartsWith("*"))
            {
                foreach (var reqv in responseHeader)
                {
                    if (reqv.First().EndsWith(patternHeader.Substring(1)))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (responseHeader.Select(x => x.First()).Contains(patternHeader))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool HeaderContains(string responseHeader, string patternHeader)
        {
            if (patternHeader.StartsWith("*"))
            {
                if (responseHeader.EndsWith(patternHeader.Substring(1)))
                {
                    return true;
                }
            }
            else
            {
                if (responseHeader == patternHeader)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
