
namespace JP.DataHub.Api.Core.Exceptions
{
    /// <summary>
    /// 非対称鍵の操作時にエラーが発生した場合にスローする例外です。
    /// </summary>
    [Serializable]
    public class AsymmetricKeyOperationException : Exception
    {
        public AsymmetricKeyOperationException()
        {
        }

        public AsymmetricKeyOperationException(string message)
            : base("Asymmetric key operation is failed: " + message)
        {
        }

        public AsymmetricKeyOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
