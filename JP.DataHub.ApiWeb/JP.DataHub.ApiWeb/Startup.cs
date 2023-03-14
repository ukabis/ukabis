using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.NLogTarget;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Unity;
using Unity.Lifetime;
using NLog.Config;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using JP.DataHub.MVC.Filters;
using JP.DataHub.MVC.Http;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApiFilter;
using JP.DataHub.ApiWeb.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using static Dapper.SqlMapper;
using Dapper.Oracle;

namespace JP.DataHub.ApiWeb
{
    public class Startup
    {
        private IConfiguration _configuration { get; }
        private bool _isUseProfiler { get; }

        private IWebHostEnvironment _environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _isUseProfiler = _configuration.GetValue<bool>("Profiler:UseProfiler");
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // ヘルスチェック
            services.AddHealthChecks();

            services.AddControllers(options =>
            {
                OpenIdAuthorizationFilter.InvalidTokenRFC7807 = ErrorCodeMessage.Code.E01406.GetRFC7807();
                OpenIdAuthorizationFilter.ExpiredTokenRFC7807 = ErrorCodeMessage.Code.E01405.GetRFC7807();
                options.Filters.Add(new OpenIdAuthorizationFilter());
                options.Filters.Add(new HttpResponseExceptionFilter());
                options.Filters.Add(new VendorSystemAuthorizationFilter());
                options.Filters.Add(new SetRequestHeaderFilter());
                options.Filters.Add(new AdditionalResponseHeaderAttribute());
                options.Filters.Add(new DisableCacheFilter());
                options.Filters.Add(new ActionLoggingFilter());
                options.Filters.Add(new StaticCacheFilter());
                if (_isUseProfiler)
                {
                    options.Filters.Add(new ProfileToHeaderFilter());
                }
            })
            .AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
                //応答400以上のエラーが自動的にRFC7807に変換されるのを抑制する
                //StatusCodeだけ返したいのにBodyが勝手に付与されてしまうため
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JP.DataHub.ApiWeb", Version = "v1" });
            });

            //キャッシュ設定
            services.AddCaching(_configuration);

            // config....
            services.Configure<List<DbProviderFactoriesConfig>>(_configuration.GetSection("DbProviderFactories"));
            services.Configure<List<ApiFilterConfig>>(_configuration.GetSection("ApiFilter"));
            services.Configure<List<AdditionalHttpResponseHeader>>(_configuration.GetSection("HttpResponseHeader"));

            // MiniProfiler
            if (_isUseProfiler)
            {
                services.AddMiniProfiler();
            }

            services.AddCors(options =>
            {
                options.AddPolicy(name: "corspolicy",
                                  policy =>
                                  {
                                      policy.AllowAnyMethod();
                                      policy.AllowAnyHeader();
                                      policy.AllowAnyOrigin();
                                  });
            });

            // 証明書認証
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

            // ApplicationInsights設定
            var instrumentationKey = _configuration.GetSection("ApplicationInsights")["InstrumentationKey"];
            if (!string.IsNullOrEmpty(instrumentationKey))
            {
                // アプリケーションログ設定
                var target = new ApplicationInsightsTarget();
                target.InstrumentationKey = instrumentationKey;

                var rule = new LoggingRule("*", NLog.LogLevel.FromString(_configuration.GetSection("ApplicationInsights")["LogLevel"]), target);
                var loggingconfig = new LoggingConfiguration();
                loggingconfig.LoggingRules.Add(rule);

                NLog.LogManager.Configuration = loggingconfig;

                // テレメトリ設定
                services.AddApplicationInsightsTelemetry();
            }

            // RequestBodyをコピーするのに必要
            var server = _configuration.GetValue<string>("Server");
            if(server == "IIS")
            { 
                services.Configure<IISServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                });
            }
            else if(server == "Kestrel")
            {
                services.Configure<KestrelServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDapperTypeMapping(_configuration.GetSection("Dapper")?.GetValue<string>("DbType"));
            app.UseUnityMiddleware();

            // 例外確認の為追加
            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JP.DataHub.ApiWeb v1"));
            }

            //Azureや他サービスだとSSL終端するかんけいでhttpsにリダイレクトされると不都合があるので
            //ローカルのみ、https使う
            //if (env.IsDevelopment())
            //{
                app.UseHttpsRedirection();
            //}

            // MiniProfiler
            if (_isUseProfiler)
            {
                app.UseMiniProfiler();
            }

            app.UseRouting();

            //UseCorsはUseRoutingの後じゃないと効かない
            //if (env.IsDevelopment())
            //{
                app.UseCors("corspolicy");
            //}

            app.UseAuthorization();

            app.UseRequestLocalization(options =>
            {
                //サポートするカルチャの設定
                string[] supportedCultures = new string[] { "ja", "en" };

                options
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures)
                    .SetDefaultCulture(supportedCultures[0])
                    ;
            });

            // ヘルスチェック
            app.UseHealthChecks("/healthz");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            // DataContainer
            container.RegisterInstance<Type>("DataContainerType", typeof(IPerRequestDataContainer));
            container.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>(new PerRequestLifetimeManager());
            container.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread", new PerThreadLifetimeManager());

            container.RegisterInstance<VendorAuthenticationJwtKeyConfig>(_configuration.GetSection("VendorAuthenticationJwtKey").Get<VendorAuthenticationJwtKeyConfig>());
            container.RegisterInstance<IConnectionStrings>(new ConnectionStrings($"connectionstring.{_environment.EnvironmentName}.json"));

            // Unity(DI)の初期化
            UnityCore.DefaultLifetimeManager = UnityCore.DataContainerLifetimeManager = new PerRequestLifetimeManager();
#if (DEBUG)
            UnityCore.IsEnableDiagnostic = true;
#endif
            UnityCore.Buildup(container, "UnityBuildup.json", _configuration);

            // DB
            container.RegisterType<IDynamicParameters, DynamicParameters>("SqlServer");
            container.RegisterType<IDynamicParameters, OracleDynamicParameters>("Oracle");
            container.RegisterInstance<DatabaseSettings>(new DatabaseSettings() { Type = _configuration.GetSection("Dapper").GetValue<string>("DbType") });
            container.Resolve<IOptions<List<DbProviderFactoriesConfig>>>()?.Value?.ForEach(x => DbProviderFactories.RegisterFactory(x.Invariant, x.Type));

            // Cache
            container.RegisterCache();

            // TransactionManager
            container.RegisterType<IJPDataHubTransactionManager, JPDataHubTransactionManager>(new PerRequestLifetimeManager());
            container.RegisterType<IJPDataHubTransactionManager, JPDataHubTransactionManager>("Multithread", new PerThreadLifetimeManager());
        }
    }
}
