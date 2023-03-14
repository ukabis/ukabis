using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Com.HashAlgorithm;
using JP.DataHub.Com.StringExtensions;
using JP.DataHub.Batch.AsyncDynamicApi.Models;
using JP.DataHub.Batch.AsyncDynamicApi.Exceptions;
using JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Batch.AsyncDynamicApi.Services.Impls
{
    public class DynamicApiService : IDynamicApiService
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILogger<DynamicApiService> Logger;
        private readonly IConfiguration config;
        private readonly IStatusManagementService asyncStatusManagementService;

        private const string JSON = "application/json";
        private const string RetryOverKey = "RetryOver";
        private const string DuplicateRequestKey = "DuplicateRequest";

        private static IMapper mapper =
            new MapperConfiguration(cfg => cfg.CreateMap<PerRequestDataContainer, PerRequestDataContainer>())
                .CreateMapper();

        private readonly string contentTypeString;
        private readonly string isInternalServerErrorResponse;

        public DynamicApiService(
            ILogger<DynamicApiService> logger, IStatusManagementService amservice)
        {
            unityContainer = AsyncDyanamicApiUnityContainer.Resolve<IUnityContainer>();
            config = AsyncDyanamicApiUnityContainer.Resolve<IConfiguration>();
            Logger = logger;
            asyncStatusManagementService = amservice;
            contentTypeString = config.GetValue<string>("AsyncDynamicApiSetting:ContentTypeString");
            isInternalServerErrorResponse = config.GetValue<string>("AsyncDynamicApiSetting:InternalServerErrorDetailResponse", "false");
        }

        /// <summary>
        /// DynamicAPI実行アクティビティ
        /// </summary>
        public async Task<(string BlobPath, TimeSpan ExecutionTime, bool isError)> DynamicApiProc(ReciveMessageModel reciveMessage, ILogger log)
        {
            // リトライ判定
            if (await asyncStatusManagementService.IsRetryOver(reciveMessage.RequestId, log))
            {
                // 4.6.1ではInnerExceptionがException型となり例外型でのエラー判定ができないためDataを使用
                var ex = new AsyncApiRetryOverException("既定のリトライ回数を超えたため処理は失敗しました。");
                ex.Data.Add(RetryOverKey, null);
                throw ex;
            }

            // リクエスト取得
            var api = unityContainer.Resolve<IDynamicApiInterface>();
            RequestModel requestModel = null;
            try
            {
                var response = await Task.Run(() => api.Request(new DynamicApiRequestModel
                {
                    MediaType = JSON,
                    RelativeUri = string.Format(config.GetValue<string>("AsyncDynamicApiSetting:GetRequestDataUrl"), reciveMessage.RequestId),
                    HttpMethod = "GET"

                }, true));
                if ((int)response.StatusCode >= 400)
                {
                    log.LogError(
                        $"RequestDataGetError StatusCode={response.StatusCode} Detail ={response.Contents?.ReadAsString() ?? ""}  ");
                    throw new Exception("RequestData Not Found");
                }

                requestModel = JsonConvert.DeserializeObject<RequestModel>(response.Contents.ReadAsString());
            }
            catch (Exception ex)
            {
                log.LogError(ex, "GetRequestData Error");
                throw;
            }

            // API実行開始時間
            TimeSpan executionTime = new TimeSpan();

            var resultBlobPath = $"{config.GetValue<string>("AsyncDynamicApiSetting:AsyncResultPath")}{DateTime.UtcNow.ToString("yyyy/MM/dd")}/{Guid.NewGuid().ToString()}";
            var resultBlobBodyPath = resultBlobPath + "/Body.bin";
            var resultBlobHeaderPath = resultBlobPath + "/Header.bin";
            bool isError;
            DynamicApiResponse dynamicApiResponse;
            try
            {
                DateTime startDate = DateTime.UtcNow;

                if (requestModel.MethodType.ToLower() == "get" && requestModel.ActionType != "gtw")
                {
                    if (requestModel.PerRequestDataContainer.XRequestContinuation == "")
                    {
                        dynamicApiResponse = await DynamicApiProcContinuationMultiFileAsync(requestModel, log, resultBlobPath);
                    }
                    else
                    {
                        dynamicApiResponse = await DynamicApiProcContinuationAsync(requestModel, log, resultBlobBodyPath, resultBlobHeaderPath);
                    }
                }
                else
                {
                    dynamicApiResponse = await DynamicApiProcSingleAsync(log, requestModel, resultBlobBodyPath, resultBlobHeaderPath);
                }

                executionTime = DateTime.UtcNow - startDate;
                isError = ((int)dynamicApiResponse.StatusCode < 200 || (int)dynamicApiResponse.StatusCode >= 500);
                if (isError)
                {
                    log.LogError($"Dynamic API Call Error.RequestId={reciveMessage.RequestId}, status={dynamicApiResponse.StatusCode},body={dynamicApiResponse.Contents.ReadAsString()}");
                }
            } 
            catch (Exception ex)
            {
                isError = true;

                // GetResultのBody設定
                log.LogError(ex, "Dynamic API Call Error");
                if (isInternalServerErrorResponse == "false")
                {
                    var rfc7807 = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10503, requestModel.Url);
                    api.SetResultOverwrite(JsonConvert.SerializeObject(rfc7807), resultBlobBodyPath);
                }
                else
                {
                    api.SetResultOverwrite(ex.Message, resultBlobBodyPath);
                }

                // GetResultのHeader設定
                api.SetResultOverwrite(JsonConvert.SerializeObject(new AsyncResultHeaderModel()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    MediaType = "text/plain",
                    CharSet = "utf-8"
                }), resultBlobHeaderPath);
            }

            return (resultBlobPath, executionTime, isError);
        }

        /// <summary>
        /// DynamicAPI実行(ページングなし)
        /// </summary>
        private async Task<DynamicApiResponse> DynamicApiProcSingleAsync(ILogger log, RequestModel requestModel, string resultBlobBodyPath, string resultBlobHeaderPath)
        {
            //call dynamicapi
            var dynamicApiResponse = CallDynamicApi(requestModel, log);

            var api = AsyncDyanamicApiUnityContainer.Resolve<IDynamicApiInterface>();
            api.SetResult(dynamicApiResponse.Contents.ReadAsStream(), resultBlobBodyPath, requestModel.Accept);
            JToken.FromObject(dynamicApiResponse.Headers);

            api.SetResult(JsonConvert.SerializeObject(new AsyncResultHeaderModel()
            {
                StatusCode = dynamicApiResponse.StatusCode,
                MediaType = dynamicApiResponse.Contents?.ContentType,
                CharSet = dynamicApiResponse.Contents?.ContentCharset,
                HttpHeaders = dynamicApiResponse.Headers,
            }), resultBlobHeaderPath);
            return dynamicApiResponse;
        }


        /// <summary>
        /// DynamicAPI実行(ページングあり、1ファイル)
        /// </summary>
        private async Task<DynamicApiResponse> DynamicApiProcContinuationAsync(RequestModel requestModel, ILogger log, string blobBodyPath, string blobHeadPath)
        {
            //取得するファイル形式ごとにセパレータ配列を作成する
            Func<string, string[]> getInitOrTermByAccept = (accept) =>
            {
                switch (accept)
                {
                    case "application/xml":
                    case "text/xml":
                        return new string[3] { $@"<?xml version=""1.0"" encoding=""utf-8""?><root xmlns:dh=""{UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:XmlNamespace")}"" dh:anonymous_array=""true"">", "", "</root>" };
                    case "text/csv":
                        return new string[3] { "", "", "" };
                    case "application/json":
                    default:
                        return new string[3] { "[", ",", "]" };

                };
            };
            string[] enumerateStrs = getInitOrTermByAccept(requestModel.Accept);
            bool first = true;
            //取得するファイル形式ごとに配列データの始端と終端をtrimするFuncを作成する
            Func<string, Func<string, string>> getTrimFuncByAccept = (accept) =>
            {
                switch (accept)
                {
                    case "application/xml":
                    case "text/xml":
                        return (string xmlStr) => xmlStr.Replace(enumerateStrs[0], "").Replace(enumerateStrs[2], "").RemoveEmptyLine().Trim();
                    case "text/csv":
                        return (string csvStr) =>
                        {
                            return first ? csvStr.RemoveEmptyLine().Trim()
                                         : "\r\n" + csvStr.RemoveEmptyLine().RemoveHeadLine(1).Trim();
                        };
                    case "application/geo+json":
                    case "application/vnd.geo+json":
                        return (string geoJsonStr) => geoJsonStr.RemoveEmptyLine().Trim();
                    case "application/json":
                    default:
                        return (string jsonStr) => jsonStr.TrimString(enumerateStrs[0], enumerateStrs[2]).RemoveEmptyLine().Trim();
                };
            };
            var TrimArray = getTrimFuncByAccept(requestModel.Accept);

            var api = AsyncDyanamicApiUnityContainer.Resolve<IDynamicApiInterface>();

            DynamicApiResponse dynamicApiResponse = null;
            var responseContinuation = "";
            var isStringContentType = false;
            var useTop = requestModel.QueryString.ToLower().Contains("$top");
            if (!useTop)
            {
                //TOP句がない場合は全件取得なので、ページングで取る
                requestModel.PerRequestDataContainer.XRequestContinuation = "";
            }

            try
            {
                do
                {
                    //call dynamicapi
                    dynamicApiResponse = CallDynamicApi(requestModel, log);

                    if ((int)dynamicApiResponse.StatusCode >= 400)
                    {
                        throw new DynamicApiFailedException();
                    }

                    //save continuation
                    if (dynamicApiResponse.Headers.ContainsKey("X-ResponseContinuation"))
                    {
                        dynamicApiResponse.Headers.TryGetValue("X-ResponseContinuation", out var val);
                        responseContinuation = val.Single();
                        requestModel.PerRequestDataContainer.XRequestContinuation = responseContinuation;
                    }

                    //stringに変換できるTypeかどうかチェックする
                    var contentType = dynamicApiResponse.Contents?.ContentType;
                    isStringContentType = contentTypeString.Split(',').Any(type => contentType.Contains(type));

                    var apiResponseStr = dynamicApiResponse.Contents.ReadAsString();
                    if (first)
                    {
                        if (string.IsNullOrWhiteSpace(responseContinuation))
                        {
                            //このデータだけなので、そのままstreamでBlobに入れる
                            api.SetResult(dynamicApiResponse.Contents.ReadAsStream(), blobBodyPath, requestModel.Accept);
                        }
                        else
                        {
                            //最初のデータ（続きあり）
                            api.SetResult(enumerateStrs[0], blobBodyPath);
                            if (isStringContentType)
                            {
                                var resStr = TrimArray(apiResponseStr) + enumerateStrs[1];
                                api.SetResult(resStr, blobBodyPath, requestModel.Accept);
                            }
                            else
                            {
                                api.SetResult(dynamicApiResponse.Contents.ReadAsStream(), blobBodyPath, requestModel.Accept);
                            }
                        }
                        first = false;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(responseContinuation))
                        {
                            // 最後のデータ
                            if (isStringContentType)
                            {
                                var resStr = TrimArray(apiResponseStr);
                                api.SetResult(resStr, blobBodyPath, requestModel.Accept);
                            }
                            else
                            {
                                api.SetResult(dynamicApiResponse.Contents.ReadAsStream(), blobBodyPath, requestModel.Accept);
                            }
                            api.SetResult(enumerateStrs[2], blobBodyPath);
                        }
                        else
                        {
                            // 途中のデータ
                            if (isStringContentType)
                            {
                                var resStr = TrimArray(apiResponseStr) + enumerateStrs[1];
                                api.SetResult(resStr, blobBodyPath, requestModel.Accept);
                            }
                            else
                            {
                                api.SetResult(dynamicApiResponse.Contents.ReadAsStream(), blobBodyPath, requestModel.Accept);
                            }
                        }
                    }

                } while (!string.IsNullOrWhiteSpace(responseContinuation));
            }
            catch (DynamicApiFailedException)
            {
                api.SetResultOverwrite(dynamicApiResponse.Contents.ReadAsString(), blobBodyPath, requestModel.Accept);
            }
            
            api.SetResult(JsonConvert.SerializeObject(new AsyncResultHeaderModel()
            {
                StatusCode = dynamicApiResponse.StatusCode,
                MediaType = dynamicApiResponse.Contents.ContentType,
                CharSet = dynamicApiResponse.Contents.ContentCharset,
                HttpHeaders = dynamicApiResponse.Headers
            }), blobHeadPath);
            return dynamicApiResponse;
        }

        /// <summary>
        /// DynamicAPI実行(ページングあり、複数ファイル)
        /// </summary>
        private async Task<DynamicApiResponse> DynamicApiProcContinuationMultiFileAsync(RequestModel requestModel, ILogger log, string blobPath)
        {
            var api = AsyncDyanamicApiUnityContainer.Resolve<IDynamicApiInterface>();
            var firstRequestContinuation = requestModel.PerRequestDataContainer.XRequestContinuation;
            DynamicApiResponse dynamicApiResponse = null;
            var responseContinuation = "";
            var saveResponseContinuation = "";

            try
            {
                do
                {
                    //call dynamicapi
                    dynamicApiResponse = CallDynamicApi(requestModel, log);

                    if ((int)dynamicApiResponse.StatusCode >= 400)
                    {
                        throw new DynamicApiFailedException();
                    }

                    //save continuation
                    if (dynamicApiResponse.Headers.ContainsKey("X-ResponseContinuation"))
                    {
                        dynamicApiResponse.Headers.TryGetValue("X-ResponseContinuation", out var val);
                        responseContinuation = val.Single();
                    }

                    var savePath = "";
                    if (string.IsNullOrEmpty(saveResponseContinuation))
                    {
                        savePath = blobPath;
                    }
                    else
                    {
                        savePath = Path.Combine(blobPath, HashCalculation.ComputeHashString(Encoding.Unicode.GetBytes(saveResponseContinuation)));
                    }

                    var apiResponseStr = dynamicApiResponse.Contents.ReadAsStream();
                    api.SetResult(apiResponseStr, Path.Combine(savePath, "Body.bin"), requestModel.Accept);
                    api.SetResult(JsonConvert.SerializeObject(new AsyncResultHeaderModel()
                    {
                        StatusCode = dynamicApiResponse.StatusCode,
                        MediaType = dynamicApiResponse.Contents.ContentType,
                        CharSet = dynamicApiResponse.Contents.ContentCharset,
                        HttpHeaders = dynamicApiResponse.Headers
                    }),Path.Combine(savePath, "Header.bin"));

                    requestModel.PerRequestDataContainer.XRequestContinuation = responseContinuation;
                    saveResponseContinuation = responseContinuation;

                } while (!string.IsNullOrWhiteSpace(responseContinuation));
            }
            catch (DynamicApiFailedException)
            {
                // 途中で失敗した場合は最初のファイルを失敗で上書き
                var apiResponseStr = dynamicApiResponse.Contents.ReadAsString();
                api.SetResultOverwrite(apiResponseStr, Path.Combine(blobPath, firstRequestContinuation, "Body.bin"), requestModel.Accept);
                api.SetResultOverwrite(JsonConvert.SerializeObject(new AsyncResultHeaderModel()
                    {
                        StatusCode = dynamicApiResponse.StatusCode,
                        MediaType = dynamicApiResponse.Contents.ContentType,
                        CharSet = dynamicApiResponse.Contents.ContentCharset,
                        HttpHeaders = dynamicApiResponse.Headers
                }),Path.Combine(blobPath, firstRequestContinuation, "Header.bin"));
            }

            return dynamicApiResponse;
        }


        /// <summary>
        /// DynamicAPI実行
        /// </summary>
        private DynamicApiResponse CallDynamicApi(RequestModel request, ILogger log)
        {
            log.LogInformation($"Metod ={request.MethodType}  Url ={request.Url} Call");
            var api = unityContainer.Resolve<IDynamicApiInterface>();

            var container = unityContainer.Resolve<IPerRequestDataContainer>();
            mapper.Map(request.PerRequestDataContainer, container);
            AsyncDyanamicApiUnityContainer.UnityContainer.RegisterInstance<IPerRequestDataContainer>(container);
            var requestModel = new DynamicApiRequestModel
            {
                MediaType = JSON,
                Accept = request.Accept,
                RelativeUri = request.Url,
                Contents = request?.RequestBody ?? "",
                QueryString = request.QueryString,
                HttpMethod = request.MethodType.ToUpper()
            };
            return api.Request(requestModel);
        }
    }
}
