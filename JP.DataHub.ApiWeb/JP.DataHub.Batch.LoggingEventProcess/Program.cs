using System.IO;
using Microsoft.Azure.WebJobs.EventHubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity;
using Unity.Microsoft.DependencyInjection;
using JP.DataHub.Com.Cache;
using JP.DataHub.Batch.LoggingEventProcess;
using JP.DataHub.Batch.LoggingEventProcess.Services.Interfaces;
using JP.DataHub.Batch.LoggingEventProcess.Services.Impl;
using JP.DataHub.Batch.LoggingEventProcess.Repository.Interfaces;
using JP.DataHub.Batch.LoggingEventProcess.Repository.Impls;
using JP.DataHub.Com.Transaction;

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
        new LoggingEventProcessUnityContainer().Buildup(container, builder.Configuration, null);
    })
    .ConfigureServices((builder, service) =>
    {
        var maxBatchSize = builder.Configuration.GetValue<int>("LoggingEventProcessSetting:EventHubMaxBatchSize", 20);
        service.PostConfigure<EventHubOptions>(o =>
        {
            o.MaxEventBatchSize = maxBatchSize;
        });
        service.AddCaching(builder.Configuration);
        service.AddSingleton<ILoggingService, LoggingService>();
        service.AddSingleton<ILoggingRepository, LoggingRepository>();
        service.AddSingleton<IJPDataHubEventHub, JPDataHubEventHub>();
    })
    .Build();

host.Run();