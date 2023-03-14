using System;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    /// <summary>
    /// データの暗号化機能を提供するアプリケーションサービスです。
    /// </summary>
    [Log]
    class CryptographyApplicationService : ICryptographyApplicationService
    {
        private ICommonKeyRepository _commonKeyRepository = UnityCore.Resolve<ICommonKeyRepository>();


        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        public PlainData Decrypt(SystemId systemId, CommonKeyId commonKeyId, EncryptData encryptData)
        {
            var commonKey = _commonKeyRepository.Get(systemId, commonKeyId);
            if (commonKey != null)
            {
                return commonKey.Decrypt(encryptData);
            }
            else
            {
                throw new CommonKeyNotFoundException();
            }
        }

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainData">平文データ</param>
        /// <returns>暗号化データ</returns>
        public EncryptData Encrypt(SystemId systemId, CommonKeyId commonKeyId, PlainData plainData)
        {
            var commonKey = _commonKeyRepository.Get(systemId, commonKeyId);
            if (commonKey != null)
            {
                return commonKey.Encrypt(plainData);
            }
            else
            {
                throw new CommonKeyNotFoundException();
            }
        }

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptStream">暗号化データストリーム</param>
        /// <returns>平文データストリーム</returns>
        public PlainStream GetDecryptStream(SystemId systemId, CommonKeyId commonKeyId, EncryptStream encryptStream)
        {
            var commonKey = _commonKeyRepository.Get(systemId, commonKeyId);
            if (commonKey != null)
            {
                return commonKey.GetDecryptStream(encryptStream);
            }
            else
            {
                throw new CommonKeyNotFoundException();
            }
        }

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainStream">平文データストリーム</param>
        /// <returns>暗号化データストリーム</returns>
        public EncryptStream GetEncryptStream(SystemId systemId, CommonKeyId commonKeyId, PlainStream plainStream)
        {
            var commonKey = _commonKeyRepository.Get(systemId, commonKeyId);
            if (commonKey != null)
            {
                return commonKey.GetEncryptStream(plainStream);
            }
            else
            {
                throw new CommonKeyNotFoundException();
            }
        }
    }
}
