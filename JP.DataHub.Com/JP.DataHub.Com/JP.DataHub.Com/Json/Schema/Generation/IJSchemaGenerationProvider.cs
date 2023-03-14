namespace JP.DataHub.Com.Json.Schema.Generation
{
    public interface IJSchemaGenerationProvider
    {
        bool CanGenerateSchema(IJSchemaTypeGenerationContext context);
        IJSchema GetSchema(IJSchemaTypeGenerationContext context);
    }
}