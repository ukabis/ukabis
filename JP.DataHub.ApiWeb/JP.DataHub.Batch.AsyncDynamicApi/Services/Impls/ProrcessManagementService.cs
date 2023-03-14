using Unity;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces;
using JP.DataHub.Batch.AsyncDynamicApi.Models;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.Batch.AsyncDynamicApi.Services.Impls
{
    public class ProrcessManagementService : IProrcessMananagementService
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILogger<ProrcessManagementService> Logger;
        private readonly IDynamicApiService dynamicApiService;
        private readonly IStatusManagementService statusManagementService;
        private readonly IConfiguration config;

        private static IMapper mapper =
            new MapperConfiguration(cfg => cfg.CreateMap<PerRequestDataContainer, PerRequestDataContainer>())
                .CreateMapper();

        public ProrcessManagementService(
            ILogger<ProrcessManagementService> logger,
            IDynamicApiService dapiservice, 
            IStatusManagementService statusservice)
        {
            unityContainer = AsyncDyanamicApiUnityContainer.Resolve<IUnityContainer>();
            Logger = logger;
            dynamicApiService = dapiservice;
            statusManagementService = statusservice;
            config = AsyncDyanamicApiUnityContainer.Resolve<IConfiguration>();
        }

        public async Task Execute(ReciveMessageModel reciveMessage, ILogger Log)
        {
            MakeContainer();
            // ステータスをStartに更新
            var updateResult = await statusManagementService.UpdateStatus(new StatusManagementModel()
            {
                RequestId = reciveMessage.RequestId,
                Status = "Start"
            }, Log);

            // DynamicApi 実行
            var apiResult = await dynamicApiService.DynamicApiProc(reciveMessage, Log);

            MakeContainer();
            // Request削除
            var endDate = statusManagementService.DeleteRequest(reciveMessage.RequestId, Log);

            // ステータスをEndに更新
            var statusresult = await statusManagementService.UpdateStatus(new StatusManagementModel()
            {
                RequestId = reciveMessage.RequestId,
                Status = apiResult.isError ? "Error" : "End",
                EndDate = endDate,
                ResultPath = apiResult.BlobPath,
                ExecutionTime = apiResult.ExecutionTime
            }, Log);
        }

        private void MakeContainer()
        {
            var systemContainer = CreateContainerSystem();
            var container = unityContainer.Resolve<IPerRequestDataContainer>();
            mapper.Map(systemContainer, container);
            AsyncDyanamicApiUnityContainer.UnityContainer.RegisterInstance<IPerRequestDataContainer>(container);
        }

        private PerRequestDataContainer CreateContainerSystem()
        {
            return new PerRequestDataContainer
            {
                ClientIpAddress = config.GetValue<string>("AsyncDynamicApiSetting:ClientIpAddress", null),
                OpenId = config.GetValue<string>("AsyncDynamicApiSetting:OpenId", null),
                SystemId = config.GetValue<string>("AsyncDynamicApiSetting:SystemId", null),
                VendorId = config.GetValue<string>("AsyncDynamicApiSetting:VendorId", null),
            };
        }
    }
}
