using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.User;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class MetaDataTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void MetadataTest_NormalScenario()
        {
            var client = new ManageApiIntegratedTestClient("test1", "DynamicApi", true);
            var api = UnityCore.Resolve<IMetaDataApi>();

            // 取得
            var rtn = client.GetWebApiResponseResult(api.MetaData()).Assert(GetSuccessExpectStatusCode).Result.ToString();

            rtn = rtn.Replace(" ", "");

            rtn.Contains(start).Is(true);
            rtn.Contains(entityIntegratedTestMetadata).Is(true);
            rtn.Contains(entityIntegratedTestMetadata2).Is(true);
            rtn.Contains(actionMetadata).Is(true);
            rtn.Contains(actionMetadata2).Is(true);
            rtn.Contains(entityContainer).Is(true);
            rtn.Contains(entitySet).Is(true);
            rtn.Contains(end).Is(true);
            rtn.Contains(hiddenApi).Is(false);
            rtn.Contains(hiddenSchema).Is(false);
            rtn.Contains(notActiveApi).Is(false);
            rtn.Contains(notActiveSchema).Is(false);
        }


        /*
         * 文字列で比較しているため、テストケース追加時に出力順に気をつけること
         * 
         * 全体の出力順：Entity→ActionとFunction→コンテナ
         * Entityの出力順：SchemaのName
         * Actionの出力順：FunctionはControllerのURL→MethodのURL
         * コンテナの出力順：ControllerのURL
         */
        public string start = @"
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.01"">
<edmx:DataServices>
<Schema Namespace=""DynamicApi"">".Replace("\r\n", "").Replace(" ", "");

        public string entityIntegratedTestMetadata = @"
<ComplexType Name=""IntegratedTestMetadata"">
<Property Name=""STR_VALUE"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"">
<Annotation Term=""Core.Description"" String=""MinimumLength 0""/>
<Annotation Term=""Core.Description"" String=""Pattern '^(https|wss)://'""/>
</Property>
<NavigationProperty Name=""IntegratedTestMetadata2"" Type=""self.IntegratedTestMetadata2"" Nullable=""false"">
<ReferentialConstraint Property=""STR_VALUE"" ReferencedProperty=""Code""/>
</NavigationProperty>
<Property Name=""STR_NULL"" Type=""Edm.String"" Nullable=""True"">
<Annotation Term=""Core.Description"" String=""ForeignKey /API/IntegratedTest/MetadataTest2?$filter=Code eq &quot;{value}&quot;""/>
<Annotation Term=""Core.Description"" String=""Protect""/>
</Property>
<Property Name=""STR_OPT1"" Type=""Edm.String"" Nullable=""False"">
<Annotation Term=""Core.Description"" String=""Reference(/API/IntegratedTest/MetadataTest2/Get/{Code},Code)""/>
</Property>
<Property Name=""STR_OPT2"" Type=""Edm.String"" Nullable=""False"">
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
<Property Name=""ENUM"" Type=""self.IntegratedTestMetadata_ENUM_enum"" Nullable=""False""/>
<Property Name=""ARRAY_ENUM"" Type=""Collection(self.IntegratedTestMetadata_ARRAY_ENUM)""/>
<Property Name=""ARY_VALUE"" Type=""Collection(self.IntegratedTestMetadata_ARY_VALUE)""/>
<Property Name=""ARY_NULL"" Type=""Collection(self.IntegratedTestMetadata_ARY_NULL)""/>
<Property Name=""OBJ_VALUE"" Type=""Collection(self.IntegratedTestMetadata_OBJ_VALUE)""/>
<Property Name=""OBJ_NULL"" Type=""Collection(self.IntegratedTestMetadata_OBJ_NULL)""/>
<Property Name=""ANYOF"" Type=""Collection(self.IntegratedTestMetadata_ANYOF1)"">
<Annotation Term=""Core.Description"" String=""AnyOf""/>
</Property>
<Property Name=""ANYOF"" Type=""Collection(self.IntegratedTestMetadata_ANYOF2)"">
<Annotation Term=""Core.Description"" String=""AnyOf""/>
</Property>
<Property Name=""ALLOF"" Type=""Collection(self.IntegratedTestMetadata_ALLOF1)"">
<Annotation Term=""Core.Description"" String=""AllOf""/>
</Property>
<Property Name=""ALLOF"" Type=""Collection(self.IntegratedTestMetadata_ALLOF2)"">
<Annotation Term=""Core.Description"" String=""AllOf""/>
</Property>
<Property Name=""ONEOF"" Type=""self.IntegratedTestMetadata_ONEOF1"">
<Annotation Term=""Core.Description"" String=""OneOf""/>
</Property>
<Property Name=""ONEOF"" Type=""self.IntegratedTestMetadata_ONEOF2"">
<Annotation Term=""Core.Description"" String=""OneOf""/>
</Property>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ARY_VALUE"">
<Property Name=""STR"" Type=""Edm.String"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ARY_NULL"">
<Property Name=""STR"" Type=""Edm.String"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_OBJ_VALUE"">
<Property Name=""STR"" Type=""Edm.String"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_OBJ_NULL"">
<Property Name=""STR"" Type=""Edm.String"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ANYOF1"">
<Property Name=""STR"" Type=""Edm.String"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ANYOF2"">
<Property Name=""INT"" Type=""Edm.Int64"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ALLOF1"">
<Property Name=""STR"" Type=""Edm.String"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ALLOF2"">
<Property Name=""INT"" Type=""Edm.Int64"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ONEOF1"">
<Property Name=""STR"" Type=""Edm.String"" Nullable=""False""/>
</ComplexType>
<ComplexType Name=""IntegratedTestMetadata_ONEOF2"">
<Property Name=""INT"" Type=""Edm.Int64"" Nullable=""False""/>
</ComplexType>
<EnumType Name=""IntegratedTestMetadata_ENUM_enum"">
<Member Name=""dog"" Value=""dog""/>
<Member Name=""cat"" Value=""cat""/>
<Member Name=""monkey"" Value=""monkey""/>
</EnumType>
<EnumType Name=""IntegratedTestMetadata_ARRAY_ENUM"">
<Member Name=""dog"" Value=""dog""/>
<Member Name=""cat"" Value=""cat""/>
<Member Name=""monkey"" Value=""monkey""/>
</EnumType>".Replace("\r\n", "").Replace(" ", "");

        public string entityIntegratedTestMetadata2 = @"
<EntityType Name=""IntegratedTestMetadata2"">
<Key>
<PropertyRef Name=""Code""/>
</Key>
<Property Name=""Code"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"">
<Annotation Term=""Core.Description"" String=""MinimumLength 0""/>
</Property>
</EntityType>".Replace("\r\n", "").Replace(" ", "");

        public string actionMetadata = @"
<Action Name=""/API/IntegratedTest/MetadataTest/DeleteAll""/>
<Function Name=""/API/IntegratedTest/MetadataTest/GetAll"">
<ReturnType Type=""self.IntegratedTestMetadata""/>
</Function>
<Action Name=""/API/IntegratedTest/MetadataTest/Regist"">
<ReturnType Type=""self.IntegratedTestMetadata""/>
</Action>".Replace("\r\n", "").Replace(" ", "");

        public string actionMetadata2 = @"
<Action Name=""/API/IntegratedTest/MetadataTest2/Delete/{Code1}/{Code2}"">
<Parameter Name=""Code1"" Type=""Edm.Int64"" Nullable=""False"">
<Annotation Term=""Core.Description"" String=""Maximum 999""/>
<Annotation Term=""Core.Description"" String=""ExclusiveMaximum False""/>
</Parameter>
<Parameter Name=""Code2"" Type=""Edm.Decimal"" Nullable=""False"" Precision=""5"" Scale=""1""/>
</Action>
<Function Name=""/API/IntegratedTest/MetadataTest2/Get/{Code}"">
<Parameter Name=""Code"" Type=""Edm.String"" Nullable=""False"" maxLength=""256"">
<Annotation Term=""Core.Description"" String=""MinimumLength 0""/>
</Parameter>
<ReturnType Type=""self.IntegratedTestMetadata2""/>
</Function>".Replace("\r\n", "").Replace(" ", "");

        public string entityContainer = @"<EntityContainer Name=""Container"">".Replace("\r\n", "").Replace(" ", "");

        public string entitySet = @"
<EntitySet Name=""/API/IntegratedTest/MetadataTest"" EntityType=""self.IntegratedTestMetadata""/>
<EntitySet Name=""/API/IntegratedTest/MetadataTest2"" EntityType=""self.IntegratedTestMetadata2""/>".Replace("\r\n", "").Replace(" ", "");

        public string end = @"
</EntityContainer>
</Schema>
</edmx:DataServices>
</edmx:Edmx>".Replace("\r\n", "").Replace(" ", "");

        public string hiddenApi = @"/API/IntegratedTest/MetadataTest2/Hidden";
        public string hiddenSchema = @"IntegratedTestMetadata2Hidden";
        public string notActiveApi = @"/API/IntegratedTest/MetadataTest2/NotActive";
        public string notActiveSchema = @"IntegratedTestMetadata2NotActive";

    }
}
