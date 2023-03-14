using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JP.DataHub.Com.Web.Authentication
{
    public abstract class InterfaceJsonConverter<TInterface> : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        protected string TypePropertyName { get; set; }
        protected Dictionary<string, Type> Mapping = new Dictionary<string, Type>();

        public override bool CanConvert(Type objectType)
        {
            return Mapping.ContainsValue(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(TypePropertyName))
            {
                throw new NullReferenceException("TypePropertyName is null");
            }

            var jsonObject = JObject.Load(reader);
            TInterface authenticationInfo = default(TInterface);
            var type = jsonObject.Value<string>(TypePropertyName);
            if (Mapping.ContainsKey(type))
            {
                authenticationInfo = (TInterface)Activator.CreateInstance(Mapping[type]);
            }
            if (authenticationInfo != null)
            {
                serializer.Populate(jsonObject.CreateReader(), authenticationInfo);
            }
            return authenticationInfo;
        }
    }
}
