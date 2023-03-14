using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public static class BsonDocumentExtensions
    {
        public static BsonArray ToBsonArray(this string str) => BsonSerializer.Deserialize<BsonArray>(str);
        public static T ToObject<T>(this BsonDocument bson) => BsonSerializer.Deserialize<T>(bson?.ToString());


        #region Decimalize：浮動小数点の誤差問題を回避するため数値をDecimalに変換

        #region BsonValue

        public static BsonValue ToDecimalizedBsonValue(this object value)
        {
            if (value is int ||
                value is long ||
                value is double)
            {
                return BsonTypeMapper.MapToBsonValue(value, BsonType.Decimal128);
            }
            else
            {
                return BsonValue.Create(value);
            }
        }

        #endregion

        #region BsonDocument

        public static BsonDocument ToDecimalizedBsonDocument(this JToken json)
        {
            return json?.ToString().ToDecimalizedBsonDocument();
        }
        public static BsonDocument ToDecimalizedBsonDocument(this string str)
        {
            return new DecimalizationBsonSerializer().DeserializeDocument(str);
        }
        public static bool TryToDecimalizedBsonDocument(this string str, out BsonDocument bson)
        {
            bson = null;

            try
            {
                bson = new DecimalizationBsonSerializer().DeserializeDocument(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region BsonArray

        public static BsonArray ToDecimalizedBsonArray(this JToken json)
        {
            return json?.ToString().ToDecimalizedBsonArray();
        }
        public static BsonArray ToDecimalizedBsonArray(this string str)
        {
            return new DecimalizationBsonSerializer().DeserializeArray(str);
        }

        #endregion

        #endregion

        #region ReverseDecimalize：Decmimal化を解除してJsonに変換

        public static JToken ToJsonWithReverseDecimalization(this BsonDocument decimalizedBson)
        {
            var json = JToken.Parse(decimalizedBson.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson }));
            ReverseDecimalize(json);
            return json;
        }

        private static void ReverseDecimalize(JToken json)
        {
            if (json.Type == JTokenType.Object)
            {
                var properties = ((JObject)json).Properties();
                if (properties.Count() == 1 && properties.First().Name == "$numberDecimal")
                {
                    var valueStr = json[properties.First().Name].Value<string>();
                    if (valueStr.Contains("."))
                    {
                        json.Replace(double.Parse(valueStr));
                    }
                    else
                    {
                        json.Replace(long.Parse(valueStr));
                    }
                    return;
                }
            }

            if (json.Type == JTokenType.Object ||
                json.Type == JTokenType.Array ||
                json.Type == JTokenType.Property)
            {
                var children = json.Children().ToList();
                for (var i = 0; i < children.Count; i++)
                {
                    ReverseDecimalize(children[i]);
                }
            }
        }

        #endregion


        /// <summary>
        /// BsonDocumentをマージする
        /// </summary>
        /// <param name="bson"></param>
        /// <param name="documents"></param>
        /// <returns></returns>
        /// <remarks>MongoDB.BsonのMergeは配列型の要素がマージされず上書きされてしまうため自作</remarks>
        public static BsonDocument MergeBson(this BsonDocument bson, params BsonDocument[] documents)
        {
            if (documents == null || documents.All(x => x == null))
            {
                return bson;
            }

            foreach (var document in documents.Where(x => x != null))
            {
                foreach (var element in document)
                {
                    if (!bson.Contains(element.Name))
                    {
                        bson.Add(element);
                    }
                    else if (bson[element.Name].IsBsonArray && element.Value.IsBsonArray)
                    {
                        // キー名が同じかつ配列型の場合要素をマージする（要素の重複は削除）
                        bson[element.Name] = new BsonArray(bson[element.Name].AsBsonArray.AddRange(element.Value.AsBsonArray).Distinct());
                    }
                }
            }
            return bson;
        }


        /// <summary>
        /// $and/$or/$norの要素かどうか
        /// </summary>
        public static bool IsLogicalOperator(this BsonElement bson)
        {
            return (
                bson.Name == "$and" ||
                bson.Name == "$or" ||
                bson.Name == "$nor"
            );
        }

        /// <summary>
        /// $and/$or/$norの要素かどうか
        /// </summary>
        public static bool IsEmptyBsonArray(this BsonElement bson)
        {
            return (
                bson.Value.IsBsonArray &&
                bson.Value.AsBsonArray.Count == 0
            );
        }
    }
}
