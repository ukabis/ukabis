using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Authentication;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.ApplicationService.Impl;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [TestClass]
    public class UnitTest_VendorSystemAuthenticationApplicationService : UnitTestBase
    {
        private string _vendorId = Guid.NewGuid().ToString();
        private string _systemId = Guid.NewGuid().ToString();
        private string _ipAddress = "123.45.67.89";
        private string _url = "/API/Test/Get";
        private string _issuer = "https://localhost";
        private string _audience = "https://localhost";
        private string _audienceSecret = "qSX1Io0X17ZzMshSmgllDCD8scYYSJRrfBQQNka5Fy63/4B7G2QafCHlE0NaR/NGJ4EmN7IPGYn+Lk1ACm8dgQ==";


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IVendorSystemAuthenticationApplicationService, VendorSystemAuthenticationApplicationService>();

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance(new VendorAuthenticationJwtKeyConfig()
            {
                Issuer = _issuer,
                Audience = _audience,
                AudienceSecret = _audienceSecret
            });
        }

        [TestMethod]
        public void AuthenticateJwtToken()
        {
            var accessTokenId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            var accessToken = new AccessToken
            {
                AccessTokenId = accessTokenId,
                ClientId = clientId,
                SystemId = _systemId
            };

            // モックの作成
            var mockAccessTokenRepository = new Mock<IAccessTokenCache>();
            mockAccessTokenRepository.Setup(s => s.Get(It.Is<string>(x => x == accessTokenId))).Returns(accessToken);
            UnityContainer.RegisterInstance(mockAccessTokenRepository.Object);

            var vendorSystemAccessTokenRepository = new Mock<IVendorSystemAccessTokenClientRepository>();
            vendorSystemAccessTokenRepository.Setup(s => s.GetByClientId(It.Is<ClientId>(x => x.Value == clientId.ToString()))).Returns(new[] { new ClientVendorSystem()
            {
                client_id = clientId.ToString(),
                client_secret = Guid.NewGuid().ToString(),
                vendor_id = _vendorId,
                system_id = _systemId,
                accesstoken_expiration_timespan = 1
            } });
            UnityContainer.RegisterInstance(vendorSystemAccessTokenRepository.Object);

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IVendorSystemAuthenticationApplicationService>();

            // トークン作成
            string token = CreateToken(accessTokenId.ToString());
            // テスト対象のメソッド実行
            var result = testClass.AuthenticateJwtToken(new VendorSystemAccessToken(token), new ClientIpaddress(_ipAddress), new ApiRelativeUrl(_url));

            // 結果をチェック
            result.IsSuccess.Is(true);
            result.VendorId.Is(_vendorId);
            result.SystemId.Is(_systemId);
            result.Title.IsNull();
        }

        [TestMethod]
        public void AuthenticateJwtToken_AccessTokenNotFound()
        {
            Guid accessTokenId = Guid.NewGuid();

            // モックの作成
            var mockAccessTokenRepository = new Mock<IAccessTokenCache>();
            UnityContainer.RegisterInstance(mockAccessTokenRepository.Object);

            var vendorSystemAccessTokenRepository = new Mock<IVendorSystemAccessTokenClientRepository>();
            UnityContainer.RegisterInstance(vendorSystemAccessTokenRepository.Object);

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IVendorSystemAuthenticationApplicationService>();

            // トークン作成
            string token = CreateToken(accessTokenId.ToString());
            // テスト対象のメソッド実行
            var result = testClass.AuthenticateJwtToken(new VendorSystemAccessToken(token), new ClientIpaddress(_ipAddress), new ApiRelativeUrl(_url));

            // 結果をチェック
            result.IsSuccess.Is(false);
            result.Title.Is(DynamicApiMessages.VendorSystemAccessTokenNotFound);
            result.Detail.Is(DynamicApiMessages.VendorSystemAccessTokenNotFound_Detail);
            result.ErrorCode.Is(ErrorCodeMessage.Code.E02406.ToString());
        }

        [TestMethod]
        public void AuthenticateJwtToken_ClientIdNotFound()
        {
            var accessTokenId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            var accessToken = new AccessToken
            {
                AccessTokenId = accessTokenId,
                ClientId = clientId,
                SystemId = _systemId
            };

            // モックの作成
            var mockAccessTokenRepository = new Mock<IAccessTokenCache>();
            mockAccessTokenRepository.Setup(s => s.Get(It.Is<string>(x => x == accessTokenId))).Returns(accessToken);
            UnityContainer.RegisterInstance(mockAccessTokenRepository.Object);

            var vendorSystemAccessTokenRepository = new Mock<IVendorSystemAccessTokenClientRepository>();
            UnityContainer.RegisterInstance(vendorSystemAccessTokenRepository.Object);

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IVendorSystemAuthenticationApplicationService>();

            // トークン作成
            string token = CreateToken(accessTokenId.ToString());
            // テスト対象のメソッド実行
            var result = testClass.AuthenticateJwtToken(new VendorSystemAccessToken(token), new ClientIpaddress(_ipAddress), new ApiRelativeUrl(_url));

            // 結果をチェック
            result.IsSuccess.Is(false);
            result.Title.Is(DynamicApiMessages.ClientIdNotFoundOrVendorSystemUnusable);
            result.Detail.Is(DynamicApiMessages.ClientIdNotFoundOrVendorSystemUnusable_Detail);
            result.ErrorCode.Is(ErrorCodeMessage.Code.E02405.ToString());
        }

        [TestMethod]
        public void AuthenticateJwtToken_Expired()
        {
            Guid accessTokenId = Guid.NewGuid();

            // モックの作成
            var mockAccessTokenRepository = new Mock<IAccessTokenCache>();
            UnityContainer.RegisterInstance(mockAccessTokenRepository.Object);
            var vendorSystemAccessTokenRepository = new Mock<IVendorSystemAccessTokenClientRepository>();
            UnityContainer.RegisterInstance(vendorSystemAccessTokenRepository.Object);

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IVendorSystemAuthenticationApplicationService>();

            // トークン作成
            string token = CreateToken(accessTokenId.ToString(), -20);
            // テスト対象のメソッド実行
            var result = testClass.AuthenticateJwtToken(new VendorSystemAccessToken(token), new ClientIpaddress(_ipAddress), new ApiRelativeUrl(_url));

            // 結果をチェック
            result.IsSuccess.Is(false);
            result.Title.Is(DynamicApiMessages.VendorSystemTokenExpired);
            result.Detail.Length.IsNot(0);
            result.ErrorCode.Is(ErrorCodeMessage.Code.E02407.ToString());
        }

        [TestMethod]
        public void AuthenticateJwtToken_InvalidSignature()
        {
            Guid accessTokenId = Guid.NewGuid();

            // モックの作成
            var mockAccessTokenRepository = new Mock<IAccessTokenCache>();
            UnityContainer.RegisterInstance(mockAccessTokenRepository.Object);
            var vendorSystemAccessTokenRepository = new Mock<IVendorSystemAccessTokenClientRepository>();
            UnityContainer.RegisterInstance(vendorSystemAccessTokenRepository.Object);

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IVendorSystemAuthenticationApplicationService>();

            // トークン作成
            string token = CreateToken(accessTokenId.ToString(), 0, "testtesttesttesttesttesttesttesttesttesttesttest");
            // テスト対象のメソッド実行
            var result = testClass.AuthenticateJwtToken(new VendorSystemAccessToken(token), new ClientIpaddress(_ipAddress), new ApiRelativeUrl(_url));

            // 結果をチェック
            result.IsSuccess.Is(false);
            result.Title.Is(DynamicApiMessages.VendorSystemTokenInvalid);
            result.Detail.Length.IsNot(0);
            result.ErrorCode.Is(ErrorCodeMessage.Code.E02408.ToString());
        }

        [TestMethod]
        public void AuthenticateJwtToken_MissingAccessTokenId()
        {
            // モックの作成
            var mockAccessTokenRepository = new Mock<IAccessTokenCache>();
            UnityContainer.RegisterInstance(mockAccessTokenRepository.Object);
            var vendorSystemAccessTokenRepository = new Mock<IVendorSystemAccessTokenClientRepository>();
            UnityContainer.RegisterInstance(vendorSystemAccessTokenRepository.Object);

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IVendorSystemAuthenticationApplicationService>();

            // トークン作成
            string token = CreateToken(null);
            // テスト対象のメソッド実行
            var result = testClass.AuthenticateJwtToken(new VendorSystemAccessToken(token), new ClientIpaddress(_ipAddress), new ApiRelativeUrl(_url));

            // 結果をチェック
            result.IsSuccess.Is(false);
            result.Title.Is(DynamicApiMessages.VendorSystemTokenInvalid);
            result.Detail.Is(DynamicApiMessages.VendorSystemAccessTokenIdMissing);
        }

        [TestMethod]
        public void AuthenticateJwtToken_InvalidFormat()
        {
            // モックの作成
            var mockAccessTokenRepository = new Mock<IAccessTokenCache>();
            UnityContainer.RegisterInstance(mockAccessTokenRepository.Object);
            var vendorSystemAccessTokenRepository = new Mock<IVendorSystemAccessTokenClientRepository>();
            UnityContainer.RegisterInstance(vendorSystemAccessTokenRepository.Object);

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IVendorSystemAuthenticationApplicationService>();

            // テスト対象のメソッド実行
            var result = testClass.AuthenticateJwtToken(new VendorSystemAccessToken("token"), new ClientIpaddress(_ipAddress), new ApiRelativeUrl(_url));

            // 結果をチェック
            result.IsSuccess.Is(false);
            result.Title.Is(DynamicApiMessages.VendorSystemTokenInvalid);
            result.Detail.Length.IsNot(0);
        }

        private string CreateToken(string accessTokenId, int addNbf = 0, string secret = null)
        {
            var issuer = _issuer;
            var audience = _audience;
            var audienceSecret = string.IsNullOrEmpty(secret) ? _audienceSecret : secret;
            var notBefore = DateTime.UtcNow.AddMinutes(addNbf);
            var expires = notBefore.AddMinutes(10);
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(accessTokenId)) claims.Add(new Claim("AccessTokenId", accessTokenId));

            // 署名用のキーを作成
            var signingKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(audienceSecret));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(issuer, audience, claims,
                notBefore, expires, signingCredentials));

            return token;
        }
    }
}
