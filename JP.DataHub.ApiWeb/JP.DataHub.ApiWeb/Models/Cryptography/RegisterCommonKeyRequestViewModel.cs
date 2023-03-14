using System;
using System.Security.Cryptography;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Models.Cryptography
{
    /// <summary>
    /// 共通鍵キーサイズ
    /// </summary>
    public enum CommonKeySizes
    {
        Bit128 = 128,
        Bit192 = 192,
        Bit256 = 256
    }


    /// <summary>
    /// 共通鍵登録のリクエストViewModel
    /// </summary>
    public class RegisterCommonKeyRequestViewModel
    {
        /// <summary>共通鍵アルゴリズム</summary>
        [DisplayName("共通鍵アルゴリズム")]
        [Required]
        [DefaultValue(CommonEncryptionAlgorithms.AES)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommonEncryptionAlgorithms EncryptionAlgorithm { get; set; }

        /// <summary>有効期限(UTC): 未指定の場合は自動設定</summary>
        [DisplayName("有効期限(UTC): 未指定の場合は自動設定")]
        [DateTimeRange("1")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>暗号化キー(Base64エンコード)</summary>
        [DisplayName("暗号化キー(Base64エンコード)")]
        [Required]
        [Base64String]
        public string Key { get; set; }

        /// <summary>初期ベクター(Base64エンコード): 未指定の場合は自動生成</summary>
        [DisplayName("初期ベクター(Base64エンコード): 未指定の場合は自動生成")]
        [Base64String]
        public string IV { get; set; }

        /// <summary>キーサイズ: 未指定の場合は128</summary>
        [DisplayName("キーサイズ: 未指定の場合は128")]
        [DefaultValue(CommonKeySizes.Bit128)]
        [EnumDataType(typeof(CommonKeySizes), ErrorMessage = "キーサイズは128, 192, 256のいずれかが指定可能です。")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CommonKeySizes KeySize { get; set; }

        /// <summary>Cipherモード: 未指定の場合は"CBC"</summary>
        [DisplayName("Cipherモード: 未指定の場合は\"CBC\"")]
        [DefaultValue(CipherMode.CBC)]
        [EnumDataType(typeof(CipherMode), ErrorMessage = "CipherモードはCBC, ECB, OFB, CFB, CTSのいずれかが指定可能です。")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CipherMode CipherMode { get; set; }

        /// <summary>パディングモード: 未指定の場合は"PKCS7"</summary>
        [DisplayName("パディングモード: 未指定の場合は\"PKCS7\"")]
        [DefaultValue(PaddingMode.PKCS7)]
        [EnumDataType(typeof(PaddingMode), ErrorMessage = "パディングモードはNone, PKCS7, Zeros, NSIX923, ISO10126のいずれかが指定可能です。")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaddingMode PaddingMode { get; set; }
    }
}