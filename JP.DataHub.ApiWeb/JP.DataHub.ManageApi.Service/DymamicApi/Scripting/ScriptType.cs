using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting
{
    internal enum ScriptType
    {
        [ScriptTypeCodeConvert("rss")]
        RoslynScript,
        [ScriptTypeCodeConvert("ejd")]
        EtlJsonDefinition,
    }

    public class ScriptTypeCodeConvertAttribute : Attribute
    {
        public string Code { get; set; }
        public ScriptTypeCodeConvertAttribute(string code)
        {
            Code = code;
        }
    }

    internal static class ScriptTypesExtensions
    {
        private static Dictionary<ScriptType, string> _map1;
        private static Dictionary<string, ScriptType> _map2;

        internal static string ToCode(this ScriptType scriptTypes)
        {
            return _map1[scriptTypes];
        }

        internal static ScriptType ToScriptType(this string scriptTypeCode)
        {
            return _map2[scriptTypeCode];
        }

        static ScriptTypesExtensions()
        {
            _map1 = new Dictionary<ScriptType, string>();
            _map2 = new Dictionary<string, ScriptType>();
            foreach (ScriptType value in Enum.GetValues(typeof(ScriptType)))
            {
                var code = ((ScriptTypeCodeConvertAttribute)value.GetType()?.GetMember(value.ToString())?.FirstOrDefault()?.GetCustomAttributes(typeof(ScriptTypeCodeConvertAttribute), false).FirstOrDefault())?.Code ?? "";
                _map1.Add(value, code);
                _map2.Add(code, value);
            }
        }
    }
}
