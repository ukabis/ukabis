using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity;
using Unity.Microsoft.DependencyInjection;
using JP.DataHub.Com.Cache;
using NLog.Web;
using JP.DataHub.Batch.Revoke;
using Microsoft.Azure.WebJobs.EventHubs;
using JP.DataHub.Batch.Revoke.Services;
using System.Net.Http;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .UseNLog()
    .UseUnityServiceProvider()
    .ConfigureAppConfiguration((builder, config) =>
    {
        config.AddJsonFile(Path.Combine(builder.HostingEnvironment.ContentRootPath, "appsettings.json"), optional: true, reloadOnChange: false);
        config.AddJsonFile(Path.Combine(builder.HostingEnvironment.ContentRootPath, $"appsettings.{builder.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
        config.AddEnvironmentVariables();
    })
    .ConfigureContainer<IUnityContainer>((builder, container) =>
    {
        new RevokeUnityContainerBuildup().Buildup(container, builder.Configuration);

    })
    .ConfigureServices((builder, service) =>
    {
        var maxBatchSize = builder.Configuration.GetValue<int>("RevokeProcessSetting:EventHubMaxBatchSize", 1);
        service.PostConfigure<EventHubOptions>(o =>
        {
            o.MaxEventBatchSize = maxBatchSize;
        });
        service.AddCaching(builder.Configuration);
        service.AddSingleton<IRevokeService, RevokeService>();
    }).Build();

host.Run();