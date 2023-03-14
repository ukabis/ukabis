using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator
{
    [TestClass]
    public class UnitTest_NumberCustomValidator : UnitTestBase
    {
        private JSchemaReaderSettings jsonSchemaSettings;
        private JSchema schema;

        [TestInitialize]
        public void JsonSearchResult_AddSingle()
        {
            JSchemaReaderSettings jsonSchemaSettings = new JSchemaReaderSettings() { Validators = new List<IJsonValidator>() { new NumberCustomValidator() } };
            string strschema = @"
{
  'type': 'object',
  'additionalProperties': false,
  'properties': {
    'x': {
      'title': 'xxx',
      'type': [
        'number',
        'null'
      ],
      'minimum': -9999.9,
      'maximum': 9999.9,
      'format': 'Number(5,1)'
    }
  }
}";
            schema = JSchema.Parse(strschema, jsonSchemaSettings);
        }

        [TestMethod]
        public void Number()
        {
            IList<ValidationError> jsonErrors;
            "{ 'x' : 1 }".ToJson().IsValid(schema, out jsonErrors).IsTrue();
            "{ 'x' : 0 }".ToJson().IsValid(schema, out jsonErrors).IsTrue();
            "{ 'x' : 1234.1 }".ToJson().IsValid(schema, out jsonErrors).IsTrue();
            "{ 'x' : -1234.9 }".ToJson().IsValid(schema, out jsonErrors).IsTrue();
            "{ 'x' : 1234.99 }".ToJson().IsValid(schema, out jsonErrors).IsFalse();
            "{ 'x' : 12345 }".ToJson().IsValid(schema, out jsonErrors).IsFalse();
        }
    }
}
