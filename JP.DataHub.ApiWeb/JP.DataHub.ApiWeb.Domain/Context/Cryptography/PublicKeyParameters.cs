using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 公開鍵パラメーターのValueObject
    /// </summary>
    internal class PublicKeyParameters : IValueObject
    {
        /// <summary>RSAアルゴリズムのExponentパラメーター</summary>
        public byte[] Exponent { get; }

        /// <summary>RSAアルゴリズムのModulusパラメーター</summary>
        public byte[] Modulus { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="exponent">RSAアルゴリズムのExponentパラメーター</param>
        /// <param name="modulus">RSAアルゴリズムのModulusパラメーター</param>
        public PublicKeyParameters(byte[] exponent, byte[] modulus)
        {
            Exponent = exponent;
            Modulus = modulus;
        }
    }
}
