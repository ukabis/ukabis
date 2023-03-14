using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Interface;

namespace JP.DataHub.ApiWeb.Filters
{
    /// <summary>
    /// レスポンスをAES暗号化する属性です。
    /// </summary>
    public class ResponseEncryptAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// レスポンスをAES暗号化します。
        /// </summary>
        /// <param name="actionExecutedContext">HttpActionExecutedContext</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // エラーの場合は処理しない
            if (context.HttpContext.Response == null ||
                context.HttpContext.Response.StatusCode < 200 ||
                context.HttpContext.Response.StatusCode >= 300)
            {
                return;
            }

            // 共通鍵IDとシステムIDが設定されている場合に暗号化する
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            if (!string.IsNullOrEmpty(perRequestDataContainer.CommonKeyId) && !string.IsNullOrEmpty(perRequestDataContainer.SystemId))
            {
                // レスポンス取得
                MemoryStream plainStream;
                int? statusCode;
                MediaTypeCollection contentType;
                if (context.Result is ObjectResult objResult && objResult.Value is Stream responseStream)
                {
                    statusCode = objResult.StatusCode;
                    contentType = objResult.ContentTypes;

                    plainStream = new();
                    responseStream.Position = 0;
                    responseStream.CopyTo(plainStream);
                }
                else
                {
                    // 必要に応じてパターン追加
                    return;
                }

                // 暗号化ストリーム取得
                plainStream.Position = 0;
                var cryptographyInterface = UnityCore.Resolve<ICryptographyInterface>();
                var encryptStream = cryptographyInterface.GetEncryptStream(perRequestDataContainer.SystemId, perRequestDataContainer.CommonKeyId, plainStream);

                // 暗号化ストリームをレスポンスに設定
                context.Result = new ObjectResult(encryptStream) 
                { 
                    StatusCode = statusCode, 
                    ContentTypes = contentType
                };
            }
        }
    }
}