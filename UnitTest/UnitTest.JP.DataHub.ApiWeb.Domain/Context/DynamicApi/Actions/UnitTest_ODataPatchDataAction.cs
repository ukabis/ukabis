using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_ODataPatchDataAction : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private readonly List<string> _odataIgnoreList = new List<string>()
        {
            "$top",
            "$select",
            "$count",
            "$orderby"
        };
        private readonly string _openId = Guid.NewGuid().ToString();


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", false);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            UnityContainer.RegisterInstance<bool>("Return.JsonValidator.ErrorDetail", true);
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
        }


        [TestMethod]
        public void ExecuteAction_RDBMS_正常_NotFound()
        {
            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 0);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.NotFound);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("I10402");
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_正常_QueryStringなし()
        {
            var action = CreateBasicAction();
            action.Query = null;

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_正常_XNoOptimistic()
        {
            var action = CreateBasicAction();
            action.IsOptimisticConcurrency = new IsOptimisticConcurrency(true);
            action.XNoOptimistic = new XNoOptimistic(true);

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
        }

        [TestMethod]
        [TestCase(true)]
        [TestCase(false)]
        public void ExecuteAction_リポジトリ2件_RDBMS_OTHER_正常()
        {
            TestContext.Run((bool isRdbmsPrimary) =>
            {

                var action = CreateBasicAction();

                var mockRdbmsRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
                var mockNonRdbmsRepository = CreateNonRdbmsDataStoreRepositoryMock(ODataPatchSupport.None);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    isRdbmsPrimary ? mockRdbmsRepository.Object : mockNonRdbmsRepository.Object,
                    isRdbmsPrimary ? mockNonRdbmsRepository.Object : mockRdbmsRepository.Object
                });

                var result = action.ExecuteAction();
                result.StatusCode.Is(HttpStatusCode.NoContent);
            });
        }

        [TestMethod]
        [TestCase(1, 0, HttpStatusCode.NoContent)]
        [TestCase(0, 1, HttpStatusCode.NotFound)]
        public void ExecuteAction_リポジトリ2件_RDBMS_RDBMS_正常()
        {
            TestContext.Run((int count1, int count2, HttpStatusCode expectedStatus) =>
            {
                var action = CreateBasicAction();

                var mockRdbms1Repository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), count1);
                var mockRdbms2Repository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), count2);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockRdbms1Repository.Object,
                    mockRdbms2Repository.Object
                });

                var result = action.ExecuteAction();
                result.StatusCode.Is(expectedStatus);

                mockRdbms1Repository.Verify(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()), Times.Exactly(1));
                mockRdbms2Repository.Verify(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()), Times.Exactly(1));
            });
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2件_RDBMS_RDBMS_正常_セカンダリ例外()
        {
            var action = CreateBasicAction();

            var mockRdbms1Repository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            var mockRdbms2Repository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), new Exception(Guid.NewGuid().ToString()));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRdbms1Repository.Object,
                mockRdbms2Repository.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);

            mockRdbms1Repository.Verify(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()), Times.Exactly(1));
            mockRdbms2Repository.Verify(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2件_RDBMS_RDBMS_異常_プライマリ例外()
        {
            var action = CreateBasicAction();

            var error = new Exception(Guid.NewGuid().ToString());
            var mockRdbms1Repository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), error);
            var mockRdbms2Repository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRdbms1Repository.Object,
                mockRdbms2Repository.Object
            });

            var ex = AssertEx.Throws<Exception>(() => action.ExecuteAction());
            ex.IsStructuralEqual(error);

            mockRdbms1Repository.Verify(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()), Times.Exactly(1));
            mockRdbms2Repository.Verify(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_非RDBMS_異常_NotImplemented()
        {
            var action = CreateBasicAction();

            var mockRepository = CreateNonRdbmsDataStoreRepositoryMock(ODataPatchSupport.SequentialUpdate);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object,
            });

            var ex = AssertEx.Throws<NotImplementedException>(() => action.ExecuteAction());

            mockRepository.Verify(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()), Times.Exactly(0));
        }


        [TestMethod]
        public void ExecuteAction_共通_異常_MethodType()
        {
            var action = CreateBasicAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10419");
        }

        [TestMethod]
        public void ExecuteAction_共通_異常_RepositoryKey()
        {
            var action = CreateBasicAction();
            action.RepositoryKey = null;

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10420");
        }

        [TestMethod]
        public void ExecuteAction_共通_異常_非対応リポジトリのみ()
        {
            var action = CreateBasicAction();

            var mockRepository = CreateNonRdbmsDataStoreRepositoryMock(ODataPatchSupport.None);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10435");
        }

        [TestMethod]
        [TestCase("hoge", "E10438")]
        [TestCase("", "E10438")]
        [TestCase("[]", "E10438")]
        [TestCase("{}", "E10438")]
        [TestCase("{ '_Where': {} }", "E10438")]
        [TestCase("{ 'id': 'hoge' }", "E10439")]
        [TestCase("{ '_Vendor_Id': 'hoge' }", "E10439")]
        [TestCase("{ 'hoge': 'hoge' }", "E10439")]
        public void ExecuteAction_共通_異常_PatchData不正_形式と項目()
        {
            TestContext.Run((string contents, string expectedErrorCode) =>
            {
                var action = CreateBasicAction();

                var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockRepository.Object
                });
                action.Contents = new Contents(contents);

                var response = action.ExecuteAction();
                response.StatusCode.Is(HttpStatusCode.BadRequest);
                var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
                result["error_code"].Value<string>().Is(expectedErrorCode);
            });
        }

        [TestMethod]
        [TestCase("{ 'STR_VALUE': null }", "E10402")]   // null制約
        [TestCase("{ 'INT_VALUE': 'hoge' }", "E10402")] // 型制約(int)
        [TestCase("{ 'BOL_VALUE': 'hoge' }", "E10402")] // 型制約(bool)
        [TestCase("{ 'ARY_VALUE': 'hoge' }", "E10402")] // 型制約(Array)
        [TestCase("{ 'OBJ_VALUE': 'hoge' }", "E10402")] // 型制約(Object)
        [TestCase("{ 'DAT_VALUE': 'hoge' }", "E10402")] // フォーマット制約
        [TestCase("{ 'STR_ENUM':  'hoge' }", "E10402")]  // Enum制約
        [TestCase("{ 'ARY_VALUE': [ 1, 2, 3, 'hoge' ] }", "E10402")]  // 配列要素の制約
        [TestCase("{ 'OBJ_VALUE': { 'Value1': null } }", "E10402")]   // 子オブジェクトの制約
        [TestCase("{ 'OBJ_VALUE': { 'Value3': 'hoge' } }", "E10402")] // 子オブジェクトのadditionalProperties
        [TestCase("{ 'OBJ_REF': { 'Value1': null } }", "E10402")]     // 子オブジェクト($ref)の制約
        [TestCase("{ 'OBJ_REF': { 'Value3': 'hoge' } }", "E10402")]   // 子オブジェクト($ref)のadditionalProperties
        public void ExecuteAction_共通_異常_PatchData不正_スキーマ違反()
        {
            TestContext.Run((string contents, string expectedErrorCode) =>
            {
                var action = CreateBasicAction();
                action.ControllerSchema = new DataSchema($@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE': {{ 'type': 'string' }},
        'STR_ENUM':  {{ 'type': 'string', 'enum': [ 'dog', 'cat', 'monkey' ] }},
        'INT_VALUE': {{ 'type': 'number' }},
        'OBJ_VALUE': {{ 
            'type': 'object',
            'properties': {{
                'Value1': {{ 'type': 'string' }},
                'Value2': {{ 'type': 'string' }},
            }},
            'required': [ 'Value' ],
            'additionalProperties': false
        }},
        'OBJ_REF':   {{ '$ref': '#/definitions/CHILD' }},
        'ARY_VALUE': {{ 'type': 'array', 'items': {{ 'type': 'number' }} }},
        'BOL_VALUE': {{ 'type': 'boolean' }},
        'DAT_VALUE': {{ 'type': 'string', 'format': 'date-time' }},
    }},
    'additionalProperties': false,
    'definitions': {{
        'CHILD': {{ 
            'properties': {{
                'Value1': {{ 'type': 'string' }},
                'Value2': {{ 'type': 'string' }}
            }},
            'required': [ 'Value' ],
            'additionalProperties': false
        }}
    }}
}}");

                var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockRepository.Object
                });
                action.Contents = new Contents(contents);

                var response = action.ExecuteAction();
                response.StatusCode.Is(HttpStatusCode.BadRequest);
                var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
                result["error_code"].Value<string>().Is(expectedErrorCode);
            });
        }

        [TestMethod]
        public void ExecuteAction_共通_異常_楽観排他ON()
        {
            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.IsOptimisticConcurrency = new IsOptimisticConcurrency(true);

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10436");
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_異常_履歴ON()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);

            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.IsDocumentHistory = new IsDocumentHistory(true);

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10437");
            result["errors"].Count().Is(1);
            result["errors"][""].Count().Is(1);
            result["errors"][""][0].Value<string>().Is("DocumentHistory is not supported ODataPatch.");
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_異常_メールテンプレートON()
        {
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", true);

            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.HasMailTemplate = new HasMailTemplate(true);
            action.HasWebhook = new HasWebhook(false);

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10437");
            result["errors"].Count().Is(1);
            result["errors"][""].Count().Is(1);
            result["errors"][""][0].Value<string>().Is("MailTemplate is not supported ODataPatch.");
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_異常_WebHookON()
        {
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", true);

            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.HasMailTemplate = new HasMailTemplate(false);
            action.HasWebhook = new HasWebhook(true);

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10437");
            result["errors"].Count().Is(1);
            result["errors"][""].Count().Is(1);
            result["errors"][""][0].Value<string>().Is("Webhook is not supported ODataPatch.");
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_異常_BlockchainON()
        {
            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.IsEnableBlockchain = new IsEnableBlockchain(true);

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10437");
            result["errors"].Count().Is(1);
            result["errors"][""].Count().Is(1);
            result["errors"][""][0].Value<string>().Is("Blockchain is not supported ODataPatch.");
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_異常_Base64()
        {
            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(JsonConvert.SerializeObject(new { FileName = "Base64File.jpg", Image = "$Base64()" }));

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10437");
            result["errors"].Count().Is(1);
            result["errors"][""].Count().Is(1);
            result["errors"][""][0].Value<string>().Is("Base64 is not supported ODataPatch.");
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_異常_PatchData項目不正()
        {
            var action = CreateBasicAction();

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(JsonConvert.SerializeObject(new { Hoge = Guid.NewGuid().ToString(), Fuga = Guid.NewGuid().ToString() }));

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10439");
            result["errors"].Count().Is(2);
            result["errors"]["Hoge"].Count().Is(1);
            result["errors"]["Hoge"][0].Value<string>().Is("This Property has not been defined and the schema does not allow additional properties.");
            result["errors"]["Fuga"].Count().Is(1);
            result["errors"]["Fuga"][0].Value<string>().Is("This Property has not been defined and the schema does not allow additional properties.");
        }

        [TestMethod]
        [TestCase("{ 'STR_VALUE': 'hoge', '_Where': { 'ColumnName': 'STR_VALUE', 'Operator': 'in', 'Object': 12345 } }", "E10440", "_Where", null)]
        [TestCase("{ 'STR_VALUE': 'hoge', '_Where': { 'ColumnName': '_Vendor_Id', 'Operator': 'IN', 'Object': [ 'hoge', 'fuga' ] } }", "E10440", "_Where/ColumnName", "'_Vendor_Id' is not available for query.")]
        [TestCase("{ 'STR_VALUE': 'hoge', '_Where': { 'ColumnName': 'hoge', 'Operator': 'IN', 'Object': [ 'hoge', 'fuga'] } }", "E10440", "_Where/ColumnName", "'hoge' is not available for query.")]
        [TestCase("{ 'STR_VALUE': 'hoge', '_Where': { 'ColumnName': '_Upduser_Id', 'Operator': 'eq', 'Object': [ 'hoge', 'fuga'] } }", "E10440", "_Where/Operator", "Undefined Operator 'eq'.")]
        [TestCase("{ 'STR_VALUE': 'hoge', '_Where': { 'ColumnName': 'STR_VALUE', 'Operator': 'in', 'Object': [] } }", "E10440", "_Where/Object", "Object cannot be empty.")]
        public void ExecuteAction_RDBMS_異常_PatchData追加条件不正()
        {
            TestContext.Run((string contents, string expectedErrorCode, string expectedColumn, string expectedMessage) =>
            {
                var action = CreateBasicAction();

                var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockRepository.Object
                });
                action.Contents = new Contents(contents);

                var response = action.ExecuteAction();
                response.StatusCode.Is(HttpStatusCode.BadRequest);
                var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
                result["error_code"].Value<string>().Is(expectedErrorCode);
                result["errors"].Count().Is(1);
                result["errors"][expectedColumn].Count().Is(1);
                if (expectedMessage != null)
                {
                    result["errors"][expectedColumn][0].Value<string>().Is(expectedMessage);
                }
            });
        }

        [TestMethod]
        public void ExecuteAction_RDBMS_異常_PatchData追加条件不正_件数上限()
        {
            var action = CreateBasicAction();

            var expectedColumn = "_Where/Object";
            var contents = JToken.Parse($"{{ 'STR_VALUE': 'hoge', '_Where': {{ 'ColumnName': 'STR_VALUE', 'Operator': 'in', 'Object': [] }} }}");
            contents["_Where"]["Object"] = JArray.FromObject(Enumerable.Range(1, 1001).Select(x => x.ToString()).ToList());

            var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(contents.ToString());

            var response = action.ExecuteAction();
            response.StatusCode.Is(HttpStatusCode.BadRequest);
            var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
            result["error_code"].Value<string>().Is("E10440");
            result["errors"].Count().Is(1);
            result["errors"][expectedColumn].Count().Is(1);
            result["errors"][expectedColumn][0].Value<string>().Is("Object items too mutch.");
        }

        [TestMethod]
        [TestCase("{ 'Hoge': 'hoge' }", "E10439", false)]
        [TestCase("{ 'Hoge': 'hoge' }", "E10439", true)]
        [TestCase("{ 'STR_VALUE': null }", "E10402", false)]
        [TestCase("{ 'STR_VALUE': null }", "E10402", true)]
        [TestCase("{ 'STR_VALUE': 'hoge', '_Where': { 'ColumnName': 'STR_VALUE', 'Operator': 'in', 'Object': 12345 } }", "E10440", false)]
        [TestCase("{ 'STR_VALUE': 'hoge', '_Where': { 'ColumnName': 'STR_VALUE', 'Operator': 'in', 'Object': 12345 } }", "E10440", true)]
        public void ExecuteAction_RDBMS_異常_PatchData不正_詳細表示制御()
        {
            UnityContainer.RegisterInstance<bool>("Return.JsonValidator.ErrorDetail", false);

            TestContext.Run((string contents, string expectedErrorCode, bool needsErrors) =>
            {
                var action = CreateBasicAction();

                var mockRepository = CreateRdbmsDataStoreRepositoryMock(ValueObjectUtil.Create<QueryParam>(action, action.Query?.GetQueryString(_odataIgnoreList)), 1);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockRepository.Object
                });
                action.Contents = new Contents(contents);

                var dataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                dataContainer.ReturnNeedsJsonValidatorErrorDetail = needsErrors;

                var response = action.ExecuteAction();
                response.StatusCode.Is(HttpStatusCode.BadRequest);
                var result = JToken.Parse(response.Content.ReadAsStringAsync().Result);
                result["error_code"].Value<string>().Is(expectedErrorCode);
                (result.SelectToken("errors") != null).Is(needsErrors);
            });
        }

        private Mock<INewDynamicApiDataStoreRepository> CreateRdbmsDataStoreRepositoryMock(QueryParam queryParam, int expectedCount)
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool>()));
            mockRepository.SetupGet(x => x.ODataPatchSupport).Returns(ODataPatchSupport.BulkUpdate);
            mockRepository.Setup(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()))
                .Callback<QueryParam, JToken>((param, patchData) => {
                    if (queryParam != null)
                    {
                        param.IsNotStructuralEqual(queryParam);
                    }
                    patchData.IsExistProperty("_Upduser_Id").IsTrue();
                    patchData["_Upduser_Id"].Value<string>().Is(_openId);
                    patchData.IsExistProperty("_Upddate").IsTrue();
                    DateTime.TryParse(patchData["_Upddate"].Value<string>(), out _).IsTrue();
                })
                .Returns(expectedCount);

            return mockRepository;
        }

        private Mock<INewDynamicApiDataStoreRepository> CreateRdbmsDataStoreRepositoryMock(QueryParam queryParam, Exception expectedException)
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool>()));
            mockRepository.SetupGet(x => x.ODataPatchSupport).Returns(ODataPatchSupport.BulkUpdate);
            mockRepository.Setup(x => x.ODataPatch(It.IsAny<QueryParam>(), It.IsAny<JToken>()))
                .Callback<QueryParam, JToken>((param, patchData) => {
                    if (queryParam != null)
                    {
                        param.IsNotStructuralEqual(queryParam);
                    }
                    patchData.IsExistProperty("_Upduser_Id").IsTrue();
                    patchData["_Upduser_Id"].Value<string>().Is(_openId);
                    patchData.IsExistProperty("_Upddate").IsTrue();
                    DateTime.TryParse(patchData["_Upddate"].Value<string>(), out _).IsTrue();
                })
                .Throws(expectedException);

            return mockRepository;
        }

        private Mock<INewDynamicApiDataStoreRepository> CreateNonRdbmsDataStoreRepositoryMock(ODataPatchSupport oDataPatchSupport)
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockRepository.SetupGet(x => x.ODataPatchSupport).Returns(oDataPatchSupport);

            return mockRepository;
        }

        private ODataPatchAction CreateBasicAction()
        {
            ODataPatchAction action = UnityCore.Resolve<ODataPatchAction>();
            action.ApiId = new ApiId(Guid.NewGuid().ToString());
            action.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            action.SystemId = new SystemId(Guid.NewGuid().ToString());
            action.VendorId = new VendorId(Guid.NewGuid().ToString());
            action.ProviderSystemId = new SystemId(Guid.NewGuid().ToString());
            action.ProviderVendorId = new VendorId(Guid.NewGuid().ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH);
            action.RepositoryKey = new RepositoryKey("/API/Private/ODataPatchTest/{Key}");
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/ODataPatchTest");
            action.ActionType = new ActionTypeVO(ActionType.ODataPatch);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.OpenId = new OpenId(_openId);
            action.Contents = new Contents(JsonConvert.SerializeObject(new { STR_VALUE = "Value" }));
            action.HasMailTemplate = new HasMailTemplate(true);
            action.HasWebhook = new HasWebhook(true);
            action.IsDocumentHistory = new IsDocumentHistory(true);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("$top"), new QueryStringValue("1") },
                { new QueryStringKey("$filter"), new QueryStringValue("col1 eq 'abc'") },
                { new QueryStringKey("$select"), new QueryStringValue("col1,col2") },
                { new QueryStringKey("$orderby"), new QueryStringValue("col3") },
            });
            action.ControllerSchema = new DataSchema($@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }}
    }},
    'additionalProperties': false
}}");
            return action;
        }
    }
}
