using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.OpenIdUser;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_OpenIdUserRepositoryForAzure : UnitTestBase
    {
        // テストパラメーター
        private const string ObjectId = "d2d03b3b-6d5c-46a8-9ebd-965faad4b46f";
        private const string SystemId = "FE652EE4-E0E5-4C28-AA0D-885FE5AC22BA";
        private string UserId => Configuration.GetSection("AppConfig:OpenIdTestMailAddress").Get<string>();
        private const string UserId2 = "user2@example.com";

        private IOpenIdUserRepository _testClass;
        private string _apiEndpoint;
        private string _applicationEndpoint;
        private FakeHttpClientHandler _handler = new FakeHttpClientHandler();
        

        public UnitTest_OpenIdUserRepositoryForAzure()
        {
            UnityContainer = UnityCore.UnityContainer = new UnityContainer();
            UnityContainer.RegisterInstance(Configuration);

            var graphApiEndpoint = Configuration.GetValue<string>("ida:GraphApiEndpoint");
            var tenant = Configuration.GetValue<string>("OpenId:Tenant");
            var graphApiVersion = Configuration.GetValue<string>("ida:GraphApiVersion");

            _apiEndpoint = string.Format(graphApiEndpoint, tenant, "users", graphApiVersion);
            _applicationEndpoint = string.Format(graphApiEndpoint, tenant, "applications", graphApiVersion);

            // レスポンスの設定
            string filter = "&$filter=startswith(displayName, 'b2c-extensions-app')";
            _handler.SetResponse(HttpMethod.Get, _applicationEndpoint + filter, HttpStatusCode.OK, Resources.OpenIdUserRepository.Responses.GetApplicationResponse);
            var builder = new UriBuilder(_applicationEndpoint);
            builder.Path += "/34fde61a-688f-440a-a516-9e3d17826f81/extensionProperties";
            _handler.SetResponse(HttpMethod.Get, builder.Uri.ToString(), HttpStatusCode.OK, Resources.OpenIdUserRepository.Responses.GetPropertyNameResponse);

            // テスト対象のインスタンスを作成
            _testClass = new OpenIdUserRepositoryForAzure(new HttpClient(_handler));
        }

        private string AddApiPath(string addPath)
        {
            var builder = new UriBuilder(_apiEndpoint);
            builder.Path += "/" + addPath;
            return builder.Uri.ToString();
        }


        [TestMethod]
        public async Task Delete()
        {
            // レスポンスの設定
            _handler.SetResponse(HttpMethod.Delete, AddApiPath(ObjectId), HttpStatusCode.NoContent, string.Empty);

            // テスト対象のメソッド実行&
            await _testClass.Delete(new ObjectId(ObjectId));
        }

        [TestMethod]
        [ExpectedException(typeof(OpenIdUserOperationException))]
        public async Task Delete_Error()
        {
            // レスポンスの設定
            _handler.SetResponse(HttpMethod.Get, AddApiPath(ObjectId), HttpStatusCode.BadRequest, Resources.OpenIdUserRepository.Responses.OpenIdUserErrorResponse);

            // テスト対象のメソッド実行&
            await _testClass.Delete(new ObjectId(ObjectId));
        }

        [TestMethod]
        public async Task Get()
        {
            // レスポンスの設定
            string mailAddress = HttpUtility.UrlEncode(UserId);
            string filter = $"&$filter=signInNames/any(n: n/value eq '{mailAddress}') or otherMails/any(m: m eq '{mailAddress}') and creationType eq 'LocalAccount'";
            _handler.SetResponse(HttpMethod.Get, _apiEndpoint + filter, HttpStatusCode.OK, Resources.OpenIdUserRepository.Responses.GetUserResponse);

            // テスト対象のメソッド実行
            var result = await _testClass.Get(new UserId(UserId));
            // 期待値を作成
            var expected = new OpenIdUser(ObjectId, UserId, SystemId, null, "テストユーザー１", DateTime.Parse("2019-02-18T10:00:33Z").ToUniversalTime());

            // 結果をチェック
            result.IsStructuralEqual(expected);
        }

        [TestMethod]
        public async Task Get_NotFound()
        {
            // レスポンスの設定
            string mailAddress = HttpUtility.UrlEncode(UserId2);
            string filter = $"&$filter=signInNames/any(n: n/value eq '{mailAddress}') or otherMails/any(m: m eq '{mailAddress}') and creationType eq 'LocalAccount'";
            _handler.SetResponse(HttpMethod.Get, _apiEndpoint + filter, HttpStatusCode.OK, "{ \"value\": [] }");

            // テスト対象のメソッド実行&
            var result = await _testClass.Get(new UserId(UserId2));
            // 結果をチェック
            result.IsNull();
        }

        [TestMethod]
        [ExpectedException(typeof(OpenIdUserOperationException))]
        public async Task Get_Error()
        {
            // レスポンスの設定
            string filter = $"&$filter=signInNames/any(n: n/value eq '{UserId}') or otherMails/any(m: m eq '{UserId}') and creationType eq 'LocalAccount'";
            _handler.SetResponse(HttpMethod.Get, _apiEndpoint + filter, HttpStatusCode.BadRequest, Resources.OpenIdUserRepository.Responses.OpenIdUserErrorResponse);

            // テスト対象のメソッド実行
            var result = await _testClass.Get(new UserId(UserId));
        }

        [TestMethod]
        public async Task GetList()
        {
            // レスポンスの設定
            var systemIdVO = new SystemId(SystemId);
            string filter = $"&$filter=extension_12345678_SystemId eq '{systemIdVO.Value}'";
            _handler.SetResponse(HttpMethod.Get, _apiEndpoint + filter, HttpStatusCode.OK, Resources.OpenIdUserRepository.Responses.GetUserResponse);

            // テスト対象のメソッド実行
            var result = await _testClass.GetList(systemIdVO);
            // 期待値を作成
            var expected = new[] { new OpenIdUser(ObjectId, UserId, SystemId, null, "テストユーザー１", DateTime.Parse("2019-02-18T10:00:33Z").ToUniversalTime()) };

            // 結果をチェック
            result.ToArray().IsStructuralEqual(expected);
        }

        [TestMethod]
        [ExpectedException(typeof(OpenIdUserOperationException))]
        public async Task GetList_Error()
        {
            // レスポンスの設定
            string filter = $"&$filter=hoge eq '{SystemId}'";
            _handler.SetResponse(HttpMethod.Get, _apiEndpoint + filter, HttpStatusCode.BadRequest, Resources.OpenIdUserRepository.Responses.OpenIdUserErrorResponse);

            // テスト対象のメソッド実行
            var result = await _testClass.Get(new UserId(UserId));
        }

        [TestMethod]
        public async Task Register()
        {
            // レスポンスの設定
            _handler.SetResponse(HttpMethod.Post, _apiEndpoint, HttpStatusCode.Created, Resources.OpenIdUserRepository.Responses.RegisterUserResponse);

            var newUser = new OpenIdUser(null, UserId, SystemId, "P@ssw0rd", "テストユーザー１", null);
            // テスト対象のメソッド実行
            var result = await _testClass.Register(newUser);
            // 期待値を作成
            var expected = new OpenIdUser(ObjectId, UserId, SystemId, null, "テストユーザー１", null);

            // 結果をチェック
            result.IsStructuralEqual(expected);
        }

        [TestMethod]
        [ExpectedException(typeof(OpenIdUserOperationException))]
        public async Task Register_Error()
        {
            // レスポンスの設定
            _handler.SetResponse(HttpMethod.Post, _apiEndpoint, HttpStatusCode.BadRequest, Resources.OpenIdUserRepository.Responses.OpenIdUserErrorResponse);

            var newUser = new OpenIdUser(null, UserId, SystemId, "P@ssw0rd", "テストユーザー１", null);
            // テスト対象のメソッド実行
            var result = await _testClass.Register(newUser);
        }

        [TestMethod]
        public async Task Update()
        {
            // レスポンスの設定
            _handler.SetResponse(new HttpMethod("PATCH"), AddApiPath(ObjectId), HttpStatusCode.NoContent, String.Empty);

            var newUser = new OpenIdUser(null, UserId, SystemId, "P@ssw0rd", "テストユーザー１", null);
            // テスト対象のメソッド実行
            await _testClass.Update(new ObjectId(ObjectId), newUser);
        }
    }
}
