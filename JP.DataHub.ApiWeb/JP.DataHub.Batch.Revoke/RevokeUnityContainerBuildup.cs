﻿using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.MVC.Unity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace JP.DataHub.Batch.Revoke
{
    public class RevokeUnityContainerBuildup
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
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            UnityContainer = container;
            UnityContainer.RegisterInstance(configuration);
            UnityContainer.AddNewExtension<Unity.Interception.Interception>();

            UnityCore.Buildup(UnityContainer, "UnityBuildup.json", configuration);
            container.BuildupApiDifinition();

            var serverbuilder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile(path: "server.json").Build();
            var serverlist = serverbuilder.FindFile().FileToJson<ServerList>();
            var serverSettings = new ServerSettings();
            ConfigurationBinder.Bind(configuration.GetSection("Server"), serverSettings);
            IServerEnvironment environment = null;
            if (serverlist.IsOnce())
            {
                environment = serverlist.FindOnce();
            }
            else
            {
                environment = serverlist.Find(serverSettings.Server, serverSettings.Environment);
            }
            if (environment == null)
            {
                throw new System.Exception("基盤の環境を指定してください");
            }
            container.RegisterInstance<IServerEnvironment>(environment);
            var authenticationService = AuthenticationServiceFactory.Create(environment.Parent.AuthenticationType);
            container.RegisterInstance<IAuthenticationService>(authenticationService);

            var authenticationInfo = AuthenticationInfoFactory.Create(environment);
            container.RegisterInstance<IAuthenticationInfo>(authenticationInfo);
            var authenticationResult = authenticationService.Authentication(environment, authenticationInfo);
            container.RegisterInstance<IAuthenticationResult>(authenticationResult);

            var accountManager = UnityCore.Resolve<IAuthenticationManager>(configuration.GetValue<string>("AccountFileName").ToCI());
            var commonAuthenticationInfo = accountManager.Find(configuration.GetValue<string>("Account"));
            if (commonAuthenticationInfo == null)
            {
                throw new System.Exception("Accountで指定した情報がみつかりません");
            }
            var accountAuthenticationResult = authenticationService.Authentication(environment, commonAuthenticationInfo);
            container.RegisterInstance<IAuthenticationResult>(accountAuthenticationResult);
            container.RegisterType<IDynamicApiClient, DynamicApiClient>(new PerThreadLifetimeManager(), new InjectionConstructor(environment, accountAuthenticationResult));

            var revokeSettings = new RevokeSettings();
            ConfigurationBinder.Bind(configuration.GetSection("RevokeSettings"), revokeSettings);
            UnityContainer.RegisterInstance<RevokeSettings>(revokeSettings);
        }

        public static T Resolve<T>(params ResolverOverride[] overrides) =>
            UnityContainer.Resolve<T>(overrides);

        public static T Resolve<T>(string name, params ResolverOverride[] overrides) => UnityContainer.Resolve<T>(name, overrides);

        public static bool IsRegistered<T>() => UnityContainer.IsRegistered<string>();

        public static bool IsRegistered<T>(string nameToCheck) => UnityContainer.IsRegistered<string>(nameToCheck);
    }
}