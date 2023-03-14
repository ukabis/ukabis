using System.Collections.Generic;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IJsonSchemaResolver
    {
        IList<JsonSchema> LoadedSchemas { get; }

        JsonSchema GetSchema(string reference);
    }
    public class JsonSchemaResolver : IJsonSchemaResolver
    {
        public IList<JsonSchema> LoadedSchemas => throw new System.NotImplementedException();

        public JsonSchema GetSchema(string reference)
        {
            throw new System.NotImplementedException();
        }
    }
}