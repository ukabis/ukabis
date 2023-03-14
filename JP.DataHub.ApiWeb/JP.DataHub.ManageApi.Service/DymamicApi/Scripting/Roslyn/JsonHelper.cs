using Newtonsoft.Json.Linq;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class JsonHelper
    {
        public static JToken ToJson(string json)
            => null;

        public static JToken ToJson(object obj)
            => null;

        public static T ToJson<T>(string json)
            => default(T);

        public static string ToJsonString(object obj)
            => null;

        public static JToken ToArray(JToken json)
            => null;

        public static JToken ToArray(JToken json, params JToken[] tokens)
            => null;

        public static JToken AddField(JToken token, string field, object val)
            => null;

        public static JToken RemoveField(JToken token, string field)
            => null;

        public static JToken RemoveFields(JToken token, string[] fields)
            => null;

        public static IEnumerable<string> GetFieldName(JToken token)
            => null;

        public static JToken SelectFields(JToken token, params string[] fields)
            => null;

        public static JToken SelectFields(JToken token, IEnumerable<string> fields)
            => null;

        public static JToken Select(JToken token, Func<JProperty, bool> selector)
            => null;

        public static JToken OrderBy(JToken json, string field)
            => null;

        public static JToken OrderBy(JToken json, string field, string field2)
            => null;

        public static JArray OrderBy(JArray array, string field)
            => null;

        public static JArray OrderBy(JArray array, string field, string field2)
            => null;
    }
}
