using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Unity;
using Newtonsoft.Json;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces;
using JP.DataHub.Batch.AsyncDynamicApi.Exceptions;
using JP.DataHub.Batch.AsyncDynamicApi.Models;

namespace JP.DataHub.Batch.AsyncDynamicApi.Services.Impls
{
    public class StatusManagementService : IStatusManagementService
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILogger<StatusManagementService> Logger;
        private readonly IConfiguration config;

        private const string JSON = "application/json";
        private const string RetryOverKey = "RetryOver";
        private const string DuplicateRequestKey = "DuplicateRequest";

        private const string DefaultRetrayInterval = "00:05:00";
        private int MaxNumberOfAttempts { get; }

        public StatusManagementService(
            ILogger<StatusManagementService> logger)
        {
            unityContainer = AsyncDyanamicApiUnityContainer.Resolve<IUnityContainer>();
            config = AsyncDyanamicApiUnityContainer.Resolve<IConfiguration>();
            Logger = logger;

            var val = config.GetValue<string>("AsyncDynamicApiSetting:MaxNumberOfAttempts", null);
            MaxNumberOfAttempts = string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out int maxNumberOfAttempts) ? 5 : maxNumberOfAttempts;
        }

        /// <summary>
        /// ステータス取得
        /// </summary>
        public async Task<AsyncDynamicApiStatusModel> GetStatus(string requestId, ILogger log)
        {
            var api = unityContainer.Resolve<IDynamicApiInterface>();
            var requestModel = new DynamicApiRequestModel
            {
                MediaType = JSON,
                RelativeUri = string.Format(config.GetValue<string>("AsyncDynamicApiSetting:GetStatusDataUrl", null), requestId),
                HttpMethod = "GET",
            };
            var getResult = await Task.Run(() => api.Request(requestModel, true));
            if ((int)getResult.StatusCode >= 400)
            {
                log.LogError($" StatusGetError  RequestId={requestId} StatusCode{getResult.StatusCode} Detail ={getResult.Contents?.ReadAsString() ?? ""}");
                throw new Exception("StatusData Not Fount");
            }

            return JsonConvert.DeserializeObject<AsyncDynamicApiStatusModel>(getResult.Contents.ReadAsString());
        }

        /// <summary>
        /// ステータス保存
        /// </summary>
        public async Task SaveStatus(AsyncDynamicApiStatusModel status, ILogger log)
        {
            var registModel = new DynamicApiRequestModel
            {
                MediaType = JSON,
                RelativeUri = config.GetValue<string>("AsyncDynamicApiSetting:RegistStatusDataUrl", null),
                Contents = JsonConvert.SerializeObject(status),
                HttpMethod = "POST"
            };

            var api = unityContainer.Resolve<IDynamicApiInterface>();
            var result = await Task.Run(() => api.Request(registModel, true));
            if (result.StatusCode != HttpStatusCode.Created)
            {
                log.LogError($" StatusUpdate Error  RequestId={status.RequestId} StatusCode{result.StatusCode} Detail ={result.Contents?.ReadAsString() ?? ""}");
                throw new Exception("StatusUpdate Error");
            }
        }

        /// <summary>
        /// リクエスト削除アクティビティ
        /// </summary>
        public DateTime DeleteRequest(string requestId, ILogger log)
        {
            var api = unityContainer.Resolve<IDynamicApiInterface>();
            var requestModel = new DynamicApiRequestModel
            {
                MediaType = JSON,
                RelativeUri = string.Format(config.GetValue<string>("AsyncDynamicApiSetting:DeleteRequestDataUrl"), requestId),
                HttpMethod = "DELETE"

            };
            api.Request(requestModel, true);
            return DateTime.UtcNow;
        }


        /// <summary>
        /// ステータス更新(エラー)アクティビティ
        /// </summary>
        public async Task<bool> ErrorStatus(string requestId, ILogger log)
        {
            var result = await UpdateStatus(new StatusManagementModel()
            {
                RequestId = requestId,
                EndDate = DateTime.UtcNow,
                ResultPath = ""
            }, log);
            return true;
        }

        /// <summary>
        /// ステータス更新アクティビティ
        /// </summary>
        public async Task<bool> UpdateStatus(StatusManagementModel statusArgs, ILogger log)
        {
            // ステータス取得
            var status = await GetStatus(statusArgs.RequestId, log);

            // まれに同じリクエストが多重起動されることがあるため起動済であれば処理中断
            if (statusArgs.Status == "Start" && status.Status != "Request")
            {
                var ex = new AsyncApiRetryOverException("このリクエストはすでに処理中のため新たな処理を開始することはできません。");
                ex.Data.Add(DuplicateRequestKey, null);
                throw ex;
            }

            // ステータス更新
            status.Status = statusArgs.Status;
            if (statusArgs.EndDate != null)
            {
                status.EndDate = statusArgs.EndDate.Value;
            }
            if (!string.IsNullOrEmpty(statusArgs.ResultPath))
            {
                status.ResultPath = statusArgs.ResultPath;
            }
            if (!string.IsNullOrEmpty(statusArgs.Status) && statusArgs.Status == "Start")
            {
                status.StartDate = DateTime.UtcNow;
            }
            if (statusArgs.ExecutionTime != null)
            {
                status.ExecutionTime = statusArgs.ExecutionTime;
            }

            await SaveStatus(status, log);

            return true;
        }

        /// <summary>
        /// リトライ判定
        /// </summary>
        public async Task<bool> IsRetryOver(string requestId, ILogger log)
        {
            // ステータス取得
            var status = await GetStatus(requestId, log);

            // リトライ判定
            if (status.InProcess)
            {
                // リトライ発生検知用ログ(Application Insightsで"[ASYNC-Retry]"で始まるログを監視)
                log.LogError($"[ASYNC-Retry] RequestId={requestId}, VendorId={status.VendorId}, SystemId={status.SystemId}, Url={status.Url}, Status={status.Status}, RetryCount={status.RetryCount}");

                // リトライ要否判定
                if (MaxNumberOfAttempts - 1 > status.RetryCount)
                {
                    status.RetryCount++;
                }
                else
                {
                    log.LogInformation($"ASYNC Request Retry Over (RequestId={requestId}, VendorId={status.VendorId}, SystemId={status.SystemId}, Url={status.Url}, Status={status.Status}, RetryCount={status.RetryCount})");
                    return true;
                }
            }
            else
            {
                status.InProcess = true;
            }

            // ステータス更新
            await SaveStatus(status, log);

            return false;
        }
    }
}
