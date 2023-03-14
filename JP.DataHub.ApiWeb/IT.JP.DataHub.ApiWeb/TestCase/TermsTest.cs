using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class TermsTest : ApiWebItTestCase
    {

        private class TestData : TestDataBase
        {
            public static ResourceGroupModel TestResourceGroupModel(string resourceId,string url)
            {
                return new ResourceGroupModel() {
                    ResourceGroupName = "IT_RESOURCEGROPNAME",
                    TermsGroupCode="IT",
                    IsRequireConsent=false,
                    Resources = new List<ResourceGroupInResourceMode>() {
                        new ResourceGroupInResourceMode(){ ControllerId = resourceId ,ControllerUrl= url}
                    }
                };
            }

            public static TermsGroupModel TestTermsGroupModel()
            {
                return new TermsGroupModel()
                {
                    TermsGroupCode = "IT",
                    TermsGroupName = "IT_TERMASGROUP",
                    ResourceGroupTypeCode = "ctr"
                };
            }

            public static TermsModel TestTermsModel(string termsGroupCode)
            {
                return new TermsModel()
                {
                    VersionNo = "1",
                    FromDate = new DateTime(2022, 11, 11),
                    TermsText = "TESTTEXT",
                    TermsGroupCode = termsGroupCode
                };
            }

            public static CertifiedApplicationModel TestCertifiedApplicationModel(string vendorId,string systemId)
            {
                return new CertifiedApplicationModel()
                {
                    ApplicationName = "IT",
                    VendorId = vendorId,
                    SystemId = systemId
                };
            }
        }


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        // 正常系
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void Termas_NormalSenario(Repository repository)
        {
            var clientA = new IntegratedTestClient("termstest", "SmartFoodChainAdmin") { TargetRepository = repository };

            var termasApiOnApi = UnityCore.Resolve<ITermasApiOn>();
            var termasApiOffApi = UnityCore.Resolve<ITermasApiOff>();
            var staticTermsApi = UnityCore.Resolve<IStaticTermsApi>();
            var staticTermsGroupApi = UnityCore.Resolve<IStaticTermsGroupApi>();
            var staticUserTermsApi = UnityCore.Resolve<IStaticUserTermsApi>();
            var staticResourceGroupApi = UnityCore.Resolve<IStaticResourceGroupApi>();

            // 規約なしで呼べないこと
            // 同意状態を取得
            var resourceGroupList = clientA.GetWebApiResponseResult(staticResourceGroupApi.GetList()).Result;
            var resourceGroup = resourceGroupList.Where(x => x.IsRequireConsent).Where(x => x.Resources.Select(x => x.ControllerUrl).Contains(termasApiOnApi.ResourceUrl)).FirstOrDefault();
            var termsList = clientA.GetWebApiResponseResult(staticTermsApi.GetList()).Result;
            var terms = termsList.Where(x => x.TermsGroupCode == resourceGroup.TermsGroupCode).Where(x => x.FromDate <= DateTime.UtcNow).OrderBy(x => x.FromDate).FirstOrDefault();

            var userTerms = clientA.GetWebApiResponseResult(staticUserTermsApi.GetList()).Result;
            if (userTerms?.Any() == true)
            {
                var latestTerms = userTerms.OrderByDescending(x => x.AgreementDate).First();
                if (latestTerms.RevokeDate != null && latestTerms.AgreementDate > latestTerms.RevokeDate)
                {
                    // 同意している場合は取り下げる
                    clientA.GetWebApiResponseResult(staticTermsApi.Revoke(latestTerms.TermsId)).Assert(GetSuccessExpectStatusCode);
                }
            }

            // APIを実行し呼び出せないことを確認する
            clientA.GetWebApiResponseResult(termasApiOnApi.GetList()).AssertErrorCode(ForbiddenExpectStatusCode, "E50412");

            // 同意する
            clientA.GetWebApiResponseResult(staticTermsApi.Agreement(terms.TermsId)).Assert(GetSuccessExpectStatusCode);

            // APIを実行し呼び出せることを確認する
            clientA.GetWebApiResponseResult(termasApiOnApi.GetList()).Assert(GetExpectStatusCodes);

            // 同意を取り下げる
            clientA.GetWebApiResponseResult(staticTermsApi.Revoke(terms.TermsId)).Assert(GetSuccessExpectStatusCode);

            // APIを実行し呼び出せないことを確認する
            clientA.GetWebApiResponseResult(termasApiOnApi.GetList()).AssertErrorCode(ForbiddenExpectStatusCode, "E50412");

            // 規約不要API
            // 同意なしで呼び出せること
            clientA.GetWebApiResponseResult(termasApiOffApi.GetList()).Assert(GetExpectStatusCodes);
        }

        // StaticApi
        [TestMethod]
        public void Termas_StaticApi()
        {
            var client = new IntegratedTestClient("termstest", "SmartFoodChainAdmin");
            var termasApTestApi = UnityCore.Resolve<ITermasApiTest>();

            var staticTermsApi = UnityCore.Resolve<IStaticTermsApi>();
            var staticTermsGroupApi = UnityCore.Resolve<IStaticTermsGroupApi>();
            var staticUserTermsApi = UnityCore.Resolve<IStaticUserTermsApi>();
            var staticResourceGroupApi = UnityCore.Resolve<IStaticResourceGroupApi>();
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();

            var testTermsGroup = TestData.TestTermsGroupModel();

            var termasGroupApiResult = client.GetWebApiResponseResult(staticTermsGroupApi.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            // 既にあったら削除する
            if(termasGroupApiResult.Any(x => x.TermsGroupCode == testTermsGroup.TermsGroupCode))
            {
                client.GetWebApiResponseResult(staticTermsGroupApi.Delete(testTermsGroup.TermsGroupCode)).Assert(DeleteSuccessStatusCode);
            }
            // 規約グループ作成
            var termsGroupRegisterResult = client.GetWebApiResponseResult(staticTermsGroupApi.Register(testTermsGroup)).Assert(RegisterSuccessExpectStatusCode).Result;
            termsGroupRegisterResult.TermsGroupCode.Is(testTermsGroup.TermsGroupCode);

            // 規約グループ取得
            termasGroupApiResult = client.GetWebApiResponseResult(staticTermsGroupApi.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            var termsGroupGetResult = termasGroupApiResult.Where(x => x.TermsGroupCode == testTermsGroup.TermsGroupCode).First();
            termsGroupGetResult.IsStructuralEqual(testTermsGroup);
            // 規約グループ更新
            termsGroupGetResult.TermsGroupName = "NAMEUPDATE";
            client.GetWebApiResponseResult(staticTermsGroupApi.Register(termsGroupGetResult)).Assert(RegisterSuccessExpectStatusCode);
            // 規約グループ取得
            termasGroupApiResult = client.GetWebApiResponseResult(staticTermsGroupApi.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            var termsGroupGetResult2 = termasGroupApiResult.Where(x => x.TermsGroupCode == testTermsGroup.TermsGroupCode).First();
            termsGroupGetResult2.IsStructuralEqual(termsGroupGetResult);

            // リソース取得
            var testResurce = clientM.GetWebApiResponseResult(manageApi.GetApiResourceFromUrl(termasApTestApi.ResourceUrl, false)).Result;
            var testResourceGroup = TestData.TestResourceGroupModel(testResurce.ApiId, testResurce.RelativeUrl);

            // リソースグループ作成
            var registerResourceGroupResult = client.GetWebApiResponseResult(staticResourceGroupApi.Register(testResourceGroup)).Assert(RegisterSuccessExpectStatusCode).Result;
            // リソースグループ取得
            var resourceGroupResult = client.GetWebApiResponseResult(staticResourceGroupApi.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            var resourceGroup = resourceGroupResult.Where(x => x.ResourceGroupId == registerResourceGroupResult.ResourceGroupId).First();
            testResourceGroup.ResourceGroupId = resourceGroup.ResourceGroupId;
            resourceGroup.IsStructuralEqual(testResourceGroup);
            // リソースグループ更新
            testResourceGroup.ResourceGroupName = "NAMEUPDATE";
            testResourceGroup.Resources = new List<ResourceGroupInResourceMode>();
            registerResourceGroupResult = client.GetWebApiResponseResult(staticResourceGroupApi.Register(testResourceGroup)).Assert(RegisterSuccessExpectStatusCode).Result;

            // リソースグループ取得
            resourceGroupResult = client.GetWebApiResponseResult(staticResourceGroupApi.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            resourceGroup = resourceGroupResult.Where(x => x.ResourceGroupId == registerResourceGroupResult.ResourceGroupId).First();
            resourceGroup.IsStructuralEqual(testResourceGroup);

            // 規約作成
            var testTerms = TestData.TestTermsModel(termsGroupGetResult.TermsGroupCode);
            var registerTermsResult = client.GetWebApiResponseResult(staticTermsApi.Register(testTerms)).Assert(RegisterSuccessExpectStatusCode).Result;
            // 規約取得
            var termsResult = client.GetWebApiResponseResult(staticTermsApi.Get(registerTermsResult.TermsId)).Assert(GetSuccessExpectStatusCode).Result;
            testTerms.TermsId = termsResult.TermsId;
            termsResult.IsStructuralEqual(testTerms);
            // 規約更新
            testTerms.TermsText = "UPDATETEXT";
            testTerms.FromDate = new DateTime(2022,10,10);
            testTerms.VersionNo = "2";
            registerTermsResult = client.GetWebApiResponseResult(staticTermsApi.Register(testTerms)).Assert(RegisterSuccessExpectStatusCode).Result;

            // 規約取得
            var termsResult2 = client.GetWebApiResponseResult(staticTermsApi.Get(registerTermsResult.TermsId)).Assert(GetSuccessExpectStatusCode).Result;
            termsResult2.IsStructuralEqual(testTerms);
            // 規約削除
            client.GetWebApiResponseResult(staticTermsApi.Delete(registerTermsResult.TermsId)).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(staticTermsApi.Get(registerTermsResult.TermsId)).Assert(NotFoundStatusCode);

            // リソースグループ削除
            client.GetWebApiResponseResult(staticResourceGroupApi.Delete(resourceGroup.ResourceGroupId)).Assert(DeleteSuccessStatusCode);
            resourceGroupResult = client.GetWebApiResponseResult(staticResourceGroupApi.GetList()).Assert(GetExpectStatusCodes).Result;
            resourceGroupResult.Any(x => x.ResourceGroupId == registerResourceGroupResult.ResourceGroupId).IsFalse();

            // 規約グループ削除
            client.GetWebApiResponseResult(staticTermsGroupApi.Delete(testTermsGroup.TermsGroupCode)).Assert(DeleteSuccessStatusCode);
            termasGroupApiResult = client.GetWebApiResponseResult(staticTermsGroupApi.GetList()).Assert(GetExpectStatusCodes).Result;
            termasGroupApiResult.Any(x => x.TermsGroupCode == testTermsGroup.TermsGroupCode).IsFalse();
        }

        // StaticApi
        [TestMethod]
        public void Termas_StaticApi_Terms_Error()
        {
            var client = new IntegratedTestClient("termstest", "SmartFoodChainAdmin");
            var api = UnityCore.Resolve<IStaticTermsApi>();

            client.GetWebApiResponseResult(api.Get("hoge")).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.Get("")).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.Get(Guid.NewGuid().ToString())).AssertErrorCode(NotFoundStatusCode, "W60406");
            client.GetWebApiResponseResult(api.GetListByTermGroupCode("hoge")).AssertErrorCode(NotFoundStatusCode, "W60406");
            client.GetWebApiResponseResult(api.Delete(Guid.NewGuid().ToString())).AssertErrorCode(NotFoundStatusCode, "W60407");

            // 登録系の入力値チェック
            var testTerms = TestData.TestTermsModel(null);
            client.GetWebApiResponseResult(api.Register(testTerms)).Assert(BadRequestStatusCode);
            testTerms = TestData.TestTermsModel("hoge");
            client.GetWebApiResponseResult(api.Register(testTerms)).AssertErrorCode(NotFoundStatusCode, "W60408");
            testTerms = TestData.TestTermsModel("hoge");
            testTerms.FromDate = null;
            client.GetWebApiResponseResult(api.Register(testTerms)).Assert(BadRequestStatusCode);
            testTerms = TestData.TestTermsModel("hoge");
            testTerms.VersionNo = null;
            client.GetWebApiResponseResult(api.Register(testTerms)).Assert(BadRequestStatusCode);
            testTerms = TestData.TestTermsModel("hoge");
            testTerms.TermsText = null;
            client.GetWebApiResponseResult(api.Register(testTerms)).Assert(BadRequestStatusCode);

            // 更新系は管理者認証が必要
            api.AddHeaders.Add(HeaderConst.XAdmin, new string[] { "" });
            client.GetWebApiResponseResult(api.Register(TestData.TestTermsModel("hoge"))).Assert(ForbiddenExpectStatusCode);
            client.GetWebApiResponseResult(api.Delete(new Guid().ToString())).Assert(ForbiddenExpectStatusCode);

            // 参照系は管理者認証は不要
            var getResult = client.GetWebApiResponseResult(api.GetList()).Assert(GetExpectStatusCodes).Result;
            if (getResult?.Any() == true)
            {
                client.GetWebApiResponseResult(api.Get(getResult.First().TermsId)).Assert(GetSuccessExpectStatusCode);
                client.GetWebApiResponseResult(api.GetListByTermGroupCode(getResult.First().TermsGroupCode)).Assert(GetSuccessExpectStatusCode);
            }
        }
        [TestMethod]
        public void Termas_StaticApi_TermsGroup_Error()
        {
            var client = new IntegratedTestClient("termstest", "SmartFoodChainAdmin");
            var api = UnityCore.Resolve<IStaticTermsGroupApi>();

            // 登録系の入力値チェック
            var testData = TestData.TestTermsGroupModel();
            // TermsGroupCodeなし
            testData.TermsGroupCode = null;
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            // TermsGroupNameなし
            testData = TestData.TestTermsGroupModel();
            testData.TermsGroupName = null;
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            // ResourceGroupTypeCodeなし
            testData = TestData.TestTermsGroupModel();
            testData.ResourceGroupTypeCode = null;
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);

            // ResourceGroupTypeCode不正
            testData = TestData.TestTermsGroupModel();
            testData.ResourceGroupTypeCode = "xxx";
            client.GetWebApiResponseResult(api.Register(testData)).AssertErrorCode(NotFoundStatusCode, "W60405");

            testData.ResourceGroupTypeCode = "xxxxxxx";
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);

            client.GetWebApiResponseResult(api.Delete(Guid.NewGuid().ToString())).AssertErrorCode(NotFoundStatusCode, "W60404");

            // 更新系は管理者認証が必要
            testData = TestData.TestTermsGroupModel();
            api.AddHeaders.Add(HeaderConst.XAdmin, new string[] { "" });
            client.GetWebApiResponseResult(api.Register(testData)).Assert(ForbiddenExpectStatusCode);
            client.GetWebApiResponseResult(api.Delete(new Guid().ToString())).Assert(ForbiddenExpectStatusCode);

            // 参照系は管理者認証は不要
            client.GetWebApiResponseResult(api.GetList()).Assert(GetExpectStatusCodes);
        }

        [TestMethod]
        public void Termas_StaticApi_UserTerms_Error()
        {
            var client = new IntegratedTestClient("termstest", "SmartFoodChainAdmin");
            var api = UnityCore.Resolve<IStaticUserTermsApi>();

            client.GetWebApiResponseResult(api.Get(Guid.NewGuid().ToString())).AssertErrorCode(NotFoundStatusCode, "W60412");
            client.GetWebApiResponseResult(api.Get(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.Get("hogehoge")).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void Termas_StaticApi_CertifiedApplication()
        {
            var client = new IntegratedTestClient("termstest", "SmartFoodChainAdmin");
            var api= UnityCore.Resolve<IStaticCertifiedApplicationApi>();
            var testData = TestData.TestCertifiedApplicationModel(AppConfig.NormalVendorId.ToString(), AppConfig.NormalSystemId.ToString());

            // 登録
            var registerResult = client.GetWebApiResponseResult(api.Register(testData)).Assert(RegisterSuccessExpectStatusCode).Result;
            var getResult = client.GetWebApiResponseResult(api.Get(registerResult.CertifiedApplicationId)).Assert(GetSuccessExpectStatusCode).Result;
            testData.CertifiedApplicationId = registerResult.CertifiedApplicationId;
            getResult.IsStructuralEqual(testData);

            // 更新
            testData.ApplicationName = "NAMEUPDATE";
            testData.VendorId = AppConfig.AdminVendorId.ToString();
            testData.SystemId = AppConfig.AdminSystemId.ToString();
            registerResult = client.GetWebApiResponseResult(api.Register(testData)).Assert(RegisterSuccessExpectStatusCode).Result;
            var getListResult = client.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            getResult = getListResult.Where(x => x.CertifiedApplicationId == registerResult.CertifiedApplicationId).First();
            getResult.IsStructuralEqual(testData);

            // 削除
            client.GetWebApiResponseResult(api.Delete(testData.CertifiedApplicationId)).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(api.Get(registerResult.CertifiedApplicationId)).Assert(NotFoundStatusCode);
        }
        [TestMethod]
        public void Termas_StaticApi_CertifiedApplication_Error()
        {
            var client = new IntegratedTestClient("termstest", "SmartFoodChainAdmin");
            var api = UnityCore.Resolve<IStaticCertifiedApplicationApi>();
            var testData = TestData.TestCertifiedApplicationModel(AppConfig.NormalVendorId.ToString(), AppConfig.NormalSystemId.ToString());

            client.GetWebApiResponseResult(api.Get("hogehoge")).Assert(BadRequestStatusCode);
            testData.ApplicationName = null;
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.Register(TestData.TestCertifiedApplicationModel(null, AppConfig.NormalSystemId.ToString()))).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.Register(TestData.TestCertifiedApplicationModel(AppConfig.NormalVendorId.ToString(), null))).Assert(BadRequestStatusCode);
            // 存在しないVendorSystem
            client.GetWebApiResponseResult(api.Register(TestData.TestCertifiedApplicationModel(Guid.NewGuid().ToString(), AppConfig.NormalSystemId.ToString()))).AssertErrorCode(BadRequestStatusCode, "E60414");
            client.GetWebApiResponseResult(api.Register(TestData.TestCertifiedApplicationModel(AppConfig.NormalVendorId.ToString(), Guid.NewGuid().ToString()))).AssertErrorCode(BadRequestStatusCode, "E60414");

            // 管理者認証失敗
            api.AddHeaders.Add(HeaderConst.XAdmin, new string[] {""});
            client.GetWebApiResponseResult(api.GetList()).Assert(ForbiddenExpectStatusCode);
            client.GetWebApiResponseResult(api.Get(new Guid().ToString())).Assert(ForbiddenExpectStatusCode);
            client.GetWebApiResponseResult(api.Register(TestData.TestCertifiedApplicationModel(AppConfig.NormalVendorId.ToString(), AppConfig.NormalSystemId.ToString()))).Assert(ForbiddenExpectStatusCode);
            client.GetWebApiResponseResult(api.Delete(new Guid().ToString())).Assert(ForbiddenExpectStatusCode);
        }
    }
}
