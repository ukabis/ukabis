using System.Data.Common;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using System.Collections.Generic;
using System;
using Dapper;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Settings;
using Unity.Injection;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.PolicyInjection;
using Dapper.Oracle;
using static Dapper.SqlMapper;

namespace JP.DataHub.Batch.LoggingSummary
{
    public class LoggingSummaryUnityContainer
    {

        /// <summary>
        /// 
        /// </summary>
        internal static IUnityContainer UnityContainer = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="configuration"></param>
        /// <exception cref="Exception"></exception>
        public void Buildup(IUnityContainer container, IConfiguration configuration, string? connectionName)
        {
            var connectionStrings = (connectionName != null)
                ? new ConnectionStrings($"connectionstring.{connectionName}.json")
                : new ConnectionStrings("connectionstring.json");

            UnityContainer = container;
            UnityContainer.RegisterInstance(configuration);
            UnityContainer.AddNewExtension<Unity.Interception.Interception>();
            UnityContainer.RegisterInstance<IConnectionStrings>(connectionStrings);
            UnityContainer.RegisterInstance<List<DbProviderFactoriesConfig>>(configuration.GetSection("DbProviderFactories").Get<List<DbProviderFactoriesConfig>>());

            if (UnityCore.UnityContainer != null)
            {
                UnityCore.UnityContainer = null;    // functionによる再呼出時のエラー回避のため
            }
            var databaseSettings = new DatabaseSettings();
            databaseSettings.Type = configuration.GetSection("Dapper").GetValue<string>("DbType");
            UnityContainer.RegisterType<IDynamicParameters, DynamicParameters>("SqlServer");
            UnityContainer.RegisterType<IDynamicParameters, OracleDynamicParameters>("Oracle");
            UnityContainer.RegisterInstance<DatabaseSettings>(databaseSettings);

            UnityCore.Buildup(UnityContainer, "UnityBuildup.json", configuration);
            UnityContainer.Resolve<List<DbProviderFactoriesConfig>>()?.ForEach(x => DbProviderFactories.RegisterFactory(x.Invariant, x.Type));

            // DB依存部分のTypeMappingを行う
            DapperApplicationBuilder.UseDapperTypeMapping(databaseSettings.Type);

            // DataBase
            if (UnityCore.IsRegistered<IConnectionStrings>() == true)
            {
                var list = UnityCore.Resolve<IConnectionStrings>();
                list?.ForEach(x =>
                {
                    container.RegisterType<IJPDataHubDbConnection, JPDataHubDbConnection>(x.Name, list.LifeTimeManager.CreateInstanseLifeTimeManager(),
                        new InjectionConstructor(new JPDataHubDbConnectionParam() { ConnectionString = x.ConnectionString, ProviderName = x.Provider, IsMultithread = false, IsTransactionManage = list.IsTransactionManage, IsTransactionScope = list.IsTransactionScope, Options = x.Options }),
                        new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                    container.RegisterType<IJPDataHubDbConnection, JPDataHubDbConnection>(x.Name + "-Multithread", new PerThreadLifetimeManager(),
                        new InjectionConstructor(new JPDataHubDbConnectionParam() { ConnectionString = x.ConnectionString, ProviderName = x.Provider, IsMultithread = true, IsTransactionManage = list.IsTransactionManage, IsTransactionScope = list.IsTransactionScope, Options = x.Options }),
                        new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                });
            }
        }

        public static T Resolve<T>(params ResolverOverride[] overrides) =>
            UnityContainer.Resolve<T>(overrides);

        public static T Resolve<T>(string name, params ResolverOverride[] overrides) => UnityContainer.Resolve<T>(name, overrides);

        public static bool IsRegistered<T>() => UnityContainer.IsRegistered<string>();

        public static bool IsRegistered<T>(string nameToCheck) => UnityContainer.IsRegistered<string>(nameToCheck);
    }
}