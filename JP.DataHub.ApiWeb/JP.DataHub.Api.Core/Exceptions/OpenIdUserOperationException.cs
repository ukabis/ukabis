
namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class OpenIdUserOperationException : DomainException
    {
        public OpenIdUserOperationException()
        {
        }

        public OpenIdUserOperationException(string message)
            : base("OpenId user operation is failed: " + message)
        {
        }

        public OpenIdUserOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
