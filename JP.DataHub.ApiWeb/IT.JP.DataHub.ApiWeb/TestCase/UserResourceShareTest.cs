using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class UserResourceShareTest : ApiWebItTestCase
    {

        #region TestData

        private class TestData : TestDataBase
        {
            public static void  Assrt(List<AreaUnitModel> a, List<AreaUnitModel> b)
            {
                var sa =a.OrderBy(x => x.AreaUnitCode).ToList();
                var sb =b.OrderBy(x => x.AreaUnitCode).ToList();
                sa.IsStructuralEqual(sb);
            }
            public List<AreaUnitModel> DataUserA = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AAA1",
                    AreaUnitName = "UserA",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BBB1",
                    AreaUnitName = "UserA",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CCC1",
                    AreaUnitName = "UserA",
                    ConversionSquareMeters = 3
                }
            };
            public List<AreaUnitModel> DataUserB = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AAA2",
                    AreaUnitName = "UserB",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BBB2",
                    AreaUnitName = "UserB",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CCC2",
                    AreaUnitName = "UserB",
                    ConversionSquareMeters = 3
                }
            };
            public List<AreaUnitModel> DataUserC = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AAA3",
                    AreaUnitName = "UserC",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BBB3",
                    AreaUnitName = "UserC",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CCC3",
                    AreaUnitName = "UserC",
                    ConversionSquareMeters = 3
                }
            };
            public List<AreaUnitModel> DataExpectUserA = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "AAA1",
                    AreaUnitName = "UserA",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BBB1",
                    AreaUnitName = "UserA",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CCC1",
                    AreaUnitName = "UserA",
                    ConversionSquareMeters = 3
                }
            };
            public List<AreaUnitModel> DataExpectUserB = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "AAA2",
                    AreaUnitName = "UserB",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BBB2",
                    AreaUnitName = "UserB",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CCC2",
                    AreaUnitName = "UserB",
                    ConversionSquareMeters = 3
                }
            };
            public List<AreaUnitModel> DataExpectUserC = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "AAA3",
                    AreaUnitName = "UserC",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BBB3",
                    AreaUnitName = "UserC",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CCC3",
                    AreaUnitName = "UserC",
                    ConversionSquareMeters = 3
                }
            };

            public AreaUnitModel RegTestData = new AreaUnitModel() { };
            public TestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }

            public static UserResourceShareModel TestUserResourceShareModel()
            {
                return new UserResourceShareModel()
                {
                    UserShareTypeCode = "nsd",
                    ResourceGroupId = Guid.NewGuid().ToString(),
                    UserGroupId = Guid.NewGuid().ToString()
                };
            }
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
        [DataRow(Repository.SqlServer)]
        public void UserResourceShare_NormalSenario(Repository repository)
        {
            var clientA = new IntegratedTestClient("userResourceShareA", "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("userResourceShareB", "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientC = new IntegratedTestClient("userResourceShareC", "SmartFoodChainAdmin") { TargetRepository = repository };
            var openIdA = clientA.GetOpenId();
            var openIdB = clientB.GetOpenId();
            var openIdC = clientC.GetOpenId();

            var api = UnityCore.Resolve<IUserResourceShareApi>();
            var staticResourceGroupApi = UnityCore.Resolve<IStaticResourceGroupApi>();
            var staticUserResourceDefineApi = UnityCore.Resolve<IStaticUserResourceDefineApi>();
            var staticUserGroupApi = UnityCore.Resolve<IStaticUserGroupApi>();

            var testData = new TestData(repository, api.ResourceUrl);

            // データ初期化
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientC.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録する
            clientA.GetWebApiResponseResult(api.RegisterList(testData.DataUserA)).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegisterList(testData.DataUserB)).Assert(RegisterSuccessExpectStatusCode);
            clientC.GetWebApiResponseResult(api.RegisterList(testData.DataUserC)).Assert(RegisterSuccessExpectStatusCode);

            // X-UserResourceSharingなしでは自分のデータのみ取得できること
            var result = clientA.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserA));
            result = clientB.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserB));
            result = clientC.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserC));

            // ルールの設定をする
            // Bは制限なしの共有設定
            // CはAにのみ共有設定
            var resourceGroupList = clientA.GetWebApiResponseResult(staticResourceGroupApi.GetList()).Result;
            var resourceGroup = resourceGroupList.Where(x => x.IsRequireConsent).Where(x => x.Resources.Select(x => x.ControllerUrl).Contains(api.ResourceUrl)).FirstOrDefault();

            // AはB,Cに対して共有設定
            //UserResourceShare登録
            UpdateUserGroup(clientA, openIdA, resourceGroup.ResourceGroupId, "stg", new List<string>() { openIdB, openIdC });
            // Bは制限なしの共有設定
            UpdateUserGroup(clientB, openIdB, resourceGroup.ResourceGroupId, "uls", new List<string>());
            // CはAにのみ共有設定
            UpdateUserGroup(clientC, openIdC, resourceGroup.ResourceGroupId, "stg", new List<string>() { openIdA });

            // X-UserResourceSharing=ALLで自身に共有されているデータが全て取得できること
            ChangeXUserResourceSharing(api,null,true);
            result = clientA.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserA,testData.DataExpectUserB, testData.DataExpectUserC));
            result = clientB.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserB, testData.DataExpectUserA));
            result = clientC.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserA,testData.DataExpectUserB, testData.DataExpectUserC));

            // X-UserResourceSharingを指定してデータの取得が行えること
            // AがBのデータを取得
            ChangeXUserResourceSharing(api, new string[] {openIdB});
            result = clientA.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserB));
            // BがCのデータを指定。公開設定されてないのでエラー
            ChangeXUserResourceSharing(api, new string[] { openIdC });
            clientB.GetWebApiResponseResult(api.GetList()).Assert(ForbiddenExpectStatusCode);
            // CがAとBのデータを取得
            ChangeXUserResourceSharing(api, new string[] { openIdA, openIdB });
            result = clientC.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserA,testData.DataExpectUserB));

            // ODataで_OwnerIdを指定して想定以外のものが取れないことを確認
            ChangeXUserResourceSharing(api, null, true);
            clientB.GetWebApiResponseResult(api.OData($"$filter=_Owner_Id eq '{openIdC}'")).Assert(NotFoundStatusCode);
            // ODataで取得できることを確認
            result = clientA.GetWebApiResponseResult(api.OData($"$filter=_Owner_Id eq '{openIdC}'")).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserC));

            // Query設定をしたAPIの呼び出しが行えること
            result = clientA.GetWebApiResponseResult(api.Query()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserA, testData.DataExpectUserB,testData.DataExpectUserC));

            // 設定を変更する　AはBにのみ共有する
            UpdateUserGroup(clientA, openIdA, resourceGroup.ResourceGroupId, "stg", new List<string>() { openIdB });
            ChangeXUserResourceSharing(api, null, true);
            result = clientC.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserB, testData.DataExpectUserC));

            // Cは無制限に共有する
            UpdateUserGroup(clientC, openIdC, resourceGroup.ResourceGroupId, "uls", new List<string>());
            result = clientB.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            TestData.Assrt(result, CreateExpectData(testData.DataExpectUserA, testData.DataExpectUserB, testData.DataExpectUserC));
        }

        // 異常系
        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void UserResourceShare_ErrorSenario(Repository repository)
        {
            var clientA = new IntegratedTestClient("userResourceShareA", "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("userResourceShareB", "SmartFoodChainAdmin") { TargetRepository = repository };
            var openIdB = clientB.GetOpenId();

            var api = UnityCore.Resolve<IUserResourceShareApi>();
            var testData = new TestData(repository, api.ResourceUrl);

            // 規定外のヘッダーを設定された場合はエラー
            api.AddHeaders.Add(HeaderConst.X_UserResourceSharing, new string[] { "hogehoge" });
            clientA.GetWebApiResponseResult(api.GetList()).Assert(BadRequestStatusCode);

            // クエリ以外を指定した場合はエラー
            ChangeXUserResourceSharing(api, new string[] { openIdB });
            clientA.GetWebApiResponseResult(api.DeleteAll()).AssertErrorCode(BadRequestStatusCode, "E50415");
            clientA.GetWebApiResponseResult(api.Register(testData.RegTestData)).AssertErrorCode(BadRequestStatusCode, "E50415");
            clientA.GetWebApiResponseResult(api.RegisterList(testData.DataUserA)).AssertErrorCode(BadRequestStatusCode, "E50415");
            clientA.GetWebApiResponseResult(api.Update("hoge",testData.RegTestData)).AssertErrorCode(BadRequestStatusCode, "E50415");

            // テーブル結合が許可されているAPIはエラー
            clientA.GetWebApiResponseResult(api.TableJoin()).AssertErrorCode(BadRequestStatusCode, "E50414");

            // 認可されていないOpenIDを指定した場合はエラー
            ChangeXUserResourceSharing(api, new string[] { Guid.NewGuid().ToString() });
            clientA.GetWebApiResponseResult(api.GetList()).AssertErrorCode(ForbiddenExpectStatusCode, "E50413");
        }
        [TestMethod]
        public void StaticApi_UserResourceDefine_Error()
        {
            var client = new IntegratedTestClient("userResourceShareA", "SmartFoodChainAdmin");
            var api = UnityCore.Resolve<IStaticUserResourceDefineApi>();
            var testData = TestData.TestUserResourceShareModel();
            testData.UserShareTypeCode = "xxxxxx";
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            testData.UserShareTypeCode = "xxx";
            client.GetWebApiResponseResult(api.Register(testData)).AssertErrorCode(BadRequestStatusCode,"E60420");

            testData = TestData.TestUserResourceShareModel();
            testData.ResourceGroupId = null;
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            testData.ResourceGroupId = "hoge";
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            testData.ResourceGroupId = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(api.Register(testData)).AssertErrorCode(BadRequestStatusCode, "E60420");

            testData = TestData.TestUserResourceShareModel();
            testData.UserGroupId = null;
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            testData.UserGroupId = "hoge";
            client.GetWebApiResponseResult(api.Register(testData)).Assert(BadRequestStatusCode);
            testData.UserGroupId = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(api.Register(testData)).AssertErrorCode(BadRequestStatusCode, "E60420");
        }

        [TestMethod]
        public void StaticApi_UserGroup_Error()
        {
            var client = new IntegratedTestClient("userResourceShareA", "SmartFoodChainAdmin");
            var api = UnityCore.Resolve<IStaticUserGroupApi>();

            client.GetWebApiResponseResult(api.Register(new UserGroupModel() { UserGroupName = "name", Members = null })).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.Register(new UserGroupModel() { UserGroupName = null, Members = new List<string>() })).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.Delete(Guid.NewGuid().ToString())).AssertErrorCode(NotFoundStatusCode, "W60417");

        }

        private void ChangeXUserResourceSharing(IUserResourceShareApi api,string[] openIds,bool isAll = false)
        {
            if(api.AddHeaders.Any(x=>x.Key == HeaderConst.X_UserResourceSharing))
            {
                api.AddHeaders.Remove(HeaderConst.X_UserResourceSharing);
            }
            if(isAll)
            {
                api.AddHeaders.Add(HeaderConst.X_UserResourceSharing, new string[] { "ALL" });
                return;
            }
            if(openIds != null)
            {
                var tmpOpenIds = new List<string>();
                openIds.ForEach(x => tmpOpenIds.Add($"'{x}'"));
                var headersOpenId = $"[{string.Join(", ", tmpOpenIds)}]";
                api.AddHeaders.Add(HeaderConst.X_UserResourceSharing, new string[] { headersOpenId });
            }
        }
        private List<AreaUnitModel> CreateExpectData(List<AreaUnitModel> areaUnitModels1, List<AreaUnitModel> areaUnitModels2 = null, List<AreaUnitModel> areaUnitModels3 = null)
        {
            var result = new List<AreaUnitModel>();
            result.AddRange(areaUnitModels1);
            if (areaUnitModels2 != null) result.AddRange(areaUnitModels2);
            if (areaUnitModels3 != null) result.AddRange(areaUnitModels3);
            return result;
        }
        private void UpdateUserGroup(IntegratedTestClient client,string openId,string resourceGroupId,string userResourceShareTypeCode,List<string> members)
        {
            var staticUserResourceDefineApi = UnityCore.Resolve<IStaticUserResourceDefineApi>();
            var staticUserGroupApi = UnityCore.Resolve<IStaticUserGroupApi>();

            var userResourceShare = client.GetWebApiResponseResult(staticUserResourceDefineApi.GetList()).Assert(GetExpectStatusCodes).Result.Where(x => x.ResourceGroupId == resourceGroupId).FirstOrDefault();
            if(userResourceShare == null)
            {
                var newUserGroup = new UserGroupModel() { UserGroupName = "name", Members = members };
                var userGroupCode = client.GetWebApiResponseResult(staticUserGroupApi.Register(newUserGroup)).Assert(RegisterSuccessExpectStatusCode).Result.UserGroupId;
                //var userGroup = client.GetWebApiResponseResult(staticUserGroupApi.GetList(openId)).Assert(GetSuccessExpectStatusCode).Result.Where(x => x. == userResourceShare.UserGroupId).FirstOrDefault();
                userResourceShare = new UserResourceShareModel() { OpenId = openId,UserGroupId = userGroupCode, UserShareTypeCode = userResourceShareTypeCode ,ResourceGroupId = resourceGroupId};
                client.GetWebApiResponseResult(staticUserResourceDefineApi.Register(userResourceShare)).Assert(RegisterSuccessExpectStatusCode);
            }
            else
            {
                if (userResourceShare.UserShareTypeCode != userResourceShareTypeCode)
                {
                    userResourceShare.UserShareTypeCode = userResourceShareTypeCode;
                    client.GetWebApiResponseResult(staticUserResourceDefineApi.Register(userResourceShare)).Assert(RegisterSuccessExpectStatusCode);
                }
                var userGroup = client.GetWebApiResponseResult(staticUserGroupApi.GetList()).Assert(GetSuccessExpectStatusCode).Result.Where(x=>x.UserGroupId == userResourceShare.UserGroupId).First();
                if (userGroup == null)
                {
                    // なかったら作成する
                    userGroup = new UserGroupModel() { UserGroupId = userResourceShare.UserGroupId, UserGroupName = "name", Members = members };
                    client.GetWebApiResponseResult(staticUserGroupApi.Register(userGroup)).Assert(RegisterSuccessExpectStatusCode);
                    userGroup = client.GetWebApiResponseResult(staticUserGroupApi.GetList()).Assert(GetSuccessExpectStatusCode).Result.Where(x => x.UserGroupId == userResourceShare.UserGroupId).FirstOrDefault();
                }
                else if (userGroup.Members.Except(members).Any() || members.Except(userGroup.Members).Any())
                {
                    userGroup.Members = members;
                    client.GetWebApiResponseResult(staticUserGroupApi.Register(userGroup)).Assert(RegisterSuccessExpectStatusCode);
                }
            }

        }
    }
}
