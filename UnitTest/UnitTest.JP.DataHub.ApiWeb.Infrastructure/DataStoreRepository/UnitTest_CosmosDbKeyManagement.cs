using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_CosmosDbKeyManagement : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>();
        }

        private Mock<IResourceVersionRepository> CreateResourceVersionRepositoryMock()
        {
            var mockVersionRepository = new Mock<IResourceVersionRepository>();
            mockVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            mockVersionRepository.Setup(x => x.GetMaxVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            return mockVersionRepository;
        }


        [TestMethod]
        public void GetGenerateKey_正常系_KeyValue()
        {
            var testClass = new CosmonsDBKeyManagement();
            var repositoryKeyField = Guid.NewGuid().ToString();
            var repositoryKeyValue = Guid.NewGuid().ToString();
            var repositoryKey = $"/API/{{{repositoryKeyField}}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(repositoryKeyField)] = new UrlParameterValue(repositoryKeyValue)
            };

            var dummyAction = GetDummyAction(repositoryKey, urlParam);
            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var result = testClass.GetGenerateKey(queryParam, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);
            result.Count.Is(3);
            result[repositoryKeyField].Is(repositoryKeyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_QueryString()
        {

            var testClass = new CosmonsDBKeyManagement();
            var repositoryKeyField = Guid.NewGuid().ToString();
            var repositoryKeyValue = Guid.NewGuid().ToString();
            var repositoryKey = $"/API/{{{repositoryKeyField}}}";
            var queryParam = new Dictionary<QueryStringKey, QueryStringValue>
            {
                [new QueryStringKey(repositoryKeyField)] = new QueryStringValue(repositoryKeyValue)
            };
            var dummyAction = GetDummyAction(repositoryKey, null, queryParam);
            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[repositoryKeyField].Is(repositoryKeyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_2個()
        {

            var repositoryKeyField = Guid.NewGuid().ToString();
            var repositoryKeyValue = Guid.NewGuid().ToString();
            var repositoryKeyField2 = Guid.NewGuid().ToString();
            var repositoryKeyValue2 = Guid.NewGuid().ToString();
            var repositoryKey = $"/API/{{{repositoryKeyField}}}/{{{repositoryKeyField2}}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(repositoryKeyField)] = new UrlParameterValue(repositoryKeyValue),
                [new UrlParameterKey(repositoryKeyField2)] = new UrlParameterValue(repositoryKeyValue2)
            };
            var dummyAction = GetDummyAction(repositoryKey, urlParam);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(4);
            result[repositoryKeyField].Is(repositoryKeyValue);
            result[repositoryKeyField2].Is(repositoryKeyValue2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_リポジトリキー無し()
        {
            var repositoryKeyField = Guid.NewGuid().ToString();
            var repositoryKeyValue = Guid.NewGuid().ToString();
            var repositoryKeyField2 = Guid.NewGuid().ToString();
            var repositoryKeyValue2 = Guid.NewGuid().ToString();
            var repositoryKey = $"/API/";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(repositoryKeyField)] = new UrlParameterValue(repositoryKeyValue),
                [new UrlParameterKey(repositoryKeyField2)] = new UrlParameterValue(repositoryKeyValue2)
            };

            var dummyAction = GetDummyAction(repositoryKey, urlParam);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ControllerSchema_KeyValue()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(keyField)] = new UrlParameterValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, urlParam, null, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[keyField].Is(keyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ControllerSchema_QueryString()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var queryParam = new Dictionary<QueryStringKey, QueryStringValue>
            {
                [new QueryStringKey(keyField)] = new QueryStringValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, null, queryParam, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[keyField].Is(keyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ControllerSchema_2個()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var keyField2 = Guid.NewGuid().ToString();
            var keyValue2 = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }},
    '{keyField2}': {{
      'title': '{keyField2}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";

            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(keyField)] = new UrlParameterValue(keyValue),
                [new UrlParameterKey(keyField2)] = new UrlParameterValue(keyValue2)
            };

            var dummyAction = GetDummyAction(null, urlParam, null, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(4);
            result[keyField].Is(keyValue);
            result[keyField2].Is(keyValue2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ResponseSchema_KeyValue()
        {

            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(keyField)] = new UrlParameterValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, urlParam, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[keyField].Is(keyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }


        [TestMethod]
        public void GetGenerateKey_正常系_ResponseSchema_QueryString()
        {

            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var queryParam = new Dictionary<QueryStringKey, QueryStringValue>
            {
                [new QueryStringKey(keyField)] = new QueryStringValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, null, queryParam, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[keyField].Is(keyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ResponseSchema_2個()
        {

            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var keyField2 = Guid.NewGuid().ToString();
            var keyValue2 = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }},
    '{keyField2}': {{
      'title': '{keyField2}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";

            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(keyField)] = new UrlParameterValue(keyValue),
                [new UrlParameterKey(keyField2)] = new UrlParameterValue(keyValue2)
            };

            var dummyAction = GetDummyAction(null, urlParam, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(4);
            result[keyField].Is(keyValue);
            result[keyField2].Is(keyValue2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ControllerSchemaResponseSchema無し()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(keyField)] = new UrlParameterValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, urlParam);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_KeyValue_一致するconditionString有()
        {

            var repositoryKeyField = Guid.NewGuid().ToString();
            var repositoryKeyValue = Guid.NewGuid().ToString();
            var repositoryKey = $"/API/{{{repositoryKeyField}}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(repositoryKeyField)] = new UrlParameterValue(repositoryKeyValue)
            };

            var dummyAction = GetDummyAction(repositoryKey, urlParam);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder($"{{{repositoryKeyField}}}"), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[repositoryKeyField].Is(repositoryKeyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_QueryString_一致するconditionString有()
        {

            var repositoryKeyField = Guid.NewGuid().ToString();
            var repositoryKeyValue = Guid.NewGuid().ToString();
            var repositoryKey = $"/API/{{{repositoryKeyField}}}";
            var queryParam = new Dictionary<QueryStringKey, QueryStringValue>
            {
                [new QueryStringKey(repositoryKeyField)] = new QueryStringValue(repositoryKeyValue)
            };

            var dummyAction = GetDummyAction(repositoryKey, null, queryParam);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder($"{{{repositoryKeyField}}}"), CreateResourceVersionRepositoryMock().Object, null);
            result.Count.Is(2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ControllerSchema_KeyValue_一致するconditionString有()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(keyField)] = new UrlParameterValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, urlParam, null, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder($"{{{keyField}}}"), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[keyField].Is(keyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ControllerSchema_QueryString_一致するconditionString有()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var queryParam = new Dictionary<QueryStringKey, QueryStringValue>
            {
                [new QueryStringKey(keyField)] = new QueryStringValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, null, queryParam, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder($"{{{keyField}}}"), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_ResponseSchema_KeyValue_一致するconditionString有()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
                [new UrlParameterKey(keyField)] = new UrlParameterValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, urlParam, null, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder($"{{{keyField}}}"), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(3);
            result[keyField].Is(keyValue);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }


        [TestMethod]
        public void GetGenerateKey_正常系_ResponseSchema_QueryString_一致するconditionString有()
        {
            var keyField = Guid.NewGuid().ToString();
            var keyValue = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";
            var queryParam = new Dictionary<QueryStringKey, QueryStringValue>
            {
                [new QueryStringKey(keyField)] = new QueryStringValue(keyValue)
            };

            var dummyAction = GetDummyAction(null, null, queryParam, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder($"{{{keyField}}}"), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        [TestMethod]
        public void GetGenerateKey_正常系_対応データ無し()
        {
            var repositoryKeyField = Guid.NewGuid().ToString();
            var repositoryKeyField2 = Guid.NewGuid().ToString();
            var repositoryKey = $"/API/{{{repositoryKeyField}}}/{{{repositoryKeyField2}}}";
            var urlParam = new Dictionary<UrlParameterKey, UrlParameterValue>
            {
            };

            var keyField = Guid.NewGuid().ToString();
            var schema = $@"{{
  'description':'test',
  'properties': {{
    '{keyField}': {{
      'title': '{keyField}',
      'type': 'string',
    }}
  }},
  'type': 'object'
}}";

            var dummyAction = GetDummyAction(repositoryKey, urlParam, null, schema, schema);

            QueryParam parm = ValueObjectUtil.Create<QueryParam>(dummyAction);
            var testClass = new CosmonsDBKeyManagement();
            var result = testClass.GetGenerateKey(parm, new StringBuilder(""), CreateResourceVersionRepositoryMock().Object, null);

            result.Count.Is(2);
            result["_Type"].Is(dummyAction.RepositoryKey.Type);
            result["_Version"].Is(1);
        }

        IDynamicApiAction GetDummyAction(
            string repositoryKey = null,
            Dictionary<UrlParameterKey, UrlParameterValue> urlParam = null,
            Dictionary<QueryStringKey, QueryStringValue> queryParam = null,
            string responseSchema = null,
            string controllerSchema = null
        )
        {
            var dummyAction = new Mock<IDynamicApiAction>();

            dummyAction.SetupAllProperties();
            dummyAction.SetupProperty(x => x.RepositoryInfo, null);
            dummyAction.SetupProperty(x => x.RepositoryKey, new RepositoryKey(repositoryKey ?? Guid.NewGuid().ToString()));
            dummyAction.SetupProperty(x => x.IsVendor, new IsVendor(false));
            dummyAction.SetupProperty(x => x.IsAutomaticId, new IsAutomaticId(false));
            dummyAction.SetupProperty(x => x.OpenId, new OpenId(Guid.NewGuid().ToString()));
            dummyAction.SetupProperty(x => x.ApiQuery, new ApiQuery(null));
            dummyAction.SetupProperty(x => x.IsOverPartition, new IsOverPartition(false));
            dummyAction.SetupProperty(x => x.RelativeUri, new RelativeUri("url"));
            dummyAction.SetupProperty(x => x.ResponseSchema, new DataSchema(responseSchema));
            dummyAction.SetupProperty(x => x.ControllerSchema, new DataSchema(controllerSchema));

            if (urlParam != null)
            {
                dummyAction.SetupProperty(x => x.KeyValue, new UrlParameter(urlParam));
            }

            if (queryParam != null)
            {
                dummyAction.SetupProperty(x => x.Query, new QueryStringVO(queryParam));
            }

            return dummyAction.Object;
        }
    }
}