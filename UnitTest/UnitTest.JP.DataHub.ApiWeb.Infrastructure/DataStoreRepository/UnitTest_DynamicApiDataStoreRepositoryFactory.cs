using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_DynamicApiDataStoreRepositoryFactory : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestMethod]
        public void Restore_リソースバージョン有効_バージョン管理対応リポジトリ()
        {
            var newDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            newDynamicApiDataStoreRepository.SetupGet(x => x.CanVersionControl).Returns(true);
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRepository>(RepositoryType.CosmosDB.ToCode(), newDynamicApiDataStoreRepository.Object);

            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            UnityContainer.RegisterInstance<IResourceVersionRepository>(bool.TrueString, mockResourceVersionRepository.Object);
            INewDynamicApiDataStoreRepository physicalResourceVersion = null;
            mockResourceVersionRepository.SetupSet(x => x.PhysicalRepository = It.IsAny<INewDynamicApiDataStoreRepository>())
                .Callback<INewDynamicApiDataStoreRepository>(x => { physicalResourceVersion = x; });
            mockResourceVersionRepository.SetupGet(x => x.PhysicalRepository).Returns(newDynamicApiDataStoreRepository.Object);

            var documentVersionRepository = new Mock<IDocumentVersionRepository>();
            UnityContainer.RegisterInstance<IDocumentVersionRepository>(documentVersionRepository.Object);
            INewDynamicApiDataStoreRepository physicalDocumentVersion = null;
            documentVersionRepository.SetupSet(x => x.PhysicalRepository = It.IsAny<INewDynamicApiDataStoreRepository>())
                .Callback<INewDynamicApiDataStoreRepository>(x => { physicalDocumentVersion = x; });
            documentVersionRepository.SetupGet(x => x.PhysicalRepository).Returns(newDynamicApiDataStoreRepository.Object);

            var dynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            IResourceVersionRepository resource = null;
            dynamicApiDataStoreRepository.SetupSet(x => x.ResourceVersionRepository = It.IsAny<IResourceVersionRepository>())
                .Callback<IResourceVersionRepository>(x => { resource = x; });
            dynamicApiDataStoreRepository.SetupGet(x => x.ResourceVersionRepository).Returns(mockResourceVersionRepository.Object);
            dynamicApiDataStoreRepository.SetupGet(x => x.CanVersionControl).Returns(true);
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRepository>(RepositoryType.CosmosDB.ToCode(), dynamicApiDataStoreRepository.Object);

            var repositoryInfo = new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "con1", false } });

            var result = new DynamicApiDataStoreRepositoryFactory().NewDataStoreRestore(RepositoryType.CosmosDB, repositoryInfo, new IsEnableResourceVersion(true));

            result.IsSameReferenceAs(dynamicApiDataStoreRepository.Object);
            result.ResourceVersionRepository.IsSameReferenceAs(mockResourceVersionRepository.Object);
            result.ResourceVersionRepository.PhysicalRepository.IsSameReferenceAs(newDynamicApiDataStoreRepository.Object);
            dynamicApiDataStoreRepository.VerifySet(x => x.RepositoryInfo = repositoryInfo);
            resource.Is(mockResourceVersionRepository.Object);
        }

        [TestMethod]
        public void Restore_リソースバージョン有効_バージョン管理非対応リポジトリ()
        {
            var newDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            newDynamicApiDataStoreRepository.SetupGet(x => x.CanVersionControl).Returns(false);
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRepository>(RepositoryType.CosmosDB.ToCode(), newDynamicApiDataStoreRepository.Object);

            var resourceVersionRepository = new Mock<IResourceVersionRepository>();
            UnityContainer.RegisterInstance<IResourceVersionRepository>(bool.FalseString, resourceVersionRepository.Object);
            INewDynamicApiDataStoreRepository physicalResourceVersion = null;
            resourceVersionRepository.SetupSet(x => x.PhysicalRepository = It.IsAny<INewDynamicApiDataStoreRepository>())
                .Callback<INewDynamicApiDataStoreRepository>(x => { physicalResourceVersion = x; });
            resourceVersionRepository.SetupGet(x => x.PhysicalRepository).Returns(newDynamicApiDataStoreRepository.Object);

            var documentVersionRepository = new Mock<IDocumentVersionRepository>();
            UnityContainer.RegisterInstance<IDocumentVersionRepository>(documentVersionRepository.Object);
            INewDynamicApiDataStoreRepository physicalDocumentVersion = null;
            documentVersionRepository.SetupSet(x => x.PhysicalRepository = It.IsAny<INewDynamicApiDataStoreRepository>())
                .Callback<INewDynamicApiDataStoreRepository>(x => { physicalDocumentVersion = x; });
            documentVersionRepository.SetupGet(x => x.PhysicalRepository).Returns(newDynamicApiDataStoreRepository.Object);

            var dynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            IResourceVersionRepository resource = null;
            dynamicApiDataStoreRepository.SetupSet(x => x.ResourceVersionRepository = It.IsAny<IResourceVersionRepository>())
                .Callback<IResourceVersionRepository>(x => { resource = x; });
            dynamicApiDataStoreRepository.SetupGet(x => x.ResourceVersionRepository).Returns(resourceVersionRepository.Object);
            dynamicApiDataStoreRepository.SetupGet(x => x.CanVersionControl).Returns(false);
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRepository>(RepositoryType.CosmosDB.ToCode(), dynamicApiDataStoreRepository.Object);

            var repositoryInfo = new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "con1", false } });

            var result = new DynamicApiDataStoreRepositoryFactory().NewDataStoreRestore(RepositoryType.CosmosDB, repositoryInfo, new IsEnableResourceVersion(true));

            result.IsSameReferenceAs(dynamicApiDataStoreRepository.Object);
            result.ResourceVersionRepository.IsSameReferenceAs(resourceVersionRepository.Object);
            result.ResourceVersionRepository.PhysicalRepository.IsSameReferenceAs(newDynamicApiDataStoreRepository.Object);
            dynamicApiDataStoreRepository.VerifySet(x => x.RepositoryInfo = repositoryInfo);
            resource.Is(resourceVersionRepository.Object);
        }

        [TestMethod]
        public void Restore_リソースバージョン無効()
        {
            var newDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRepository>(RepositoryType.CosmosDB.ToCode(), newDynamicApiDataStoreRepository.Object);

            var resourceVersionRepository = new Mock<IResourceVersionRepository>();
            UnityContainer.RegisterInstance<IResourceVersionRepository>(bool.FalseString, resourceVersionRepository.Object);
            INewDynamicApiDataStoreRepository physicalResourceVersion = null;
            resourceVersionRepository.SetupSet(x => x.PhysicalRepository = It.IsAny<INewDynamicApiDataStoreRepository>())
                .Callback<INewDynamicApiDataStoreRepository>(x => { physicalResourceVersion = x; });
            resourceVersionRepository.SetupGet(x => x.PhysicalRepository).Returns(newDynamicApiDataStoreRepository.Object);

            var documentVersionRepository = new Mock<IDocumentVersionRepository>();
            UnityContainer.RegisterInstance<IDocumentVersionRepository>(documentVersionRepository.Object);
            INewDynamicApiDataStoreRepository physicalDocumentVersion = null;
            documentVersionRepository.SetupSet(x => x.PhysicalRepository = It.IsAny<INewDynamicApiDataStoreRepository>())
                .Callback<INewDynamicApiDataStoreRepository>(x => { physicalDocumentVersion = x; });
            documentVersionRepository.SetupGet(x => x.PhysicalRepository).Returns(newDynamicApiDataStoreRepository.Object);

            var dynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            IResourceVersionRepository resource = null;
            dynamicApiDataStoreRepository.SetupSet(x => x.ResourceVersionRepository = It.IsAny<IResourceVersionRepository>())
                .Callback<IResourceVersionRepository>(x => { resource = x; });
            dynamicApiDataStoreRepository.SetupGet(x => x.ResourceVersionRepository).Returns(resourceVersionRepository.Object);
            dynamicApiDataStoreRepository.SetupGet(x => x.CanVersionControl).Returns(true);
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRepository>(RepositoryType.CosmosDB.ToCode(), dynamicApiDataStoreRepository.Object);

            var repositoryInfo = new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "con1", false } });

            var result = new DynamicApiDataStoreRepositoryFactory().NewDataStoreRestore(RepositoryType.CosmosDB, repositoryInfo, new IsEnableResourceVersion(false));

            result.IsSameReferenceAs(dynamicApiDataStoreRepository.Object);
            result.ResourceVersionRepository.IsSameReferenceAs(resourceVersionRepository.Object);
            result.ResourceVersionRepository.PhysicalRepository.IsSameReferenceAs(newDynamicApiDataStoreRepository.Object);
            dynamicApiDataStoreRepository.VerifySet(x => x.RepositoryInfo = repositoryInfo);
            resource.Is(resourceVersionRepository.Object);
        }
    }
}