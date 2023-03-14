namespace JP.DataHub.Com.Json.Schema
{
    public interface IJsonValidatorContext
    {
        IJSchema Schema { get; }

        void RaiseError(string message);
        void RaiseError(string message, object value);
    }
    public class JsonValidatorContext : IJsonValidatorContext
    {
        public IJSchema Schema => throw new System.NotImplementedException();

        public void RaiseError(string message)
        {
            throw new System.NotImplementedException();
        }

        public void RaiseError(string message, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}