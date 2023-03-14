using System;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IResolveSchemaContext
    {
        Uri ResolvedSchemaId { get; set; }
        Uri ResolverBaseUri { get; set; }
        Uri SchemaId { get; set; }
    }
}