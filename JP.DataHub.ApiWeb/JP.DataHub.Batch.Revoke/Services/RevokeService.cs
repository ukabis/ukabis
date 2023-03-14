using JP.DataHub.Batch.Revoke.Api;
using JP.DataHub.Batch.Revoke.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Batch.Revoke.Services
{
    public class RevokeService : IRevokeService
    {
        private readonly IDynamicApiClient _apiClient = UnityCore.Resolve<IDynamicApiClient>();
        private readonly ILogger<RevokeService> _logger;
        private readonly IAsyncPolicy _deleteApiRetryPolicyAsync;

        public RevokeService(ILogger<RevokeService> logger)
        {
            this.AutoInjection();
            _logger = logger;
            _deleteApiRetryPolicyAsync = Policy.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(30));
        }
        public void Revoke(RevokeNotifyModel model)
        {
            var revokeApi = UnityCore.Resolve<IRevokeResource>();
            var resourceGroupApi = UnityCore.Resolve<IResourceGroupResource>();
            var settings = UnityCore.UnityContainer.Resolve<RevokeSettings>();

            // 開始を記録
            var apiResult = _apiClient.GetWebApiResponseResult(revokeApi.Start(model.UserTermsId,model.OpenId));
            if(apiResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Revoke Start Failed. {apiResult.RawContentString}");
            }
            var userRevokeModel = apiResult.Result;

            // 対象を取得
            var resourceGroupResult = _apiClient.GetWebApiResponseResult(resourceGroupApi.GetList());
            if (resourceGroupResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Get Resource List Faild. {resourceGroupResult.RawContentString}");
            }
            var resourceGroup = resourceGroupResult.Result.Where(x => x.ResourceGroupId.ToLower() == model.ResourceGroupId.ToLower()).FirstOrDefault();
            if (resourceGroup == null)
            {
                throw new Exception($"Not Found ResourceGroup. ResourceGroupId = {model.ResourceGroupId}");
            }

            foreach (var resource in resourceGroup.Resources.Where(x => x.IsPerson))
            {
                DeleteResourceData(userRevokeModel.UserRevokeId, resource.ControllerId,resource.ControllerUrl, model.OpenId);
            }

            // 終了を記録
            var stopResult = _apiClient.GetWebApiResponseResult(revokeApi.Stop(userRevokeModel.UserRevokeId, model.OpenId));
            if (stopResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Revoke Stop Faild. {apiResult.RawContentString}");
            }
        }

        private void DeleteResourceData(string userRevokeId,string resourceId,string resourceUrl,string openId)
        {
            _logger.LogInformation($"delete resource data. userRevokeId={userRevokeId},resourceId={resourceId},resourceUrl={resourceUrl},openId={openId}");
            var revokeApi = UnityCore.Resolve<IRevokeResource>();
            var dynamicApi = UnityCore.Resolve<IDynamicApiResource>();
            dynamicApi.ResourceUrl = resourceUrl;
            // 削除開始を記録
            var apiResult = _apiClient.GetWebApiResponseResult(revokeApi.RemoveResourceStart(userRevokeId, resourceId, openId));
            if (apiResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Resource Delete Start Faild. {apiResult.RawContent}");
            }
            var revokeHistory = apiResult.Result;
            // データを削除
            Policy.Handle<Exception>().Retry(3).Execute(() =>
            {
                var deleteResult = _apiClient.GetWebApiResponseResult(dynamicApi.ODataDeleteOverPartition(openId));
                _logger.LogInformation($"ODataDeleteOverPartition Result StatusCode={deleteResult.StatusCode}. userRevokeId={userRevokeId},resourceId={resourceId},resourceUrl={resourceUrl},openId={openId}");

                if (deleteResult.StatusCode == System.Net.HttpStatusCode.NotImplemented)
                {
                    _logger.LogWarning($"ODataDeleteOverPartition NotImplemented. userRevokeId={userRevokeId},resourceId={resourceId},resourceUrl={resourceUrl},openId={openId}");
                }
                else if (!deleteResult.IsSuccessStatusCode && deleteResult.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"Resource Delete Faild. {deleteResult.ContentString}");
                }
            });
            // 削除終了を記録
            var stopResult = _apiClient.GetWebApiResponseResult(revokeApi.RemoveResourceStop(revokeHistory.RevokeHistoryId, openId));
            if (stopResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Resource Delete Stop Faild. {stopResult.ContentString}");
            }
        }
    }
}
