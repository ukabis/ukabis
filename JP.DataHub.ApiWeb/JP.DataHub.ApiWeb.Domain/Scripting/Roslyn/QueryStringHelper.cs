using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    [RoslynScriptHelp]
    public static class QueryStringHelper
    {
        /// <summary>
        /// KeyValueListからkeyに合致するものを探し、型Tに変換するヘルパークラス。キーが存在したか、型変換は行われたかなどの情報を返す。
        /// </returns>
        public static KeyValueResult<string> Find(this Dictionary<string, string> dic, string key) => find<string>(dic, key);

        /// <summary>
        /// <ja>KeyValueListからkeyに合致するものを探し、型Tに変換する。キーが存在したか、型変換は行われたかなどの情報を返す。</ja>
        /// <en>Find the one that matches the key from the KeyValueList and convert it to type T. Returns information such as whether the key existed and whether the type conversion was done.</en>
        /// </summary>
        /// <typeparam name="T">
        /// <ja>型T</ja>
        /// <en>Type T</en>
        /// </typeparam>
        /// <param name="dic">
        /// <ja>string型のDictionary</ja>
        /// <en>Dictionary of string type</en>
        /// </param>
        /// <param name="key">
        /// <ja>キー</ja>
        /// <en>key</en>
        /// </param>
        /// <returns>
        /// <ja>string型のDictionaryからキーを探した結果のKeyValueResult&lt;string&gt;型</ja>
        /// <en>KeyValueResult &lt;string&gt; type as a result of searching for a key from a dictionary of string type</en>
        /// </returns>
        public static KeyValueResult<T> Find<T>(this Dictionary<string, string> dic, string key) => find<T>(dic, key);

        private static KeyValueResult<T> find<T>(Dictionary<string, string> dic, string key)
        {
            var result = new KeyValueResult<T>();
            var hit = dic.Keys.Contains(key);
            if (hit == true)
            {
                result.HasKey = true;
                result.Object = dic[key].Convert(typeof(T));
                result.IsValid = dic[key].IsValid<T>();
                result.Source = dic[key];
            }
            result.Type = typeof(T);
            return result;
        }
    }
}