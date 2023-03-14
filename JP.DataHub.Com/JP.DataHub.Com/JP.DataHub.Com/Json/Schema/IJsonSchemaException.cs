namespace JP.DataHub.Com.Json.Schema
{
    public interface IJsonSchemaException
    {
        int LineNumber { get; }
        int LinePosition { get; }
        string Path { get; }
    }
    public class JsonSchemaException : IJsonSchemaException
    {
        public int LineNumber => throw new System.NotImplementedException();

        public int LinePosition => throw new System.NotImplementedException();

        public string Path => throw new System.NotImplementedException();
    }
}