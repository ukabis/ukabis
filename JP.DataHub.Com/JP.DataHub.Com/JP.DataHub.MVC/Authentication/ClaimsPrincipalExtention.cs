using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Authentication
{
    public static class ClaimsPrincipalExtention
    {
        /// <summary>
        /// NameIdentifier変換処理
        /// </summary>
        /// <param name="principal">クレーム</param>
        /// <returns>NameIdentifierの設定値、または設定値から生成したGUID</returns>
        /// <remarks>
        /// Oracle Identity Domainsを使用したOpenID認証の場合、
        /// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier にはGUIDではなくメールアドレスが設定される。
        /// 本システムはOpenIDユーザの値がGUIDであるという前提で構築されているため、メールアドレスをGUIDに変換する必要がある。
        /// そこで、MD5でメールアドレスのハッシュ値（16バイト）を生成し、その値をGUIDとするようにした。
        /// </remarks>
        public static string GetNameIdentifier(this ClaimsPrincipal principal)
        {
            var result = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var original = result;
#if Oracle
            if (result != null)
            {
                if (Guid.TryParse(result, out var _))
                {
                    throw new InvalidOperationException("すでにGUIDに変換された文字列を再変換しようとしています。");
                }
                using MD5 myMD5 = MD5.Create();
                result = new Guid(myMD5.ComputeHash(Encoding.UTF8.GetBytes(result))).ToString();
            }
#endif
            return result;
        }
    }
}
