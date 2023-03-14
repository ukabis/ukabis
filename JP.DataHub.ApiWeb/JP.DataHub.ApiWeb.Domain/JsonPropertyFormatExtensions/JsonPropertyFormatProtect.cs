using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;

namespace JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    // .NET6
    internal class JsonPropertyFormatProtect
    {
        private static readonly JPDataHubLogger Logger = new JPDataHubLogger(typeof(JsonPropertyFormatProtect));
        private bool enableThreadingOfReference = false;

        public enum PropertyFormatType
        {
            UpStreamBeforeValidation = 0x01,
            UpStream = 0x02,
            DownStream = 0x04,
            UPDownStream = 0x06,
        }

        private class FormatMember
        {
            public string Path { get; set; }
            public JToken OriginalValue { get; set; }
            public JToken UpdateValue { get; set; }
            public JSchema Schema { get; set; }
            public List<JToken> Data { get; set; }
        }

        private class TypeAction
        {
            public bool IsUpStream { get; set; }
            public JsonPropertyFormatParser.JsonFormatType Type { get; set; }
            public Func<JSchema, bool, string, List<JToken>, bool> Processing { get; set; }
        }

        public JSchema Schema { get; set; }

        public JToken Update { get; set; }

        public Func<JToken, bool, JToken> GetOriginalData { get; set; }

        public Func<string, bool, Tuple<HttpStatusCode, JToken, bool, string, List<string>>> GetOtherResource { get; set; }
        public UpdateValueCollection UpdateOtherResource { get; set; } = new UpdateValueCollection();
        public Func<string, Tuple<HttpStatusCode, JToken>> GetHistoryReference { get; set; }

        private PropertyFormatType Type { get; set; }
        private List<FormatMember> protect;
        private List<TypeAction> processMap = new List<TypeAction>();

        private const string NOTIFY_UPDATEAPI = "UpdateById";
        private const string NOTIFY_REGISTERAPI = "Register";
        private const string NOTIFY_DELETEAPI = "DeleteById";

        private static Regex regexArray = new Regex("\\[(?<master>.*?)\\]$", RegexOptions.Singleline);
        private List<Task> tasks = new List<Task>();

        public JsonPropertyFormatProtect(bool? isUpStream = null)
        {
            if (isUpStream == null)
            {
                Type = PropertyFormatType.UPDownStream;
            }
            else if (isUpStream == true)
            {
                Type = PropertyFormatType.UpStream;
            }
            else if (isUpStream == false)
            {
                Type = PropertyFormatType.DownStream;
            }
            Init();
        }

        public JsonPropertyFormatProtect(PropertyFormatType type)
        {
            Type = type;
            Init();
        }

        private void Init()
        {
            if (Type.HasFlag(PropertyFormatType.UpStream))
            {
                processMap.Add(new TypeAction() { IsUpStream = true, Type = JsonPropertyFormatParser.JsonFormatType.Reference, Processing = UpstreamNullProcess });
                processMap.Add(new TypeAction() { IsUpStream = true, Type = JsonPropertyFormatParser.JsonFormatType.Reference, Processing = UpstreamValueProcess });
                processMap.Add(new TypeAction() { IsUpStream = true, Type = JsonPropertyFormatParser.JsonFormatType.Protect, Processing = UpstreamProtectProcess });
            }
            if (Type.HasFlag(PropertyFormatType.DownStream))
            {
                processMap.Add(new TypeAction() { IsUpStream = false, Type = JsonPropertyFormatParser.JsonFormatType.Reference, Processing = DownstreamReferenceProcess });
            }
            if (Type.HasFlag(PropertyFormatType.UpStreamBeforeValidation))
            {
                processMap.Add(new TypeAction() { IsUpStream = false, Type = JsonPropertyFormatParser.JsonFormatType.Reference, Processing = UpStreamBeforeValidationProcess });
            }

            enableThreadingOfReference = UnityCore.Resolve<IConfiguration>().GetValue<bool>("AppConfig:EnableThreadingOfReference", false);
        }

        public void Recovery()
        {
            if (Schema == null)
            {
                return;
            }
            // Formatの拡張があるか？
            if (IsExistsProtectProperty() == false)
            {
                return;
            }

            // 元のデータを取得する
            JToken original = null;
            if (GetOriginalData != null)
            {
                original = GetOriginalData(Update, false);
            }
            if (Type.HasFlag(PropertyFormatType.UpStream) && original == null)
            {
                return;
            }

            // Format情報の取得しつつ、もとに戻すべき値を戻す
            protect = new List<FormatMember>();
            var walk = new JsonWalk(Update, original) { Schema = this.Schema };
            walk.Walk(JsonWalkRecovery);

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                Logger.Error($"WaitAll Exception.{ae.Message}", ae);
                foreach (var e in ae.InnerExceptions)
                {
                    Logger.Error(e.Message, e);
                }
                throw;
            }
        }

        private void JsonWalkProperty(JSchema schema, bool isArray, string path, List<JToken> data)
        {
            if (string.IsNullOrEmpty(schema?.Format))
            {
                return;
            }
            var parse = JsonPropertyFormatParser.ParseFormat(schema.Format)?.Where(x => x.FormatType != JsonPropertyFormatParser.JsonFormatType.Other).Any();
            if (parse.Value)
            {
                protect.Add(new FormatMember() { Path = path, UpdateValue = data.GetIndex(0), OriginalValue = data.GetIndex(1), Data = data, Schema = schema });
            }
        }

        private bool IsExistsProtectProperty()
        {
            protect = new List<FormatMember>();
            var walk = new JsonWalk(Update) { Schema = this.Schema };
            walk.Walk(JsonWalkProperty);
            return protect?.Count() > 0;
        }

        private void JsonWalkRecovery(JSchema schema, bool isArray, string path, List<JToken> data)
        {
            if (schema?.Format == null)
            {
                return;
            }
            var parse = JsonPropertyFormatParser.ParseFormat(schema?.Format);
            foreach (var x in processMap)
            {
                if (parse.Where(p => p.FormatType == x.Type).Count() > 0)
                {
                    if (x.Processing(schema, isArray, path, data) == false)
                    {
                        break;
                    }
                }
            }
        }

        private bool UpStreamBeforeValidationProcess(JSchema schema, bool isArray, string path, List<JToken> data)
        {
            // データを取得
            var target = data[0];
            var val = target[path]?.ToString();
            var fererence = JsonPropertyFormatParser.ParseFormat(schema?.Format).Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.Reference).FirstOrDefault();
            if (fererence != null)
            {
                if (isArray == false)
                {
                    var updprop = target.FindProperty(path);
                    if (updprop != null)
                    {
                        updprop.Value = schema.Type.Value.ToConvert(val).ToJValue();
                    }
                }
                else
                {
                    var key = path;
                    Match m = regexArray.Match(key);
                    if (m.Success == true)
                    {
                        JArray array = (JArray)val;
                        int pos = key.Substring<int>(m.Index + 1, m.Length - 2);
                        array[pos].AddAfterSelf(schema.Type.Value.ToConvert(val).ToJValue());
                        array.RemoveAt(pos);
                    }
                }
            }
            else
            {
                target.RemoveField(path);
            }

            return true;
        }

        private bool UpstreamValueProcess(JSchema schema, bool isArray, string path, List<JToken> data)
        {
            // データを取得
            var target = data[0];
            var property = target.FindProperty(path);
            if (property != null)
            {
                var val = target[path]?.ToString();
                JsonPropertyFormatParser.JsonFormatInfo notify = null;
                // SchemaにNotifyの指定があり、値には$Referenceがある場合は、外部リソースを更新するための情報を保持する
                if (JsonPropertyFormatParser.ParseFormat(schema?.Format).Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.Notify).FirstOrDefault() != null)
                {
                    var original = data[1];
                    var orgval = original[path]?.ToString();
                    notify = JsonPropertyFormatParser.ParseFormat(orgval).Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.DollarReference).FirstOrDefault();
                    if (notify != null)
                    {
                        List<JsonPropertyFormatParser.JsonFormatInfo> parse = JsonPropertyFormatParser.ParseFormat(val);
                        var valparse = parse.Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.DollarValue).FirstOrDefault();
                        var trueval = parse.Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.Other).FirstOrDefault();
                        object obj = null;
                        if (valparse != null)
                        {
                            obj = schema.Type.Value.ToConvert(valparse.KeyValue1);
                        }
                        else if (trueval != null)
                        {
                            obj = schema.Type.Value.ToConvert(trueval?.FormatTypeName);
                        }

                        if (GetOtherResource == null)
                        {
                            UpdateOtherResource.Add(new UpdateValue() { Url = notify.KeyValue1?.Replace("/Get/", "/Update/"), OriginalProperty = path, Property = notify.KeyValue2, Value = obj });
                        }
                        else
                        {
                            var result = GetOtherResource(notify.KeyValue1, true);
                            // NotifyがODataクエリか
                            var isNotifyQueryOData = result.Item3;
                            // 参照先のControllerUrl
                            var refSourceControllerUrl = result.Item4;
                            // リポジトリキー
                            var keyProperties = result.Item5;
                            // Notify先データ
                            var sourcedata = result.Item1 == HttpStatusCode.OK ? result.Item2 : null;

                            if (refSourceControllerUrl == null && keyProperties == null)
                            {
                                throw new InvalidNotifyDataException("Not Found Notify Source Api");
                            }

                            //NotifyのURL(クエリ)がOData かどうか
                            if (isNotifyQueryOData)
                            {
                                //IDを突合せして、Patch対象、Post対象、Delete対象を設定する
                                SetUpdateOtherResourceOData(sourcedata, refSourceControllerUrl, path, notify, obj == null ? new JArray().ToJson() : obj.ToJson(), keyProperties);
                            }
                            else
                            {
                                var id = sourcedata == null ? "" : sourcedata[0]["id"].ToString();
                                if (string.IsNullOrEmpty(id))
                                {
                                    throw new InvalidNotifyDataException("Not Found Notify Source Data");
                                }
                                var sourceValue = sourcedata[0].GetPropertyValue(notify.KeyValue2);
                                object rollbackValue = null;
                                if (sourceValue == null || sourceValue.Type == JTokenType.Null)
                                {
                                    rollbackValue = null;
                                }
                                else
                                {
                                    rollbackValue = schema.Type.Value.ToConvert(sourceValue);
                                }
                                UpdateOtherResource.Add(new UpdateValue() { Url = $"{refSourceControllerUrl}/{NOTIFY_UPDATEAPI}/{id}", ControllerUrl = refSourceControllerUrl, OriginalProperty = path, Property = notify.KeyValue2, Value = obj, RollbackUrl = $"{refSourceControllerUrl}/{NOTIFY_UPDATEAPI}/{id}", RollbackValue = rollbackValue });
                            }
                        }
                    }
                }
                ResetOriginal(schema, isArray, path, data, val);
            }
            return true;
        }

        /// <summary>
        /// IDを突合せして、Patch対象、Post対象、Delete対象を設定する
        /// </summary>
        /// <param name="refSourceData"></param>
        /// <param name="controllerUrl"></param>
        /// <param name="path"></param>
        /// <param name="notify"></param>
        /// <param name="requestData"></param>
        /// <param name="reposKeys"></param>
        private void SetUpdateOtherResourceOData(JToken refSourceData, string controllerUrl, string path, JsonPropertyFormatParser.JsonFormatInfo notify, JToken requestData, List<string> reposKeys)
        {
            var reqDataList = requestData.ToList();
            var refDataList = refSourceData == null ? new List<JToken>() : refSourceData.ToList();
            //IDかリポジトリキー、どちらもないリクエストが１つでもあれば、BadRequest
            if (!CheckKeys(reqDataList, "id", reposKeys))
            {
                throw new InvalidNotifyDataException("One or Some request data does not include ID or Repository Key(s) property.");
            }

            //Update対象
            int i = reqDataList.Count;
            //リクエストデータをキーにチェック
            while (i > 0)
            {
                var u1 = reqDataList[0];
                //Notify先データと比較して、同じ ID or リポジトリキーは、Update対象
                (var id, var idx) = GetPropertyId(u1, "id", reposKeys, refDataList);
                if (id != null)
                {
                    var rollback = refSourceData.Where(x => x["id"].ToString() == id.ToString()).First();
                    UpdateOtherResource.Add(new UpdateValue() { Url = $"{controllerUrl}/{NOTIFY_UPDATEAPI}/{id}", ControllerUrl = controllerUrl, OriginalProperty = null, Property = null, Value = u1, RollbackUrl = $"{controllerUrl}/{NOTIFY_UPDATEAPI}/{id}", RollbackValue = rollback });
                    //見つけたら削除する
                    reqDataList.RemoveAt(0);
                    refDataList.RemoveAt(idx);
                }
                i--;
            }

            //reqDataに残っているのは、Regist対象
            foreach (var r1 in reqDataList)
            {
                UpdateOtherResource.Add(new UpdateValue() { Url = $"{controllerUrl}/{NOTIFY_REGISTERAPI}", ControllerUrl = controllerUrl, Value = r1, TargetHttpMethod = HttpMethod.Post, RollbackUrl = null, RollbackHttpMethod = HttpMethod.Delete });
            }

            //refSourceに残っているのはDelete対象
            foreach (var d1 in refDataList)
            {
                UpdateOtherResource.Add(new UpdateValue() { Url = $"{controllerUrl}/{NOTIFY_DELETEAPI}/{d1["id"]}", ControllerUrl = controllerUrl, OriginalProperty = null, Property = null, Value = d1, TargetHttpMethod = HttpMethod.Delete, RollbackValue = d1, RollbackUrl = $"{controllerUrl}/{NOTIFY_REGISTERAPI}", RollbackHttpMethod = HttpMethod.Post });
            }
        }

        private (string, int) GetPropertyId(JToken reqData, string checkKeyId, List<string> checkReposKeys, List<JToken> refSourceData)
        {
            for (int i = 0; i < refSourceData.Count; i++)
            {
                var s1 = refSourceData[i];
                //リクエストデータにIDがあれば、ID優先
                if (reqData[checkKeyId] != null)
                {
                    if (reqData[checkKeyId].ToString() == s1[checkKeyId].ToString())
                    {
                        return (s1[checkKeyId].ToString(), i);
                    }
                }
                //IDが不一致か、ID無しの場合は、リポジトリキーを見る
                var flg = new List<bool>();
                //リポジトリキー
                foreach (var p1 in checkReposKeys)
                {
                    var data1 = reqData[p1].ToString();
                    var data2 = s1[p1]?.ToString();
                    if (data1 != data2)
                    {
                        flg.Add(false);
                    }
                    else
                    {
                        flg.Add(true);
                    }
                }
                if (flg.Count != 0 && !flg.Contains(false))
                {
                    return (s1["id"].ToString(), i);
                }
            }
            return (null, -1);
        }

        private bool CheckKeys(List<JToken> reqDataList, string checkPropId, List<string> reposKeys)
        {
            foreach (var req1 in reqDataList)
            {
                //IDが取得可能か
                var val1 = req1[checkPropId];
                JToken val2 = null;
                //リポジトリキーは複合キーの可能性があるので、全部あるのが条件
                foreach (var c1 in reposKeys)
                {
                    val2 = req1[c1];
                    if (val2 == null)
                    {
                        break;
                    }
                }
                //IDかリポジトリキー、どちらかが無いとNG
                if (val1 == null && val2 == null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool UpstreamNullProcess(JSchema schema, bool isArray, string path, List<JToken> data)
        {
            // データを取得
            var update = data[0];
            var val = update == null ? null : update[path]?.ToString();
            var valparse = JsonPropertyFormatParser.ParseFormat(val).Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.DollarNull).FirstOrDefault();
            if (valparse != null)
            {
                var original = data[1];
                var orgprop = original.FindProperty(path);
                var dollref = JsonPropertyFormatParser.ParseFormat(orgprop.Value?.ToString()).Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.DollarReference).FirstOrDefault();
                if (dollref != null)
                {
                    var updprop = update.FindProperty(path);
                    if (isArray == false)
                    {
                        updprop.Value = $"$Reference({dollref.KeyValue1},{dollref.KeyValue2})";
                    }
                    return false;
                }
            }
            return true;
        }

        private bool UpstreamProtectProcess(JSchema schema, bool isArray, string path, List<JToken> data)
        {
            ResetOriginal(schema, isArray, path, data);
            return true;
        }

        private bool DownstreamReferenceProcess(JSchema schema, bool isArray, string path, List<JToken> data)
        {
            // データを取得
            var target = data[0];
            var val = target == null ? null : target.GetPropertyValue(path)?.ToString();
            var list = JsonPropertyFormatParser.ParseFormat(val);

            // $Valueがある場合は書き換えているため、その内容を返す
            var valparse = list.Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.DollarValue).FirstOrDefault();
            if (valparse != null)
            {
                var updprop = target.FindProperty(path);
                if (isArray == false)
                {
                    updprop.Value = valparse.KeyValue1;
                }
                return true;
            }

            // $Referenceがある場合は、参照元の情報で返す
            valparse = list.Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.DollarReference).FirstOrDefault();
            if (valparse != null)
            {
                try
                {
                    var updprop = target.FindProperty(path);
                    Action action = () => GetReferenceValue(valparse, val, schema, updprop);
                    if (enableThreadingOfReference)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception e)
                            {
                                Logger.Error("DownstreamReferenceProcess Task.Run()" + e.Message, e);
                                throw;
                            }
                        }));
                    }
                    else
                    {
                        action();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("DownstreamReferenceProcess" + e.Message, e);
                    throw;
                }

                return true;
            }
            return true;
        }

        private void GetReferenceValue(JsonPropertyFormatParser.JsonFormatInfo valparse, string val, JSchema schema, JProperty updprop)
        {
            try
            {
                var url = valparse.KeyValue1;
                var propname = valparse.KeyValue2;
                var propnames2 = valparse.KeyValues2;

                Tuple<HttpStatusCode, JToken> result = null;
                //通常のデータ取得
                if (GetOtherResource != null)
                {
                    Tuple<HttpStatusCode, JToken, bool, string, List<string>> resultGetOtherResource = null;
                    try
                    {
                        resultGetOtherResource = GetOtherResource(url, false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"GetReferenceValue. GetOtherResource. url={url}");
                        Logger.Error(ex.Message, ex);
                        throw;
                    }

                    if (resultGetOtherResource != null && resultGetOtherResource.Item1 != HttpStatusCode.OK &&
                        resultGetOtherResource.Item1 != HttpStatusCode.NotFound)
                    {
                        throw new Exception(
                            $"「{val}」error. Data can't be obtained from the URL. HttpStatusCode is {result.Item1}");
                    }

                    result = new Tuple<HttpStatusCode, JToken>(resultGetOtherResource.Item1,
                        resultGetOtherResource.Item2);
                }
                //履歴データ取得
                else if (GetHistoryReference != null)
                {
                    try
                    {
                        result = GetHistoryReference(url);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"GetReferenceValue. GetHistoryReference. url={url}");
                        Logger.Error(ex.Message, ex);
                        throw;
                    }
                }

                var json = result?.Item2;
                if (json != null)
                {
                    // ↓はデータが取得できないときはエラーとする場合
                    // throw new Exception($"「{val}」error. Data can't be obtained from the URL.");

                    //呼び先から配列で返ってきた場合
                    //[array, null]などの場合があるので、array でANDしてチェックする
                    if ((json.Type & JTokenType.Array) == JTokenType.Array)
                    {
                        //呼び先からarrayで返って来ていて、呼び元もarray定義である場合は、arrayでデータ返却
                        //[array, null]などの場合があるので、array でANDしてチェックする
                        if ((schema.Type & JSchemaType.Array) == JSchemaType.Array)
                        {
                            var DUMMY = "__dummy__";

                            var datas = new List<JToken>();
                            //データ取り出し
                            foreach (var jsondata in json)
                            {
                                JToken objarr = null;
                                //プロパティは複数か
                                if (propnames2 != null)
                                {
                                    //{propname:data}にして返却
                                    objarr = $"{{'{DUMMY}':null}}".ToJson();
                                    foreach (var p in propnames2)
                                    {
                                        //プロパティ名には空白が入っている可能性があるので、除去した上で使う
                                        var tmpjson = jsondata.GetPropertyValue(p.Replace(" ", ""));
                                        objarr.FirstOrDefault().AddAfterSelf(new JProperty(p, tmpjson));
                                    }

                                    objarr.RemoveField(DUMMY);
                                }
                                else
                                {
                                    //{propname:data}にして返却
                                    objarr = $"{{'{DUMMY}':null}}".ToJson();
                                    var tmpjson = jsondata.GetPropertyValue(propname);
                                    objarr.FirstOrDefault().AddAfterSelf(new JProperty(propname, tmpjson));
                                    objarr.RemoveField(DUMMY);
                                }

                                datas.Add(objarr?.ToJson());
                            }

                            updprop.Value = JArray.FromObject(datas).ToJson();
                            return;
                        }
                        //呼び元が配列設定でない場合は、1行目のみ返却

                        json = json[0];
                    }

                    //単一オブジェクト返却
                    var obj = json.GetPropertyValue(propname);
                    updprop.Value =
                        schema.Type.Value.ToJValue(obj, obj == null ? JSchemaType.Null : (JSchemaType)obj.Type);
                }
                else
                {
                    updprop.Value = null;
                }
            }
            catch (Exception e)
            {
                Logger.Error("GetReferenceValue" + e.Message, e);
                throw;
            }
        }

        private void ResetOriginal(JSchema schema, bool isArray, string path, List<JToken> data, string additionalValue = null)
        {
            var update = data[0];
            var original = data[1];
            if (original == null || update == null || path == null)
            {
                return;
            }

            if (isArray == false)
            {
                var orgval = original.ShallowGetPropertyValue(path);
                if (additionalValue != null)
                {
                    var parse = JsonPropertyFormatParser.ParseFormat(orgval?.ToString()).Where(x => x.FormatType != JsonPropertyFormatParser.JsonFormatType.DollarValue).ToList();
                    orgval = $"{string.Join(";", parse.Select(x => x.FormatTypeName).ToList())};$Value({additionalValue})".ToJValue();
                }
                var updprop = update.FindProperty(path);
                if (updprop != null)
                {
                    updprop.Value = orgval;
                }
                else if (orgval != null)
                {
                    update.Children().FirstOrDefault().AddAfterSelf(new JProperty(path, orgval));
                }
                else if (orgval == null)
                {
                    update.RemoveField(path);
                }
            }
            else
            {
                var key = path;
                Match m = regexArray.Match(key);
                if (m.Success == true)
                {
                    JArray array = (JArray)update;
                    int pos = key.Substring<int>(m.Index + 1, m.Length - 2);
                    var orgval = original[pos];
                    if (additionalValue != null)
                    {
                        orgval += $";{additionalValue}";
                    }
                    array[pos].AddAfterSelf(orgval);
                    array.RemoveAt(pos);
                }
            }
        }
    }
}
