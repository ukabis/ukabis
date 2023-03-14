using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public class DecimalizationBsonSerializer
    {
        private DecimalizationBsonDocumentSerializer BsonDocumentSerializer { get; set; }
        private DecimalizationBsonArraySerializer BsonArraySerializer { get; set; }

        public DecimalizationBsonSerializer()
        {
            BsonDocumentSerializer = new DecimalizationBsonDocumentSerializer();
            BsonArraySerializer = new DecimalizationBsonArraySerializer();

            BsonDocumentSerializer.BsonArraySerializer = BsonArraySerializer;
            BsonArraySerializer.BsonDocumentSerializer = BsonDocumentSerializer;
        }


        /// <summary>
        /// BsonDocumentのデシリアライズ
        /// </summary>
        public BsonDocument DeserializeDocument(string json)
        {
            using (var reader = new JsonReader(json))
            {
                var context = BsonDeserializationContext.CreateRoot(reader);
                return BsonDocumentSerializer.Deserialize(context);
            }
        }

        /// <summary>
        /// BsonArrayのデシリアライズ
        /// </summary>
        public BsonArray DeserializeArray(string json)
        {
            using (var reader = new JsonReader(json))
            {
                var context = BsonDeserializationContext.CreateRoot(reader);
                return BsonArraySerializer.Deserialize(context);
            }
        }


        /// <summary>
        /// 数値をDecimalとしてデシリアライズする拡張BsonDocumentシリアライザー
        /// </summary>
        public class DecimalizationBsonDocumentSerializer : BsonDocumentSerializer
        {
            public DecimalizationBsonArraySerializer BsonArraySerializer { get; set; }

            protected override BsonDocument DeserializeValue(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                var bsonReader = context.Reader;

                bsonReader.ReadStartDocument();
                var document = new BsonDocument(allowDuplicateNames: context.AllowDuplicateElementNames);
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var name = bsonReader.ReadName();
                    var type = bsonReader.GetCurrentBsonType();
                    BsonValue value;

                    switch (type)
                    {
                        case BsonType.Int32:
                            value = BsonValue.Create(Convert.ToDecimal(bsonReader.ReadInt32()));
                            break;
                        case BsonType.Int64:
                            value = BsonValue.Create(Convert.ToDecimal(bsonReader.ReadInt64()));
                            break;
                        case BsonType.Double:
                            value = BsonValue.Create(Convert.ToDecimal(bsonReader.ReadDouble()));
                            break;
                        case BsonType.Document:
                            value = Deserialize(context, args);
                            break;
                        case BsonType.Array:
                            value = BsonArraySerializer.Deserialize(context, args);
                            break;
                        default:
                            value = BsonValueSerializer.Instance.Deserialize(context, args);
                            break;
                    }

                    document.Add(name, value);
                }
                bsonReader.ReadEndDocument();

                return document;
            }
        }

        /// <summary>
        /// 数値をDecimalとしてデシリアライズする拡張BsonArrayシリアライザー
        /// </summary>
        public class DecimalizationBsonArraySerializer : BsonArraySerializer
        {
            public DecimalizationBsonDocumentSerializer BsonDocumentSerializer { get; set; }

            protected override BsonArray DeserializeValue(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                var bsonReader = context.Reader;

                bsonReader.ReadStartArray();
                var array = new BsonArray();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var type = bsonReader.GetCurrentBsonType();
                    BsonValue item;

                    switch (type)
                    {
                        case BsonType.Int32:
                            item = BsonValue.Create(Convert.ToDecimal(bsonReader.ReadInt32()));
                            break;
                        case BsonType.Int64:
                            item = BsonValue.Create(Convert.ToDecimal(bsonReader.ReadInt64()));
                            break;
                        case BsonType.Double:
                            item = BsonValue.Create(Convert.ToDecimal(bsonReader.ReadDouble()));
                            break;
                        case BsonType.Document:
                            item = BsonDocumentSerializer.Deserialize(context, args);
                            break;
                        case BsonType.Array:
                            item = Deserialize(context, args);
                            break;
                        default:
                            item = BsonValueSerializer.Instance.Deserialize(context);
                            break;
                    }

                    array.Add(item);
                }
                bsonReader.ReadEndArray();

                return array;
            }
        }
    }
}
