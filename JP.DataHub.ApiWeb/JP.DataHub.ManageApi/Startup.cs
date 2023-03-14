using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.NLogTarget;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Unity;
using Unity.Lifetime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using JP.DataHub.MVC.Filters;
using JP.DataHub.MVC.Http;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.ManageApi.Filters;
using JP.DataHub.ManageApi.Core.DataContainer;
using Newtonsoft.Json.Serialization;
using JP.DataHub.Com.Settings;
using Dapper.Oracle;
using static Dapper.SqlMapper;

namespace JP.DataHub.ManageApi
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
            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // ヘルスチェック
            services.AddHealthChecks();

            services.AddControllers(options =>
            {
                OpenIdAuthorizationFilter.InvalidTokenRFC7807 = ErrorCodeMessage.Code.E01406.GetRFC7807();
                OpenIdAuthorizationFilter.ExpiredTokenRFC7807 = ErrorCodeMessage.Code.E01405.GetRFC7807();
                options.Filters.Add(new OpenIdAuthorizationFilter());
                //options.Filters.Add(new HttpResponseExceptionFilter());
                options.Filters.Add(new VendorSystemAuthorizationFilter());
                options.Filters.Add(new SetRequestHeaderFilter());
                options.Filters.Add(new AdditionalResponseHeaderAttribute());
                options.Filters.Add(new DisableCacheFilter());
                options.Filters.Add(new ActionLoggingFilter());
                if (_isUseProfiler)
                {
                    options.Filters.Add(new ProfileToHeaderFilter());
                }
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
                //応答400以上のエラーが自動的にRFC7807に変換されるのを抑制する
                //StatusCodeだけ返したいのにBodyが勝手に付与されてしまうため
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JP.DataHub.ManageApi", Version = "v1" });
            });

            //キャッシュ設定
            services.AddCaching(_configuration);

            // config....
            services.Configure<List<DbProviderFactoriesConfig>>(_configuration.GetSection("DbProviderFactories"));
            services.Configure<List<AdditionalHttpResponseHeader>>(_configuration.GetSection("HttpResponseHeader"));

            services.AddControllers().AddNewtonsoftJson(o => 
            { 
                o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            // MiniProfiler
            if (_isUseProfiler)
            {
                services.AddMiniProfiler();
            }
            services.AddMvc().AddWebApiConventions();
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JP.DataHub.ManageApi v1"));
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
            // AppConfig
            var appconfig = _configuration.GetSection("AppConfig");
            container.RegisterInstance("LoggingHttpHeaders", appconfig.GetValue<string>("LoggingHttpHeaders", "*"));
            container.RegisterInstance("SupportedCultures", appconfig.GetValue<string>("SupportedCultures", "ja").Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
            container.RegisterInstance("LoggingHttpHeaders", appconfig.GetValue<string>("LoggingHttpHeaders", "*"));
            var listOperatingVendor = appconfig.GetSection("OperatingVendorVendorId").GetValue<string>();
            listOperatingVendor = listOperatingVendor.Select(x => x.ToLower()).ToList();
            container.RegisterInstance("OperatingVendorVendorId", listOperatingVendor);
            container.RegisterInstance("VendorApiPrefix", appconfig.GetValue<string>("VendorApiPrefix", "*"));
            container.RegisterInstance("AllowResourceDependencyChangeByManageApi", appconfig.GetValue<bool>("AllowResourceDependencyChangeByManageApi", false));

            // DataContainer
            container.RegisterInstance<Type>("DataContainerType", typeof(IPerRequestDataContainer));
            container.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>(new PerRequestLifetimeManager());
            container.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread", new PerThreadLifetimeManager());

            container.RegisterInstance<VendorAuthenticationJwtKeyConfig>(_configuration.GetSection("VendorAuthenticationJwtKey").Get<VendorAuthenticationJwtKeyConfig>());
            container.RegisterInstance<IConnectionStrings>(new ConnectionStrings($"connectionstring.{_environment.EnvironmentName}.json"));

            container.RegisterInstance("VendorSystemAuthenticationDefaultVendorId", appconfig.GetValue<string>("VendorSystemAuthenticationDefaultVendorId", "00000000-0000-0000-0000-000000000001"));
            container.RegisterInstance("VendorSystemAuthenticationDefaultSystemId", appconfig.GetValue<string>("VendorSystemAuthenticationDefaultSsytemId", "00000000-0000-0000-0000-000000000001"));
            container.RegisterInstance("VendorSystemAuthenticationDefaultVendorId", appconfig.GetValue<Guid>("VendorSystemAuthenticationDefaultVendorId", Guid.Parse("00000000-0000-0000-0000-000000000001")));
            container.RegisterInstance("VendorSystemAuthenticationDefaultSystemId", appconfig.GetValue<Guid>("VendorSystemAuthenticationDefaultSsytemId", Guid.Parse("00000000-0000-0000-0000-000000000001")));

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

            // SingleとしてのQueryに対してデータが取得できなかった場合は、NotFoundExceptionをスローする
            AbstractJPDataHubDbConnection.IsQuerySingleFailThrowNotFoundException = true;

        }
    }
}
