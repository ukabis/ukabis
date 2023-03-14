using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 公開鍵のEntity
    /// </summary>
    internal class PublicKey : IEntity
    {
        /// <summary>公開鍵ID</summary>
        public PublicKeyId PublicKeyId { get; }

        /// <summary>システムID</summary>
        public SystemId SystemId { get; }

        /// <summary>公開鍵パラメーター</summary>
        public PublicKeyParameters Parameters { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="publicKeyId">公開鍵ID</param>
        /// <param name="systemId">システムID</param>
        /// <param name="exponent">RSAアルゴリズムのExponentパラメーター</param>
        /// <param name="modulus">RSAアルゴリズムのModulusパラメーター</param>
        public PublicKey(string publicKeyId, SystemId systemId, byte[] exponent, byte[] modulus)
        {
            PublicKeyId = new PublicKeyId(publicKeyId);
            SystemId = systemId;
            Parameters = new PublicKeyParameters(exponent, modulus);
        }
    }
}
