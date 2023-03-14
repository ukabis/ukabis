using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity;
using Unity.Microsoft.DependencyInjection;
using JP.DataHub.Com.Cache;
using JP.DataHub.Batch.DomainDataSync;
using JP.DataHub.Batch.DomainDataSync.Usecase;
using JP.DataHub.Batch.DomainDataSync.Repository;

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
        new DomainDataSyncUnityContainer().Buildup(container, builder.Configuration);
    })
    .ConfigureServices((builder, service) =>
    {
        service.AddCaching(builder.Configuration);
        service.AddSingleton<ISyncRepository, SyncRepository>();
        service.AddSingleton<ISyncUsecase, SyncUsecase>();
    })
    .Build();

host.Run();