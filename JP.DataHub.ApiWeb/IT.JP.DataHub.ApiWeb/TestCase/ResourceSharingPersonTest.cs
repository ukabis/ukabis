using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ResourceSharingPersonTest : ApiWebItTestCase
    {
        #region TestData

        private class ResourceSharingPersonTestData : TestDataBase
        {
            public ResourceSharingModel Data1 = new ResourceSharingModel()
            {
                key1 = "key-1",
                kid = "hogehoge"
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~ResourceSharingPerson~{WILDCARD}"
            };
            public ResourceSharingModel Data1GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharingPerson~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-1",
                kid = "hogehoge"
            };

            public ResourceSharingModel Data2 = new ResourceSharingModel()
            {
                key1 = "key-2",
                kid = "hogehoge2"
            };
            public ResourceSharingModel Data2GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharingPerson~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-2",
                kid = "hogehoge2"
            };

            public ResourceSharingModel Data3 = new ResourceSharingModel()
            {
                key1 = "key-3",
                kid = "hogehoge3"
            };
            public ResourceSharingModel Data3GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharingPerson~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-3",
                kid = "hogehoge3"
            };

            public ResourceSharingModel Data4 = new ResourceSharingModel()
            {
                key1 = "key-4",
                kid = "hogehoge4"
            };
            public RegisterResponseModel Data4RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~ResourceSharingPerson~{WILDCARD}"
            };
            public ResourceSharingModel Data4GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharingPerson~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-4",
                kid = "hogehoge4"
            };

            public ResourceSharingModel Data5 = new ResourceSharingModel()
            {
                key1 = "key-5",
                kid = "hogehoge5"
            };
            public ResourceSharingModel Data5GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharingPerson~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-5",
                kid = "hogehoge5"
            };

            public ResourceSharingPersonTestData(Repository repository, string resouceUrl) : base(repository, resouceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        // 正常系
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResorceSharingPerson_ParsonDependApi_NormalSenario(Repository repository)
        {
            // A,Bを用意
            var clientA = new IntegratedTestClient("test1", "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChain2TestSystem") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingPersonApi>();
            var testData = new ResourceSharingPersonTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Aがデータを2件登録
            clientA.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // AがAの全取得（2レコード）
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data3GetExpected });

            // Bがデータを3件登録
            clientB.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // BがBの全取得（3レコード）
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });

            // "X-ResourceSharingPrivate指定して、他人のデータを取得できるか確認
            // To A From B(AがBのデータを取得できる3件)
            var other = clientB.GetOpenId();
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, other);
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });

            // ヘッダーを外すと自分のデータが取得できる
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingPerson);
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data3GetExpected });

            // ルールが登録されていないと自分のデータが返る（ヘッダーあり）
            other = clientA.GetOpenId();
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, other);
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });
        }

        // 登録・更新・削除が行えるか
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResorceSharingPerson_ParsonDependApi_RegistAndUpdateAndDeleteAction(Repository repository)
        {
            // A,Bを用意
            var clientA = new IntegratedTestClient("test1", "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChain2TestSystem") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingPersonApi>();
            var testData = new ResourceSharingPersonTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Aがデータを1件登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode);

            // AがAの全取得（1レコード）
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data2GetExpected });

            // AがBにデータを2件登録
            var other = clientB.GetOpenId();
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, other);
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            var regA = clientA.GetWebApiResponseResult(api.Regist(testData.Data4)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).Result;

            // BがBの全取得（2レコード）
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data4GetExpected });

            // ヘッダーがないとAはBのデータは更新できない
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingPerson);
            clientA.GetWebApiResponseResult(api.Update(regA.id, testData.Data5)).AssertErrorCode(UpdateErrorExpectStatusCode, "E10407");

            // AがBのデータを更新
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, other);
            clientA.GetWebApiResponseResult(api.Update(regA.id, testData.Data5)).Assert(UpdateSuccessExpectStatusCode);

            // BがBの全取得（2レコード）
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data5GetExpected });

            // AがBのデータを削除
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);

            // BがBのデータを全取得
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // ルールが登録されていないとBはAのデータを更新できない
            other = clientA.GetOpenId();
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, other);
            clientB.GetWebApiResponseResult(api.Update(regA.id, testData.Data3)).Assert(UpdateErrorExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResorceSharingPerson_ODataAction(Repository repository)
        {
            // A,Bを用意
            var clientA = new IntegratedTestClient("test1", "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChain2TestSystem") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingPersonApi>();
            var testData = new ResourceSharingPersonTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを1件登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // データを5件登録
            var data = new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            clientB.GetWebApiResponseResult(api.RegistList(data)).Assert(RegisterSuccessExpectStatusCode);

            // 自分のデータを取得(select) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$select=key1,kid,_Owner_Id,id")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(filter) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$filter=key1 eq 'key-1'")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(top) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$top=3")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(orderby) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$orderby=kid")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(count) ヘッダーなし
            var count = clientA.GetWebApiResponseResult(api.OData("$count=true")).Assert(GetSuccessExpectStatusCode).RawContentString;
            JArray.Parse(count)[0].Value<int>().Is(1);

            // AがBのデータを取得できる(5件, ヘッダーあり)
            var other = clientB.GetOpenId();
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, other);

            // AがBのデータを取得(select)
            clientA.GetWebApiResponseResult(api.OData("$select=key1,kid,_Owner_Id,id")).Assert(GetSuccessExpectStatusCode, 
                new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected, testData.Data4GetExpected, testData.Data5GetExpected });
            // AがBのデータを取得(filter)
            clientA.GetWebApiResponseResult(api.OData("$filter=key1 eq 'key-2'")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data2GetExpected });
            // AがBのデータを取得(top)
            clientA.GetWebApiResponseResult(api.OData("$top=3")).Assert(GetSuccessExpectStatusCode, 
                new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });
            // AがBのデータを取得(orderby)
            clientA.GetWebApiResponseResult(api.OData("$orderby=kid desc")).Assert(GetSuccessExpectStatusCode, 
                new List<ResourceSharingModel>() { testData.Data5GetExpected, testData.Data4GetExpected, testData.Data3GetExpected, testData.Data2GetExpected, testData.Data1GetExpected });
            // AがBのデータを取得(count)
            count = clientA.GetWebApiResponseResult(api.OData("$count=true")).Assert(GetSuccessExpectStatusCode).RawContentString;
            JArray.Parse(count)[0].Value<int>().Is(5);

        }

        // ヘッダー異常
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharingPerson_HeaderCheck(Repository repository)
        {
            // A,Bを用意
            var clientA = new IntegratedTestClient("test1", "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChain2TestSystem") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingPersonApi>();
            var testData = new ResourceSharingPersonTestData(repository, api.ResourceUrl);

            // 空文字
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, "");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(BadRequestStatusCode);

            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingPerson);

            // GUIDでない
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, "NON-GUID");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(BadRequestStatusCode);
        }
    }
}
