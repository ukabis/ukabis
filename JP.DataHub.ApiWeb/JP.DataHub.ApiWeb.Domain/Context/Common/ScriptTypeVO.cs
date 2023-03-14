using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ScriptTypeVO : IValueObject
    {
        public ScriptType Value { get; }

        public string Code {  get => Value.ToCode(); } 

        public ScriptTypeVO(ScriptType value)
        {
            this.Value = value;
        }

        public static bool operator ==(ScriptTypeVO me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ScriptTypeVO me, object other) => !me?.Equals(other) == true;
    }

    internal static class ScriptTypeExtension
    {
        public static ScriptTypeVO ToScriptTypeVO(this string val) => val == null ? null : new ScriptTypeVO(ScriptTypesExtensions.ToScriptType(val));
        public static ScriptTypeVO ToScriptTypeVO(this ScriptType val) => new ScriptTypeVO(val);
    }
}
