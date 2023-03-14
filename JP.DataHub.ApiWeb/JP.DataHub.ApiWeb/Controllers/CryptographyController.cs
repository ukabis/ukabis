using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;
using JP.DataHub.ApiWeb.Models.Cryptography;

namespace JP.DataHub.ApiWeb.Controllers
{
    /// <summary>
    /// データの暗号化で使用する鍵やパラメーターの管理を行います。
    /// </summary>
    [ApiController]
    [Route("API/[controller]/[action]")]
    [ManageApi("c87764f8-0381-4b67-a14f-5a7f098b18d9")]
    [AuthorizeUsingOpenIdConnect]
    public class CryptographyController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PublicKeyModel, PublicKeyViewModel>()
                    .ForMember(d => d.Exponent, o => o.MapFrom(s => Convert.ToBase64String(s.Exponent)))
                    .ForMember(d => d.Modulus, o => o.MapFrom(s => Convert.ToBase64String(s.Modulus)));
                cfg.CreateMap<RegisterCommonKeyRequestViewModel, CommonKeyModel>()
                    .ForMember(d => d.Key, o => o.MapFrom(s => Convert.FromBase64String(s.Key)))
                    .ForMember(d => d.IV, o => o.MapFrom(s => Convert.FromBase64String(s.IV)));
                cfg.CreateMap<CommonKeyModel, RegisterCommonKeyResponseViewModel>()
                    .ForMember(d => d.IV, o => o.MapFrom(s => Convert.ToBase64String(s.IV)));
            });

            return config.CreateMapper();
        });
        private static IMapper s_mapper => s_lazyMapper.Value;

        private ICryptographyManagementInterface _cryptographyManagementInterface = UnityCore.Resolve<ICryptographyManagementInterface>();


        /// <summary>
        /// 認証用アクセストークンから取得したシステムIDに対応する公開鍵を返却します。
        /// </summary>
        /// <returns>公開鍵</returns>
        [HttpGet]
        [ManageAction("775387a6-6904-438e-9710-9c6261e8cc1a")]
        public async Task<IActionResult> GetPublicKey()
        {
            // SystemIdが取得できない場合はエラーを返す
            string systemId = PerRequestDataContainer?.SystemId;
            if (string.IsNullOrEmpty(systemId))
            {
                return BadRequest("Can not get SystemId.");
            }

            try
            {
                // 公開鍵を取得
                var publicKey = await _cryptographyManagementInterface.GetPublicKey(PerRequestDataContainer.SystemId);

                // 返却データを作成
                var result = s_mapper.Map<PublicKeyViewModel>(publicKey);

                return Ok(result);
            }
            catch (AsymmetricKeyOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 利用できる共通鍵アルゴリズムの一覧を返却します。
        /// </summary>
        /// <returns>共通鍵アルゴリズムの一覧</returns>
        [HttpGet]
        [ManageAction("04e1bdf2-7e90-42ee-8d7f-d1d7577a5caa")]
        public IEnumerable<EncryptionAlgorithmViewModel> GetEncryptionAlgorithms()
        {
            return new[] {
                new EncryptionAlgorithmViewModel
                {
                    EncryptionAlgorithm = CommonEncryptionAlgorithms.AES,
                    Recommend = true
                }
            };
        }

        /// <summary>
        /// DynamicApiのリクエスト/レスポンスの暗号化に使用する共通鍵を登録します。
        /// 共通鍵IDと暗号化に必要なパラメータを付加した共通鍵の情報を返します。
        /// </summary>
        /// <param name="encryptedCommonKey">RSA暗号化されたキーを含む共通鍵の情報</param>
        /// <returns>暗号化パラメータを付加した共通鍵の情報</returns>
        [HttpPost]
        [ManageAction("d5731710-8248-403d-af68-8189eec07940")]
        public async Task<IActionResult> RegisterCommonKey()
        {
            // SystemIdが取得できない場合はエラーを返す
            string systemId = PerRequestDataContainer?.SystemId;
            if (string.IsNullOrEmpty(systemId))
            {
                return BadRequest("Can not get SystemId.");
            }

            var stream = new MemoryStream();
            Request.BodyReader.AsStream().CopyTo(stream);
            var encryptedCommonKey = stream.ToArray();

            RegisterCommonKeyRequestViewModel commonKey;
            try
            {
                // RAS秘密鍵で復号
                var plainData = await _cryptographyManagementInterface.Decrypt(systemId, encryptedCommonKey);
                if (plainData == null)
                {
                    return BadRequest("Decrypt failed.");
                }

                // 復号されたデータをデシリアライズ
                commonKey = JsonConvert.DeserializeObject<RegisterCommonKeyRequestViewModel>(Encoding.UTF8.GetString(plainData));
            }
            catch (AsymmetricKeyOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (JsonException ex)
            {
                return BadRequest("Deserialize json failed: " + ex.Message);
            }

            // リクエストモデルを検証
            if (TryValidateModel(commonKey))
            {
                // 登録データを作成
                var request = s_mapper.Map<CommonKeyModel>(commonKey);

                // 共通鍵を登録
                var registeredCommonKey = _cryptographyManagementInterface.RegisterCommonKey(systemId, request);

                // 返却データを作成
                var result = s_mapper.Map<RegisterCommonKeyResponseViewModel>(registeredCommonKey);
                var json = JsonConvert.SerializeObject(result);

                return new ContentResult() { Content = json, ContentType = MEDIATYPE_JSON };
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// 共通鍵登録APIのリクエストパラメータのスキーマ生成用アクション
        /// </summary>
        /// <param name="encryptedCommonKey">RSA暗号化されたキーを含む共通鍵の情報</param>
        /// <returns>
        /// </returns>
        [HttpPost]
        [ManageAction("c868cf5a-8ac1-4f63-92e3-aec97db59258")]
        public IActionResult RegisterCommonKeyRequestParameter(RegisterCommonKeyRequestViewModel encryptedCommonKey)
        {
            return Ok();
        }
    }
}
