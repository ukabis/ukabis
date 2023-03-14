using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using AutoMapper;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;
using Unity.Interception;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.DataContainer;
using System.Xml.XPath;
using Microsoft.Extensions.Configuration;

namespace JP.DataHub.Com.Unity
{
    /// <summary>
    /// JP.DataHubのUnityContainerの便利クラスいろいろ
    /// </summary>
    public static class UnityCore
    {
        /// <summary>
        /// UnityContainerのインスタンス
        /// </summary>
        public static IUnityContainer UnityContainer { get; set; }

        /// <summary>
        /// デフォルトのLifetimeManager
        /// </summary>
        public static ITypeLifetimeManager DefaultLifetimeManager { get; set; }

        /// <summary>
        /// DataContainerのLifetimeManager
        /// </summary>
        public static ITypeLifetimeManager DataContainerLifetimeManager { get; set; }

        /// <summary>
        /// マルチスレッド用のDataContainerのコンテナ名
        /// </summary>
        public static readonly string DATACONTAINER_MULTITHREAD_NAME = "MultithreadDataContainer";

        /// <summary>
        /// UnityDiagnosticの拡張機能を有効にするか？
        /// </summary>
        public static bool IsEnableDiagnostic = false;

        #region Mapper
        public static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDataContainer, IDataContainer>();
            });

            return mappingConfig.CreateMapper();
        });
        public static IMapper Mapper { get => _Mapper.Value; }
        #endregion

        public static void Reset()
        {
            UnityContainer = null;
            DefaultLifetimeManager = null;
            DataContainerLifetimeManager = null;
            IsEnableDiagnostic = false;
        }

        /// <summary>
        /// UnityContainerのビルドアップメソッド。実行プロセスから呼び出すこと
        /// </summary>
        /// <param name="container">Unityコンテナ</param>
        /// <param name="jsonConfigFileName">Buildupするファイル名を記述したconfigファイル(json)</param>
        public static void Buildup(IUnityContainer container, string jsonConfigFileName = null, IConfiguration configuration = null) => Buildup<CoreDataContainer>(container, configuration, null, jsonConfigFileName);

        /// <summary>
        /// UnityContainerのビルドアップメソッド。実行プロセスから呼び出すこと
        /// </summary>
        /// <param name="container">Unityコンテナ</param>
        /// <param name="jsonConfigFileName">Buildupするファイル名を記述したconfigファイル(json)</param>
        public static void Buildup<DataContainerT>(IUnityContainer container, string jsonConfigFileName = null, IConfiguration configuration = null) where DataContainerT : IDataContainer => Buildup<DataContainerT>(container, configuration, null, jsonConfigFileName);

        /// <summary>
        /// UnityContainerのビルドアップメソッド。実行プロセスから呼び出すこと
        /// </summary>
        /// <param name="container">Unityコンテナ</param>
        /// <param name="registerDataContainerAction">DataContainerのRegisterType/RegisterInstanceするためのAction</param>
        /// <param name="jsonConfigFileName">Buildupするファイル名を記述したconfigファイル(json)</param>
        public static void Buildup<DataContainerT>(IUnityContainer container, IConfiguration configuration, Action<IUnityContainer, string> registerDataContainerAction = null, string jsonConfigFileName = null) where DataContainerT : IDataContainer
        {
            container.AddNewExtension<Interception>();
            if (IsEnableDiagnostic == true)
            {
                container.AddExtension(new Diagnostic());
            }

            if (configuration != null && container.IsRegistered<IConfiguration>() == false)
            {
                container.RegisterInstance<IConfiguration>(configuration);
            }

            if (UnityContainer != null)
            {
                throw new Exception("UnityCore.Buildupはすでに行われています。");
            }
            UnityContainer = container;

            // DataContainerの初期化
            if (registerDataContainerAction == null)
            {
                if (DataContainerLifetimeManager != null && UnityContainer.IsRegistered<IDataContainer>() == false)
                {
                    UnityContainer.RegisterType<IDataContainer, DataContainerT>(DataContainerLifetimeManager);
                }
            }
            else
            {
                registerDataContainerAction(UnityContainer, DATACONTAINER_MULTITHREAD_NAME);
            }
            UnityContainer.RegisterType<IDataContainer, DataContainerT>(DATACONTAINER_MULTITHREAD_NAME);

            // ほかのモジュールのBuildupをする
            Assembly[] assemblies = new Assembly[] { };
            if (string.IsNullOrEmpty(jsonConfigFileName))
            {
            }
            else if (System.IO.File.Exists(jsonConfigFileName) == true)
            {
                assemblies = FileNameToAssembly(jsonConfigFileName.FileToJson<UnityConfig>()?.Buildup);
            }
            // 全モジュールからBuildupをする
            foreach (var type in FindUnityBuildupClass(assemblies))
            {
                var target = type.Create<IUnityBuildup>();
                target?.Buildup(container, configuration);
            }

            //foreach (var type in FindUnityBuildupClass(assemblies))
            //{
            //    var target = type.Create<IUnityBuildup>();
            //    target?.BuildupAfter(container, configuration);
            //}
        }

        public static bool IsRegistered<T>() => UnityContainer.IsRegistered<T>();

        /// <summary>
        /// Resolveするのは一緒だが、引数とConstractorInjectionに変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static T ResolveCI<T>(params object[] objs)
        {
            var overrides = new List<ParameterOverride>();
            objs.ToList().ForEach(x => overrides.Add(x.ToCI()));
            return Resolve<T>(overrides.ToArray());
        }

        public static T Resolve<T>(params ResolverOverride[] overrides) => UnityContainer.Resolve<T>(overrides);

        public static T Resolve<T>(string name, params ResolverOverride[] overrides) => UnityContainer.Resolve<T>(name, overrides);

        public static IEnumerable<T> ResolveAll<T>(params ResolverOverride[] overrides) => UnityContainer.ResolveAll<T>(overrides);

        public static T ResolveOrDefault<T>(params ResolverOverride[] overrides)
        {
            try
            {
                return UnityContainer.Resolve<T>(overrides);
            }
            catch (ResolutionFailedException)
            {
                return default;
            }
        }

        public static T ResolveOrDefault<T>(T defaultValue, params ResolverOverride[] overrides)
        {
            try
            {
                return UnityContainer.Resolve<T>(overrides);
            }
            catch (ResolutionFailedException)
            {
                return defaultValue;
            }
        }

        public static T ResolveOrDefault<T>(string name, params ResolverOverride[] overrides)
        {
            try
            {
                return UnityContainer.Resolve<T>(name, overrides);
            }
            catch (ResolutionFailedException)
            {
                return default;
            }
        }

        public static T ResolveOrDefault<T>(string name, T defaultValue, params ResolverOverride[] overrides)
        {
            try
            {
                return UnityContainer.Resolve<T>(name, overrides);
            }
            catch (ResolutionFailedException)
            {
                return defaultValue;
            }
        }

        public static T ResolveDefaultTo<T>(string name, T def)
        {
            try
            {
                var result = UnityContainer.Resolve<T>(name);
                result?.AutoInjection();
                return result;
            }
            catch
            {
                return def;
            }
        }

        public static object Resolve(Type type, string name = null, params ResolverOverride[] overrides)
            => UnityContainer.Resolve(type, name, overrides);

        public static IUnityContainer RegisterInstance(Type type, object instance)
             => UnityContainer.RegisterInstance(type, instance);

        public static IUnityContainer RegisterInstance(Type type, object instance, IInstanceLifetimeManager lifetimeManager)
            => UnityContainer.RegisterInstance(type, instance, lifetimeManager);

        public static IUnityContainer RegisterInstance(Type type, string name, object instance)
             => UnityContainer.RegisterInstance(type, name, instance);

        public static IUnityContainer RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager)
            => UnityContainer.RegisterInstance(type, name, instance, lifetimeManager);

        public static IUnityContainer RegisterInstance<TInterface>(string name, TInterface instance, IInstanceLifetimeManager lifetimeManager)
            => UnityContainer.RegisterInstance<TInterface>(name, instance, lifetimeManager);

        public static IUnityContainer RegisterInstance<TInterface>(string name, TInterface instance)
             => UnityContainer.RegisterInstance<TInterface>(name, instance);

        public static IUnityContainer RegisterInstance<TInterface>(TInterface instance)
             => UnityContainer.RegisterInstance<TInterface>(instance);

        public static IUnityContainer RegisterInstance<TInterface>(TInterface instance, IInstanceLifetimeManager lifetimeManager)
             => UnityContainer.RegisterInstance<TInterface>(instance, lifetimeManager);

        //private static Type[] GetTypes<T>(this Assembly assembly) where T : Attribute =>
        //    assembly.GetTypes().Where(x => x.GetCustomAttribute<T>() != null && x.HasInheritance<IUnityBuildup>() == true).ToArray();

        private static IEnumerable<Type> FindUnityBuildupClass(IEnumerable<Assembly> assemblies)
        {
            List<Type> result = new List<Type>();
            if (assemblies != null)
            {
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes<UnityBuildupAttribute, IUnityBuildup>();
                        if (types != null && types.Length != 0)
                        {
                            foreach (var t in types)
                            {
                                result.Add(t);
                            }
                        }
                    }
                    catch
                    {
                        // 無視
                    }
                }
            }
            return result;
        }

        private static Assembly[] FileNameToAssembly(IEnumerable<string> fileNames)
        {
            List<Assembly> result = new List<Assembly>();
            foreach (var filename in fileNames)
            {
                try
                {
                    result.Add(Assembly.Load(filename));
                }
                catch
                {
                    // 無視
                }
            }
            return result.ToArray();
        }

        private static IEnumerable<Type> FindUnityBuildupClass(IEnumerable<string> fileNames)
        {
            foreach (var filename in fileNames)
            {
                Type[] types = null;
                try
                {
                    var assembly = Assembly.Load(filename);
                    types = assembly.GetTypes<UnityBuildupAttribute, IUnityBuildup>();
                }
                catch
                {
                    // Loadで失敗しても無視する
                }
                if (types != null) foreach (var t in types) yield return t;
            }
            yield break;
        }
    }
}
