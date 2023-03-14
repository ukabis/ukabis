using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.ActionInjector;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ActionInjector
{
    [TestClass]
    public class UnitTest_GetVersionInfoInjector : UnitTestBase
    {
        private RegistAction CreateRegistDataAction() => ActionInjectorCommon.CreateRegistDataAction(UnityContainer);


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
        }

        [TestMethod]
        public void Execute_バージョン有()
        {
            var versionInfo = @"{""currentversion"":1,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}";
            var mock = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, CreateRegistDataAction(), 1, false, versionInfo);
            var mockResourceVersionRepository = mock.Item2;

            var target = UnityCore.Resolve<GetVersionInfoInjector>();
            target.Target = mock.Item3;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(versionInfo)
            };

            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(versionInfo);

            mockResourceVersionRepository.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(0));
        }


        [TestMethod]
        public void Execute_バージョン無_Refresh_リポジトリ1つ()
        {
            var versionInfo = @"{""currentversion"":0,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}";

            var mock = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, CreateRegistDataAction(), 1, false, versionInfo);
            var mockRepository = mock.Item1;
            var mockResourceVersionRepository = mock.Item2;
            var action = mock.Item3;

            var target = UnityCore.Resolve<GetVersionInfoInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(versionInfo) };
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(versionInfo);

            mockResourceVersionRepository.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_バージョン無_Refresh_リポジトリ1つ_NotImplementedException()
        {
            var versionInfo = @"{""currentversion"":0,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}";

            var mock = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, CreateRegistDataAction(), 1, true, versionInfo);
            var mockRepository = mock.Item1;
            var mockResourceVersionRepository = mock.Item2;
            var action = mock.Item3;

            var target = UnityCore.Resolve<GetVersionInfoInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(versionInfo) };
            target.Execute(() => { });

            mockResourceVersionRepository.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_バージョン無_Refresh_リポジトリ2つ()
        {
            var versionInfo = new List<string>
            {
                @"{""currentversion"":0,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}",
                @"{""currentversion"":1,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}"
            };

            var action = CreateRegistDataAction();
            var mock1 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 1, false, versionInfo[0]);
            var mockRepository1 = mock1.Item1;
            var mockResourceVersionRepository1 = mock1.Item2;
            var mock2 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 2, false, versionInfo[1]);
            var mockRepository2 = mock2.Item1;
            var mockResourceVersionRepository2 = mock2.Item2;

            var target = UnityCore.Resolve<GetVersionInfoInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(versionInfo[0]) };
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(versionInfo[0]);

            mockResourceVersionRepository1.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
            mockResourceVersionRepository2.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_バージョン無_Refresh_リポジトリ2つ_1つNotImplementedException()
        {
            var versionInfo = new List<string>
            {
                @"{""currentversion"":0,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}",
                @"{""currentversion"":1,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}"
            };

            var action = CreateRegistDataAction();
            var mock1 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 1, true, versionInfo[0]);
            var mockRepository1 = mock1.Item1;
            var mockResourceVersionRepository1 = mock1.Item2;
            var mock2 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 2, false, versionInfo[1]);
            var mockRepository2 = mock2.Item1;
            var mockResourceVersionRepository2 = mock2.Item2;

            var target = UnityCore.Resolve<GetVersionInfoInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(versionInfo[0]) };
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(versionInfo[0]);

            mockResourceVersionRepository1.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
            mockResourceVersionRepository2.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_バージョン無_Refresh_リポジトリ2つ_2つNotImplementedException()
        {
            var versionInfo = new List<string>
            {
                @"{""currentversion"":0,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}",
                @"{""currentversion"":1,""_Upduser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Upddate"":""2018-04-06T00:52:16.1449409Z"",""documentversions"":[{""version"":1,""_Reguser_Id"":""43bd9ccd-696b-4514-b295-766175b2921f"",""_Regdate"":""2018-04-05T03:53:00.8851393Z"",""is_current"":true}]}"
            };

            var respositoryInfo = new List<RepositoryInfo>
            {
                new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "con1", false } }),
                new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "con2", false } }),
            };

            var action = CreateRegistDataAction();
            var mock1 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 1, true, versionInfo[0]);
            var mockRepository1 = mock1.Item1;
            var mockResourceVersionRepository1 = mock1.Item2;
            var mock2 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 2, true, versionInfo[1]);
            var mockRepository2 = mock2.Item1;
            var mockResourceVersionRepository2 = mock2.Item2;

            var target = UnityCore.Resolve<GetVersionInfoInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(versionInfo[0])
            };
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(versionInfo[0]);

            mockResourceVersionRepository1.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
            mockResourceVersionRepository2.Verify(x => x.RefreshVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }
    }
}
