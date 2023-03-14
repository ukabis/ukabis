using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity;
using Unity.Microsoft.DependencyInjection;
using JP.DataHub.Com.Cache;
using JP.DataHub.Batch.TrailEventProcess;
using JP.DataHub.Batch.TrailEventProcess.Services.Interfaces;
using JP.DataHub.Batch.TrailEventProcess.Services.Impls;
using JP.DataHub.Batch.TrailEventProcess.Repository.Interfaces;
using JP.DataHub.Batch.TrailEventProcess.Repository.Impls;
using Microsoft.Azure.WebJobs.EventHubs;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .UseUnityServiceProvider()
    .ConfigureAppConfiguration((builder, config) =>
    {
        config.AddJsonFile(Path.Combine(builder.HostingEnvironment.ContentRootPath, "appsettings.json"), optional: true, reloadOnChange: false);
        config.AddJsonFile(Path.Combine(builder.HostingEnvironment.ContentRootPath, $"appsettings.{builder.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
        config.AddEnvironmentVariables();
    })
    .ConfigureContainer<IUnityContainer>((builder, container) =>
    {
        new TrailEventProcessUnityContainer().Buildup(container, builder.Configuration, null);
    })
    .ConfigureServices((builder, service) =>
    {
        var maxBatchSize = builder.Configuration.GetValue<int>("TrailEventProcessSetting:EventHubMaxBatchSize", 10);
        service.PostConfigure<EventHubOptions>(o =>
        {
            o.MaxEventBatchSize = maxBatchSize;
        });
        service.AddCaching(builder.Configuration);
        service.AddSingleton<IProrcessMananagementService, ProrcessManagementService>();
        service.AddSingleton<ITrailService, TrailService>();
        service.AddSingleton<ITrailRepository, TrailRepository>();
        service.AddSingleton<IPhysicalRepositoryGroupRepository, PhysicalRepositoryGroupRepository>();
    })
    .Build();

host.Run();