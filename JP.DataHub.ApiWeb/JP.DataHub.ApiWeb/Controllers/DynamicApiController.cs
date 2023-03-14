using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Filters;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("API/{*param}")]
    [ResponseEncrypt]
    public class DynamicApiController : AbstractController
    {
        private Lazy<ICryptographyInterface> _lazyCryptographyInterface = new Lazy<ICryptographyInterface>(() => UnityCore.Resolve<ICryptographyInterface>());
        private ICryptographyInterface CryptographyInterface => _lazyCryptographyInterface.Value;


        public DynamicApiController()
            : base()
        {
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = Api.Request(CreateRequestModel());
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var result = Api.Request(CreateRequestModel());
            return ToActionResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put()
        {
            var result = Api.Request(CreateRequestModel());
            return ToActionResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var result = Api.Request(CreateRequestModel());
            return ToActionResult(result);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch()
        {
            var result = Api.Request(CreateRequestModel());
            return ToActionResult(result);
        }


        private static string ParseContentType(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return !MediaTypeHeaderValue.TryParse(input, out var mediaTypeHeader) ? input : mediaTypeHeader.MediaType;
        }

        private DynamicApiRequestModel CreateRequestModel()
        {
            var requestModel = new DynamicApiRequestModel
            {
                HttpMethod = Request.Method,
                Header = ToModelHeaders(Request.Headers),
                MediaType = ParseContentType(Request.ContentType) ?? MediaTypeConst.ApplicationJson,
                RelativeUri = Request.Path.Value,
                QueryString = Request.QueryString.ToString(),
                Accept = Request.Headers.ContainsKey(HeaderConst.Accept) ? Request.Headers[HeaderConst.Accept].FirstOrDefault() : MediaTypeConst.Wildcard,
                ContentRange = Request.Headers.ContainsKey(HeaderConst.ContentRange) ? Request.Headers[HeaderConst.ContentRange].FirstOrDefault() : null,
                ContentType = Request.ContentType,
                ContentLength = Request.ContentLength,
            };
            
            // RequestBodyを複数回読むために必要（AOPFilterでも読みたいから）
            Request.EnableBuffering();

            // 共通鍵IDとシステムIDが設定されている場合に復号する
            if (!string.IsNullOrEmpty(PerRequestDataContainer.CommonKeyId) && !string.IsNullOrEmpty(PerRequestDataContainer.SystemId))
            {
                try
                {
                    // リクエストデータ取得
                    var stream = new MemoryStream();
                    Request.BodyReader.AsStream().CopyTo(stream);
                    byte[] encryptData = stream.ToArray();

                    // 復号実行
                    byte[] plainData = CryptographyInterface.Decrypt(PerRequestDataContainer.SystemId, PerRequestDataContainer.CommonKeyId, encryptData);

                    // 平文データ設定
                    requestModel.Contents = Encoding.UTF8.GetString(plainData);

                    // リクエストストリーム取得
                    var encryptStream = Request.BodyReader.AsStream();

                    // 復号ストリーム設定
                    requestModel.ContentsStream = CryptographyInterface.GetDecryptStream(PerRequestDataContainer.SystemId, PerRequestDataContainer.CommonKeyId, encryptStream);
                }
                catch (CommonKeyNotFoundException)
                {
                    throw new Rfc7807Exception(ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10442));
                }
                catch (CryptographicException)
                {
                    throw new Rfc7807Exception(ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10443));
                }
            }
            else
            {
                requestModel.ContentsStream = Request.Body;
            }

            return requestModel;
        }

        private HttpHeaderModel ToModelHeaders(IHeaderDictionary headers)
        {
            var result = new HttpHeaderModel();
            foreach (var key in headers.Keys)
            {
                result.Add(key, headers[key]);
            }
            return result;
        }
    }
}