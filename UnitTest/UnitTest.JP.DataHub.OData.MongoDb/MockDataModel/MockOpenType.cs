using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitTest.JP.DataHub.OData.MongoDb.MockDataModel
{
    [DataContract]
    public enum MockEnum
    {
        [EnumMember]
        ZERO,

        [EnumMember]
        ONE,

        [EnumMember]
        TWO
    }


    [DataContract]
    public class MockOpenType
    {
        /// <summary>
        /// The property name for EnglishName
        /// </summary>
        public const string EnglishNamePropertyName = "englishName";

        /// <summary>
        /// The property name for EnumNumber
        /// </summary>
        public const string EnumNumberPropertyName = "enumNumber";

        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// EnglishName
        /// </summary>
        [DataMember(Name = EnglishNamePropertyName)]
        [JsonProperty(PropertyName = EnglishNamePropertyName)]
        public string EnglishName { get; set; }

        /// <summary>
        /// EnumNumber
        /// </summary>
        [DataMember(Name = EnumNumberPropertyName)]
        [JsonProperty(PropertyName = EnumNumberPropertyName)]
        public MockEnum EnumNumber { get; set; }

        /// <summary>
        /// PropertyBag to make Edm open-typed
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; set; }
    }
}
