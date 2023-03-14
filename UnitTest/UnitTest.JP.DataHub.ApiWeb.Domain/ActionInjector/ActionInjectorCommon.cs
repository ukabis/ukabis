using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal static class ActionInjectorCommon
    {
        private static Guid ApiId = Guid.NewGuid();
        private static Guid ControllerId = Guid.NewGuid();
        private static Guid VendorId = Guid.NewGuid();
        private static Guid SystemId = Guid.NewGuid();

        public static RegistAction CreateRegistDataAction(IUnityContainer unity)
        {
            RegistAction action = unity.Resolve<RegistAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.ActionType = new ActionTypeVO(ActionType.Regist);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);

            action.RequestSchema = new DataSchema(null);
            return action;
        }

        public static Tuple<Mock<INewDynamicApiDataStoreRepository>, Mock<IResourceVersionRepository>, RegistAction, Guid> CreateRepositoryMock(IUnityContainer unity, RegistAction action, int? version, bool isThrow = false, string versionInfo = null)
        {
            var guid = new Guid();
            if (unity != null)
            {
                var perRequestDataContainer = new PerRequestDataContainer();
                perRequestDataContainer.UserId = guid;
                unity.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            }

            ResourceVersion versionResult = null;
            if (version != null)
            {
                versionResult = new ResourceVersion(version.Value);
            }
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            if (isThrow == true)
            {
                mockResourceVersionRepository.Setup(x => x.GetVersionInfo(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                    })
                    .Returns(versionInfo);
                mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                    })
                    .Throws(new NotImplementedException());
                mockResourceVersionRepository.Setup(x => x.RefreshVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                    })
                    .Throws(new NotImplementedException());
                mockResourceVersionRepository.Setup(x => x.AddNewVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                    })
                    .Throws(new NotImplementedException());
                mockResourceVersionRepository.Setup(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                        if (unity != null)
                        {
                            unity.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(guid);
                        }
                    })
                    .Throws(new NotImplementedException());
                mockResourceVersionRepository.Setup(x => x.CompleteRegisterVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                        if (unity != null)
                        {
                            unity.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(guid);
                        }
                    })
                    .Throws(new NotImplementedException());
            }
            else
            {
                mockResourceVersionRepository.Setup(x => x.GetVersionInfo(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                    })
                    .Returns(versionInfo);
                mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                    })
                    .Returns(versionResult);
                mockResourceVersionRepository.Setup(x => x.RefreshVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                    });
                mockResourceVersionRepository.Setup(x => x.AddNewVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                        if (unity != null)
                        {
                            unity.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(guid);
                        }
                    })
                    .Returns(versionResult);
                mockResourceVersionRepository.Setup(x => x.CreateRegisterVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                        if (unity != null)
                        {
                            unity.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(guid);
                        }
                    })
                    .Returns(versionResult);
                mockResourceVersionRepository.Setup(x => x.CompleteRegisterVersion(It.IsAny<RepositoryKey>()))
                    .Callback<RepositoryKey>((repositoryKey) =>
                    {
                        repositoryKey.IsSameReferenceAs(action.RepositoryKey);
                        if (unity != null)
                        {
                            unity.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(guid);
                        }
                    })
                    .Returns(versionResult);
            }
            mockRepository.SetupProperty(x => x.ResourceVersionRepository, mockResourceVersionRepository.Object);

            if (action.DynamicApiDataStoreRepository == null)
            {
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            }
            else
            {
                var list = action.DynamicApiDataStoreRepository.ToList();
                list.Add(mockRepository.Object);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(list);
            }

            return Tuple.Create(mockRepository, mockResourceVersionRepository, action, guid);
        }
    }
}
