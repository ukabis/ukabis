using System.Runtime.Serialization;

namespace JP.DataHub.Com.Json.Schema
{
    public enum ErrorType
    {
        [EnumMember(Value = "none")]
        None,

        [EnumMember(Value = "multipleOf")]
        MultipleOf,

        [EnumMember(Value = "maximum")]
        Maximum,

        [EnumMember(Value = "minimum")]
        Minimum,

        [EnumMember(Value = "maxLength")]
        MaximumLength,

        [EnumMember(Value = "minLength")]
        MinimumLength,

        [EnumMember(Value = "pattern")]
        Pattern,

        [EnumMember(Value = "additionalItems")]
        AdditionalItems,

        [EnumMember(Value = "items")]
        Items,

        [EnumMember(Value = "maxItems")]
        MaximumItems,

        [EnumMember(Value = "minItems")]
        MinimumItems,

        [EnumMember(Value = "uniqueItems")]
        UniqueItems,

        [EnumMember(Value = "maxProperties")]
        MaximumProperties,

        [EnumMember(Value = "minProperties")]
        MinimumProperties,

        [EnumMember(Value = "required")]
        Required,

        [EnumMember(Value = "additionalProperties")]
        AdditionalProperties,

        [EnumMember(Value = "dependencies")]
        Dependencies,

        [EnumMember(Value = "enum")]
        Enum,

        [EnumMember(Value = "type")]
        Type,

        [EnumMember(Value = "allOf")]
        AllOf,

        [EnumMember(Value = "anyOf")]
        AnyOf,

        [EnumMember(Value = "oneOf")]
        OneOf,

        [EnumMember(Value = "not")]
        Not,

        [EnumMember(Value = "format")]
        Format,

        [EnumMember(Value = "id")]
        Id,

        [EnumMember(Value = "patternProperties")]
        PatternProperties,

        [EnumMember(Value = "validator")]
        Validator,

        [EnumMember(Value = "valid")]
        Valid,

        [EnumMember(Value = "const")]
        Const,

        [EnumMember(Value = "contains")]
        Contains,

        [EnumMember(Value = "contentEncoding")]
        ContentEncoding,

        [EnumMember(Value = "contentMediaType")]
        ContentMediaType,

        [EnumMember(Value = "then")]
        Then,

        [EnumMember(Value = "else")]
        Else,

        [EnumMember(Value = "maxContains")]
        MaximumContains,

        [EnumMember(Value = "minContains")]
        MinimumContains,

        [EnumMember(Value = "unevaluatedItems")]
        UnevaluatedItems,

        [EnumMember(Value = "unevaluatedProperties")]
        UnevaluatedProperties,

        [EnumMember(Value = "ref")]
        Ref
    }
}
