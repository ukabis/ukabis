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

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ActionTypeVO : IValueObject
    {
        private static List<ActionType> s_listAttachFileActionType = new List<ActionType>() {
            ActionType.AttachFileUpload,
            ActionType.AttachFileDelete,
            ActionType.AttachFileDownload,
            ActionType.DriveOutAttachFileDocument,
            ActionType.GetAttachFileDocumentHistory,
            ActionType.ReturnAttachFileDocument,
        };

        public ActionType Value { get; }

        public bool IsAttachFileAction { get => s_listAttachFileActionType.Contains(Value); }

        public ActionTypeVO(ActionType value)
        {
            Value = value;
        }

        public string Code { get => Value.ToCode(); }

        public static bool operator ==(ActionTypeVO me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ActionTypeVO me, object other) => !me?.Equals(other) == true;
    }

    internal static class ActionTypeVOExtension
    {
        public static ActionTypeVO ToActionTypeVO(this string actionTypeCode)
        {
            foreach (ActionType value in Enum.GetValues(typeof(ActionType)))
            {
                // Script,Asyncは内部的にActionTypeとして扱うためパースできないようにする。
                if (value.ToCode() == actionTypeCode && value != ActionType.Script && value != ActionType.Async)
                {
                    return new ActionTypeVO(value);
                }
            }
            return new ActionTypeVO(ActionType.Unknown);
        }
    }
}
