using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;

namespace JP.DataHub.ApiWeb.Domain.Scripting
{
    public class ScriptArgumentParameters
    {
        public string VendorId { get; set; }

        public string SystemId { get; set; }
        
        public string OpenId { get; set; }

        public Dictionary<string, string> QueryString { get; set; }

        public Dictionary<string, string> KeyValue { get; set; }

        public string Contents { get; set; }

        public ScriptHelper ScriptHelper { get; set; }
    }
}
