using System.Security.Cryptography;

namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    /// <summary>
    /// 共通鍵のModel
    /// </summary>
    public class CommonKeyModel
    {
        /// <summary>共通鍵ID</summary>
        public string CommonKeyId { get; set; }

        /// <summary>有効期限(UTC)</summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>暗号化キー</summary>
        public byte[] Key { get; set; }

        /// <summary>初期ベクター</summary>
        public byte[] IV { get; set; }

        /// <summary>キーサイズ</summary>
        public int KeySize { get; set; }

        /// <summary>Cipherモード</summary>
        public CipherMode CipherMode { get; set; }

        /// <summary>パディングモード</summary>
        public PaddingMode PaddingMode { get; set; }
    }
}
