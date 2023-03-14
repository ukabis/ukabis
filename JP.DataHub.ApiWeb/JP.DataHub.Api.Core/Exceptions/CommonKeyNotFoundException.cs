
namespace JP.DataHub.Api.Core.Exceptions
{
    /// <summary>
    /// 共通鍵がリポジトリに存在しなかった場合にスローする例外です。
    /// </summary>
    [Serializable]
    public class CommonKeyNotFoundException : Exception
    {
        public CommonKeyNotFoundException()
            : base("The specified CommonKeyId is expired or unregistered.")
        {
        }

        public CommonKeyNotFoundException(string message)
            : base(message)
        {
        }

        public CommonKeyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
