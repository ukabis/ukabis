using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;

namespace JP.DataHub.Com.Extensions
{
    public static class NJsonSchemaExtensions
    {
        public static JsonSchemaProperty FindProperty(this JsonSchema jsonSchema, string propertyName)
        {
            if (propertyName.Contains('.') == false)
            {
                if (jsonSchema?.Properties?.ContainsKey(propertyName) == true)
                {
                    return jsonSchema.Properties[propertyName];
                }
                return null;
            }

            var schema = jsonSchema;
            JsonSchemaProperty result = null;
            var props = propertyName.Split('.');
            for (int i = 0; i < props.Length; i++)
            {
                if (schema.Properties.ContainsKey(props[i]) == false)
                {
                    return null;
                }
                result = schema.Properties[props[i]];
                schema = result.ActualSchema;
            }
            return result;
        }

        public static JsonSchema FindSchema(this JsonSchema jsonSchema, string propertyName)
        {
            if (jsonSchema?.Properties?.ContainsKey(propertyName) == true)
            {
                return jsonSchema.Properties[propertyName];
            }

            var schema = jsonSchema;
            var props = propertyName.Split('.');
            for (int i = 0; i < props.Length; i++)
            {
                if (schema.Properties.ContainsKey(props[i]) == false)
                {
                    return null;
                }
                schema = schema.Properties[props[i]]?.ActualSchema;
            }
            return schema;
        }
    }
}
