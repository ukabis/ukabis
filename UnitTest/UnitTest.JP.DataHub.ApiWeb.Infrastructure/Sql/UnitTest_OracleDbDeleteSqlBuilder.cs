using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using Unity.Injection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection;
using Unity.Resolution;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    [TestClass]
    public class UnitTest_OracleDbDeleteSqlBuilder : UnitTestBase
    {
        private string TableName = Guid.NewGuid().ToString();


        [TestInitialize]
        public override void TestInitialize()
        {
            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterType<IDeleteSqlBuilder, OracleDbDeleteSqlBuilder>(RepositoryType.OracleDb.ToCode(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<DeleteParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null)));
        }

        [TestMethod]
        public void BuildUp()
        {
            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($"{{ 'id': '{id}', 'key': 'value' }}");

            var repositoryName = RepositoryType.OracleDb.ToCode();
            var deleteParam = ValueObjectUtil.Create<DeleteParam>(json);
            var repositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(TableName);

            var builder = UnityContainer.Resolve<IDeleteSqlBuilder>(
                repositoryName,
                new ParameterOverride("deleteParam", new InjectionParameter<DeleteParam>(deleteParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

            builder.BuildUp();
            builder.Sql.Is($"DELETE FROM \"{TableName}\" WHERE \"id\" = :p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { ":p_1", id }
            });
        }
    }
}
