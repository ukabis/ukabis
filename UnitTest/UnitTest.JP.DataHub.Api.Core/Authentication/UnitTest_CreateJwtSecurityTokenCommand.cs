using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using JP.DataHub.Api.Core.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.JP.DataHub.Api.Core.Authentication
{
    [TestClass]
    public class UnitTest_CreateJwtSecurityTokenCommand
    {
        private string _issuer = "https://localhost";
        private string _audience = "https://localhost";
        private string _audienceSecret = "qSX1Io0X17ZzMshSmgllDCD8scYYSJRrfBQQNka5Fy63/4B7G2QafCHlE0NaR/NGJ4EmN7IPGYn+Lk1ACm8dgQ==";

        [TestMethod]
        public void CreateJwtSecurityTokenCommand_CreateJwtSecurityToken()
        {
            var now = DateTime.UtcNow;
            var target = new CreateJwtSecurityTokenCommand(_issuer, _audience,
                new List<Claim> { new Claim("Test", "hoge") }, now, now.AddMinutes(10), _audienceSecret);
            var result = target.CreateJwtSecurityToken();
            result.IsNotNull();
            result.IsNot("");

            // 署名検証用のキーを作成
            var signingKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(_audienceSecret));

            // 検証パラメータを作成
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.FromSeconds(1)
            };

            // JWTを検証
            SecurityToken validatedToken;
            var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(result, validationParameters, out validatedToken);
            claimsPrincipal.Identity.IsAuthenticated.IsTrue();
            claimsPrincipal.Claims.Where(x => x.Type == "Test").FirstOrDefault().Value.Is("hoge");
            DateTimeOffset.FromUnixTimeSeconds(long.Parse(claimsPrincipal.Claims.Where(x => x.Type == "nbf").FirstOrDefault().Value)).DateTime.ToString("yyyy/MM/dd HH:mm:ss").Is(now.ToString("yyyy/MM/dd HH:mm:ss"));
            DateTimeOffset.FromUnixTimeSeconds(long.Parse(claimsPrincipal.Claims.Where(x => x.Type == "exp").FirstOrDefault().Value)).DateTime.ToString("yyyy/MM/dd HH:mm:ss").Is(now.AddMinutes(10).ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }
}
