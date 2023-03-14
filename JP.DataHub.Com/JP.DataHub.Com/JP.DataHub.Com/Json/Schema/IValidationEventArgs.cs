namespace JP.DataHub.Com.Json.Schema
{
    public interface IValidationEventArgs
    {
        JsonSchemaException Exception { get; }
        string Message { get; }
        string Path { get; }
    }
    public class ValidationEventArgs : IValidationEventArgs
    {
        public JsonSchemaException Exception => throw new System.NotImplementedException();

        public string Message => throw new System.NotImplementedException();

        public string Path => throw new System.NotImplementedException();
    }
}