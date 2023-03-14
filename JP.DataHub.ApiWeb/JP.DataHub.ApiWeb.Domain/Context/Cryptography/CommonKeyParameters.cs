using System.Security.Cryptography;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 共通鍵パラメーターのValueObject
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class CommonKeyParameters : IValueObject
    {
        /// <summary>暗号化キー</summary>
        [Key(0)]
        public byte[] Key { get; }

        /// <summary>初期ベクター</summary>
        [Key(1)]
        public byte[] IV { get; }

        /// <summary>キーサイズ</summary>
        [Key(2)]
        public int KeySize { get; }

        /// <summary>Cipherモード</summary>
        [Key(3)]
        public CipherMode CipherMode { get; }

        /// <summary>パディングモード</summary>
        [Key(4)]
        public PaddingMode PaddingMode { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="key">暗号化キー</param>
        /// <param name="iv">初期ベクター</param>
        /// <param name="keySize">キーサイズ</param>
        /// <param name="cipherMode">Cipherモード</param>
        /// <param name="paddingMode">パディングモード</param>
        public CommonKeyParameters(byte[] key, byte[] iv, int keySize,
            CipherMode cipherMode, PaddingMode paddingMode)
        {
            Key = key;
            IV = iv;
            KeySize = keySize;
            CipherMode = cipherMode;
            PaddingMode = paddingMode;

            // 初期ベクターが未指定の場合は生成する
            if (IV == null || IV.Length == 0)
            {
                using (var aes = Aes.Create())
                {
                    aes.GenerateIV();
                    IV = aes.IV;
                }
            }
        }

        /// <summary>
        /// 暗号化パラメーターを設定します。
        /// </summary>
        /// <param name="aes">AesCryptoServiceProvider</param>
        public void SetCryptoParameter(Aes aes)
        {
            aes.KeySize = KeySize;
            aes.Mode = CipherMode;
            aes.Padding = PaddingMode;
            aes.IV = IV;
            aes.Key = Key;
        }
    }
}
