using Unity;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using JP.DataHub.Batch.TrailEventProcess.Services.Interfaces;
using JP.DataHub.Batch.TrailEventProcess.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using System.Text;
using JP.DataHub.Com.Settings;
using JP.DataHub.Infrastructure.Core.Storage;

namespace JP.DataHub.Batch.TrailEventProcess.Services.Impls
{
    public class ProrcessManagementService : IProrcessMananagementService
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILogger<ProrcessManagementService> Logger;
        private readonly ITrailService _trailService;
        private readonly IConfiguration config;
        private readonly string Container;
        private readonly BlobStorageClient blobClient;

        public ProrcessManagementService(
            ILogger<ProrcessManagementService> logger,
            ITrailService trailService)
        {
            unityContainer = TrailEventProcessUnityContainer.Resolve<IUnityContainer>();
            config = TrailEventProcessUnityContainer.Resolve<IConfiguration>();
            Logger = logger;
            _trailService = trailService;
            Container = config.GetValue<string>("TrailEventProcessSetting:TrailBackupPath");
            blobClient = new BlobStorageClient(config.GetValue<string>("ConnectionStrings:TrailLogBackUpStorage"),
                config.GetValue<string>("TrailEventProcessSetting:ContainerName"), config.GetValue<string>("TrailEventProcessSetting:RootPath"));
        }

        public async Task Execute(TrailEventModel trailEvent, string trailId, ILogger Log)
        {
            Logger.LogInformation($"Trail ExecuteActivity TrailId = {trailId}");

            var trailInfo = CreateTrailInfo(trailEvent);

            // 証跡を登録
            var dbSettings = UnityCore.Resolve<DatabaseSettings>();
            _trailService.Register(trailInfo, dbSettings.GetDbType());
            SaveBackUpFile(trailInfo);
            DeleteTmpBlob(trailEvent);
        }

        public bool IsBlobStorageStore(object detail)
        {
            return detail != null && detail.ToString().Contains("UnregisteredBackupTrailUrl");
        }

        public TrailInfoModel CreateTrailInfo(TrailEventModel model)
        {
            if (IsBlobStorageStore(model.Detail))
            {
                var trailnfo = _trailService.GetTrailInfo(model.UnregisteredBackupTrailUrl);
                return new TrailInfoModel(trailnfo.TrailId, trailnfo.TrailType, trailnfo.Result, trailnfo.Detail);
            }
            else
            {
                return new TrailInfoModel(model.TrailId, (TrailTypeEnum)model.TrailType, model.Result, model.Detail);
            }
        }

        private  void SaveBackUpFile(TrailInfoModel trailInfo)
        {
            try
            {
                var blobName = $"{Container}/{DateTime.UtcNow.ToString("yyyy/MM/dd/HH")}/{trailInfo.TrailId}.json"; 
                blobClient.CopyTo(blobName, new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(trailInfo))));
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"SaveBackUpFile Error:message = {ex.Message}");
            }
        }
        public void DeleteTmpBlob(TrailEventModel model)
        {
            if (IsBlobStorageStore(model.Detail))
            {
                _trailService.DeleteTrailTempBlob(model.UnregisteredBackupTrailUrl);
            }
        }
    }
}
