using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.MVC.Authentication;

namespace JP.DataHub.MVC.Filters
{
    /// <summary>
    /// AuthorizationヘッダーのOpenIDトークンを検証します。
    /// 実際に認可するかどうかは後続の処理に任せています。
    /// </summary>
    public class OpenIdAuthorizationFilter : IAuthorizationFilter
    {
        private const string OPENID_KEY = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private static readonly JPDataHubLogger s_logger = new JPDataHubLogger(typeof(OpenIdAuthorizationFilter));
        private static Lazy<IConfiguration> _lazyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        private static IConfiguration _configuration { get => _lazyConfiguration.Value; }
        private static Lazy<string> _lazyWebApiClientId = new Lazy<string>(() => _configuration.GetValue<string>("OpenId:WebApiClientId"));
        private static string _webApiClientId { get => _lazyWebApiClientId.Value; }

        public static RFC7807ProblemDetailExtendErrors InvalidTokenRFC7807 { get; set; }
        public static RFC7807ProblemDetailExtendErrors ExpiredTokenRFC7807 { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues values))
            {
                if (TryGetToken(values.FirstOrDefault(), out string token))
                {
                    var oidcAccessTokenValidator = OidcAccessTokenValidator.CreateFromConfig();
                    try
                    {
                        context.HttpContext.User = oidcAccessTokenValidator.Validate(token);
                        // DataContainerにも認証情報を登録
                        SetUserInfo(context.HttpContext.User);
                    }
                    catch (ArgumentException ex)
                    {
                        s_logger.Info("トークンの形式が不正です。:" + ex.Message);
                        SetAuthorizationError(InvalidTokenRFC7807, "AuthorizationToken is invalid", ex.Message);
                    }
                    catch (SecurityTokenExpiredException ex)
                    {
                        s_logger.Info("トークンの有効期限が切れました。:" + ex.Message);
                        SetAuthorizationError(ExpiredTokenRFC7807, "AuthorizationToken is expired", ex.Message);
                    }
                    catch (Exception ex) when (ex is SecurityTokenException || ex is SecurityTokenInvalidSignatureException)
                    {
                        s_logger.Info("トークンの検証に失敗しました。:" + ex.Message);
                        SetAuthorizationError(InvalidTokenRFC7807, "AuthorizationToken is invalid", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        s_logger.Error("トークンの検証で予期せぬエラーが発生しました。", ex);
                        SetAuthorizationError(InvalidTokenRFC7807, "AuthorizationToken is invalid", ex.Message);
                    }
                }
            }
        }

        private void SetUserInfo(ClaimsPrincipal user)
        {
            var dataContainer = DataContainerUtil.ResolveDataContainer();
            dataContainer.OpenId = user.GetNameIdentifier();
            // audが複数件含まれている場合があるが、URL形式のものは使用しないので除外する
            dataContainer.Claims = user.Claims.Where(c => (c.Type != "aud" || !c.Value.StartsWith("http"))).ToDictionary(c => c.Type, c => c.Value);
            dataContainer.IsDeveloper = user.FindFirst("aud")?.Value == _webApiClientId;
        }

        private void SetAuthorizationError(RFC7807ProblemDetailExtendErrors error, string message, string detail)
        {
            var dataContainer = DataContainerUtil.ResolveDataContainer();
            if (error == null)
            {
                dataContainer.AuthorizationError = JsonConvert.SerializeObject(new { Message = message, Detail = detail });
            }
            else
            {
                dataContainer.AuthorizationError = JsonConvert.SerializeObject(error);
            }
        }

        private bool TryGetToken(string authorization, out string token)
        {
            token = string.Empty;
            if (authorization != null && authorization.Length > 7 &&
                authorization.StartsWith("Bearer ", true, CultureInfo.CurrentCulture))
            {
                token = authorization.Substring(7);
                return true;
            }
            return false;
        }
    }
}