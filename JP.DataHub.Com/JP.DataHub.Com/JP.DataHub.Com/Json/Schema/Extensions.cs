using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.Com.Json.Schema
{
    public static class Extensions
    {

        public static bool IsValid(this JToken source, JSchema propertySchema)
        {
            throw new NotImplementedException();
        }

        public static bool IsValid(this JToken source, JSchema schema, out IList<string> errorMessages)
        {
            throw new NotImplementedException();
        }

        public static bool IsValid(this JToken source, JsonSchema schema, out IList<string> errorMessages)
        {
            throw new NotImplementedException();
        }

        public static bool IsValid(this JToken source, JSchema schema, out IList<ValidationError> errors)
        {
            throw new NotImplementedException();
        }

        public static void Validate(this JToken source, JSchema schema)
        {
            throw new NotImplementedException();
        }

        public static void Validate(this JToken source, JsonSchema schema)
        {
            throw new NotImplementedException();
        }

        public static void Validate(this JToken source, JsonSchema schema, ValidationEventHandler validationEventHandler)
        {
            throw new NotImplementedException();
        }
    }
}