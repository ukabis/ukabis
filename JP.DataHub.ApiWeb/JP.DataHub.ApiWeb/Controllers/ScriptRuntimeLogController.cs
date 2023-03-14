using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Interface;

namespace JP.DataHub.ApiWeb.Controllers
{
    /// <summary>
    /// RoslynScriptのログを取得します。
    /// </summary>
    [ApiController]
    [Route("API/[controller]/[action]")]
    [ManageApi("366895EB-DCE2-4614-A219-C19FF5ED3A87")]
    public class ScriptRuntimeLogController : AbstractController
    {
        private static readonly bool s_isEnableScriptRuntimeLogService = UnityCore.Resolve<bool>("IsEnableScriptRuntimeLogService");
        private static readonly bool s_internalServerErrorDetailResponse = UnityCore.Resolve<bool>("IsInternalServerErrorDetailResponse");

        private IScriptRuntimeLogFileInterface _scriptRuntimeLogInterface = UnityCore.Resolve<IScriptRuntimeLogFileInterface>();


        /// <summary>
        /// ログIDを指定してログを取得します。
        /// </summary>
        /// <param name="logId">ログID</param>
        /// <returns>ログ</returns>
        [HttpGet]
        [ManageApi("BA48AF6C-B49B-4DB9-A0C2-FB1B46F064E1")]
        public async Task<IActionResult> Get(string logId)
        {
            if (!s_isEnableScriptRuntimeLogService)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotImplemented);
            }

            if (!Guid.TryParse(logId, out Guid _logId) |
                !Guid.TryParse(PerRequestDataContainer.VendorId, out Guid vendorId))
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            try
            {
                var file = _scriptRuntimeLogInterface.Get(_logId, vendorId);
                return new ContentResult() { StatusCode = (int)HttpStatusCode.OK, Content = await file.Content.ReadAsStringAsync() };
            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (NotFoundException)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                if (s_internalServerErrorDetailResponse) 
                { 
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                throw;
            }
        }


        /// <summary>
        /// ログIDを指定してログを削除します。
        /// </summary>
        /// <param name="logId">ログID</param>
        /// <returns></returns>
        [HttpDelete]
        [ManageApi("D9F4BD8D-C4F5-45EE-AB29-FBA46F00D9D1")]
        public IActionResult Delete(string logId)
        {
            if (!s_isEnableScriptRuntimeLogService)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotImplemented);
            }

            if (!Guid.TryParse(logId, out Guid _logId) |
                !Guid.TryParse(PerRequestDataContainer.VendorId, out Guid vendorId))
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            try
            {
                bool result = _scriptRuntimeLogInterface.Delete(_logId, vendorId);
                return new ContentResult() { StatusCode = (int)(result ? HttpStatusCode.OK : HttpStatusCode.NotFound) };
            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (NotFoundException)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                if (s_internalServerErrorDetailResponse) 
                { 
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                throw;
            }
        }
    }
}