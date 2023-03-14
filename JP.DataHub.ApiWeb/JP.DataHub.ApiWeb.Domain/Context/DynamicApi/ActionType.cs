using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    // .NET6
    internal enum ActionType
    {
        [ActionTypeConvert("xxx")]
        [ActionClassConvert(1, null)]
        Unknown,

        [ActionTypeConvert("reg")]
        [ActionClassConvert(1, typeof(Actions.RegistAction))]
        Regist,

        [ActionTypeConvert("quy")]
        [ActionClassConvert(1, typeof(Actions.QueryAction))]
        Query,

        [ActionTypeConvert("del")]
        [ActionClassConvert(1, typeof(Actions.DeleteDataAction))]
        DeleteData,

        [ActionTypeConvert("gtw")]
        [ActionClassConvert(1, typeof(Actions.GatewayAction))]
        Gateway,

        [ActionTypeConvert("scr")]
        [ActionClassConvert(1, typeof(Actions.ScriptAction))]
        Script,

        [ActionTypeConvert("oda")]
        [ActionClassConvert(1, typeof(Actions.ODataAction))]
        OData,

        [ActionTypeConvert("upd")]
        [ActionClassConvert(1, typeof(Actions.UpdateAction))]
        Update,

        [ActionTypeConvert("async")]
        [ActionClassConvert(1, typeof(Actions.AsyncAction))]
        Async,

        [ActionTypeConvert("equery")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.EnumerableQueryAction))]
        EnumerableQuery,

        [ActionTypeConvert("aup")]
        [ActionClassConvert(1, typeof(Actions.AttachFileUploadAction))]
        AttachFileUpload,

        [ActionTypeConvert("adl")]
        [ActionClassConvert(1, typeof(Actions.AttachFileDownloadAction))]
        AttachFileDownload,

        [ActionTypeConvert("ade")]
        [ActionClassConvert(1, typeof(Actions.AttachFileDeleteAction))]
        AttachFileDelete,

        [ActionTypeConvert("odd")]
        [ActionClassConvert(1, typeof(Actions.ODataDeleteAction))]
        ODataDelete,

        [ActionTypeConvert("odp")]
        [ActionClassConvert(1, typeof(Actions.ODataPatchAction))]
        ODataPatch,

        [ActionTypeConvert("gdv")]
        [ActionClassConvert(1, typeof(Actions.GetDocumentVersionAction))]
        GetDocumentVersion,

        [ActionTypeConvert("gdh")]
        [ActionClassConvert(1, typeof(Actions.GetDocumentHistoryAction))]
        GetDocumentHistory,

        [ActionTypeConvert("dod")]
        [ActionClassConvert(1, typeof(Actions.DriveOutDocumentAction))]
        DriveOutDocument,

        [ActionTypeConvert("rtd")]
        [ActionClassConvert(1, typeof(Actions.ReturnDocumentAction))]
        ReturnDocument,

        [ActionTypeConvert("gah")]
        [ActionClassConvert(1, typeof(Actions.GetAttachFileDocumentHistoryAction))]
        GetAttachFileDocumentHistory,

        [ActionTypeConvert("dad")]
        [ActionClassConvert(1, typeof(Actions.DriveOutAttachFileDocumentAction))]
        DriveOutAttachFileDocument,

        [ActionTypeConvert("rad")]
        [ActionClassConvert(1, typeof(Actions.ReturnAttachFileDocumentAction))]
        ReturnAttachFileDocument,

        [ActionTypeConvert("rrd")]
        [ActionClassConvert(1, typeof(Actions.RegisterRawDataAction))]
        RegisterRawData,

        [ActionTypeConvert("ord")]
        [ActionClassConvert(1, typeof(Actions.ODataRawDataAction))]
        ODataRawData,

        [ActionTypeConvert("hta")]
        [ActionClassConvert(1, typeof(Actions.HistoryThrowAwayAction))]
        HistoryThrowAway,

        [ActionTypeConvert("ars")]
        [ActionClassConvert(1, typeof(Actions.AdaptResourceSchemaAction))]
        AdaptResourceSchema,

        [ActionTypeConvert("grs")]
        [ActionClassConvert(1, typeof(Actions.GetResourceSchemaAction))]
        GetResourceSchema,
    }

    public class ActionTypeConvertAttribute : Attribute
    {
        public string Code { get; set; }

        public ActionTypeConvertAttribute(string code)
        {
            Code = code;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ActionClassConvertAttribute : Attribute
    {
        public int Version { get; set; }

        public Type ActionClass { get; set; }

        public ActionClassConvertAttribute(int version = 1, Type actionClass = null)
        {
            Version = version;
            ActionClass = actionClass;
        }
    }

    internal static class ActionTypeExtensions
    {
        private static Dictionary<ActionType, string> s_ListCode;

        private static Dictionary<ActionType, Dictionary<int, Type>> s_listClass;

        internal static string ToCode(this ActionType actionType)
        {
            return s_ListCode[actionType];
        }

        internal static Dictionary<int, Type> GetActionClasses(this ActionType actionType)
        {
            return s_listClass[actionType];
        }

        internal static string GetRegistorName(this ActionType actionType, int version)
        {
            return $"{ actionType.ToCode()}_{version}";
        }


        static ActionTypeExtensions()
        {
            s_ListCode = new Dictionary<ActionType, string>();
            s_listClass = new Dictionary<ActionType, Dictionary<int, Type>>();
            foreach (ActionType value in Enum.GetValues(typeof(ActionType)))
            {
                var attrCode = value.GetType()?.GetMember(value.ToString())?.FirstOrDefault()?.GetCustomAttribute<ActionTypeConvertAttribute>();
                var attrClasses = value.GetType()?.GetMember(value.ToString())?.FirstOrDefault()?.GetCustomAttributes<ActionClassConvertAttribute>().ToList();
                if (attrCode != null)
                {
                    s_ListCode.Add(value, attrCode.Code);
                }
                if (attrClasses != null && attrClasses.Count > 0)
                {
                    var addDict = new Dictionary<int, Type>();
                    foreach (ActionClassConvertAttribute attrClass in attrClasses)
                    {
                        addDict.Add(attrClass.Version, attrClass.ActionClass);
                    }
                    s_listClass.Add(value, addDict);
                }
            }
        }

        public static void RegisterActionType(this IUnityContainer container)
        {
            var actionTypes = Enum.GetValues(typeof(ActionType)).Cast<ActionType>().ToList();
            foreach (ActionType actionType in actionTypes)
            {
                foreach (var classdef in actionType.GetActionClasses().Where(x => x.Value != null))
                {
                    var tmp = $"{classdef.Value.Name} : {actionType.GetRegistorName(classdef.Key)}";
                    container.RegisterType(typeof(IDynamicApiAction), classdef.Value, actionType.GetRegistorName(classdef.Key), new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                }
            }
            container.RegisterType<IGatewayAction, GatewayAction>(new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IScriptAction, ScriptAction>(new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAdaptResourceSchemaAction, AdaptResourceSchemaAction>(new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
        }
    }
}
