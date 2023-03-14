using System.IO;
using Microsoft.Azure.WebJobs.EventHubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity;
using Unity.Microsoft.DependencyInjection;
using JP.DataHub.Batch.AsyncDynamicApi.Services.Impls;
using JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces;
using JP.DataHub.Com.Cache;
using JP.DataHub.Batch.AsyncDynamicApi;
using NLog.Web;

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
                new AsyncDyanamicApiUnityContainer().Buildup(container, builder.Configuration, null);
            })
            .ConfigureServices((builder, service) =>
            {
                var maxBatchSize = builder.Configuration.GetValue<int>("AsyncDynamicApiSetting:EventHubMaxBatchSize", 10);
                service.PostConfigure<EventHubOptions>(o =>
                {
                    o.MaxEventBatchSize = maxBatchSize;
                });
                service.AddCaching(builder.Configuration);
                service.AddSingleton<IProrcessMananagementService, ProrcessManagementService>();
                service.AddSingleton<IStatusManagementService, StatusManagementService>();
                service.AddSingleton<IDynamicApiService, DynamicApiService>();
            })
            .Build();

host.Run();