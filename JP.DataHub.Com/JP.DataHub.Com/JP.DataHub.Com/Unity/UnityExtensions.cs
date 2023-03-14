using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Interception;
using Unity.Resolution;

namespace JP.DataHub.Com.Unity
{
    public static class UnityExtensions
    {
        //public static T Resolve<T>(this IUnityContainer container) => container.Resolve<T>();
        //public static T Resolve<T>(this IUnityContainer container, params ResolverOverride[] overrides) => container.Resolve<T>(overrides);
        public static ParameterOverride ToCI(this object obj) => ToConstructorInjection(obj);
        public static ParameterOverride ToCI(this object obj, string name) => ToConstructorInjection(obj, name);
        public static ParameterOverride ToCI(this object obj, Type type) => ToConstructorInjection(obj, type);
        public static ParameterOverride ToCI(this object obj, Type type, string name) => ToConstructorInjection(obj, type, name);
        public static ParameterOverride ToCI<T>(this object obj) => ToConstructorInjection<T>(obj);
        public static ParameterOverride ToConstructorInjection(this object obj) => obj == null ? null : new ParameterOverride(obj.GetType(), obj);
        public static ParameterOverride ToConstructorInjection(this object obj, string name) => obj == null ? null : new ParameterOverride(obj.GetType(), name, obj);
        public static ParameterOverride ToConstructorInjection(this object obj, Type type) => obj == null ? null : new ParameterOverride(type, obj);
        public static ParameterOverride ToConstructorInjection(this object obj, Type type, string name) => obj == null ? null : new ParameterOverride(type, name, obj);
        public static ParameterOverride ToConstructorInjection<T>(this object obj) => obj == null ? null : new ParameterOverride(typeof(T), obj);
        public static T ResolveOrDefault<T>(this IUnityContainer container, params ResolverOverride[] overrides)
        {
            try
            {
                return container.Resolve<T>(overrides);
            }
            catch (ResolutionFailedException)
            {
                return default;
            }
        }
        public static T ResolveOrDefault<T>(this IUnityContainer container, T defaultValue, params ResolverOverride[] overrides)
        {
            try
            {
                return container.Resolve<T>(overrides);
            }
            catch (ResolutionFailedException)
            {
                return defaultValue;
            }
        }

        public static T ResolveOrDefault<T>(this IUnityContainer container, string name, params ResolverOverride[] overrides)
        {
            try
            {
                return container.Resolve<T>(name, overrides);
            }
            catch (ResolutionFailedException)
            {
                return default;
            }
        }

        public static T ResolveOrDefault<T>(this IUnityContainer container, string name, T defaultValue, params ResolverOverride[] overrides)
        {
            try
            {
                return container.Resolve<T>(name, overrides);
            }
            catch (ResolutionFailedException)
            {
                return defaultValue;
            }
        }

        public static T ResolveDefaultTo<T>(this IUnityContainer container, string name, T def)
        {
            try
            {
                var result = container.Resolve<T>(name);
                result?.AutoInjection();
                return result;
            }
            catch
            {
                return def;
            }
        }
    }
}
