using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    /// <summary>
    /// APIがOpenId認証されているか検証します。
    /// </summary>
    static class OpenIdAuthorizationValidator
    {
        /// <summary>
        /// 検証結果
        /// </summary>
        public enum ValidationStatus
        {
            Valid,
            AuthorizationInvalid,
            AudienceInvalid
        }

        /// <summary>
        /// APIがOpenId認証されているか検証します。
        /// </summary>
        /// <param name="openId">OpenId認証されたユーザーのオブジェクトID</param>
        /// <param name="isDeveloper">開発者向けのテナントでOpenId認証されたかどうか</param>
        /// <param name="claims">OpenId認証時のアクセストークンから生成したClaims</param>
        /// <param name="openIdAllowedApplications">OpenId認証を許可するアプリケーションのリスト</param>
        /// <returns>ValidationStatus</returns>
        public static ValidationStatus Validate(string openId, bool isDeveloper, Dictionary<string, string> claims, IEnumerable<OpenIdAllowedApplication> openIdAllowedApplications)
        {
            // HttpContextを取得
            var context = UnityCore.Resolve<IHttpContextAccessor>()?.HttpContext;
            // HttpContextが存在し、認証されていない場合は拒否
            if (context != null && !context.User.Identity.IsAuthenticated) return ValidationStatus.AuthorizationInvalid;
            // OpenIdが設定されていない場合は拒否
            if (string.IsNullOrEmpty(openId)) return ValidationStatus.AuthorizationInvalid;
            // 開発者で認証されていれば許可
            if (isDeveloper) return ValidationStatus.Valid;
            // audienceが取得できなければ拒否
            if (claims == null || !claims.TryGetValue("aud", out string audience)) return ValidationStatus.AudienceInvalid;
            // audienceが許可リストに含まれているか返却
            return openIdAllowedApplications != null && openIdAllowedApplications.Any(a => a.Value == audience) ? ValidationStatus.Valid : ValidationStatus.AudienceInvalid;
        }
    }
}
