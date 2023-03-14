using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;

namespace JP.DataHub.Com.Extensions
{
    public static class JsonExtensions
    {
        public static JToken ToJson(this string str) => JToken.FromObject(JsonConvert.DeserializeObject(str, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal }));
        public static JToken ToJson(this object obj) => JToken.FromObject(obj);
        public static string ToJsonString(this object obj) => obj.ToJson().ToString();
        public static string ToJsonString(this object obj, bool ignoreNullValue)
            => JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = ignoreNullValue ? NullValueHandling.Ignore : NullValueHandling.Include });
        public static DateTime ToDateTime(this JValue value) => DateTime.Parse(value.ToString());
        public static DateTime ToDateTime(this JToken value) => DateTime.Parse(value.ToString());

        public static JToken ToJsonArray(this JToken json)
        {
            var result = new JArray { json };
            return result;
        }

        public static JToken ToArray(this JToken json, params JToken[] tokens)
        {
            var array = new JArray(json);
            if (tokens == null) return array;

            foreach (var token in tokens)
            {
                array?.Add(token);
            }
            return array;
        }

        public static T ToJson<T>(this string str) => string.IsNullOrEmpty(str) ? default(T) : (T)JsonConvert.DeserializeObject(str, typeof(T), new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });

        public static T ToObject<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(T);
            }
            else
            {
                var type = typeof(T);
                try
                {
                    return (T)JsonConvert.DeserializeObject(str, type, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });
                }
                catch
                {
                    // string及びprimitive型(nullable含む)
                    if (Nullable.GetUnderlyingType(type) != null)
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    return (T)Convert.ChangeType(str, type);
                }
            }
        }
        public static object ToObject(this string str, Type type)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject(str, type, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });
                }
                catch
                {
                    // string及びprimitive型(nullable含む)
                    if (Nullable.GetUnderlyingType(type) != null)
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    return Convert.ChangeType(str, type);
                }
            }
        }

        public static object ToJson(this string str, Type type) => string.IsNullOrEmpty(str) ? null : JsonConvert.DeserializeObject(str, type, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });

        public static JValue ToJValue(this string str) => (JValue)(str);
        public static JValue ToJValue(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is JToken)
            {
                return (JValue)(JToken)obj;
            }
            return new JValue(obj);
        }
        public static JProperty ToJProperty(this string str) => str.ToJson().FirstOrDefault() as JProperty;
        public static JArray ToJArray(this string str) => ($"{{ 'x' : [ {str} ] }}".ToJson().FirstOrDefault() as JProperty).Value as JArray;

        public static JToken RemoveField(this JToken token, string field)
        {
            return field != null ? RemoveFields(token, new string[] { field }) : token;
        }

        public static JToken RemoveFields(this JToken token, params string[] fields)
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

        private static JToken RemoveFields(JToken token, string[] fields, string[] removeIgnoreFields = null)
        {
            foreach (var field in fields)
            {
                if (removeIgnoreFields != null && removeIgnoreFields.Contains(field))
                {
                    continue;
                }
                var result = token.SelectToken(field);
                if (result != null)
                {
                    result.Parent.Remove();
                }
            }

            return token;
        }

        public static string ConvertToString(this JValue value)
        {
            if (value == null)
            {
                return null;
            }
            switch (value.Type)
            {
                case JTokenType.String:
                case JTokenType.Date:
                case JTokenType.Guid:
                    return $"\"{value.Value}\"";
                default:
                    return value.Value.ToString();
            }
        }

        private static Regex regexArray = new Regex("^\\[(?<master>.*?)\\]$", RegexOptions.Singleline);

        public static List<JToken> GetValue(this List<JToken> token, string path)
        {
            var result = new List<JToken>();
            token.ForEach(x => result.Add(x.ShallowGetPropertyValue(path)));
            return result;
        }

        public static List<List<JToken>> ChildrenList(this List<JToken> token)
        {
            var result = new List<List<JToken>>();
            token.ForEach(x => result.Add(x?.Children().ToList()));
            return result;
        }

        public static JToken GetIndex(this List<JToken> token, int pos) => token == null || pos >= token.Count() ? null : token[pos];

        public static int MaxItemCount(this List<List<JToken>> token) => token.Select(x => x == null ? 0 : x.Count).Max();

        public static List<JToken> GetIndex(this List<List<JToken>> token, int pos) => token.ToList().Select(x => x == null ? null : x.GetIndex(pos)).ToList();

        public static object ToConvert(this JSchemaType type, JToken token) => type.ToConvert(token?.ToString());

        public static object ToConvert(this JSchemaType type, string str)
        {
            if (str == null)
            {
                return null;
            }

            Type ptype = typeof(object);
            switch (type & ~JSchemaType.Null)
            {
                case JSchemaType.Boolean: ptype = typeof(bool); break;
                case JSchemaType.Integer: ptype = typeof(int); break;
                case JSchemaType.Number: ptype = typeof(decimal); break;
                case JSchemaType.String: ptype = typeof(string); break;
                default: ptype = typeof(object); break;
            }
            if (ptype == typeof(string))
            {
                return str;
            }
            return ptype == typeof(object) ? str.ToJson() : Convert.ChangeType(str, ptype);
        }

        public static JToken ToJValue(this JSchemaType type, object obj, JSchemaType refSourceType = JSchemaType.Null)
        {
            try
            {
                if (obj == null)
                {
                    return null;
                }
                if (obj is JObject)
                {
                    return obj as JObject;
                }
                //nullチェック
                if (obj.GetType().Name == typeof(JValue).Name)
                {
                    var tk = obj.ToJValue();
                    //プロパティのタイプがnull 且つ、value もnull なら、null を返す
                    if ((tk.Type & JTokenType.Null) == JTokenType.Null && tk.Value == null)
                    {
                        return null;
                    }
                }
                //元先一緒なら、そのまま返却
                //refSourceType は予測される型 が和で入る(「1」 なら、String | Integer）ので&して、schematypeと一致するか確認
                if ((type & refSourceType) == type) return obj.ToJValue();

                string str = obj.ToString();

                Type ptype = typeof(object);
                if ((type & JSchemaType.Boolean) == JSchemaType.Boolean)
                {
                    ptype = typeof(bool);
                }
                else if ((type & JSchemaType.Integer) == JSchemaType.Integer)
                {
                    ptype = typeof(int);
                }
                else if ((type & JSchemaType.Number) == JSchemaType.Number)
                {
                    ptype = str.Contains(".") == false && long.TryParse(str, out var ret) == true ? typeof(long) : typeof(decimal);
                }
                else if ((type & JSchemaType.String) == JSchemaType.String)
                {
                    ptype = typeof(string);
                }
                else
                {
                    ptype = typeof(object);
                }
                return ptype == typeof(object) ? str.ToJson() : Convert.ChangeType(str, ptype).ToJValue();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Jsonオブジェクトに指定したフィールドを追加する
        /// </summary>
        /// <param name="token"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static JToken AddField(this JToken token, string field, object val)
        {
            token.First.AddAfterSelf(new JProperty(field, val));
            return token;
        }


        /// <summary>
        /// Jsonオブジェクトの指定したフィールドを新しい値で置き換える
        /// </summary>
        /// <param name="token"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static JToken ReplaceField(this JToken token, string field, object val)
        {
            token.RemoveField(field).First.AddAfterSelf(new JProperty(field, val));
            return token;
        }


    }

    public static class JTokenGetSetValue
    {
        private static Regex regexArray = new Regex("^.+\\[(?<array>.*?)\\]$", RegexOptions.Singleline);

        public static JProperty FindProperty(this JToken json, string propname) => FindPropertyEx(json, propname)?.Item1;
        public static JToken FindToken(this JToken json, string propname) => FindPropertyEx(json, propname)?.Item2;
        public static bool IsExistProperty(this JToken json, string propname) => FindPropertyEx(json, propname) == null ? false : true;

        private static Tuple<JProperty, JToken> FindPropertyEx(this JToken json, string propname)
        {
            var orgpropname = propname;
            string[] prps = propname.Split(".".ToCharArray());
            JProperty resultprop = null;
            JToken resulttoken = json;
            foreach (var p in prps)
            {
                var pname = p;
                var match = regexArray.Match(pname);
                int no = -1;
                if (match.Success)
                {
                    var nostr = pname.Substring(match.Groups["array"].Index, match.Groups["array"].Length);
                    if (int.TryParse(nostr, out no) == false)
                    {
                        throw new Exception("Array element is not a number.");
                    }
                    pname = pname.Substring(0, match.Groups["array"].Index - 1);
                }

                resultprop = json?.Children().ToList().Where(x => ((JProperty)x).Name == pname).FirstOrDefault() as JProperty;
                if (resultprop == null)
                {
                    return null;
                }

                if (match.Success)
                {
                    json = (resultprop.Value as JArray).Skip(no).FirstOrDefault();
                    resultprop = null;
                    resulttoken = json;
                }
                else
                {
                    json = resultprop.Value;
                }
            }

            return new Tuple<JProperty, JToken>(resultprop, resulttoken);
        }

        public static JToken SetValue(this Tuple<JProperty, int?> json, object obj)
        {
            if (json.Item2 == null)
            {
                json.Item1.Value = obj.ToJValue();
            }
            else
            {
                var array = json.Item1.Value as JArray;
                var target = array[json.Item2];
                target.AddAfterSelf(obj.ToJValue());
            }
            return json.Item1;
        }

        public static JToken ShallowGetPropertyValue(this JToken json, string propname)
        {
            try
            {
                return json == null ? null : json[propname];
            }
            catch
            {
                return null;
            }
        }

        public static JToken GetPropertyValue(this JToken json, string propname)
        {
            if (json == null || string.IsNullOrEmpty(propname))
            {
                return null;
            }

            var orgpropname = propname;
            string[] prps = propname.Split(".".ToCharArray());
            foreach (var p in prps)
            {
                var pname = p;
                var match = regexArray.Match(pname);
                int no = -1;
                if (match.Success)
                {
                    var nostr = pname.Substring(match.Groups["array"].Index, match.Groups["array"].Length);
                    if (int.TryParse(nostr, out no) == false)
                    {
                        throw new Exception("Array element is not a number.");
                    }
                    pname = pname.Substring(0, match.Groups["array"].Index - 1);
                }

                var property = json?.Children().ToList().Where(x => ((JProperty)x).Name == pname).FirstOrDefault() as JProperty;
                if (property == null)
                {
                    return null;
                }

                if (match.Success)
                {
                    json = (property.Value as JArray)?.Skip(no).FirstOrDefault();
                }
                else
                {
                    json = property.Value;
                }
            }

            return json;
        }

        /// <summary>
        /// json 内の指定propnameに、value を設定する
        /// {"hoge": null} というjson に、propname "hoge", value "1"として実行すると、
        /// {"hoge":"1"} を設定する
        /// Notifyの処理でNotifyされる側の処理想定で、{"hoge[0].hogeClass.HogeProp":"1"}というリクエストが来た場合に
        /// {"hoge" [ { "hogeClass" : { "HogeProp" : "1" }}]} というデータを作成する
        /// </summary>
        /// <param name="json"></param>
        /// <param name="propname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JToken SetPropertyValue(this JToken json, string propname, object value)
        {
            var root = json;
            if (json == null || string.IsNullOrEmpty(propname))
            {
                return root;
            }

            var orgpropname = propname;
            string[] prps = propname.Split(".".ToCharArray());
            for (int i = 0; i < prps.Length; i++)
            {
                bool islast = (i + 1) == prps.Length;
                var pname = prps[i];
                //propnameが配列指定かどうか
                var match = regexArray.Match(pname);
                int no = -1;
                if (match.Success)
                {
                    //指定の配列添え字番号 (hoge[0]の、0)を取り出す
                    var nostr = pname.Substring(match.Groups["array"].Index, match.Groups["array"].Length);
                    if (int.TryParse(nostr, out no) == false)
                    {
                        throw new Exception("Array element is not a number.");
                    }
                    //指定の配列プロパティ名を取り出す(hoge[0] の、hoge)
                    pname = pname.Substring(0, match.Groups["array"].Index - 1);
                }
                //プロパティをJPropertyで取り出して、Value を指定Valueに設定する
                var property = json?.Children().ToList().Where(x => ((JProperty)x).Name == pname).FirstOrDefault() as JProperty;
                if (property == null)
                {
                    if (islast)
                    {
                        //jsonが、nullや空{}ではなければ、一旦プロパティをnullで作って値をセットし、終了
                        if (json?.Count() > 0)
                        {
                            json.Last().AddAfterSelf(new JProperty(pname, JValue.CreateNull()));
                            property = json?.Children().ToList().Where(x => ((JProperty)x).Name == pname).FirstOrDefault() as JProperty;
                            SetProp(property, match.Success, no, propname, value);
                        }
                        return root;
                    }
                    else
                    {
                        //lastでは無い場合は、作って後続に処理を流す
                        json.Last().AddAfterSelf(new JProperty(pname, JValue.CreateNull()));
                        property = json?.Children().ToList().Where(x => ((JProperty)x).Name == pname).FirstOrDefault() as JProperty;

                    }
                }
                if (islast == true)
                {
                    SetProp(property, match.Success, no, propname, value);
                    break;
                }
                //hoge[0].hogeClass などで下の階層がある場合は、下の階層を取り出して次ループに行く
                if (match.Success)
                {
                    //下の階層を取る
                    json = (property.Value as JArray)?.Skip(no).FirstOrDefault();
                    //指定は更に下階層だが(!islast)、次のプロパティが取れない場合は、一旦指定のオブジェクトを値null で作る
                    if (!islast && json == null)
                    {
                        //下階層が配列
                        var jarr = property?.Value as JArray;
                        //null なら新規作成
                        if (jarr == null)
                        {
                            jarr = new JArray();
                            //下階層がnullなため、添え字指定を0にする
                            no = 0;
                            var token = "{'__dummy__':null}".ToJson();
                            //指定プロパティを、値nullで作成
                            token.First().AddAfterSelf(new JProperty(prps[i + 1], JValue.CreateNull()));
                            token.RemoveField("__dummy__");
                            jarr.Add(token);
                            //jarray が新規に作成なら、指定のプロパティに新規作成のJArray を設定する
                            property.Value = jarr;
                        }
                        else
                        {
                            //配列件数より大きい添え字の指定の場合は、添え字最大値にする
                            no = (jarr.Count - 1);
                        }
                        //新たに作った下階層を取得し、次の処理へ
                        json = (property.Value as JArray)?.Skip(no).FirstOrDefault();
                    }
                }
                else
                {
                    if (property.Value.Type == JTokenType.Null && !islast)
                    {
                        json[pname] = new JObject(new JProperty(prps[i + 1], JValue.CreateNull()));
                    }
                    json = property.Value;
                }
            }

            return root;
        }

        private static void SetProp(JProperty property, bool isDesignatedArray, int designatedArrayIdx, string propname, object value)
        {
            if (property.Value is JArray array && isDesignatedArray)
            {
                int add = designatedArrayIdx - array.Count + 1;
                for (int j = 0; j < add; j++)
                {
                    array.Add(null);
                }
                var pos = array[designatedArrayIdx] as JValue;
                if (value is JValue v)
                {
                    pos.Value = v.Value;
                }
                else if (pos == null)
                {
                    array.Insert(designatedArrayIdx, value as JToken);
                    array.RemoveAt(designatedArrayIdx + 1);
                }
                else
                {
                    pos.Value = value;
                }
            }
            else if (value == null)
            {
                property.Value = null;
            }
            else if (value is JArray)
            {
                property.Value = (JArray)value;
            }
            else if (value is JObject)
            {
                var parent = property.Parent;
                property.Remove();
                parent.FirstOrDefault().AddAfterSelf(new JProperty(propname, value as JObject));
            }
            else if (value is JValue)
            {
                if (isDesignatedArray)
                {
                    //配列指定なので、配列を作ってデータを設定する
                    var jarr = new JArray();
                    jarr.Add((JValue)value);
                    property.Value = jarr;
                }
                else
                {
                    property.Value = (JValue)value;
                }
            }
            else
            {
                if (isDesignatedArray)
                {
                    //配列指定なので、配列を作ってデータを設定する
                    var jarr = new JArray();
                    jarr.Add(value.ToJValue());
                    property.Value = jarr;
                }
                else
                {
                    property.Value = value.ToJValue();
                }
            }
        }

        public static JToken ToJsonArray(this IEnumerable<string> data)
        {
            var array = new JArray();
            if (data != null)
            {
                foreach (string d in data)
                {
                    array.Add(d.ToJson());
                }
            }
            return array;
        }
    }
}
