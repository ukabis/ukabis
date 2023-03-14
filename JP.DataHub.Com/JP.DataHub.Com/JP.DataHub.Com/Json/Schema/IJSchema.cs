using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IJSchema
    {
        JSchema AdditionalItems { get; set; }
        JSchema AdditionalProperties { get; set; }
        IList<JSchema> AllOf { get; }
        bool AllowAdditionalItems { get; set; }
        bool AllowAdditionalItemsSpecified { get; set; }
        bool AllowAdditionalProperties { get; set; }
        bool AllowAdditionalPropertiesSpecified { get; set; }
        bool? AllowUnevaluatedItems { get; set; }
        bool? AllowUnevaluatedProperties { get; set; }
        string Anchor { get; set; }
        IList<JSchema> AnyOf { get; }
        JToken Const { get; set; }
        JSchema Contains { get; set; }
        string ContentEncoding { get; set; }
        string ContentMediaType { get; set; }
        JToken Default { get; set; }
        IDictionary<string, object> Dependencies { get; }
        IDictionary<string, IList<string>> DependentRequired { get; }
        IDictionary<string, JSchema> DependentSchemas { get; }
        string Description { get; set; }
        JSchema Else { get; set; }
        IList<JToken> Enum { get; }
        bool ExclusiveMaximum { get; set; }
        bool ExclusiveMinimum { get; set; }
        IDictionary<string, JToken> ExtensionData { get; }
        string Format { get; set; }
        Uri Id { get; set; }
        JSchema If { get; set; }
        IList<JSchema> Items { get; }
        bool ItemsPositionValidation { get; set; }
        double? Maximum { get; set; }
        long? MaximumContains { get; set; }
        long? MaximumItems { get; set; }
        long? MaximumLength { get; set; }
        long? MaximumProperties { get; set; }
        double? Minimum { get; set; }
        long? MinimumContains { get; set; }
        long? MinimumItems { get; set; }
        long? MinimumLength { get; set; }
        long? MinimumProperties { get; set; }
        double? MultipleOf { get; set; }
        JSchema Not { get; set; }
        IList<JSchema> OneOf { get; }
        string Pattern { get; set; }
        IDictionary<string, JSchema> PatternProperties { get; }
        IDictionary<string, JSchema> Properties { get; }
        JSchema PropertyNames { get; set; }
        bool? ReadOnly { get; set; }
        bool? RecursiveAnchor { get; set; }
        Uri RecursiveReference { get; set; }
        JSchema Ref { get; set; }
        Uri Reference { get; set; }
        IList<string> Required { get; }
        Uri SchemaVersion { get; set; }
        JSchema Then { get; set; }
        string Title { get; set; }
        JSchemaType? Type { get; set; }
        JSchema UnevaluatedItems { get; set; }
        JSchema UnevaluatedProperties { get; set; }
        bool UniqueItems { get; set; }
        bool? Valid { get; set; }
        List<IJsonValidator> Validators { get; }
        bool? WriteOnly { get; set; }

        string ToString();
        string ToString(SchemaVersion version);
        void WriteTo(JsonWriter writer);
    }

    public class JSchema : IJSchema
    {
        public JSchema AdditionalItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema AdditionalProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<JSchema> AllOf => throw new NotImplementedException();

        public bool AllowAdditionalItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AllowAdditionalItemsSpecified { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AllowAdditionalProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AllowAdditionalPropertiesSpecified { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? AllowUnevaluatedItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? AllowUnevaluatedProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Anchor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<JSchema> AnyOf => throw new NotImplementedException();

        public JToken Const { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema Contains { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentEncoding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentMediaType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JToken Default { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDictionary<string, object> Dependencies => throw new NotImplementedException();

        public IDictionary<string, IList<string>> DependentRequired => throw new NotImplementedException();

        public IDictionary<string, JSchema> DependentSchemas => throw new NotImplementedException();

        public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema Else { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<JToken> Enum => throw new NotImplementedException();

        public bool ExclusiveMaximum { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ExclusiveMinimum { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDictionary<string, JToken> ExtensionData => throw new NotImplementedException();

        public string Format { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Uri Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema If { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<JSchema> Items => throw new NotImplementedException();

        public bool ItemsPositionValidation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double? Maximum { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MaximumContains { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MaximumItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MaximumLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MaximumProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double? Minimum { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MinimumContains { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MinimumItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MinimumLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? MinimumProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double? MultipleOf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema Not { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<JSchema> OneOf => throw new NotImplementedException();

        public string Pattern { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDictionary<string, JSchema> PatternProperties => throw new NotImplementedException();

        public IDictionary<string, JSchema> Properties => throw new NotImplementedException();

        public JSchema PropertyNames { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? ReadOnly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? RecursiveAnchor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Uri RecursiveReference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema Ref { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Uri Reference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<string> Required => throw new NotImplementedException();

        public Uri SchemaVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema Then { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchemaType? Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema UnevaluatedItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public JSchema UnevaluatedProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool UniqueItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? Valid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<IJsonValidator> Validators => throw new NotImplementedException();

        public bool? WriteOnly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ToString(SchemaVersion version)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(JsonWriter writer)
        {
            throw new NotImplementedException();
        }


        public static JSchema Parse(string json)
        {
            throw new NotImplementedException();
        }

        public static JSchema Parse(string json, JSchemaResolver resolver)
        {
            throw new NotImplementedException();
        }

        public static JSchema Parse(string json, JSchemaReaderSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}