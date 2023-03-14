using System;
using System.Collections.Generic;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IJSchemaReaderSettings
    {
        Uri BaseUri { get; set; }
        IJSchemaResolver Resolver { get; set; }
        bool ResolveSchemaReferences { get; set; }
        bool ValidateVersion { get; set; }
        IList<IJsonValidator> Validators { get; set; }

        event SchemaValidationEventHandler ValidationEventHandler;
    }

    public class JSchemaReaderSettings : IJSchemaReaderSettings
    {
        public Uri BaseUri { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IJSchemaResolver Resolver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ResolveSchemaReferences { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ValidateVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<IJsonValidator> Validators { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event SchemaValidationEventHandler ValidationEventHandler;
    }
}