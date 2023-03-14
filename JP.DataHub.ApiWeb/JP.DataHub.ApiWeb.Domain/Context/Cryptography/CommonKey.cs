using System.Configuration;
using System.Security.Cryptography;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 共通鍵のEntity
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class CommonKey : IEntity
    {
        /// <summary>共通鍵ID</summary>
        [Key(0)]
        public CommonKeyId CommonKeyId { get; }

        /// <summary>システムID</summary>
        [Key(1)]
        public SystemId SystemId { get; }

        /// <summary>有効期限(UTC)</summary>
        [Key(2)]
        public ExpirationDate ExpirationDate { get; }

        /// <summary>共通鍵パラメーター</summary>
        [Key(3)]
        public CommonKeyParameters Parameters { get; }


        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="systemId">システムID</param>
        /// <param name="expirationDate">有効期限(UTC)</param>
        /// <param name="parameters">共通鍵パラメーター</param>
        public CommonKey(CommonKeyId commonKeyId, SystemId systemId, ExpirationDate expirationDate, CommonKeyParameters parameters)
        {
            CommonKeyId = commonKeyId;
            SystemId = systemId;
            ExpirationDate = expirationDate;
            Parameters = parameters;
        }


        /// <summary>
        /// 新規のインスタンスを生成します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="expirationDate">有効期限(UTC)</param>
        /// <param name="parameters">共通鍵パラメーター</param>
        public static CommonKey CreateNewCommonKey(string systemId, DateTime? expirationDate, CommonKeyParameters parameters)
        {
            ExpirationDate expDate;
            if (expirationDate.HasValue)
            {
                expDate = new ExpirationDate(expirationDate.Value);
            }
            else
            {
                // 有効期限が未指定の場合は自動生成する
                TimeSpan ts;
                if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["CommonKeyDefaultLifeTime"], out ts))
                {
                    ts = TimeSpan.Parse("00:10:00");
                }
                expDate = new ExpirationDate(DateTime.UtcNow.Add(ts));
            }

            return new CommonKey(new CommonKeyId(), new SystemId(systemId), expDate, parameters);
        }

        /// <summary>
        /// 平文データをAES暗号化します。
        /// </summary>
        /// <param name="plainData">平文データ</param>
        /// <returns>暗号化データ</returns>
        public EncryptData Encrypt(PlainData plainData)
        {
            using (var aes = Aes.Create())
            {
                // パラメーター設定
                Parameters.SetCryptoParameter(aes);

                using (var encryptor = aes.CreateEncryptor())
                {
                    // 暗号化
                    var encrypted = encryptor.TransformFinalBlock(plainData.Content, 0, plainData.Content.Length);
                    return new EncryptData(encrypted);
                }
            }
        }

        /// <summary>
        /// AES暗号化データを復号します。
        /// </summary>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        public PlainData Decrypt(EncryptData encryptData)
        {
            using (var aes = Aes.Create())
            {
                // パラメーター設定
                Parameters.SetCryptoParameter(aes);

                using (var decryptor = aes.CreateDecryptor())
                {
                    // 復号
                    var decrypted = decryptor.TransformFinalBlock(encryptData.Content, 0, encryptData.Content.Length);
                    return new PlainData(decrypted);
                }
            }
        }

        /// <summary>
        /// 平文データをAES暗号化するストリームを取得します。
        /// </summary>
        /// <param name="plainStream">平文データストリーム</param>
        /// <returns>暗号化データストリーム</returns>
        public EncryptStream GetEncryptStream(PlainStream plainStream)
        {
            using (var aes = Aes.Create())
            {
                // パラメーター設定
                Parameters.SetCryptoParameter(aes);

                var encryptor = aes.CreateEncryptor();
                // 復号ストリーム作成
                var stream = new CryptoStream(plainStream.Value, encryptor, CryptoStreamMode.Read);
                return new EncryptStream(stream);
            }
        }

        /// <summary>
        /// AES暗号化データを復号するストリームを取得します。
        /// </summary>
        /// <param name="encryptStream">暗号化データストリーム</param>
        /// <returns>平文データストリーム</returns>
        public PlainStream GetDecryptStream(EncryptStream encryptStream)
        {
            using (var aes = Aes.Create())
            {
                // パラメーター設定
                Parameters.SetCryptoParameter(aes);

                var decryptor = aes.CreateDecryptor();
                // 復号ストリーム作成
                var stream = new CryptoStream(encryptStream.Value, decryptor, CryptoStreamMode.Read);
                return new PlainStream(stream);
            }
        }

        /// <summary>
        /// 現在から有効期限までのTimeSpanを返します。
        /// </summary>
        /// <returns>TimeSpan</returns>
        public TimeSpan GetExpirationTimeSpanFromNow()
            => ExpirationDate.Value - DateTime.UtcNow;
    }
}
