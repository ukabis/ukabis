using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// Jsonとオブジェクトの相互変換のためのヘルパークラス
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    [RoslynScriptHelp]
    public class JsonHelper
    {
        /// <summary>
        /// <ja>Json文字列をJsonオブジェクトへ変換します。</ja>
        /// <en>Converts JSON strings to JSON objects.</en>
        /// </summary>
        /// <param name="json">
        /// <ja>Json文字列</ja>
        /// <en>JSON string</en>
        /// </param>
        /// <returns>
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </returns>
        public static JToken ToJson(string json)
        {
            return JToken.FromObject(JsonConvert.DeserializeObject(json, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal }));
        }

        /// <summary>
        /// <ja>オブジェクトをJTokenに変換する</ja>
        /// <en>Convert object to JToken</en>
        /// </summary>
        /// <param name="obj"><ja>オブジェクト</ja><en>object</en></param>
        /// <returns>JToken</returns>
        public static JToken ToJson(object obj) => JToken.FromObject(obj);

        /// <summary>
        /// <ja>Json文字列を指定したオブジェクトへ変換します。</ja>
        /// <en>Converts a JSON string to the specified object.</en>
        /// </summary>
        /// <typeparam name="T">
        /// <ja>変換先オブジェクトの型名</ja>
        /// <en>The type of the object to be converted to</en>
        /// </typeparam>
        /// <param name="json">
        /// <ja>Json文字列</ja>
        /// <en>JSON string</en>
        /// </param>
        /// <returns>
        /// <ja>指定したオブジェクト</ja>
        /// <en>The specified object</en>
        /// </returns>
        public static T ToJson<T>(string json)
        {
            return (T)JsonConvert.DeserializeObject(json, typeof(T), new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });
        }


        /// <summary>
        /// <ja>指定したオブジェクトをJson文字列へ変換します。</ja>
        /// <en>Converts the specified object to a JSON string.</en>
        /// </summary>
        /// <param name="obj">
        /// <ja>オブジェクト</ja>
        /// <en>Object</en>
        /// </param>
        /// <returns>
        /// <ja>Json文字列</ja>
        /// <en>JSON string</en>
        /// </returns>
        public static string ToJsonString(object obj) => JsonConvert.SerializeObject(obj);

        /// <summary>
        /// <ja>Jsonオブジェクトを配列型のJsonオブジェクト(JArray)へ変換します。</ja>
        /// <en>Converts a JSON object to an array-type JSON object (JArray).</en>
        /// </summary>
        /// <param name="json">
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </param>
        /// <returns>
        /// <ja>配列型のJsonオブジェクト(JArray)</ja>
        /// <en>Array-type JSON object (JArray)</en>
        /// </returns>
        public static JToken ToArray(JToken json)
        {
            return new JArray(json);
        }

        /// <summary>
        /// <ja>Jsonオブジェクトを配列型のJsonオブジェクトに追加し、新しい配列型のJsonオブジェクト(JArray)を作成します。</ja>
        /// <en>Adds a JSON object to an array-type JSON object and creates a new array-type JSON object (JArray).</en>
        /// </summary>
        /// <param name="json">
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </param>
        /// <param name="tokens">
        /// <ja>配列型のJsonオブジェクト(JArray)</ja>
        /// <en>Array-type JSON object (JArray)</en>
        /// </param>
        /// <returns>
        /// <ja>配列型のJsonオブジェクト(JArray)</ja>
        /// <en>Array-type JSON object (JArray)</en>
        /// </returns>
        public static JToken ToArray(JToken json, params JToken[] tokens)
        {
            var array = new JArray(json);
            if (tokens != null)
            {
                foreach (JToken token in tokens)
                {
                    array.Add(token);
                }
            }
            return array;
        }


        /// <summary>
        /// <ja>Jsonオブジェクトに指定したフィールドを追加します。</ja>
        /// <en>Adds the specified field to a JSON object.</en>
        /// </summary>
        /// <param name="token">
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </param>
        /// <param name="field">
        /// <ja>フィールド名</ja>
        /// <en>Field name</en>
        /// </param>
        /// <param name="val">
        /// <ja>フィールド値</ja>
        /// <en>Field value</en>
        /// </param>
        /// <returns>
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </returns>
        public static JToken AddField(JToken token, string field, object val)
        {
            token.First.AddAfterSelf(new JProperty(field, val));
            return token;
        }


        /// <summary>
        /// <ja>Jsonオブジェクトから指定したフィールドを削除します。</ja>
        /// <en>Deletes the specified field from a JSON object.</en>
        /// </summary>
        /// <param name="token">
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </param>
        /// <param name="field">
        /// <ja>フィールド名</ja>
        /// <en>Field name</en>
        /// </param>
        /// <returns>
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </returns>
        public static JToken RemoveField(JToken token, string field)
        {
            return field != null ? RemoveFields(token, new string[] { field }) : token;
        }


        /// <summary>
        /// <ja>Jsonオブジェクトから指定したフィールド群を削除します。</ja>
        /// <en>Deletes the specified fields from a JSON object.</en>
        /// </summary>
        /// <param name="token">
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </param>
        /// <param name="fields">
        /// <ja>フィールド群</ja>
        /// <en>Field names</en>
        /// </param>
        /// <returns>
        /// <ja>Jsonオブジェクト</ja>
        /// <en>JSON object</en>
        /// </returns>
        public static JToken RemoveFields(JToken token, string[] fields)
        {
            if (fields != null && fields.Count() > 0)
            {
                foreach (var field in fields)
                {
                    var result = token.SelectToken(field);
                    if (result != null)
                    {
                        result.Parent.Remove();
                    }
                }
            }
            return token;
        }

        /// <summary>
        /// <ja>jsonからトップレベルのプロパティ名を取得する</ja>
        /// <en>Get top level property name from json</en>
        /// </summary>
        /// <param name="token">JToken</param>
        /// <returns>
        /// <ja>プロパティ名の列挙</ja>
        /// <en>Property name enumeration</en>
        /// </returns>
        public static IEnumerable<string> GetFieldName(JToken token)
        {
            var result = new List<string>();
            for (token = token.First(); token != null; token = token.Next)
            {
                yield return ((JProperty)token)?.Name;
            }
        }

        /// <summary>
        /// <ja>jsonから指定したプロパティだけの抜き出したJsonを返す</ja>
        /// <en>Returns the extracted Json of only the specified property from json</en>
        /// </summary>
        /// <param name="token">JToken</param>
        /// <param name="fields"><ja>プロパティ</ja><en>Properties</en></param>
        /// <returns><ja>新しいJToken</ja><en>New JToken</en></returns>
        public static JToken SelectFields(JToken token, params string[] fields) => _selectFields(token, fields);

        /// <summary>
        /// <ja>jsonから指定したプロパティだけの抜き出したJsonを返す</ja>
        /// <en>Returns the extracted Json of only the specified property from json</en>
        /// </summary>
        /// <param name="token">JToken</param>
        /// <param name="fields"><ja>プロパティ</ja><en>Properties</en></param>
        /// <returns><ja>新しいJToken</ja><en>New JToken</en></returns>
        public static JToken SelectFields(JToken token, IEnumerable<string> fields) => _selectFields(token, fields);

        private static JToken _selectFields(JToken token, IEnumerable<string> fields)
        {
            if (token == null)
            {
                return null;
            }
            var result = new JObject();
            var propnames = JsonHelper.GetFieldName(token).ToList();
            foreach (var field in fields)
            {
                if (propnames.Contains(field))
                {
                    var prop = token[field];
                    result.Add(field, prop);
                }
            }
            return result;
        }

        /// <summary>
        /// <ja>jsonから指定したプロパティだけの抜き出したJsonを返す</ja>
        /// <en>Returns the extracted Json of only the specified property from json</en>
        /// </summary>
        /// <param name="token">JToken</param>
        /// <param name="selector"><ja>プロパティを選択するセレクター</ja><en>Selector to select a property</en></param>
        /// <returns><ja>新しいJToken</ja><en>New JToken</en></returns>
        public static JToken Select(JToken token, Func<JProperty, bool> selector)
        {
            if (selector == null)
            {
                return null;
            }
            var fields = new List<string>();
            for (var child = token.First(); child != null; child = child.Next)
            {
                var prop = child as JProperty;
                if (selector(prop))
                {
                    fields.Add(prop.Name);
                }
            }
            return SelectFields(token, fields.ToArray());
        }

        /// <summary>
        /// <ja>
        /// JTokenのfieldで指定したプロパティの値を使ってソートする
        /// ※ ソートができるのはJArrayのみ
        /// </ja>
        /// <en>
        /// Sort by the value of the property specified in the field of JToken
        /// ※ Only JArray can be sorted
        /// </en>
        /// </summary>
        /// <param name="json">
        /// <ja>json(実態はJArray)</ja>
        /// <en>json (actually JArray)</en>
        /// </param>
        /// <param name="field">
        /// <ja>ソートするプロパティ名</ja>
        /// <en>Property name to sort</en>
        /// </param>
        /// <returns>
        /// <ja>ソートされたJToken</ja>
        /// <en>Sorted JToken</en>
        /// </returns>
        public static JToken OrderBy(JToken json, string field) => OrderBy(json as JArray, field) ?? json;

        /// <summary>
        /// <ja>
        /// JTokenのfieldで指定したプロパティの値を使ってソートする
        /// ※ ソートができるのはJArrayのみ
        /// </ja>
        /// <en>
        /// Sort by the value of the property specified in the field of JToken
        /// ※ Only JArray can be sorted
        /// </en>
        /// </summary>
        /// <param name="json">
        /// <ja>json(実態はJArray)</ja>
        /// <en>json (actually JArray)</en>
        /// </param>
        /// <param name="field">
        /// <ja>ソートするプロパティ名</ja>
        /// <en>Property name to sort</en>
        /// </param>
        /// <param name="field2">
        /// <ja>第２ソートキー</ja>
        /// <en>2nd sort key</en>
        /// </param>
        /// <returns>
        /// <ja>ソートされたJToken</ja>
        /// <en>Sorted JToken</en>
        /// </returns>
        public static JToken OrderBy(JToken json, string field, string field2) => OrderBy(json as JArray, field, field2) ?? json;

        /// <summary>
        /// <ja>
        /// JTokenのfieldで指定したプロパティの値を使ってソートする
        /// ※ ソートができるのはJArrayのみ
        /// </ja>
        /// <en>
        /// Sort by the value of the property specified in the field of JToken
        /// ※ Only JArray can be sorted
        /// </en>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="field">
        /// <ja>ソートするプロパティ名</ja>
        /// <en>Property name to sort</en>
        /// </param>
        /// <returns>
        /// <ja>ソートされたJToken</ja>
        /// <en>Sorted JToken</en>
        /// </returns>
        public static JArray OrderBy(JArray array, string field)
        {
            if (array == null)
            {
                return null;
            }
            var result = new JArray();
            array.OrderBy(x => x[field]).ToList().ForEach(x => result.Add(x));
            return result;
        }

        /// <summary>
        /// <ja>
        /// JTokenのfieldで指定したプロパティの値を使ってソートする
        /// ※ ソートができるのはJArrayのみ
        /// </ja>
        /// <en>
        /// Sort by the value of the property specified in the field of JToken
        /// ※ Only JArray can be sorted
        /// </en>
        /// </summary>
        /// <param name="array">
        /// <ja>json(実態はJArray)</ja>
        /// <en>json (actually JArray)</en>
        /// </param>
        /// <param name="field">
        /// <ja>ソートするプロパティ名</ja>
        /// <en>Property name to sort</en>
        /// </param>
        /// <param name="field2">
        /// <ja>第２ソートキー</ja>
        /// <en>2nd sort key</en>
        /// </param>
        /// <returns>
        /// <ja>ソートされたJToken</ja>
        /// <en>Sorted JToken</en>
        /// </returns>
        public static JArray OrderBy(JArray array, string field, string field2)
        {
            if (array == null)
            {
                return null;
            }
            var result = new JArray();
            array.OrderBy(x => x[field]).OrderBy(x => x[field2]).ToList().ForEach(x => result.Add(x));
            return result;
        }
    }
}
