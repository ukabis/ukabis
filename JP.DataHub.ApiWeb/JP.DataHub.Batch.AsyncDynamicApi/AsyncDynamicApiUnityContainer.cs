using System.Data.Common;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;
using Microsoft.Extensions.Configuration;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using Dapper;
using Dapper.Oracle;
using static Dapper.SqlMapper;

namespace JP.DataHub.Batch.AsyncDynamicApi
{
    public class AsyncDyanamicApiUnityContainer
    {

        public static IUnityContainer? UnityContainer = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unityContainer"></param>
        [UnityBuildup]
        public void Buildup(IUnityContainer container, IConfiguration configuration, string? connectionName)
        {
            UnityContainer = container;
            UnityContainer.RegisterInstance(configuration);
            UnityContainer.AddNewExtension<Unity.Interception.Interception>();

            var connectionStrings = (connectionName != null)
                ? new ConnectionStrings($"connectionstring.{connectionName}.json")
                : new ConnectionStrings("connectionstring.json");

            if (UnityCore.UnityContainer != null)
            {
                UnityCore.UnityContainer = null;    // functionによる再呼出時のエラー回避のため
            }
            UnityContainer.RegisterInstance<IConnectionStrings>(connectionStrings);
            UnityContainer.RegisterInstance<List<DbProviderFactoriesConfig>>(configuration.GetSection("DbProviderFactories").Get<List<DbProviderFactoriesConfig>>());
            UnityCore.DefaultLifetimeManager = UnityCore.DataContainerLifetimeManager = new PerRequestLifetimeManager();
            UnityContainer.RegisterInstance<Type>("DataContainerType", typeof(IPerRequestDataContainer));
            UnityContainer.RegisterCache();
            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>(new PerRequestLifetimeManager());
            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread", new PerThreadLifetimeManager());

            //var databaseSettings = new DatabaseSettings();
            //databaseSettings.Type = configuration.GetValue<string>("Dapper:DbType", "Oracle");
            //UnityContainer.RegisterInstance<DatabaseSettings>(databaseSettings);

            var databaseSettings = new DatabaseSettings();
            databaseSettings.Type = configuration.GetSection("Dapper").GetValue<string>("DbType");
            UnityContainer.RegisterType<IDynamicParameters, DynamicParameters>("SqlServer");
            UnityContainer.RegisterType<IDynamicParameters, OracleDynamicParameters>("Oracle");
            UnityContainer.RegisterInstance<DatabaseSettings>(databaseSettings);

            UnityCore.Buildup(UnityContainer, "UnityBuildup.json", configuration);

            UnityContainer.Resolve<List<DbProviderFactoriesConfig>>()?.ForEach(x => DbProviderFactories.RegisterFactory(x.Invariant, x.Type));

            // DB依存部分のTypeMappingを行う
            DapperApplicationBuilder.UseDapperTypeMapping(databaseSettings.Type);

            // TransactionManager
            container.RegisterType<IJPDataHubTransactionManager, JPDataHubTransactionManager>(new PerRequestLifetimeManager());
            container.RegisterType<IJPDataHubTransactionManager, JPDataHubTransactionManager>("Multithread", new PerThreadLifetimeManager());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="overrides"></param>
        /// <returns></returns>
        public static T Resolve<T>(params ResolverOverride[] overrides) =>
            UnityContainer.Resolve<T>(overrides);

        public static T Resolve<T>(string name, params ResolverOverride[] overrides) => UnityContainer.Resolve<T>(name, overrides);

        public static bool IsRegistered<T>() => UnityContainer.IsRegistered<string>();

        public static bool IsRegistered<T>(string nameToCheck) => UnityContainer.IsRegistered<string>(nameToCheck);
    }
}