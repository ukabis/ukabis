using JP.DataHub.Com.Unity;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using System.Net;
using JP.DataHub.UnitTest.Com.Extensions;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class AuthorizationFilterTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        public void AuthorizationFilter_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var applicationApi = UnityCore.Resolve<IApplicationApi>();
            var authorizeUserApi = UnityCore.Resolve<IAuthorizeUserApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();
            var authorizeApi = UnityCore.Resolve<IAuthorizeApi>();
            var functionApi = UnityCore.Resolve<IFunctionApi>();
            
            const string testApplicationId1 = "37713EF6-B086-4419-AE21-AA4090DE4312";
            const string testAuthorizeUserId = "C0371873-0422-48C8-B385-2D88563EC052";
            const string testPrivateRoleId = "A2328B73-9DE0-4A9F-92E0-7D602856774E";
            const string testFunctionId1 = "8956161B-AE1D-4F8A-9B54-382CB7F4CB0C";
            const string testFunctionId2 = "0AB9EA4E-C169-433C-BE86-539E7F1DB2B9";
            const string testFunctionId3 = "FC617700-013B-4D0C-B5C6-B520E81468C6";
            const string testVendorIdDummy = "C59F1C22-92F8-42F2-A6AE-E6712215EB58";
            const string testSystemIdDummy = "24033A45-179D-495F-8219-1B51BA59C9F4";
            var openId = client.GetOpenId();

            // テストデータ登録
            var registerApplicationData = new ApplicationModel
            { 
                ApplicationId = testApplicationId1,
                ApplicationName = "テスト用Application",
                IsEnable = true,
                Manager = new [] { openId },
                VendorId = testVendorIdDummy,
                SystemId = testSystemIdDummy,
            };
            client.GetWebApiResponseResult(applicationApi.Register(registerApplicationData)).Assert(RegisterSuccessExpectStatusCode);
            
            var registerFunctionData = new List<FunctionModel>
            {
                new()
                {
                    FunctionId = testFunctionId1,
                    FunctionName = "テスト用Function",
                    ApplicationId = testApplicationId1,
                },
                new()
                {
                    FunctionId = testFunctionId2,
                    FunctionName = "テスト用Function",
                    ApplicationId = testApplicationId1,
                },
                new()
                {
                    FunctionId = testFunctionId3,
                    FunctionName = "テスト用Function",
                    ApplicationId = testApplicationId1,
                }
            };
            client.GetWebApiResponseResult(functionApi.RegisterList(registerFunctionData, testApplicationId1)).Assert(RegisterSuccessExpectStatusCode);

            var registerRoleData = new RoleModel()
            {
                PrivateRoleId = testPrivateRoleId,
                RoleName = "テスト用Role",
                ApplicationId = testApplicationId1,
                Functions = new[] { testFunctionId1 }
            };
            client.GetWebApiResponseResult(roleApi.Register(registerRoleData, testApplicationId1)).Assert(RegisterSuccessExpectStatusCode);

            var registerAuthorizeUserData = new AuthorizeUserModel()
            {
                AuthorizeUserId = testAuthorizeUserId,
                ApplicationId = testApplicationId1,
                OpenId = openId,
                PrivateRoleId = new[] { testPrivateRoleId },
                Functions = new[] { testFunctionId2 },
            };
            client.GetWebApiResponseResult(authorizeUserApi.Register(registerAuthorizeUserData, testApplicationId1)).Assert(RegisterSuccessExpectStatusCode);
            
            // テスト対象のFilterにアクセス
            var result = client.GetWebApiResponseResult(authorizeApi.IsAuthorize(testApplicationId1)).Assert(HttpStatusCode.OK);
            result.Result.ApplicationId.Is(testApplicationId1);
            result.Result.FunctionList.Any(x=>x == testFunctionId1).IsTrue();
            result.Result.FunctionList.Any(x=>x == testFunctionId2).IsTrue();
            result.Result.FunctionList.Any(x=>x == testFunctionId3).IsFalse();
        }

        [TestMethod]
        public void AuthorizationFilter_ErrorScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var applicationApi = UnityCore.Resolve<IApplicationApi>();
            var authorizeUserApi = UnityCore.Resolve<IAuthorizeUserApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();
            var authorizeApi = UnityCore.Resolve<IAuthorizeApi>();
            
            const string testApplicationId2 = "3DCB8C37-2045-4D97-B32D-1608BD4BC690";
            const string testApplicationId3 = "3C7DEFC4-E475-4624-9E2F-D00DC099AAA1";
            const string testAuthorizeUserId3 = "3F19A127-AFE9-45DB-88C5-6101775C58EF";
            const string testPrivateRoleId3 = "BF52570D-2991-439F-BF27-BA546F4D6C1D";
            const string testOpenIdDummy = "7CEC4DC8-CA26-463E-ABB2-E5F599204C96";
            const string testVendorIdDummy = "C59F1C22-92F8-42F2-A6AE-E6712215EB58";
            const string testSystemIdDummy = "24033A45-179D-495F-8219-1B51BA59C9F4";
            var openId = client.GetOpenId();
            
            // ApplicationIdなし
            client.GetWebApiResponseResult(authorizeApi.IsAuthorize("")).AssertErrorCode(HttpStatusCode.BadRequest, "E101400");
            
            // 未登録のApplicationId
            client.GetWebApiResponseResult(authorizeApi.IsAuthorize(Guid.NewGuid().ToString())).AssertErrorCode(HttpStatusCode.BadRequest, "E101401");

            // ApplicationがあってAuthorizeUserが未登録
            var registerApplicationData2 = new ApplicationModel
            { 
                ApplicationId = testApplicationId2,
                ApplicationName = "テスト用Application",
                IsEnable = true,
                Manager = new [] { testOpenIdDummy },
                VendorId = testVendorIdDummy,
                SystemId = testSystemIdDummy,
            };
            client.GetWebApiResponseResult(applicationApi.Register(registerApplicationData2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(authorizeApi.IsAuthorize(testApplicationId2)).AssertErrorCode(HttpStatusCode.BadRequest, "E101403");

            // AuthorizeUserにRoleは必須なのでRoleが未登録パターンは作れない

            // Functionだけないパターン
            var registerApplicationData3 = new ApplicationModel
            { 
                ApplicationId = testApplicationId3,
                ApplicationName = "テスト用Application",
                IsEnable = true,
                Manager = new [] { openId },
                VendorId = testVendorIdDummy,
                SystemId = testSystemIdDummy,
            };
            client.GetWebApiResponseResult(applicationApi.Register(registerApplicationData3)).Assert(RegisterSuccessExpectStatusCode);
            
            var registerRoleData4 = new RoleModel()
            { 
                PrivateRoleId = testPrivateRoleId3,
                RoleName = "テスト用Role",
                ApplicationId = testApplicationId3
            };
            client.GetWebApiResponseResult(roleApi.Register(registerRoleData4, testApplicationId3)).Assert(RegisterSuccessExpectStatusCode);
            
            var registerAuthorizeUserData3 = new AuthorizeUserModel()
            { 
                AuthorizeUserId = testAuthorizeUserId3,
                ApplicationId = testApplicationId3,
                OpenId = openId,
                PrivateRoleId = new []{testPrivateRoleId3} ,
            };
            client.GetWebApiResponseResult(authorizeUserApi.Register(registerAuthorizeUserData3, testApplicationId3)).Assert(RegisterSuccessExpectStatusCode);

            var result = client.GetWebApiResponseResult(authorizeApi.IsAuthorize(testApplicationId3)).Assert(HttpStatusCode.OK);
            result.Result.FunctionList.Length.Is(0);
            
        }
    }
}
