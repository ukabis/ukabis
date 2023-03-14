namespace JP.DataHub.Com.Json.Schema
{
    public interface ISchemaValidationEventArgs
    {
        string Message { get; }
        string Path { get; }
        IValidationError ValidationError { get; }
    }
    public class SchemaValidationEventArgs : ISchemaValidationEventArgs
    {
        public string Message => throw new System.NotImplementedException();

        public string Path => throw new System.NotImplementedException();

        public IValidationError ValidationError => throw new System.NotImplementedException();
    }
}