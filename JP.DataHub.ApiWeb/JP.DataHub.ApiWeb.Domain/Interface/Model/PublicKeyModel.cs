
namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    /// <summary>
    /// 公開鍵のModel
    /// </summary>
    public class PublicKeyModel
    {
        /// <summary>RSAアルゴリズムのExponentパラメーター</summary>
        public byte[] Exponent { get; set; }

        /// <summary>RSAアルゴリズムのModulusパラメーター</summary>
        public byte[] Modulus { get; set; }
    }
}
