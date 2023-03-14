using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace JP.DataHub.Com.Json.Schema.Generation
{
    public interface IJSchemaGenerator
    {
        IContractResolver ContractResolver { get; set; }
        Required DefaultRequired { get; set; }
        IList<IJSchemaGenerationProvider> GenerationProviders { get; }
        SchemaIdGenerationHandling SchemaIdGenerationHandling { get; set; }
        SchemaLocationHandling SchemaLocationHandling { get; set; }
        SchemaPropertyOrderHandling SchemaPropertyOrderHandling { get; set; }
        SchemaReferenceHandling SchemaReferenceHandling { get; set; }

        IJSchema Generate(Type type);
        IJSchema Generate(Type type, bool rootSchemaNullable);
    }
    public class JSchemaGenerator : IJSchemaGenerator
    {
        public IContractResolver ContractResolver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Required DefaultRequired { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<IJSchemaGenerationProvider> GenerationProviders => throw new NotImplementedException();

        public SchemaIdGenerationHandling SchemaIdGenerationHandling { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SchemaLocationHandling SchemaLocationHandling { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SchemaPropertyOrderHandling SchemaPropertyOrderHandling { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SchemaReferenceHandling SchemaReferenceHandling { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IJSchema Generate(Type type)
        {
            throw new NotImplementedException();
        }

        public IJSchema Generate(Type type, bool rootSchemaNullable)
        {
            throw new NotImplementedException();
        }
    }
}