using System.IO;
using Microsoft.Azure.WebJobs.EventHubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity;
using Unity.Microsoft.DependencyInjection;
using JP.DataHub.Batch.LoggingSummary.Services.Impls;
using JP.DataHub.Batch.LoggingSummary.Services.Interfaces;
using JP.DataHub.Batch.LoggingSummary;
using JP.DataHub.Com.Cache;
using JP.DataHub.Batch.LoggingSummary.Repository.Interfaces;
using JP.DataHub.Batch.LoggingSummary.Repository.Impls;
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
        new LoggingSummaryUnityContainer().Buildup(container, builder.Configuration, null);
    })
    .ConfigureServices((builder, service) =>
    {
        var maxBatchSize = builder.Configuration.GetValue<int>("LoggingSummarySetting:EventHubMaxBatchSize", 10);
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