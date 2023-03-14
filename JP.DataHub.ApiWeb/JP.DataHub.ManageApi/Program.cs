using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JP.DataHub.MVC.HostService;
using Unity.Microsoft.DependencyInjection;

namespace JP.DataHub.ManageApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            ThreadPool.GetMinThreads(out var _, out var ioMin);
            ThreadPool.SetMinThreads(400, ioMin);
            ThreadPool.GetMinThreads(out var workerMin, out ioMin);
            logger.Info($"MinimumWokerThread:{workerMin} MinimumIOThread:{ioMin}");
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                logger.Error(e, "Error Occured");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    //if (environment != Environments.Development)
                    //{
                    //    //Azure Linux はUseUrlが未サポートなのでスキップ
                    //    if (environment?.Contains("Azure") == false)
                    //    {
                    //        //webBuilder.UseUrls("http://localhost:50001");
                    //    }
                    //    webBuilder.UseKestrel(options =>
                    //    {
                    //        options.AddServerHeader = false;
                    //    });
                    //}
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.None);
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<GCHostedService>();
                    services.AddRazorPages();
                })
                .UseNLog()
                .UseUnityServiceProvider();
    }
}