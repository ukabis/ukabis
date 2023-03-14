using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Authentication;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    [TestClass]
    public class UnitTest_Client : UnitTestBase
    {
        [TestMethod]
        public void Client_VerificationClientSecret_IsActiveFalse()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.ClientIpAddress = "192.168.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var target = new Client();
            target.AccessTokenExpirationTimeSpan = new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(19));
            target.ClientId = new ClientId(Guid.NewGuid());
            target.ClientSecret = new ClientSecret("hoge");
            target.VendorId = new VendorId(Guid.NewGuid().ToString());
            target.SystemId = new SystemId(Guid.NewGuid().ToString());
            target.VerificationClientSecret(new ClientSecretVO("aaaa")).IsFalse();
        }

        [TestMethod]
        public void Client_VerificationClientSecret_ClientSeacretFaile()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.ClientIpAddress = "192.168.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var target = new Client();
            target.AccessTokenExpirationTimeSpan = new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(19));
            target.ClientId = new ClientId(Guid.NewGuid());
            target.ClientSecret = new ClientSecret("hoge");
            target.VendorId = new VendorId(Guid.NewGuid().ToString());
            target.SystemId = new SystemId(Guid.NewGuid().ToString());
            target.VerificationClientSecret(new ClientSecretVO("aaaa")).IsFalse();
        }

        [TestMethod]
        public void Client_VerificationClientSecret_True()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.ClientIpAddress = "192.168.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var target = new Client();
            target.AccessTokenExpirationTimeSpan = new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(19));
            target.ClientId = new ClientId(Guid.NewGuid());
            target.ClientSecret = new ClientSecret("hoge");
            target.VendorId = new VendorId(Guid.NewGuid().ToString());
            target.SystemId = new SystemId(Guid.NewGuid().ToString());
            target.VerificationClientSecret(new ClientSecretVO("hoge")).IsTrue();
        }

        [TestMethod]
        public void Client_VerificationClientSecret_CreateAccessToken()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.ClientIpAddress = "192.168.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var target = new Client();
            target.AccessTokenExpirationTimeSpan = new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(19));
            target.ClientId = new ClientId(Guid.NewGuid());
            target.ClientSecret = new ClientSecret("hoge");
            target.VendorId = new VendorId(Guid.NewGuid().ToString());
            target.SystemId = new SystemId(Guid.NewGuid().ToString());
            target.VerificationClientSecret(new ClientSecretVO("hoge")).IsTrue();

            var accessToken = target.CreateAccessToken();
            accessToken.AccessTokenId.IsNot(Guid.Empty.ToString());
            accessToken.ClientId.Is(target.ClientId.Value);
            accessToken.ExpirationTimeSpan.Is(target.AccessTokenExpirationTimeSpan.Value.Add(TimeSpan.FromMinutes(5)));

        }

        [TestMethod]
        public void Client_VerificationClientSecret_CreateClient()
        {
            var accessTokenExpirationTimeSpan = new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(19));
            var clientId = new ClientId(Guid.NewGuid());
            var clientSecret = new ClientSecret("hoge");
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());

            var result = Client.Create(clientId, clientSecret, systemId, accessTokenExpirationTimeSpan, vendorId);

            result.ClientId.IsStructuralEqual(clientId);
            result.ClientSecret.IsStructuralEqual(clientSecret);
            result.VendorId.IsStructuralEqual(vendorId);
            result.SystemId.IsStructuralEqual(systemId);
            result.AccessTokenExpirationTimeSpan.IsStructuralEqual(accessTokenExpirationTimeSpan);
        }

        [TestMethod]
        public void Client_VerificationClientSecret_CreateClientClientIdIsNull()
        {
            var accessTokenExpirationTimeSpan = new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(19));
            var clientSecret = new ClientSecret("hoge");
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());

            var result = Client.Create(null, clientSecret, systemId, accessTokenExpirationTimeSpan, vendorId);
            result.ClientId.Value.IsNot(Guid.Empty.ToString());
            result.ClientSecret.IsStructuralEqual(clientSecret);
            result.VendorId.IsStructuralEqual(vendorId);
            result.SystemId.IsStructuralEqual(systemId);
            result.AccessTokenExpirationTimeSpan.IsStructuralEqual(accessTokenExpirationTimeSpan);
        }
    }
}
