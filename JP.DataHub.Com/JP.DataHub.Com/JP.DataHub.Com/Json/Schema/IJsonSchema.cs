using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IJsonSchema
    {
        JsonSchema AdditionalItems { get; set; }
        JsonSchema AdditionalProperties { get; set; }
        bool AllowAdditionalItems { get; set; }
        bool AllowAdditionalProperties { get; set; }
        JToken Default { get; set; }
        string Description { get; set; }
        JsonSchemaType? Disallow { get; set; }
        double? DivisibleBy { get; set; }
        IList<JToken> Enum { get; set; }
        bool? ExclusiveMaximum { get; set; }
        bool? ExclusiveMinimum { get; set; }
        IList<JsonSchema> Extends { get; set; }
        string Format { get; set; }
        bool? Hidden { get; set; }
        string Id { get; set; }
        IList<JsonSchema> Items { get; set; }
        double? Maximum { get; set; }
        int? MaximumItems { get; set; }
        int? MaximumLength { get; set; }
        double? Minimum { get; set; }
        int? MinimumItems { get; set; }
        int? MinimumLength { get; set; }
        string Pattern { get; set; }
        IDictionary<string, JsonSchema> PatternProperties { get; set; }
        bool PositionalItemsValidation { get; set; }
        IDictionary<string, JsonSchema> Properties { get; set; }
        bool? ReadOnly { get; set; }
        bool? Required { get; set; }
        string Requires { get; set; }
        string Title { get; set; }
        bool? Transient { get; set; }
        JsonSchemaType? Type { get; set; }
        bool UniqueItems { get; set; }

        string ToString();
        void WriteTo(JsonWriter writer);
        void WriteTo(JsonWriter writer, JsonSchemaResolver resolver);
    }
    public class JsonSchema : IJsonSchema
    {
        public JsonSchema AdditionalItems { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public JsonSchema AdditionalProperties { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool AllowAdditionalItems { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool AllowAdditionalProperties { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public JToken Default { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Description { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public JsonSchemaType? Disallow { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double? DivisibleBy { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IList<JToken> Enum { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool? ExclusiveMaximum { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool? ExclusiveMinimum { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IList<JsonSchema> Extends { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Format { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool? Hidden { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IList<JsonSchema> Items { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double? Maximum { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int? MaximumItems { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int? MaximumLength { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double? Minimum { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int? MinimumItems { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int? MinimumLength { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Pattern { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IDictionary<string, JsonSchema> PatternProperties { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool PositionalItemsValidation { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IDictionary<string, JsonSchema> Properties { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool? ReadOnly { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool? Required { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Requires { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Title { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool? Transient { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public JsonSchemaType? Type { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool UniqueItems { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void WriteTo(JsonWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void WriteTo(JsonWriter writer, JsonSchemaResolver resolver)
        {
            throw new System.NotImplementedException();
        }

        public static JsonSchema Parse(string json)
        {
            throw new System.NotImplementedException();
        }

        public static JsonSchema Parse(string json, JsonSchemaResolver resolver)
        {
            throw new System.NotImplementedException();
        }
    }
}