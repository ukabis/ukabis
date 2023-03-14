using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    public class JsonWalk
    {
        public JSchema Schema { get; set; }
        public List<JToken> Data { get; set; } = new List<JToken>();

        public JsonWalk()
        {
        }

        public JsonWalk(params JToken[] data)
        {
            data.ToList().ForEach(x => Data.Add(x));
        }

        public JsonWalk(IEnumerable<JToken> data)
        {
            data.ToList().ForEach(x => Data.Add(x));
        }

        public void Walk(Action<JSchema, bool, string, List<JToken>> jsonProp) => Walk(Schema, jsonProp, Data);

        private string GetPath(string root, string node) => string.IsNullOrEmpty(root) ? node : $"{root}.{node}";
        private string GetPath(string root, string path1, string path2) => GetPath(root, GetPath(path1, path2));
        private static JSchema ToJSchema(string str) => str == null ? null : JSchema.Parse(str, new JSchemaReaderSettings());

        private void Walk(JSchema schema, Action<JSchema, bool, string, List<JToken>> actionProp, List<JToken> data, string fullpath = null, string path = null)
        {
            if (schema?.Properties?.Keys == null)
            {
                return;
            }

            foreach (var key in schema?.Properties?.Keys)
            {
                // additionalPropertiesは無視する（渡さない）
                if (key == "additionalProperties")
                {
                    continue;
                }

                var childschema = schema.Properties[key];
                var dataval = data.GetValue(key);

                actionProp(childschema, false, key, data);

                if (childschema.Type == JSchemaType.Object)
                {
                    // オブジェクトの中身
                    Walk(childschema, actionProp, dataval, path == null ? key : path + "." + key);
                    //childschema.Properties.Keys.ToList().ForEach(x => {
                    //    Walk(childschema.Properties[x], actionProp, dataval.GetValue(x), x);
                    //});
                }
                else if (childschema.Type == JSchemaType.Array)
                {
                    var childlist = dataval.ChildrenList();
                    int count = childlist.MaxItemCount();
                    // 配列の中身
                    for (int i = 0; i < count; i++)
                    {
                        var nextschema = childschema.Items.FirstOrDefault();
                        if (nextschema == null)
                        {
                            if (childschema.ExtensionData.ContainsKey("item"))
                            {
                                var item = childschema.ExtensionData["item"];
                                var itemtokens = item.Children().ToList();
                                if (i < itemtokens.Count)
                                {
                                    nextschema = ToJSchema(itemtokens[i].ToString());
                                }
                                else if (itemtokens.Count > 0)
                                {
                                    nextschema = ToJSchema(itemtokens[0].ToString());
                                }
                                actionProp(nextschema, true, GetPath(fullpath, $"{key}[{i}]"), dataval);
                            }
                        }
                        else
                        {
                            actionProp(childschema, true, GetPath(fullpath, $"{key}[{i}]"), dataval);
                            var td = childlist.GetIndex(i);
                            Walk(nextschema, actionProp, td, GetPath(fullpath, $"{key}[{i}]"));
                        }
                    }
                }
            }
        }
    }
}
