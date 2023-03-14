using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Unity
{
    public static class IParameterCollectionExtensions
    {
        public static T GetType<T>(this IParameterCollection arguments)
        {
            foreach (var param in arguments)
            {
                if (param != null && param.GetType() == typeof(T))
                {
                    return (T)param;
                }
            }
            return default(T);
        }

        /// <summary>
        /// AOPによるMethodInvocation引数リストから、objectPathの値を取り出して、json形式のようなkey : value形式として返す
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="objectPath"></param>
        /// <returns></returns>
        public static string MethodArgumentToParamValue(this IParameterCollection argument, string objectPath)
        {
            var path = objectPath.Split('.');
            return MethodArgumentToValue(argument, objectPath);
        }

        /// <summary>
        /// AOPによるMethodInvocation引数リストから、objectPathの値（ただし文字列に変換したもの）を返す
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="objectPath"></param>
        /// <returns></returns>
        public static string MethodArgumentToValue(this IParameterCollection argument, string objectPath)
        {
            var path = objectPath.Split('.');
            if (argument.ContainsParameter(path[0]) == false)
            {
                return null;
            }
            var obj = argument[path[0]];
            if (obj == null)
            {
                return null;
            }
            if (path.Length == 1)
            {
                return obj?.ToString();
            }
            return obj.FindObjectPath(path.Skip(1).ToArray())?.ToString();
        }

        public static IList<string> MethodArgumentToValueList(this IParameterCollection argument, string objectPath)
        {
            var path = objectPath.Split('.').ToList();
            var match = ObjectExtensions.ArrayDescriptor(path[0]);
            if (match.Success == true)
            {
                path[0] = path[0].Substring(0, match.Index);
            }
            if (argument.ContainsParameter(path[0]) == false)
            {
                return null;
            }
            var obj = argument[path[0]];
            if (obj == null)
            {
                return null;
            }
            if (path.Count == 1)
            {
                return new List<string>() { obj?.ToString() };
            }
            var target = new List<object>();
            if (obj is IEnumerable enums)
            {
                foreach (var item in enums)
                {
                    target.Add(item);
                }
            }
            else
            {
                target.Add(obj);
            }
            return target.FindObjectPathList(path.Skip(1).ToArray()).Select(x => x?.ToString()).ToList();
        }

        public static string MethodResultToValue(this object target, string objectPath)
        {
            if (objectPath == ".")
            {
                return target?.ToString();
            }
            var path = objectPath.Split('.');
            return target.FindObjectPath(path)?.ToString();
        }

        public static IList<string> MethodResultToValueList(this object obj, string objectPath)
        {
            if (obj == null)
            {
                return null;
            }

            var target = new List<object>();
            if (obj is IEnumerable enums)
            {
                foreach (var item in enums)
                {
                    target.Add(item);
                }
            }
            else
            {
                target.Add(obj);
            }
            if (objectPath == ".")
            {
                return new List<string>() { target?.ToString() };
            }
            var path = objectPath.Split('.');
            return target.FindObjectPathList(path)?.Select(x => x?.ToString())?.ToList();
        }
    }
}
