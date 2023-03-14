using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace JP.DataHub.Com.Json.Schema.Generation
{
    public interface IJSchemaTypeGenerationContext
    {
        IJSchemaGenerator Generator { get; }
        JsonProperty MemberProperty { get; }
        Type ObjectType { get; }
        JsonContainerContract ParentContract { get; }
        Required Required { get; }
        string SchemaDescription { get; }
        string SchemaTitle { get; }
    }
}