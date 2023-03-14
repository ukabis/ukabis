using System.ComponentModel;

namespace JP.DataHub.ApiWeb.Models.Cryptography
{
    /// <summary>
    /// 公開鍵情報
    /// </summary>
    public class PublicKeyViewModel
    {
        /// <summary>
        /// RSAアルゴリズムのExponentパラメーター(Base64エンコード)
        /// </summary>
        [DisplayName("RSAアルゴリズムのExponentパラメーター(Base64エンコード)")]
        public string Exponent { get; set; }

        /// <summary>
        /// RSAアルゴリズムのModulusパラメーター(Base64エンコード)
        /// </summary>
        [DisplayName("RSAアルゴリズムのModulusパラメーター(Base64エンコード)")]
        public string Modulus { get; set; }

        /// <summary>
        /// 暗号化アルゴリズム: 固定値 "RSA-OAEP"
        /// </summary>
        [DisplayName("暗号化アルゴリズム: 固定値 \"RSA-OAEP\"")]
        public string EncryptionAlgorithm { get; } = "RSA-OAEP";
    }
}