using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_VendorSystemAccessToken : UnitTestBase
    {
        private string _issuer = "https://localhost";
        private string _audience = "https://localhost";
        private string _audienceSecret = "qSX1Io0X17ZzMshSmgllDCD8scYYSJRrfBQQNka5Fy63/4B7G2QafCHlE0NaR/NGJ4EmN7IPGYn+Lk1ACm8dgQ==";


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance(new VendorAuthenticationJwtKeyConfig()
            {
                Issuer = _issuer,
                Audience = _audience,
                AudienceSecret = _audienceSecret
            });
        }

        [TestMethod]
        public void VendorSystemAccessToken_Validate_FormatErrorToken()
        {
            var target = new VendorSystemAccessToken("aaaaaaa");
            AssertEx.Catch<ArgumentException>(() => { target.Validate(); });
        }

        [TestMethod]
        public void VendorSystemAccessToken_Validate_TokenEmpty()
        {
            var target = new VendorSystemAccessToken("");
            AssertEx.Catch<ArgumentNullException>(() => { target.Validate(); });
        }

        [TestMethod]
        public void VendorSystemAccessToken_Validate_FakeToken()
        {
            string accessTokenId = Guid.NewGuid().ToString();
            string token = CreateToken(accessTokenId, 0, "testtesttesttesttesttesttesttesttesttesttesttest");

            var target = new VendorSystemAccessToken(token);
            AssertEx.Catch<SecurityTokenSignatureKeyNotFoundException>(() => { target.Validate(); });
        }

        [TestMethod]
        public void VendorSystemAccessToken_Validate_ExpireToken()
        {
            string accessTokenId = Guid.NewGuid().ToString();
            string token = CreateToken(accessTokenId, -20);

            var target = new VendorSystemAccessToken(token);
            AssertEx.Catch<SecurityTokenExpiredException>(() => { target.Validate(); });
        }

        [TestMethod]
        public void VendorSystemAccessToken_Validate_Ok()
        {
            string accessTokenId = Guid.NewGuid().ToString();
            string token = CreateToken(accessTokenId);

            var target = new VendorSystemAccessToken(token);
            target.Validate().FindFirst("AccessTokenId").Value.Is(accessTokenId);
        }

        private string CreateToken(string accessTokenId, int addNbf = 0, string secret = null)
        {
            var audienceSecret = string.IsNullOrEmpty(secret) ? _audienceSecret : secret;
            var notBefore = DateTime.UtcNow.AddMinutes(addNbf);
            var expires = notBefore.AddMinutes(10);
            var claims = new List<Claim> { new Claim("AccessTokenId", accessTokenId) };

            // 署名用のキーを作成
            var signingKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(audienceSecret));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_issuer, _audience, claims,
                notBefore, expires, signingCredentials));

            return token;
        }
    }
}