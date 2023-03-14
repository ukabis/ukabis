using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ActionId : IValueObject
    {
        public string Value { get; }
        public Guid? ToGuid
        {
            get
            {
                Guid tmp;
                if (Guid.TryParse(Value, out tmp))
                {
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionId(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(ActionId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ActionId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ActionIdExtension
    {
        public static ActionId ToActionId(this string val) => val == null ? null : new ActionId(val);
        public static ActionId ToActionId(this Guid? val) => val == null ? null : new ActionId(val.ToString());
        public static ActionId ToActionId(this Guid val) => new ActionId(val.ToString());
    }
}