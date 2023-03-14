using System;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ActionInjector;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ActionInjector
{
    [TestClass]
    public class UnitTest_CreateRegisterVersionActionInjector : UnitTestBase
    {
        private RegistAction CreateRegistDataAction() => ActionInjectorCommon.CreateRegistDataAction(UnityContainer);


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
        }

        [TestMethod]
        public void Execute_リポジトリ1つ()
        {
            var mock = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, CreateRegistDataAction(), 1);
            var mockRepository = mock.Item1;
            var mockResourceVersionRepository = mock.Item2;
            var action = mock.Item3;

            var target = UnityCore.Resolve<CreateRegisterVersionActionInjector>();
            target.Target = action;
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.Created);
            result.Content.ReadAsStringAsync().Result.Is(JToken.FromObject($"{{ \"RegisterVersion\" : 1 }}").ToString());

            mockResourceVersionRepository.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_リポジトリ1つ_結果null()
        {
            var mock = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, CreateRegistDataAction(), null);
            var mockRepository = mock.Item1;
            var mockResourceVersionRepository = mock.Item2;
            var action = mock.Item3;

            var target = UnityCore.Resolve<CreateRegisterVersionActionInjector>();
            target.Target = action;
            AssertEx.Catch<NotImplementedException>(() => target.Execute(() => { }));

            mockResourceVersionRepository.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_リポジトリ1つ_NotImplementedException()
        {
            var mock = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, CreateRegistDataAction(), 1, true);
            var mockRepository = mock.Item1;
            var mockResourceVersionRepository = mock.Item2;
            var action = mock.Item3;

            var target = UnityCore.Resolve<CreateRegisterVersionActionInjector>();
            target.Target = action;
            AssertEx.Catch<NotImplementedException>(() => target.Execute(() => { }));

            mockResourceVersionRepository.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_リポジトリ2つ()
        {
            var action = CreateRegistDataAction();
            var mock1 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 1);
            var mockRepository1 = mock1.Item1;
            var mockResourceVersionRepository1 = mock1.Item2;
            var mock2 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 2);
            var mockRepositor2 = mock2.Item1;
            var mockResourceVersionRepository2 = mock2.Item2;

            var target = UnityCore.Resolve<CreateRegisterVersionActionInjector>();
            target.Target = action;
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.Created);
            result.Content.ReadAsStringAsync().Result.Is(JToken.FromObject($"{{ \"RegisterVersion\" : 1 }}").ToString());

            mockResourceVersionRepository1.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
            mockResourceVersionRepository2.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_リポジトリ2つ_1つNotImplementedException()
        {
            var action = CreateRegistDataAction();
            var mock1 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 1, true);
            var mockRepository1 = mock1.Item1;
            var mockResourceVersionRepository1 = mock1.Item2;
            var mock2 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 2);
            var mockRepositor2 = mock2.Item1;
            var mockResourceVersionRepository2 = mock2.Item2;

            var target = UnityCore.Resolve<CreateRegisterVersionActionInjector>();
            target.Target = action;
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.Created);
            result.Content.ReadAsStringAsync().Result.Is(JToken.FromObject($"{{ \"RegisterVersion\" : 2 }}").ToString());

            mockResourceVersionRepository1.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
            mockResourceVersionRepository2.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Execute_リポジトリ2つ_2つNotImplementedException()
        {
            var action = CreateRegistDataAction();
            var mock1 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 1, true);
            var mockRepository1 = mock1.Item1;
            var mockResourceVersionRepository1 = mock1.Item2;
            var mock2 = ActionInjectorCommon.CreateRepositoryMock(UnityContainer, action, 2, true);
            var mockRepositor2 = mock2.Item1;
            var mockResourceVersionRepository2 = mock2.Item2;

            var target = UnityCore.Resolve<CreateRegisterVersionActionInjector>();
            target.Target = action;
            AssertEx.Catch<AggregateException>(() => target.Execute(() => { }));

            mockResourceVersionRepository1.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
            mockResourceVersionRepository2.Verify(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()), Times.Exactly(1));
        }
    }
}
