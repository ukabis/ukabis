using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
using JP.DataHub.MVC.Misc;
using JP.DataHub.MVC.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Filters
{
    public class VendorSystemAuthorizationFilter : ActionFilterAttribute
    {
        private static JPDataHubLogger _logger = new JPDataHubLogger(typeof(VendorSystemAuthorizationFilter));

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
                catch (Exception ex)
                {
                    _logger.Error("VendorSystemAuthenticationError", ex);
                    var err = ErrorCodeMessage.Code.E10502.GetRFC7807(context.HttpContext.Request.Path.Value);
                    err.Detail = ex.Message;
                    context.Result = new ObjectResult(new
                    {
                        error_code = err.ErrorCode,
                        title = err.Title,
                        detail = err.Detail,
                        status = err.Status
                    }) { StatusCode = (int)HttpStatusCode.InternalServerError };
                    return;
                }
            }
            else if (context.HttpContext.Connection.ClientCertificate != null)
            {
                try
                {
                    validResult = ValidationByClientCertificate(dataContainer, context.HttpContext.Connection.ClientCertificate, context);
                }
                catch (Exception ex)
                {
                    _logger.Error("ClientCertificateAuthenticationError", ex);
                    var err = ErrorCodeMessage.Code.E10502.GetRFC7807(context.HttpContext.Request.Path.Value);
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
            // トークンなし
            else
            {
                // デフォルトのベンダーシステムIDをセット
                dataContainer.VendorId = UnityCore.Resolve<string>("VendorSystemAuthenticationDefaultVendorId");
                dataContainer.SystemId = UnityCore.Resolve<string>("VendorSystemAuthenticationDefaultSystemId");
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
            var authService = UnityCore.Resolve<IVendorSystemAuthenticationApplicationService>();
            var result = authService.AuthenticateJwtToken(new VendorSystemAccessToken(accessToken), new ClientIpaddress(dataContainer.ClientIpAddress), new ApiRelativeUrl(context.HttpContext.Request.Path));
            if (result.IsSuccess)
            {
                SetVendorSystemId(dataContainer, accessToken, result.VendorId, result.SystemId, context);
                return null;
            }
            dataContainer.VendorSystemAuthenticated = false;

            var message = new {
                error_code = result.ErrorCode,
                title = result.Title,
                detail = result.Detail,
                status = (int)result.Status
            };
            return new ObjectResult(message) { StatusCode = (int)result.Status };
        }

        /// <summary>
        /// クライアント証明書による認証を行います
        /// </summary>
        /// <param name="container">データコンテナ</param>
        /// <param name="clientCertificate">クライアント証明書</param>
        /// <param name="context">HttpActionContext</param>
        private IActionResult ValidationByClientCertificate(IPerRequestDataContainer dataContainer, X509Certificate2 clientCertificate, ActionExecutingContext context)
        {
            var authService = UnityCore.Resolve<IVendorSystemClientCertificateAuthenticationApplicationService>();
            var result = authService.AuthenticateCetificate(clientCertificate);
            if (result.IsSuccess)
            {
                SetVendorSystemId(dataContainer, null, result.VendorId, result.SystemId, context);
                dataContainer.XVendorSystemCertificateAuthenticated = true;
                return null;
            }
            dataContainer.VendorSystemAuthenticated = false;

            var message = new
            {
                error_code = result.ErrorCode,
                title = result.Title,
                detail = result.Detail,
                status = (int)result.Status
            };
            return new ObjectResult(message) { StatusCode = (int)result.Status };
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
