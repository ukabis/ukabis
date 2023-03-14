using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using Unity.Interception.PolicyInjection.Pipeline;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Net.Http
{
    public class Resource : IResource
    {
        public Type ModelType { get; set; }

        public IServerEnvironment ServerEnvironment { get; protected set; }
        public String ServerUrl { get; protected set; }
        public string ResourceUrl { get; set; }
        public string ActionUrl { get; set; }
        public IDictionary<string, string[]> AddHeaders { get; set; } = new Dictionary<string, string[]>();

        public Resource()
        {
            InitializeResourceUrl();
        }

        public Resource(string serverUrl)
        {
            ServerUrl = serverUrl;
            InitializeResourceUrl();
        }

        public Resource(IServerEnvironment serverEnvironment)
        {
            ServerEnvironment = serverEnvironment;
            ServerUrl = ServerEnvironment.Url;
            InitializeResourceUrl();
        }

        protected string GetDomainUrl()
        {
            var attr = GetType().GetCustomAttribute<DomainUrlAttribute>();
            if (attr == null)
            {
                var xxx = GetType().GetInterfaces();
                var yyy = xxx.ToList().Select(x => x.GetCustomAttribute<DomainUrlAttribute>()).Where(x => x != null).ToList();
                attr = yyy.Any() ? yyy[0] : null;
            }
            if (attr != null)
            {
                var attrServerUrl = ServerEnvironment.GetType().GetProperty(attr.Name)?.GetValue(ServerEnvironment)?.ToString();
                if (attrServerUrl != null)
                {
                    return attrServerUrl;
                }
            }
            return ServerUrl;
        }

        public T MakeApiRequestModel<T>(object[] param = null)
        {
            var type = this.GetType();
            var sf = new StackFrame(1, true);
            var mb = sf.GetMethod();
            var api = GetActionInfo(this, mb);
            if (string.IsNullOrEmpty(api.ResourceName))
            {
                api.ResourceName = this.ResourceUrl;
            }
            if (string.IsNullOrEmpty(this.ActionUrl) == false)
            {
                api.ActionName = this.ActionUrl;
            }
            var requestModelType = typeof(T);
            var requestModel = Activator.CreateInstance(requestModelType);
            requestModelType.SetPropertyValue<IResource>(requestModel, "Resource", this);
            requestModelType.SetPropertyValue<string>(requestModel, "ResourceUrl", api.ResourceName);
            requestModelType.SetPropertyValue<HttpMethod>(requestModel, "HttpMethod", api.HttpMehod ?? HttpMethod.Get);
            requestModelType.SetPropertyValue<string>(requestModel, "ServerUrl", GetDomainUrl());
            requestModelType.SetPropertyValue<string>(requestModel, "Action", MakeAction(api.IsNoUrlEncode, api.ActionName, mb.GetParameters(), param));
            if (this.AddHeaders?.Any() == true)
            {
                requestModelType.SetPropertyValue(requestModel, "Header", this.AddHeaders);
            }
            if (api.IsPost && param?.Length > 0)
            {
                try
                {
                    var json = param == null ? null : param.Length == 1 ? param[0].ToJson().ToString() : param.ToJson().ToString();
                    if (json != null)
                    {
                        requestModelType.SetPropertyValue(requestModel, "ContentsStream", json.ToStream());
                    }
                }
                catch (Exception e)
                {
                    bool isStream = false;
                    foreach (var p in param)
                    {
                        if (p.GetType() == typeof(MemoryStream))
                        {
                            requestModelType.SetPropertyValue(requestModel, "ContentsStream", p);
                            isStream = true;
                        }
                    }

                    if (!isStream)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            if (api.HttpMehod == HttpMethod.Patch && param?.Length > 0)
            {
                var json = param[1] == null ? null : param[1].ToJson().ToString();
                if (json != null)
                {
                    requestModelType.SetPropertyValue(requestModel, "ContentsStream", json.ToStream());
                }
            }
            return (T)requestModel;
        }

        private void InitializeResourceUrl()
        {
            var type = this.GetType();
            var interfaces = type.GetTypeInfo().ImplementedInterfaces;
            foreach (var implinterface in interfaces)
            {
                var url = implinterface.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
                if (url != null)
                {
                    this.ResourceUrl = url;
                    return;
                }
            }

            this.ResourceUrl = type.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
        }


        public static object MakeApiResult(IMethodInvocation input, string serverUrl = null)
        {
            if (input.Target is IResource == false)
            {
                throw new Exception($"{input.Target}がIResourceを実装していません。");
            }

            var api = GetActionInfo(input.Target, input.MethodBase);
            //if (string.IsNullOrEmpty(api.ResourceName))
            //{
            //    api.ResourceName = this.ResourceUrl;
            //}
            //if (string.IsNullOrEmpty(this.ActionUrl) == false)
            //{
            //    api.ActionName = this.ActionUrl;
            //}
            var requestModelType = input.MethodBase.GetType().GetProperty("ReturnType")?.GetValue(input.MethodBase) as Type;
            var requestModel = Activator.CreateInstance(requestModelType);
            requestModelType.SetPropertyValue<IResource>(requestModel, "Resource", input.Target as IResource);
            requestModelType.SetPropertyValue<string>(requestModel, "ResourceUrl", api.ResourceName);
            requestModelType.SetPropertyValue<HttpMethod>(requestModel, "HttpMethod", api.HttpMehod ?? HttpMethod.Get);
            requestModelType.SetPropertyValue<string>(requestModel, "ServerUrl", api.ResourceName.IsHttpPrefix() == true ? null : serverUrl);
            requestModelType.SetPropertyValue<string>(requestModel, "Action", MakeAction(api.IsNoUrlEncode, api.ActionName, input.MethodBase.GetParameters(), input.Inputs));
            if (input.Target is IResource resource)
            {
                requestModelType.SetPropertyValue(requestModel, "Header", resource.AddHeaders);
            }
            if (api.IsPost && input?.Inputs?.Count > 0)
            {
                if (input.Inputs[0] is Stream)
                {
                    requestModelType.SetPropertyValue(requestModel, "ContentsStream", input.Inputs[0]);
                }
                else
                {
                    var json = input.Inputs[0] == null ? null : input.Inputs[0].ToJson().ToString();
                    if (json != null)
                    {
                        requestModelType.SetPropertyValue(requestModel, "ContentsStream", json.ToStream());
                    }
                }
            }
            if ((api.HttpMehod == HttpMethod.Patch || api.HttpMehod == HttpMethod.Put) && input?.Inputs?.Count > 0)
            {
                var json = input.Inputs[1] == null ? null : input.Inputs[1].ToJson().ToString();
                if (json != null)
                {
                    requestModelType.SetPropertyValue(requestModel, "ContentsStream", json.ToStream());
                }
            }
            return requestModel;
        }

        private static Type[] WebApiAttributes = { typeof(WebApiAttribute), typeof(WebApiGetAttribute), typeof(WebApiDeleteAttribute), typeof(WebApiHeadAttribute), typeof(WebApiOptionsAttribute), typeof(WebApiPostAttribute), typeof(WebApiPutAttribute), typeof(WebApiTraceAttribute) };

        private static (HttpMethod HttpMehod, string ActionName, string ResourceName, bool IsPost) GetActionInfo(object target, int caller)
        {
            var type = target.GetType();
            var sf = new StackFrame(2, true);
            var mb = sf.GetMethod();
            var targetMethod = type.GetMethod(mb.Name);
            WebApiAttribute attr = WebApiAttributes.Select(x => targetMethod.GetCustomAttribute(x)).Where(x => x != null).FirstOrDefault() as WebApiAttribute;
            if (attr == null)
            {
                var interfaces = type.GetTypeInfo().ImplementedInterfaces;
                foreach (var implinterface in interfaces)
                {
                    var interfaceMethod = implinterface.GetMethod(mb.Name);
                    if (interfaceMethod != null)
                    {
                        attr = WebApiAttributes.Select(x => interfaceMethod?.GetCustomAttribute(x))?.Where(x => x != null)?.FirstOrDefault() as WebApiAttribute;
                        if (attr != null)
                        {
                            break;
                        }
                    }
                }
            }
            string nameAction = null;
            var httpMethod = HttpMethod.Get;
            bool isPost = false;
            if (attr != null)
            {
                nameAction = attr.ActionName;
                httpMethod = attr.HttpMethod;
                isPost = attr is WebApiPostAttribute || attr is WebApiPutAttribute;
            }
            if (string.IsNullOrEmpty(nameAction))
            {
                nameAction = mb.Name;
            }
            string nameResource = null;
            if (target is IResource resource)
            {
                nameResource = resource.ResourceUrl;
            }
            if (nameResource == null)
            {
                nameResource = targetMethod?.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
            }
            if (nameResource == null)
            {
                var interfaces = type.GetTypeInfo().ImplementedInterfaces;
                foreach (var implinterface in interfaces)
                {
                    nameResource = implinterface.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
                    if (nameResource != null)
                    {
                        break;
                    }
                }
            }
            if (nameResource == null)
            {
                nameResource = type.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
            }
            return (httpMethod, nameAction, nameResource, isPost);
        }

        private static (HttpMethod HttpMehod, string ActionName, string ResourceName, bool IsPost, bool IsNoUrlEncode) GetActionInfo(object target, MethodBase mb)
        {
            var type = target.GetType();
            var ms = type.GetMethods();
            var targetMethod = ms.FirstOrDefault(x => x.ToString() == mb.ToString());
            if (targetMethod == null)
            {
                targetMethod = type.GetMethod(mb.Name);
            }
            WebApiAttribute attr = WebApiAttributes.Select(x => targetMethod?.GetCustomAttribute(x))?.Where(x => x != null)?.FirstOrDefault() as WebApiAttribute;
            if (attr == null)
            {
                var interfaces = type.GetTypeInfo().ImplementedInterfaces;
                foreach (var implinterface in interfaces)
                {
                    var interfaceMethod = implinterface.GetMethod(mb.Name);
                    if (interfaceMethod != null)
                    {
                        attr = WebApiAttributes.Select(x => interfaceMethod?.GetCustomAttribute(x))?.Where(x => x != null)?.FirstOrDefault() as WebApiAttribute;
                        if (attr != null)
                        {
                            break;
                        }
                    }
                }
            }
            string nameAction = null;
            var httpMethod = HttpMethod.Get;
            bool isPost = false;
            bool isNoUrlEncode = false;
            if (attr != null)
            {
                nameAction = attr.ActionName;
                httpMethod = attr.HttpMethod;
                isPost = attr is WebApiPostAttribute || attr is WebApiPutAttribute;
                isNoUrlEncode = attr.IsNoUrlEncoding;
            }
            if (string.IsNullOrEmpty(nameAction))
            {
                nameAction = mb.Name;
            }
            string nameResource = null;
            if (target is IResource resource)
            {
                nameResource = resource.ResourceUrl;
            }
            if (nameResource == null)
            {
                nameResource = targetMethod?.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
            }
            if (nameResource == null)
            {
                var interfaces = type.GetTypeInfo().ImplementedInterfaces;
                foreach (var implinterface in interfaces)
                {
                    nameResource = implinterface.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
                    if (nameResource != null)
                    {
                        break;
                    }
                }
            }
            if (nameResource == null)
            {
                nameResource = type.GetCustomAttribute<WebApiResourceAttribute>(true)?.ResourceName;
            }
            return (httpMethod, nameAction, nameResource, isPost, isNoUrlEncode);
        }

        private static string MakeAction(bool isNoUrlEncode, string actionName, ParameterInfo[] parameters, IParameterCollection param)
        {
            string result = actionName;
            for (int i = 0; i < parameters.Length; i++)
            {
                result = result.Replace($"{{{parameters[i].Name}}}", isNoUrlEncode == true ? param[i]?.ToString() : HttpUtility.UrlEncode(param[i]?.ToString()));
                result = result.Replace($"{{{parameters[i].Name.ToLower()}}}", isNoUrlEncode == true ? param[i]?.ToString() : HttpUtility.UrlEncode(param[i]?.ToString()));
            }
            return result;
        }

        private static string MakeAction(bool isNoUrlEncode, string actionName, ParameterInfo[] parameters, object[] param)
        {
            string result = actionName;
            if (parameters?.Length == param?.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    result = result.Replace($"{{{parameters[i].Name}}}", isNoUrlEncode == true ? param[i]?.ToString() : HttpUtility.UrlEncode(param[i]?.ToString()));
                }
            }
            return result;
        }
    }
}
