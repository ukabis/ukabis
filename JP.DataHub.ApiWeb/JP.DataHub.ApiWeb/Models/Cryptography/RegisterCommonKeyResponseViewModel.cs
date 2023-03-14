using System;
using System.ComponentModel;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JP.DataHub.ApiWeb.Models.Cryptography
{
    /// <summary>
    /// 共通鍵登録のレスポンス
    /// </summary>
    public class RegisterCommonKeyResponseViewModel
    {
        /// <summary>
        /// 共通鍵アルゴリズム
        /// </summary>
        [DisplayName("共通鍵アルゴリズム")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommonEncryptionAlgorithms EncryptionAlgorithm { get; set; } = CommonEncryptionAlgorithms.AES;

        /// <summary>
        /// 共通鍵ID
        /// </summary>
        [DisplayName("共通鍵ID")]
        public string CommonKeyId { get; set; }

        /// <summary>
        /// 有効期限(UTC)
        /// </summary>
        [DisplayName("有効期限(UTC)")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 初期ベクター(Base64エンコード)
        /// </summary>
        [DisplayName("初期ベクター(Base64エンコード)")]
        public string IV { get; set; }

        /// <summary>
        /// キーサイズ
        /// </summary>
        [DisplayName("キーサイズ")]
        public int KeySize { get; set; }

        /// <summary>
        /// Cipherモード
        /// </summary>
        [DisplayName("Cipherモード")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CipherMode CipherMode { get; set; }

        /// <summary>
        /// パディングモード
        /// </summary>
        [DisplayName("パディングモード")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaddingMode PaddingMode { get; set; }
    }
}