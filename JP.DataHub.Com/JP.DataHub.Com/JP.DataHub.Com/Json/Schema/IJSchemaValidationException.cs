namespace JP.DataHub.Com.Json.Schema
{
    public interface IJSchemaValidationException
    {
        int LineNumber { get; }
        int LinePosition { get; }
        string Path { get; }
        IValidationError ValidationError { get; }
    }
    public class JSchemaValidationException : System.Exception, IJSchemaValidationException
    {
        public int LineNumber => throw new System.NotImplementedException();

        public int LinePosition => throw new System.NotImplementedException();

        public string Path => throw new System.NotImplementedException();

        public IValidationError ValidationError => throw new System.NotImplementedException();
    }
}