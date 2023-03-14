using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler
{

    [TestClass]
    public class UnitTest_SqlServerApiQueryCompiler : UnitTestBase
    {
        public TestContext TestContext { get; set; }


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance<IConfiguration>(Configuration);

            var dataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(dataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);
        }


        [TestMethod]
        public void Compile_QueryがNull()
        {
            var action = CreateQueryAction();
            action.ApiQuery = null;

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            actual.Item1.IsNull();
            actual.Item2.IsNull();

            action.ApiQuery = new ApiQuery("");

            actual = target.Compile(action);
            actual.Item1.IsNull();
            actual.Item2.IsNull();
        }

        [TestMethod]
        public void Compile_QueryTypeがOData()
        {
            var action = CreateQueryAction();
            action.ApiQuery = new ApiQuery(InvalidColQuery);
            action.QueryType = new QueryType(QueryTypes.ODataQuery);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            actual.Item1.IsNull();
            actual.Item2.IsNull();
        }


        [TestMethod]
        public void Compile_異常系_ResourceId指定のクエリ()
        {
            var action = CreateQueryAction();
            var query = @"
select * from {TABLE_NAME} a
inner join {TABLE_NAME:13620ea5-36b6-4f1b-bac6-7382782c9fd6} b
on a.id = b.id
";
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            var expect = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10431);

            actual.Item1.IsNull();
            actual.Item2.IsNotNull();
            actual.Item2.ErrorCode.Is(expect.ErrorCode);
            actual.Item2.Detail.Is(expect.Detail);
            actual.Item2.Title.Is(expect.Title);
        }


        const string InvalidColQuery = @"
select {TABLE_NAME},* from {TABLE_NAME} a
";

        const string InvalidColQueryBySubQuery = @"
select ,* from {TABLE_NAME} a
inner join (
    select {TABLE_NAME} from {TABLE_NAME}
) b
on a.id = b.id
;";

        const string InvalidColQueryWithSelfResourceName = @"
select Resource(Me),* from Resource(Me) a
";

        const string InvalidColQueryBySubQueryWithSelfResourceName = @"
select ,* from Resource(Me) a
inner join (
    select Resource(Me) from Resource(Me)
) b
on a.id = b.id
;";

        const string InvalidColQueryWithOtherResourceName = @"
select Resource(Me),* from Resource(/API/Other) a
";

        const string InvalidColQueryBySubQueryWithOtherResourceName = @"
select ,* from Resource(/API/Other) a
inner join (
    select Resource(/API/Other) from Resource(/API/Other)
) b
on a.id = b.id
;";

        const string InvalidColQueryWithTop = @"
select top {TABLE_NAME},* from {TABLE_NAME} a
";

        [TestMethod]
        [TestCase(InvalidColQuery)]
        [TestCase(InvalidColQueryBySubQuery)]
        [TestCase(InvalidColQueryWithSelfResourceName)]
        [TestCase(InvalidColQueryBySubQueryWithSelfResourceName)]
        [TestCase(InvalidColQueryWithOtherResourceName)]
        [TestCase(InvalidColQueryBySubQueryWithOtherResourceName)]
        [TestCase(InvalidColQueryWithTop)]
        public void Compile_異常系_Select句にTableName指定のクエリ()
        {
            TestContext.Run<string>((query) =>
            {
                var action = CreateQueryAction();
                action.ApiQuery = new ApiQuery(query);
                action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

                var target = new SqlServerApiQueryCompiler();
                var actual = target.Compile(action);
                var expect = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10434);

                actual.Item1.IsNull();
                actual.Item2.IsNotNull();
                actual.Item2.ErrorCode.Is(expect.ErrorCode);
                actual.Item2.Detail.Is(expect.Detail);
                actual.Item2.Title.Is(expect.Title);
            });
        }

        [TestMethod]
        public void Compile_正常系_置換対象なし()
        {
            var action = CreateQueryAction();
            var query = @"
select * from {TABLE_NAME} a
";
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            actual.Item1.IsNotNull();
            actual.Item1.Value.Is(query);
            actual.Item2.IsNull();
        }


        [TestMethod]
        public void Compile_正常系_SelfResource指定の置換()
        {
            var action = CreateQueryAction();
            var query = @"
select * from Resource(Me) a
";
            var expectQuery = @"
select * from {TABLE_NAME} a
";
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            actual.Item1.IsNotNull();
            actual.Item1.Value.Is(expectQuery);
            actual.Item2.IsNull();
        }


        [TestMethod]
        public void Compile_正常系_置換対象複数_すべて検索ヒット()
        {
            var query = @"
select * from {TABLE_NAME} a
inner join Resource(/API/Hoge/Fuga/Get/{key}) b
on a.id = b.id
inner join Resource(/API/Hoge/Piyo/Get?id={val}) c
on a.id = c.id
";

            var expectQuery = $@"
select * from {{TABLE_NAME}} a
inner join {{TABLE_NAME:{ResourceIdJoinToApi}}} b
on a.id = b.id
inner join {{TABLE_NAME:{ResourceIdJoinToApi}}} c
on a.id = c.id
";

            var action = CreateQueryAction();
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<GetQuery>())).Returns(CreateApi(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mockDynamicApiRepository.Object);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            actual.Item1.IsNotNull();
            actual.Item1.Value.Is(expectQuery);
            actual.Item2.IsNull();
        }

        [TestMethod]
        public void Compile_異常系_置換対象複数_一部存在しないAPI()
        {
            var query = @"
select * from {TABLE_NAME} a
inner join Resource(/API/Hoge/Fuga/Get/{key}) b
on a.id = b.id
inner join Resource(/API/NotExist/Piyo/Get?id={val}) c
on a.id = c.id
";

            var action = CreateQueryAction();
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<GetQuery>())).Returns(CreateApi(new HttpResponseMessage()));
            mockDynamicApiRepository.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(y => y.Value.Contains("NotExist")), It.IsAny<GetQuery>())).Returns((IMethod)null);
            UnityContainer.RegisterInstance(mockDynamicApiRepository.Object);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            var expect = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10432, "/API/NotExist/Piyo/Get?id={val}");
            actual.Item1.IsNull();
            actual.Item2.IsNotNull();
            actual.Item2.ErrorCode.Is(expect.ErrorCode);
            actual.Item2.Detail.Is(expect.Detail);
            actual.Item2.Instance.ToString().Is(expect.Instance.ToString());
            actual.Item2.Title.Is(expect.Title);
        }

        [TestMethod]
        public void Compile_異常系_置換対象複数_一部外部アクセス不可のAPI()
        {
            var query = @"
select * from {TABLE_NAME} a
inner join Resource(/API/Hoge/Fuga/Get/{key}) b
on a.id = b.id
inner join Resource(/API/Forbidden/Piyo/Get?id={val}) c
on a.id = c.id
";

            var action = CreateQueryAction();
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<GetQuery>())).Returns(CreateApi(new HttpResponseMessage()));
            mockDynamicApiRepository.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(y => y.Value.Contains("Forbidden")), It.IsAny<GetQuery>())).Returns(CreateApi(new HttpResponseMessage(), false));
            UnityContainer.RegisterInstance(mockDynamicApiRepository.Object);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            var expect = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10433, "/API/Forbidden/Piyo/Get?id={val}");
            actual.Item1.IsNull();
            actual.Item2.IsNotNull();
            actual.Item2.ErrorCode.Is(expect.ErrorCode);
            actual.Item2.Detail.Is(expect.Detail);
            actual.Item2.Title.Is(expect.Title);
        }

        [TestMethod]
        public void Compile_異常系_結合元が外部アクセス不可のAPI()
        {
            var query = @"
select * from {TABLE_NAME} a
inner join Resource(/API/Hoge/Fuga/Get/{key}) b
on a.id = b.id
";

            var action = CreateQueryAction();
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);
            action.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(false);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<GetQuery>())).Returns(CreateApi(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mockDynamicApiRepository.Object);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            var expect = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10433, apiPrivateQuerytestId);
            actual.Item1.IsNull();
            actual.Item2.IsNotNull();
            actual.Item2.ErrorCode.Is(expect.ErrorCode);
            actual.Item2.Detail.Is(expect.Detail);
            actual.Item2.Title.Is(expect.Title);
        }



        [TestMethod]
        public void Compile_異常系_置換対象複数_一部API認証エラーのAPI()
        {
            var query = @"
select * from {TABLE_NAME} a
inner join Resource(/API/Hoge/Fuga/Get/{key}) b
on a.id = b.id
inner join Resource(/API/ApiForbidden/Piyo/Get?id={val}) c
on a.id = c.id
";

            var action = CreateQueryAction();
            action.ApiQuery = new ApiQuery(query);
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);

            var expect = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E02404);
            var mockFinder = new Mock<IDynamicApiRepository>();
            mockFinder.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<GetQuery>())).Returns(CreateApi(new HttpResponseMessage()));
            mockFinder.Setup(x => x.FindApi(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(y => y.Value.Contains("ApiForbidden")), It.IsAny<GetQuery>())).Returns(CreateApi(ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E02404)));
            UnityContainer.RegisterInstance(mockFinder.Object);

            var target = new SqlServerApiQueryCompiler();
            var actual = target.Compile(action);
            actual.Item1.IsNull();
            actual.Item2.IsNotNull();
            actual.Item2.ErrorCode.Is(ErrorCodeMessage.Code.E02404.ToString());
            actual.Item2.Detail.Is(expect.Detail);
            actual.Item2.Title.Is(expect.Title);
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private Guid ProviderVendorId = Guid.NewGuid();
        private Guid ProviderSystemId = Guid.NewGuid();

        const string apiPrivateQuerytestId = "/API/Private/QueryTest/";
        private Guid ResourceIdJoinToApi = Guid.NewGuid();


        private IMethod CreateApi(HttpResponseMessage authResult, bool isSqlAccess = true)
        {
            var api = new Mock<IMethod>();
            api.SetupProperty(x => x.IsOtherResourceSqlAccess, new IsOtherResourceSqlAccess(isSqlAccess));
            api.SetupProperty(x => x.RelativeUri, new RelativeUri(apiPrivateQuerytestId));
            api.SetupProperty(x => x.ControllerId, new ControllerId(ResourceIdJoinToApi.ToString()));
            api.Setup(x => x.Authenticate()).Returns(authResult);
            return api.Object;
        }

        private QueryAction CreateQueryAction(bool isHistoryTest = false, bool isArrayTest = false)
        {
            //QueryAction action = DomainUnityContainer.Resolve<QueryAction>();
            var action = new QueryAction();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(ProviderSystemId.ToString());
            action.ProviderVendorId = new VendorId(ProviderVendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.RelativeUri = new RelativeUri(apiPrivateQuerytestId);
            action.ActionType = new ActionTypeVO(ActionType.Query);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.IsVendor = new IsVendor(false);
            action.IsPerson = new IsPerson(false);
            action.OpenId = new OpenId(Guid.Empty.ToString());
            action.ResourceSharingPersonRules = new List<ResourceSharingPersonRule> { };
            action.PostDataType = isArrayTest ? new PostDataType("array") : null;
            action.ApiQuery = new ApiQuery("");
            action.QueryType = new QueryType(QueryTypes.NativeDbQuery);
            action.Accept = new Accept("*/*");
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.IsUseBlobCache = new IsUseBlobCache(false);
            action.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(true);
            return action;
        }
    }
}
