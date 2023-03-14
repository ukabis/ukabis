using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using Moq;
using Unity;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.MetadataInfo;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [TestClass]
    public class UnitTest_MetadataInfoApplicationService : UnitTestBase
    {
        private IMetadataInfoApplicationService _testClass;

        Guid _urlSchemaId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        Guid _urlSchemaId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        Guid _responseSchemaId1 = Guid.Parse("00000000-0000-0000-0000-000000000011");
        Guid _responseSchemaId2 = Guid.Parse("00000000-0000-0000-0000-000000000012");


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            _testClass = UnityContainer.Resolve<IMetadataInfoApplicationService>();
        }


        private List<ApiDescription> CreateApis()
        {
            return new List<ApiDescription>
            {
                new ApiDescription
                {
                    ApiId = Guid.NewGuid(),
                    RelativePath = "/API/Test",
                    ApiSchemaId = _responseSchemaId1,
                    Methods = new List<MethodDescription>
                    {
                        new MethodDescription
                        {
                            MethodId = Guid.NewGuid(),
                            RelativePath = "Delete/{Code1}",
                            HttpMethod = "delete",
                            UrlSchemaId = _urlSchemaId1,
                        },
                        new MethodDescription
                        {
                            MethodId = Guid.NewGuid(),
                            RelativePath = "Get/{Code1}/{Code2}",
                            HttpMethod = "get",
                            UrlSchemaId = _urlSchemaId2,
                            ResponseSchemaId = _responseSchemaId1,
                        },
                        new MethodDescription
                        {
                            MethodId = Guid.NewGuid(),
                            RelativePath = "Regist",
                            HttpMethod = "post",
                            RequestSchemaId = _responseSchemaId1,
                        }
                    }
                },
                new ApiDescription
                {
                    ApiId = Guid.NewGuid(),
                    RelativePath = "/API/Test2",
                    ApiSchemaId = _responseSchemaId2,
                    Methods = new List<MethodDescription>
                    {
                        new MethodDescription
                        {
                            MethodId = Guid.NewGuid(),
                            RelativePath = "Delete/{Code1}",
                            HttpMethod = "delete",
                            UrlSchemaId = _urlSchemaId1,
                        },
                        new MethodDescription
                        {
                            MethodId = Guid.NewGuid(),
                            RelativePath = "Get/{Code1}",
                            HttpMethod = "get",
                            UrlSchemaId = _urlSchemaId1,
                            ResponseSchemaId = _responseSchemaId2,
                        },
                        new MethodDescription
                        {
                            MethodId = Guid.NewGuid(),
                            RelativePath = "Regist",
                            HttpMethod = "post",
                            RequestSchemaId = _responseSchemaId2,
                        }
                    }
                }
            };
        }

        private List<SchemaDescription> CreateSchemas()
        {
            return new List<SchemaDescription>
            {
                new SchemaDescription
                {
                    SchemaId = _responseSchemaId1.ToString(),
                    SchemaName = "TestSchema",
                    JsonSchema = @"{
    'type': 'object',
    'properties': {
        'STR_VALUE':    { 'type': 'string' , 'maxLength':256, 'minLength':0, 'pattern':'^(https|wss)://', 
            'format':'ForeignKey /API/IntegratedTest/MetadataTest2/Get/{Code}'},
        'STR_NULL':     { 'type': ['string', 'null'] , 'format':'ForeignKey /API/IntegratedTest/MetadataTest2?$filter=Code eq ""{value}"";Protect'},
        'STR_OPT1':    { 'type': 'string', 'format':'Reference(/API/IntegratedTest/MetadataTest2/Get/{Code},Code)'},
        'STR_OPT2':    { 'type': 'string', 'format': 'date-time' },
        'INT_VALUE':    { 'type': 'integer', 'minimum':0, 'exclusiveMinimum':true, 'maximum': 999, 'exclusiveMaximum':true, 'multipleOf':5 },
        'INT_NULL':     { 'type': ['integer', 'null'], 'minimum':10, 'exclusiveMinimum':false, 'maximum': 20, 'exclusiveMaximum':false },
        'NUM_VALUE':    { 'type': 'number', 'format':'Number(5,1)'},
        'NUM_NULL':     { 'type': ['number', 'null'] },
        'BOL_VALUE':    { 'type': 'boolean' },
        'BOL_NULL':     { 'type': ['boolean', 'null'] },
        'ENUM':    { 'type': 'string', 'enum': [ 'dog', 'cat', 'monkey' ] },
        'ARRAY_ENUM':    { 'type': 'array', 'items': { 'type': 'string', 'enum': [ 'dog', 'cat', 'monkey' ] }},
        'ARY_VALUE':    { 'type': 'array', 'items': { '$ref': '#/definitions/CHILD' }},
        'ARY_NULL':    { 'type': ['array', 'null'], 'items': { '$ref': '#/definitions/CHILD' }},
        'OBJ_VALUE':    { 'type': 'object', 'items': { '$ref': '#/definitions/CHILD' }},
        'OBJ_NULL':    { 'type': ['object', 'null'], 'items': { '$ref': '#/definitions/CHILD' }},
        'ANYOF':{'anyOf': [{'$ref': '#/definitions/CHILD'},{'$ref': '#/definitions/CHILD2'}]},
        'ALLOF':{'allOf': [{'$ref': '#/definitions/CHILD'},{'$ref': '#/definitions/CHILD2'}]},
        'ONEOF':{'oneOf': [{'$ref': '#/definitions/CHILD'},{'$ref': '#/definitions/CHILD2'}]},
    },
    'definitions': {
        'CHILD':{'properties': {
            'STR':    { 'type': 'string' },
        }},
        'CHILD2':{'properties': {
            'INT':    { 'type': 'integer' },
        }}
    }
}",
                },
                new SchemaDescription
                {
                    SchemaId = _responseSchemaId2.ToString(),
                    SchemaName = "TestSchema2",
                    JsonSchema = @"{
    'type': 'object',
    'properties': {
        'Code1':    { 'type': 'string' , 'maxLength':256, },
        'Name':     { 'type': ['string', 'null'] },
    }
}",
                }
            };
        }

        private List<SchemaDescription> CreateUrlSchemas()
        {
            return new List<SchemaDescription>
            {
                new SchemaDescription
                {
                    SchemaId = _urlSchemaId1.ToString(),
                    SchemaName = "UrlSchema",
                    JsonSchema = @"{
    'type': 'object',
    'properties': {
        'Code1': { 'type': 'string' , 'maxLength':256 }
    }
}"
                },
                new SchemaDescription
                {
                    SchemaId = _urlSchemaId2.ToString(),
                    SchemaName = "UrlSchema2",
                    JsonSchema = @"{
    'type': 'object',
    'properties': {
        'Code1': { 'type': 'string' , 'maxLength':256 },
        'Code2': { 'type': 'string' }
    }
}"
                }
            };
        }

        [TestMethod]
        public void MetadataApplicationService_CreateMetadata()
        {
            StringWriter stringWriterExpected = new StringWriter();
            XmlTextWriter xmlWriterExpected = new XmlTextWriter(stringWriterExpected);
            var apis = CreateApis();
            var schemas = CreateSchemas();
            var urlSchemas = CreateUrlSchemas();

            //XML
            var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.01"">
  <edmx:DataServices>
    <Schema Namespace=""DynamicApi"">
      <ComplexType Name=""TestSchema"">
        <Property Name=""STR_VALUE"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"">
          <Annotation Term=""Core.Description"" String=""MinimumLength 0"" />
          <Annotation Term=""Core.Description"" String=""Pattern '^(https|wss)://'"" />
          <Annotation Term=""Core.Description"" String=""ForeignKey /API/IntegratedTest/MetadataTest2/Get/{Code}"" />
        </Property>
        <Property Name=""STR_NULL"" Type=""Edm.String"" Nullable=""True"">
          <Annotation Term=""Core.Description"" String=""ForeignKey /API/IntegratedTest/MetadataTest2?$filter=Code eq &quot;{value}&quot;"" />
          <Annotation Term=""Core.Description"" String=""Protect"" />
        </Property>
        <Property Name=""STR_OPT1"" Type=""Edm.String"" Nullable=""False"">
          <Annotation Term=""Core.Description"" String=""Reference(/API/IntegratedTest/MetadataTest2/Get/{Code},Code)"" />
        </Property>
        <Property Name=""STR_OPT2"" Type=""Edm.String"" Nullable=""False"">
          <Annotation Term=""Core.Description"" String=""date-time"" />
        </Property>
        <Property Name=""INT_VALUE"" Type=""Edm.Int64"" Nullable=""False"">
          <Annotation Term=""Core.Description"" String=""Maximum 999"" />
          <Annotation Term=""Core.Description"" String=""ExclusiveMaximum True"" />
          <Annotation Term=""Core.Description"" String=""Maximum 0"" />
          <Annotation Term=""Core.Description"" String=""ExclusiveMaximum True"" />
        </Property>
        <Property Name=""INT_NULL"" Type=""Edm.Int64"" Nullable=""True"">
          <Annotation Term=""Core.Description"" String=""Maximum 20"" />
          <Annotation Term=""Core.Description"" String=""ExclusiveMaximum False"" />
          <Annotation Term=""Core.Description"" String=""Maximum 10"" />
          <Annotation Term=""Core.Description"" String=""ExclusiveMaximum False"" />
        </Property>
        <Property Name=""NUM_VALUE"" Type=""Edm.Decimal"" Nullable=""False"" Precision=""5"" Scale=""1"" />
        <Property Name=""NUM_NULL"" Type=""Edm.Decimal"" Nullable=""True"" />
        <Property Name=""BOL_VALUE"" Type=""Edm.Boolean"" Nullable=""False"" />
        <Property Name=""BOL_NULL"" Type=""Edm.Boolean"" Nullable=""True"" />
        <Property Name=""ENUM"" Type=""self.TestSchema_ENUM_enum"" Nullable=""False"" />
        <Property Name=""ARRAY_ENUM"" Type=""Collection(self.TestSchema_ARRAY_ENUM)"" />
        <Property Name=""ARY_VALUE"" Type=""Collection(self.TestSchema_ARY_VALUE)"" />
        <Property Name=""ARY_NULL"" Type=""Collection(self.TestSchema_ARY_NULL)"" />
        <Property Name=""OBJ_VALUE"" Type=""Collection(self.TestSchema_OBJ_VALUE)"" />
        <Property Name=""OBJ_NULL"" Type=""Collection(self.TestSchema_OBJ_NULL)"" />
        <Property Name=""ANYOF"" Type=""Collection(self.TestSchema_ANYOF1)"">
          <Annotation Term=""Core.Description"" String=""AnyOf"" />
        </Property>
        <Property Name=""ANYOF"" Type=""Collection(self.TestSchema_ANYOF2)"">
          <Annotation Term=""Core.Description"" String=""AnyOf"" />
        </Property>
        <Property Name=""ALLOF"" Type=""Collection(self.TestSchema_ALLOF1)"">
          <Annotation Term=""Core.Description"" String=""AllOf"" />
        </Property>
        <Property Name=""ALLOF"" Type=""Collection(self.TestSchema_ALLOF2)"">
          <Annotation Term=""Core.Description"" String=""AllOf"" />
        </Property>
        <Property Name=""ONEOF"" Type=""self.TestSchema_ONEOF1"">
          <Annotation Term=""Core.Description"" String=""OneOf"" />
        </Property>
        <Property Name=""ONEOF"" Type=""self.TestSchema_ONEOF2"">
          <Annotation Term=""Core.Description"" String=""OneOf"" />
        </Property>
      </ComplexType>
      <ComplexType Name=""TestSchema_ARY_VALUE"">
        <Property Name=""STR"" Type=""Edm.String"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_ARY_NULL"">
        <Property Name=""STR"" Type=""Edm.String"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_OBJ_VALUE"">
        <Property Name=""STR"" Type=""Edm.String"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_OBJ_NULL"">
        <Property Name=""STR"" Type=""Edm.String"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_ANYOF1"">
        <Property Name=""STR"" Type=""Edm.String"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_ANYOF2"">
        <Property Name=""INT"" Type=""Edm.Int64"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_ALLOF1"">
        <Property Name=""STR"" Type=""Edm.String"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_ALLOF2"">
        <Property Name=""INT"" Type=""Edm.Int64"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_ONEOF1"">
        <Property Name=""STR"" Type=""Edm.String"" Nullable=""False"" />
      </ComplexType>
      <ComplexType Name=""TestSchema_ONEOF2"">
        <Property Name=""INT"" Type=""Edm.Int64"" Nullable=""False"" />
      </ComplexType>
      <EnumType Name=""TestSchema_ENUM_enum"">
        <Member Name=""dog"" Value=""dog"" />
        <Member Name=""cat"" Value=""cat"" />
        <Member Name=""monkey"" Value=""monkey"" />
      </EnumType>
      <EnumType Name=""TestSchema_ARRAY_ENUM"">
        <Member Name=""dog"" Value=""dog"" />
        <Member Name=""cat"" Value=""cat"" />
        <Member Name=""monkey"" Value=""monkey"" />
      </EnumType>
      <ComplexType Name=""TestSchema2"">
        <Property Name=""Code1"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""True"" />
      </ComplexType>
      <Action Name=""/API/Test/Delete/{Code1}"">
        <Parameter Name=""Code1"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"" />
      </Action>
      <Function Name=""/API/Test/Get/{Code1}/{Code2}"">
        <Parameter Name=""Code1"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"" />
        <Parameter Name=""Code2"" Type=""Edm.String"" Nullable=""False"" />
        <ReturnType Type=""self.TestSchema"" />
      </Function>
      <Action Name=""/API/Test/Regist"" />
      <Action Name=""/API/Test2/Delete/{Code1}"">
        <Parameter Name=""Code1"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"" />
      </Action>
      <Function Name=""/API/Test2/Get/{Code1}"">
        <Parameter Name=""Code1"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"" />
        <ReturnType Type=""self.TestSchema2"" />
      </Function>
      <Action Name=""/API/Test2/Regist"" />
      <EntityContainer Name=""Container"">
        <EntitySet Name=""/API/Test"" EntityType=""self.TestSchema"" />
        <EntitySet Name=""/API/Test2"" EntityType=""self.TestSchema2"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(expected);
            xmlDocument.WriteContentTo(xmlWriterExpected);

            var result = _testClass.CreateMetadata(apis, schemas, urlSchemas);

            // 検証
            result.Is(stringWriterExpected.ToString());
        }

        [TestMethod]
        public void MetadataApplicationService_CreateXmlEntity()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            StringWriter stringWriterExpected = new StringWriter();
            XmlTextWriter xmlWriterExpected = new XmlTextWriter(stringWriterExpected);
            var apis = CreateApis();
            var schema =
                new SchemaDescription
                {
                    SchemaId = Guid.NewGuid().ToString(),
                    SchemaName = "TestSchema",
                    JsonSchema = @"{
    'type': 'object',
    'properties': {
        'STR_VALUE':    { 'type': 'string' , 'maxLength':256, 'minLength':0, 'pattern':'^(https|wss)://' },
        'STR_NULL':     { 'type': ['string', 'null'] },
        'STR_OPT1':    { 'type': 'string', 'format': 'date-time' },
        'INT_VALUE':    { 'type': 'integer', 'minimum':0, 'exclusiveMinimum':true, 'maximum': 999, 'exclusiveMaximum':true, 'multipleOf':5 },
        'INT_NULL':     { 'type': ['integer', 'null'], 'minimum':10, 'exclusiveMinimum':false, 'maximum': 20, 'exclusiveMaximum':false },
        'NUM_VALUE':    { 'type': 'number', 'format':'Number(5,1)'},
        'NUM_NULL':     { 'type': ['number', 'null'] },
        'BOL_VALUE':    { 'type': 'boolean' },
        'BOL_NULL':     { 'type': ['boolean', 'null'] },
    }
}",
                };
            List<SchemaDescription> parentSchemas = new List<SchemaDescription>();
            List<SchemaDescription> allSchemas = new List<SchemaDescription>();

            //XML
            var expected = @"
<ComplexType Name=""TestSchema"">
    <Property Name=""STR_VALUE"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"">
        <Annotation Term=""Core.Description"" String=""MinimumLength 0""/>
        <Annotation Term=""Core.Description"" String=""Pattern '^(https|wss)://'""/>
    </Property>
    <Property Name=""STR_NULL"" Type=""Edm.String"" Nullable=""True""/>
    <Property Name=""STR_OPT1"" Type=""Edm.String"" Nullable=""False"">
        <Annotation Term=""Core.Description"" String=""date-time""/>
    </Property>
    <Property Name=""INT_VALUE"" Type=""Edm.Int64"" Nullable=""False"">
        <Annotation Term=""Core.Description"" String=""Maximum 999""/>
        <Annotation Term=""Core.Description"" String=""ExclusiveMaximum True""/>
        <Annotation Term=""Core.Description"" String=""Maximum 0""/>
        <Annotation Term=""Core.Description"" String=""ExclusiveMaximum True""/>
    </Property>
    <Property Name=""INT_NULL"" Type=""Edm.Int64"" Nullable=""True"">
        <Annotation Term=""Core.Description"" String=""Maximum 20""/>
        <Annotation Term=""Core.Description"" String=""ExclusiveMaximum False""/>
        <Annotation Term=""Core.Description"" String=""Maximum 10""/>
        <Annotation Term=""Core.Description"" String=""ExclusiveMaximum False""/>
    </Property>
    <Property Name=""NUM_VALUE"" Type=""Edm.Decimal"" Nullable=""False"" Precision=""5"" Scale=""1""/>
    <Property Name=""NUM_NULL"" Type=""Edm.Decimal"" Nullable=""True""/>
    <Property Name=""BOL_VALUE"" Type=""Edm.Boolean"" Nullable=""False""/>
    <Property Name=""BOL_NULL"" Type=""Edm.Boolean"" Nullable=""True""/>
</ComplexType>
";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(expected);
            xmlDocument.WriteContentTo(xmlWriterExpected);

            // 実行
            _testClass.GetType().InvokeMember(
                "CreateXmlEntity",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                _testClass,
                new object[] { schema, "STR_VALUE", xmlWriter, parentSchemas, apis, allSchemas });

            // 検証
            stringWriter.ToString().Is(stringWriterExpected.ToString());
        }

        [TestMethod]
        public void MetadataApplicationService_CreateXmlEnum()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            StringWriter stringWriterExpected = new StringWriter();
            XmlTextWriter xmlWriterExpected = new XmlTextWriter(stringWriterExpected);
            //enumの内容
            var json = JSchema.Parse("{'enum':['0','1','2']}");

            IList<JToken> enumValue = json.Enum;
            //XML
            var expected = @"
<EnumType Name=""enumTest"">
<Member Name=""0"" Value=""0"" />
<Member Name=""1"" Value=""1"" />
<Member Name=""2"" Value=""2"" />
</EnumType>";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(expected);
            xmlDocument.WriteContentTo(xmlWriterExpected);

            // 実行
            _testClass.GetType().InvokeMember(
                "CreateXmlEnum",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                _testClass,
                new object[] { xmlWriter, "enumTest", enumValue });

            // 検証
            stringWriter.ToString().Is(stringWriterExpected.ToString());
        }

        [TestMethod]
        public void MetadataApplicationService_CreateXmlActionFunction()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            StringWriter stringWriterExpected = new StringWriter();
            XmlTextWriter xmlWriterExpected = new XmlTextWriter(stringWriterExpected);
            var apis = CreateApis();
            var schemas = CreateSchemas();
            var urlSchemas = CreateUrlSchemas();
            var allSchemas = new List<SchemaDescription>();
            allSchemas.AddRange(schemas);
            allSchemas.AddRange(urlSchemas);

            //XML
            var expected = @"
<Schema>
<Action Name=""/API/Test/Delete/{Code1}"">
    <Parameter Name=""Code1"" Type=""Edm.String"" Nullable=""False"" maxLength=""256""/>
</Action>
<Function Name=""/API/Test/Get/{Code1}/{Code2}"">
<Parameter Name=""Code1"" Type=""Edm.String"" Nullable=""False"" maxLength=""256""/>
<Parameter Name=""Code2"" Type=""Edm.String"" Nullable=""False""/>
<ReturnType Type=""self.TestSchema""/>
</Function>
<Action Name=""/API/Test/Regist""/>
</Schema>";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(expected);
            xmlDocument.WriteContentTo(xmlWriterExpected);

            // 実行
            xmlWriter.WriteStartElement("Schema");
            _testClass.GetType().InvokeMember(
                "CreateXmlActionFunction",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                _testClass,
                new object[] { xmlWriter, apis.First(), urlSchemas, allSchemas });
            xmlWriter.WriteEndElement();

            // 検証
            stringWriter.ToString().Is(stringWriterExpected.ToString());
        }

        [TestMethod]
        public void MetadataApplicationService_CreateXmlContainer()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            StringWriter stringWriterExpected = new StringWriter();
            XmlTextWriter xmlWriterExpected = new XmlTextWriter(stringWriterExpected);
            var apis = CreateApis();
            var schemas = CreateSchemas();
            var urlSchemas = CreateUrlSchemas();
            var allSchemas = new List<SchemaDescription>();
            allSchemas.AddRange(schemas);
            allSchemas.AddRange(urlSchemas);

            //XML
            var expected = @"
<EntityContainer Name=""Container"">
<EntitySet Name=""/API/Test"" EntityType=""self.TestSchema""/>
<EntitySet Name=""/API/Test2"" EntityType=""self.TestSchema2"" />
</EntityContainer>";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(expected);
            xmlDocument.WriteContentTo(xmlWriterExpected);

            // 実行
            _testClass.GetType().InvokeMember(
                "CreateXmlContainer",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                _testClass,
                new object[] { xmlWriter, apis, schemas });

            // 検証
            stringWriter.ToString().Is(stringWriterExpected.ToString());
        }
    }
}
