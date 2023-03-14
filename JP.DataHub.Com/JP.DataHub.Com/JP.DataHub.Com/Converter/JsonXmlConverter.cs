using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.Com.Converter
{
    public class JsonXmlConverter
    {

        static readonly private string TypeAnonymousArrayKey = "anonymous_array";
        static readonly private string TypeArrayKey = "array";
        static readonly private string TypeTypeKey = "type";

        static readonly private string AnonymousArrayValue = "true";
        static readonly private string ArrayValue = "true";
        static readonly private string TypeNumberValue = "number";
        static readonly private string DefaultNamespacePrefix = "dh";


        private string NamespacePerfix { get; set; } = DefaultNamespacePrefix;

        private string NamespaceProperty
        {
            get
            {
                return ($"@xmlns:{NamespacePerfix}");
            }
        }

        private string AnonymousArrayKey
        {
            get
            {
                return ($"{NamespacePerfix}:{TypeAnonymousArrayKey}");
            }
        }
        private string ArrayKey
        {
            get
            {
                return ($"{NamespacePerfix}:{TypeArrayKey}");
            }
        }
        private string TypeKey
        {
            get
            {
                return ($"{NamespacePerfix}:{TypeTypeKey}");
            }
        }

        /// <summary>
        /// XMLからJsonに変換
        /// </summary>
        public string XmlToJson(string xml,string xmlNameSpace)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml.Replace("\r\n", ""));
            var pre = xmlDocument.DocumentElement.GetPrefixOfNamespace(xmlNameSpace);
            if (!string.IsNullOrEmpty(pre))
            {
                NamespacePerfix = pre;
            }

            //独自NameSpaceは削除する
            var json = JToken.Parse(JsonConvert.SerializeXmlNode(xmlDocument.DocumentElement, Newtonsoft.Json.Formatting.None, true));
            var namespaceProperty = json.Children().Where(x => x.Type == JTokenType.Property).Cast<JProperty>().Where(x => x.Name == NamespaceProperty).FirstOrDefault();
            if (namespaceProperty != null)
            {
                namespaceProperty.Remove();
            }

            var result = ConvertTypeJToken(json);
            result = ConvertArrayTypeJToken(json);
            result = ConvertAnonymousArray(result);
            return result.ToString();
        }

        /// <summary>
        /// JSONからXMLに変換
        /// </summary>
        public XDocument JsonToXml(string json,string xmlNameSpace)
        {
            var jtoken = JToken.Parse(json);
            jtoken = AddType(jtoken);
            jtoken = AddAnonymousArrayType(jtoken);
            jtoken = AddArrayType(jtoken);
            //独自Namespaceを追加する
            var namespaceProperty = jtoken.Children().Where(x => x.Type == JTokenType.Property).Cast<JProperty>().Where(x => x.Value.ToString() == xmlNameSpace).FirstOrDefault();
            if (namespaceProperty == null)
            {
                ((JObject)jtoken).AddFirst(new JProperty(NamespaceProperty, xmlNameSpace));
            }
            else
            {
                NamespacePerfix = namespaceProperty.Name;
            }
            //rootエンティティを強制付与
            XDocument xdoc = JsonConvert.DeserializeXNode(jtoken.ToString(), "root");
            return xdoc;
        }
        /// <summary>
        /// 匿名配列に「@dh_type:anonymous_array」を付与する
        /// </summary>
        private JToken AddAnonymousArrayType(JToken jToken)
        {
            JObject result = new JObject();
            var list = jToken.Children().ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                jToken.Children().ToList()[i] = AddAnonymousArrayType(list[i]);
            }

            if (jToken.Type == JTokenType.Array && jToken.Parent == null)
            {
                result.Add($"@{AnonymousArrayKey}", new JValue(AnonymousArrayValue));
                result.Add("item", jToken);
            }
            if (jToken.Type == JTokenType.Array && jToken.Parent != null && jToken.Parent.Type != JTokenType.Property)
            {
                result.Add($"@{AnonymousArrayKey}", new JValue(AnonymousArrayValue));
                result.Add("item", jToken);
            }
            if (result.First == null)
            {
                return jToken;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// タイプに「@dh_type:XXX」を付与する
        /// </summary>
        private JToken AddType(JToken jToken)
        {
            foreach (var jc in jToken.Children())
            {
                var hoge = AddType(jc);
            }

            if (jToken.Type == JTokenType.Property)
            {
                var value = ((JProperty)jToken).Value;
                if (value.Type == JTokenType.Float || value.Type == JTokenType.Integer)
                {
                    var ob = new JObject();
                    ob.Add(new JProperty($"@{TypeKey}", TypeNumberValue));
                    ob.Add(new JProperty("#text", value));
                    ((JProperty)jToken).Value = ob;
                }
            }
            if (jToken.Type == JTokenType.Array)
            {
                var ja = (JArray)jToken;
                for (int i = 0; i < jToken.Children().Count(); i++)
                {
                    if (ja[i].Type == JTokenType.Integer || ja[i].Type == JTokenType.Float)
                    {
                        JObject jObject = new JObject();
                        jObject.Add(new JProperty($"@{TypeKey}", new JValue(TypeNumberValue)));
                        jObject.Add(new JProperty("#text", ((JArray)jToken)[i]));
                        ja[i] = jObject;
                    }
                }
            }

            return jToken;
        }


        /// <summary>
        /// Arrayに「@dh_type:array」を付与する
        /// </summary>
        private JToken AddArrayType(JToken jToken)
        {
            if (jToken.Children().Count() == 0)
            {
                return jToken;
            }

            if (jToken.Type == JTokenType.Array)
            {
                //配列のオブジェクト
                foreach (var o in jToken.Children())
                {
                    if (o.Type == JTokenType.Object)
                    {
                        if (!o.Children().Where(x => x.Type == JTokenType.Property).Any(x => ((JProperty)x).Name == $"@{ArrayKey}" && ((JProperty)x).Value.ToString() == ArrayValue))
                        {
                            ((JObject)o).Add(new JProperty($"@{ArrayKey}", new JValue(ArrayValue)));
                        }
                    }
                }

                //配列の値
                var child = jToken.Children().Where(x => x.Type != JTokenType.Object);
                List<JObject> convList = new List<JObject>();
                for (int i = child.Count() - 1; i >= 0; i--)
                {
                    JObject jObject = new JObject();
                    jObject.Add(new JProperty($"@{ArrayKey}", new JValue(ArrayValue)));
                    jObject.Add(new JProperty("#text", ((JArray)jToken)[i]));
                    convList.Add(jObject);
                    ((JArray)jToken)[i].Remove();
                }
                foreach (var c in convList)
                {
                    ((JArray)jToken).AddFirst(c);
                }

            }

            foreach (var jc in jToken.Children())
            {
                AddArrayType(jc);
            }
            return jToken;
        }



        private JToken ConvertAnonymousArray(JToken json)
        {
            JToken result;
            if (!HasAnonymousArray(json))
            {
                return json;
            }
            var path = GetDHTypePath(AnonymousArrayKey, AnonymousArrayValue, json);

            var targetToken = JObject.Parse(json.SelectToken(path).ToString());
            var convertToken = ConvertArrayAnonymousJToken(targetToken);

            if (string.IsNullOrEmpty(path))
            {
                //Topが配列
                result = convertToken;
            }
            else
            {
                var pathSplit = path.Split('.');
                var targetProperty = pathSplit[pathSplit.Length - 2];

                var rep = json.SelectTokens(path).FirstOrDefault().Parent.Parent.Parent.Parent;
                var p = ((JObject)rep).Property(targetProperty);
                p.Remove();
                ((JObject)rep).Add(targetProperty, convertToken);
                result = json;
            }
            if (HasAnonymousArray(result))
            {
                return ConvertAnonymousArray(result);
            }

            return result;
        }

        /// <summary>
        /// @dh_type:numberがついているものを数値に変換
        /// </summary>
        private JToken ConvertTypeJToken(JToken json)
        {
            JToken result;
            if (!HasNumber(json))
            {
                return json;
            }
            var path = GetDHTypePath(TypeKey, TypeNumberValue, json);
            var convertToken = ConvertNumberTypeJToken(json.SelectToken(path));
            if (convertToken == null)
            {
                return json;
            }
            var pathSplit = path.Split('.');
            var targetProperty = pathSplit[pathSplit.Length - 1];

            var rep = json.SelectTokens(path).FirstOrDefault();
            if (rep.Parent.Type == JTokenType.Property)
            {
                var p = ((JProperty)rep.Parent);
                p.Value = convertToken;
            }
            result = json;

            if (HasNumber(result))
            {
                return ConvertTypeJToken(result);
            }

            return result;
        }




        /// <summary>
        /// @dh_type:arrayがついているものを配列に変換
        /// </summary>
        private JToken ConvertArrayTypeJToken(JToken json)
        {
            JToken result;
            if (!HasArray(json))
            {
                return json;
            }
            var path = GetDHTypePath(ArrayKey, ArrayValue, json, true);
            var convertToken = ConvertArrayJToken(json.SelectToken(path));
            if (convertToken == null)
            {
                return json;
            }
            var pathSplit = path.Split('.');
            var targetProperty = pathSplit[pathSplit.Length - 1];

            var rep = json.SelectTokens(path).FirstOrDefault();
            if (rep.Parent.Type == JTokenType.Property)
            {
                var p = ((JProperty)rep.Parent);
                var pp = p.Parent;
                p.Remove();
                pp.Add(new JProperty(p.Name, convertToken));
            }
            result = json;

            if (HasArray(result))
            {
                return ConvertArrayTypeJToken(result);
            }

            return result;
        }


        private string GetDHTypePath(string key, string value, JToken jToken, bool isArray = false)
        {
            string result = "";
            foreach (var content in jToken.Children())
            {
                if (content is JProperty)
                {
                    if (IsDHType(key, value, content))
                    {
                        if (isArray)
                        {
                            //配列の場合は親のパスを返す
                            return jToken.Parent.Path;
                        }
                        else
                        {
                            return jToken.Path;
                        }
                    }
                }

                result = GetDHTypePath(key, value, content, isArray);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            return result;
        }

        private JToken ConvertArrayAnonymousJToken(JToken jToken)
        {
            foreach (var content in jToken.Children())
            {
                if (!HasAnonymousArray(content))
                {
                    foreach (var ar in content.Children())
                    {
                        if (ar is JArray)
                        {
                            return ar;
                        }
                        else
                        {
                            JArray jArray = new JArray();
                            jArray.Add(ar);
                            return jArray;
                        }
                    }
                }
            }
            return null;
        }

        private JToken ConvertArrayJToken(JToken jToken)
        {
            if (jToken == null)
            {
                return jToken;
            }
            if (jToken.Type == JTokenType.Array)
            {
                var tmpJtoken = jToken.Children().ToArray();
                //値配列
                for (int i = 0; i < tmpJtoken.Count(); i++)
                {
                    var ob = tmpJtoken[i];
                    if (ob.Type != JTokenType.Object) continue;
                    var pros = ob.Children().Where(x => x.Type == JTokenType.Property).Select(x => (JProperty)x);
                    if (pros.Any(x => x.Name == $"@{ArrayKey}" && x.Value.ToString() == ArrayValue))
                    {
                        var value = pros.Where(x => x.Name == "#text").FirstOrDefault();
                        //{"@dh_type": "array","#text": "Hoge"}のパターンの場合はObjectを上書き
                        if (pros.Count() == 2 && value != null)
                        {
                            ((JArray)jToken)[i] = (value.Value);
                        }
                        else
                        {
                            var ps = ((JArray)jToken)[i].Where(x => x.Type == JTokenType.Property).Cast<JProperty>();
                            ps.Where(x => x.Name == $"@{ArrayKey}" && x.Value.ToString() == ArrayValue).First().Remove();
                        }
                    }
                }
            }
            if (jToken.Type == JTokenType.Object)
            {
                var remove = jToken.Children().Where(x => x.Type == JTokenType.Property).Cast<JProperty>();
                var removeTarget = remove.Where(x => x.Name == $"@{ArrayKey}" && x.Value.ToString() == ArrayValue).FirstOrDefault();
                if (removeTarget != null)
                {
                    removeTarget.Remove();
                    if (jToken.Parent != null && jToken.Parent.Type != JTokenType.Array)
                    {
                        if (jToken.Children().Count() == 1)
                        {
                            //{"#text": "Hoge"}のパターンの場合はValueのみにする
                            var prop = jToken.Children().First();
                            if (prop.Type == JTokenType.Property)
                            {
                                if (((JProperty)prop).Name == "#text")
                                {
                                    jToken = ((JProperty)prop).Value;
                                }
                            }
                        }

                        //親が配列じゃないものは配列を入れてあげる。
                        JArray jArray = new JArray();
                        jArray.Add(jToken);
                        jToken = jArray;
                    }
                }
            }
            return jToken;
        }
        private JToken ConvertNumberTypeJToken(JToken jToken)
        {
            if (jToken == null)
            {
                return jToken;
            }

            if (jToken.Type == JTokenType.Object)
            {
                var remove = jToken.Children().Where(x => x.Type == JTokenType.Property).Cast<JProperty>();
                var removeTarget = remove.Where(x => x.Name == $"@{TypeKey}" && x.Value.ToString() == TypeNumberValue).FirstOrDefault();
                if (removeTarget != null)
                {
                    removeTarget.Remove();
                    if (jToken.Parent != null && (jToken.Parent.Type == JTokenType.Property || jToken.Parent.Type == JTokenType.Array))
                    {
                        if (jToken.Children().Count() == 1)
                        {
                            //{"#text": "Hoge"}のパターンの場合はValueのみにする
                            var prop = jToken.Children().First();
                            if (prop.Type == JTokenType.Property)
                            {
                                if (((JProperty)prop).Name == "#text")
                                {
                                    var p = ConvertNumberValue(((JProperty)prop).Value.ToString());
                                    if (p != null)
                                    {
                                        ((JProperty)prop).Value = p;
                                        jToken = ((JProperty)prop).Value;
                                    }

                                }
                            }
                        }
                        else
                        {
                            var target = jToken.Children().Where(x => x.Type == JTokenType.Property).Cast<JProperty>().Where(x => x.Name == "#text").FirstOrDefault();
                            if (target != null)
                            {
                                var p = ConvertNumberValue(target.Value.ToString());
                                target.Value = p;
                            }
                        }
                    }
                }
            }
            return jToken;
        }

        private JValue ConvertNumberValue(string value)
        {
            int result;
            decimal result2;
            if (int.TryParse(value, out result))
            {
                return new JValue(result);
            }
            else if (decimal.TryParse(value, out result2))
            {
                return new JValue(result2);
            }
            return null;
        }

        private bool HasAnonymousArray(JToken jToken)
        {
            if (IsDHType(AnonymousArrayKey, AnonymousArrayValue, jToken))
            {
                return true;
            }

            foreach (var content in jToken.Children())
            {
                if (content is JProperty)
                {
                    if (IsDHType(AnonymousArrayKey, AnonymousArrayValue, content))
                    {
                        return true;
                    }
                }
                if (HasAnonymousArray(content))
                {
                    return true;
                }
            }
            return false;
        }
        private bool HasArray(JToken jToken)
        {
            foreach (var content in jToken.Children())
            {
                if (content is JProperty)
                {
                    if (IsDHType(ArrayKey, ArrayValue, content))
                    {
                        return true;
                    }
                }
                if (HasArray(content))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasNumber(JToken jToken)
        {
            foreach (var content in jToken.Children())
            {
                if (content is JProperty)
                {
                    if (IsDHType(TypeKey, TypeNumberValue, content))
                    {
                        return true;
                    }
                }
                if (HasNumber(content))
                {
                    return true;
                }
            }
            return false;
        }


        private bool IsDHType(string key, string value, JToken jToken)
        {
            if (jToken is JProperty)
            {
                if (((JProperty)jToken).Name == $"@{key}")
                {
                    if (((JProperty)jToken).Value.ToString() == value)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
