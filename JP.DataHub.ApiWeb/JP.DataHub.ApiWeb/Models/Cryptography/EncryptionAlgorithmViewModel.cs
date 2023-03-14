using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JP.DataHub.ApiWeb.Models.Cryptography
{
    /// <summary>
    /// <ja>共通鍵アルゴリズム</ja>
    /// <en>Common key algorithm</en>
    /// </summary>
    public enum CommonEncryptionAlgorithms
    {
        AES = 1
    }


    /// <summary>
    /// 共通鍵アルゴリズム情報
    /// </summary>
    public class EncryptionAlgorithmViewModel
    {
        /// <summary>
        /// <ja>共通鍵アルゴリズム</ja>
        /// <en>Common Key Algorithm</en>
        /// </summary>
        [DisplayName("共通鍵アルゴリズム")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommonEncryptionAlgorithms EncryptionAlgorithm { get; set; }

        /// <summary>
        /// <ja>推奨を表すフラグ</ja>
        /// <en>Recommend</en>
        /// </summary>
        [DisplayName("推奨を表すフラグ")]
        public bool Recommend { get; set; }
    }
}