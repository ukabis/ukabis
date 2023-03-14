using System.IO;
using System.Net;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IJSchemaResolver
    {
        ICredentials Credentials { set; }

        Stream GetSchemaResource(IResolveSchemaContext context, ISchemaReference reference);
        IJSchema GetSubschema(ISchemaReference reference, IJSchema rootSchema);
        ISchemaReference ResolveSchemaReference(IResolveSchemaContext context);
    }
    public class JSchemaResolver : IJSchemaResolver
    {
        public ICredentials Credentials { set => throw new System.NotImplementedException(); }

        public Stream GetSchemaResource(IResolveSchemaContext context, ISchemaReference reference)
        {
            throw new System.NotImplementedException();
        }

        public IJSchema GetSubschema(ISchemaReference reference, IJSchema rootSchema)
        {
            throw new System.NotImplementedException();
        }

        public ISchemaReference ResolveSchemaReference(IResolveSchemaContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}