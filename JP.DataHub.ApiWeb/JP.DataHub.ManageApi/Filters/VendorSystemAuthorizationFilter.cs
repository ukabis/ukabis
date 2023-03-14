using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using JP.DataHub.Com.Authentication;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Resources;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.MVC.Misc;
using JP.DataHub.MVC.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ManageApi.Core.DataContainer;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Filters
{
    public class VendorSystemAuthorizationFilter : ActionFilterAttribute
    {
        private static JPDataHubLogger s_logger = new JPDataHubLogger(typeof(VendorSystemAuthorizationFilter));

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dataContainer = UnityCore.Resolve<IPerRequestDataContainer>();

            // ClientIpAddress
            string ipAddress = RequestUtil.GetRequestIP();
            dataContainer.ClientIpAddress = ParseIPAddressPriorityV4(ipAddress).ToString();

            // X-Authorization
            IActionResult validResult = null;
            if (context.HttpContext.Request.Headers.TryGetValue(HeaderConst.XAuthorization, out StringValues x_authorization))
            {
                try
                {
                    validResult = ValidationByVendorSystemToken(dataContainer, x_authorization.ToString(), context);
                }
                catch (Rfc7807Exception ex)
                {
                    var err = (ex.Rfc7807 as RFC7807ProblemDetailExtendErrors) ?? ErrorCodeMessage.Code.E02408.GetRFC7807(context.HttpContext.Request.Path.Value);
                    context.Result = new ObjectResult(new
                    {
                        error_code = err.ErrorCode,
                        title = err.Title,
                        detail = err.Detail,
                        status = err.Status
                    }) { StatusCode = err.Status };
                    return;
                }
                catch (Exception ex)
                {
                    s_logger.Error("VendorSystemAuthenticationError", ex);
                    var err = ErrorCodeMessage.Code.E02408.GetRFC7807(context.HttpContext.Request.Path.Value);
                    err.Detail = ex.Message;
                    context.Result = new ObjectResult(new
                    {
                        error_code = err.ErrorCode,
                        title = err.Title,
                        detail = err.Detail,
                        status = err.Status
                    })
                    { StatusCode = (int)HttpStatusCode.InternalServerError };
                    return;
                }
            }

            if (dataContainer.VendorSystemAuthenticated) { return; }

            // X-VendorId, X-SystemIdが指定されている場合はそれを採用する
            if (context.HttpContext.Request.Headers.TryGetValue(HeaderConst.XVendor, out var x_vendorid) &&
                context.HttpContext.Request.Headers.TryGetValue(HeaderConst.XSystem, out var x_systemid) &&
                x_vendorid.Any(x => !string.IsNullOrEmpty(x)) && x_systemid.Any(x => !string.IsNullOrEmpty(x)))
            {
                SetVendorSystemId(dataContainer, null, x_vendorid.First(), x_systemid.First(), context);
            }
            // トークンは送ってきたが誤っている
            else if (validResult is not null)
            {
                context.Result = validResult;
            }
        }
        /// <summary>
        /// アクセストークンによる認証を行います
        /// </summary>
        /// <param name="accessToken">アクセストークン></param>
        /// <param name="container">データコンテナ</param>
        /// <param name="context">HttpActionContext</param>
        private IActionResult ValidationByVendorSystemToken(IPerRequestDataContainer dataContainer, string accessToken, ActionExecutingContext context)
        {
            var service = UnityCore.Resolve<IVendorAuthenticationService>();
            var result = service.AuthenticateJwtToken(new VendorAccessToken(accessToken), dataContainer.ClientIpAddress, context.HttpContext.Request.Path);
            if (result?.IsSuccess == true)
            {
                SetVendorSystemId(dataContainer, accessToken, result.VendorId, result.SystemId, context);
                return null;
            }
            dataContainer.VendorSystemAuthenticated = false;

            return null;
        }

        private void SetVendorSystemId(IPerRequestDataContainer dataContainer, string accessToken, string vendorId, string systemId, ActionExecutingContext context)
        {
            if (!Guid.TryParse(vendorId, out Guid vendorGuid))
            {
                var err = ErrorCodeMessage.Code.E02408.GetRFC7807(context.HttpContext.Request.Path.Value);
                context.Result = new ObjectResult(new
                {
                    error_code = err.ErrorCode,
                    title = err.Title,
                    detail = err.Detail,
                    status = err.Status

                })
                { StatusCode = (int)HttpStatusCode.Forbidden };
                return;
            }

            dataContainer.VendorId = vendorGuid.ToString();

            if (!Guid.TryParse(systemId, out Guid systemGuid))
            {
                var err = ErrorCodeMessage.Code.E02408.GetRFC7807(context.HttpContext.Request.Path.Value);
                context.Result = new ObjectResult(new
                {
                    error_code = err.ErrorCode,
                    title = err.Title,
                    detail = err.Detail,
                    status = err.Status

                })
                { StatusCode = (int)HttpStatusCode.Forbidden };
                return;
            }

            dataContainer.SystemId = systemGuid.ToString();
            dataContainer.OriginalAccessToken = accessToken;
            dataContainer.VendorSystemAuthenticated = true;
        }

        /// <summary>
        /// IPAddressにパースする。IPV4に変換可能なものはIPV4に変換する
        /// </summary>
        private IPAddress ParseIPAddressPriorityV4(string ipAddress)
        {
            var parseAddress = IPAddress.Parse(ipAddress);
            if (parseAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return parseAddress;
            }
            if (parseAddress.IsIPv4MappedToIPv6)
            {
                return parseAddress.MapToIPv4();
            }
            if (parseAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && IPAddress.IsLoopback(parseAddress))
            {
                return IPAddress.Loopback;
            }
            return parseAddress;
        }
    }
}
