using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JP.DataHub.OData.Interface;
using JP.DataHub.OData.SqlServerDb.ODataToSqlTranslator;
using UnitTest.JP.DataHub.OData.SqlServerDb.MockDataModel;

namespace UnitTest.JP.DataHub.OData.SqlServerDb
{
    [TestClass]
    public class UnitTest_ODataToSqlTranslator
    {
        private static ODataQueryContext oDataQueryContext { get; set; }

        private static HttpContext HttpContext { get; set; }

        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class
        /// </summary>
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            var type = typeof(MockEnum);
            var builder = new ODataConventionModelBuilder();
            builder.AddEnumType(type);
            var enumModel = builder.GetEdmModel();

            var entityType = new EdmEntityType("ns", "ns", null, false, true);
            var edmModel = new EdmModel();
            edmModel.AddElement(entityType);
            edmModel.AddReferencedModel(enumModel);

            oDataQueryContext = new ODataQueryContext(edmModel, entityType, new ODataPath());
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test 
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            var collection = new ServiceCollection();
            collection.AddControllers().AddOData();
            collection.AddODataQueryFilter();
            collection.AddTransient<ODataUriResolver>();
            collection.AddTransient<ODataQueryValidator>();
            collection.AddTransient<TopQueryValidator>();
            collection.AddTransient<FilterQueryValidator>();
            collection.AddTransient<SkipQueryValidator>();
            collection.AddTransient<OrderByQueryValidator>();
            collection.AddLogging();

            var provider = collection.BuildServiceProvider();
            var routeBuilder = new RouteBuilder(Mock.Of<IApplicationBuilder>(x => x.ApplicationServices == provider));

            HttpContext = new DefaultHttpContext() { RequestServices = provider };
        }


        [TestMethod]
        public void TranslateSelectAllSample()
        {
            var uri = new Uri("http://localhost");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} ", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateSelectSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("enumNumber, id")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT \"enumNumber\", \"id\" FROM {TABLE_NAME} ", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateSelectAllTopSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId DESC,id ASC")}&$top={HttpUtility.UrlEncode("15")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT TOP 15 * FROM {TABLE_NAME} ", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateSelectTopSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("p1, p2, p3")}&$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId DESC,id ASC")}&$top={HttpUtility.UrlEncode("15")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT TOP 15 \"p1\", \"p2\", \"p3\" FROM {TABLE_NAME} ", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft' and intField le 5 and doubleField ge 5.5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE \"englishName\" = @o_param1 AND \"intField\" <= @o_param2 AND \"doubleField\" >= @o_param3", sqlQuery);
            parameters.Count.Is(3);
            parameters["@o_param1"].Is("Microsoft");
            parameters["@o_param2"].Is(5m);
            parameters["@o_param3"].Is(5.5m);
        }

        [TestMethod]
        public void TranslateWhereSampleWithBool()
        {
            var validator = new Mock<IODataFilterValidator>();
            validator.Setup(x => x.ValidateAndFormat(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.IsBooleanProperty(It.IsAny<string>())).Returns(true);

            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("trueField eq true and falseField eq false")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE \"trueField\" = @o_param1 AND \"falseField\" = @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is(true);
            parameters["@o_param2"].Is(false);
        }

        [TestMethod]
        public void TranslateWhereSampleWithGUID()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("hoge eq 2ED27DF5-F505-4A06-B168-7321C6B4AD0C")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE \"hoge\" = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("2ED27DF5-F505-4A06-B168-7321C6B4AD0C");
        }

        [TestMethod]
        public void TranslateWhereWithEnumSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("enumNumber eq UnitTest.JP.DataHub.OData.SqlServerDb.MockDataModel.MockEnum'ONE' and intField le 5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE \"enumNumber\" = @o_param1 AND \"intField\" <= @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is("ONE");
            parameters["@o_param2"].Is(5m);
        }

        [TestMethod]
        public void TranslateWhereSampleIn()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("id in ('A','B')")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE \"id\" in (@o_param1,@o_param2)", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is("A");
            parameters["@o_param2"].Is("B");
        }

        [TestMethod]
        public void TranslateAdditionalWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft' and intField le 5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, "\"dataType\" = 'MockOpenType'", null, out var parameters);
            Assert.AreEqual("WHERE (\"dataType\" = 'MockOpenType') AND (\"englishName\" = @o_param1 AND \"intField\" <= @o_param2)", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is("Microsoft");
            parameters["@o_param2"].Is(5m);
        }


        [TestMethod]
        public void TranslateSelectWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, "\"dataType\" = 'MockOpenType'", null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE (\"dataType\" = 'MockOpenType') AND (\"englishName\" = @o_param1)", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateOrderBySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId desc,id asc")}");
            var sqlQuery = Translate(uri, TranslateOptions.ORDERBY_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("ORDER BY \"companyId\" DESC, \"id\" ASC ", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateSelectOrderBySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId desc,id asc")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE \"property\" != @o_param1 ORDER BY \"companyId\" DESC, \"id\" ASC ", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("str1");
        }

        [TestMethod]
        public void TranslateContainsSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("contains(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE \"englishName\" LIKE '%' + @o_param1 + '%'", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateStartswithSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("startswith(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE \"englishName\" LIKE @o_param1 + '%'", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateEndswithSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("endswith(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE \"englishName\" LIKE '%' + @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateUpperAndLowerSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("toupper(englishName) eq 'MICROSOFT' or tolower(englishName) eq 'microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE UPPER(\"englishName\") = @o_param1 OR LOWER(\"englishName\") = @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is("MICROSOFT");
            parameters["@o_param2"].Is("microsoft");
        }

        [TestMethod]
        public void TranslateLengthSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("length(englishName) ge 10 and length(englishName) lt 15")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE LEN(\"englishName\") >= @o_param1 AND LEN(\"englishName\") < @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is(10m);
            parameters["@o_param2"].Is(15m);
        }

        [TestMethod]
        public void TranslateIndexOfSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("indexof(englishName,'soft') eq 4")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE (CHARINDEX(@o_param1,\"englishName\") - 1) = @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is("soft");
            parameters["@o_param2"].Is(4m);
        }

        [TestMethod]
        public void TranslateSubstring2ParamSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("substring(englishName, 1) eq 'icrosoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual($"SELECT * FROM {{TABLE_NAME}} WHERE SUBSTRING(\"englishName\",@o_param1 + 1,{int.MaxValue}) = @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is(1m);
            parameters["@o_param2"].Is("icrosoft");
        }

        [TestMethod]
        public void TranslateSubstring3ParamSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("substring(englishName, 1, length(englishName)) eq 'icrosoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE SUBSTRING(\"englishName\",@o_param1 + 1,LEN(\"englishName\")) = @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is(1m);
            parameters["@o_param2"].Is("icrosoft");
        }

        [TestMethod]
        public void TranslateTrimSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("trim(englishName) eq 'Microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE TRIM(\"englishName\") = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateConcatSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("concat(englishName, ' Canada') eq 'Microsoft Canada'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE CONCAT(\"englishName\",@o_param1) = @o_param2", sqlQuery);
            parameters.Count.Is(2);
            parameters["@o_param1"].Is(" Canada");
            parameters["@o_param2"].Is("Microsoft Canada");
        }

        [TestMethod]
        public void TranslateMasterSample()
        {
            var uri = new Uri($"http://localhost/Post?$select={HttpUtility.UrlEncode("id, englishName")}&$filter={HttpUtility.UrlEncode("title eq 'title1' and field ne 'val' or viewedCount ge 5 and (likedCount ne 3 or enumNumber eq UnitTest.JP.DataHub.OData.SqlServerDb.MockDataModel.MockEnum'TWO')")}&$orderby={HttpUtility.UrlEncode("_lastClientEditedDateTime asc, createdDateTime desc")}&$top={HttpUtility.UrlEncode("30")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL, "\"dataType\" = 'MockOpenType'", null, out var parameters);
            Assert.AreEqual("SELECT TOP 30 \"id\", \"englishName\" FROM {TABLE_NAME} WHERE (\"dataType\" = 'MockOpenType') AND (\"title\" = @o_param1 AND \"field\" != @o_param2 OR \"viewedCount\" >= @o_param3 AND (\"likedCount\" != @o_param4 OR \"enumNumber\" = @o_param5)) ORDER BY \"_lastClientEditedDateTime\" ASC, \"createdDateTime\" DESC ", sqlQuery);
            parameters.Count.Is(5);
            parameters["@o_param1"].Is("title1");
            parameters["@o_param2"].Is("val");
            parameters["@o_param3"].Is(5m);
            parameters["@o_param4"].Is(3m);
            parameters["@o_param5"].Is("TWO");
        }

        [TestMethod]
        public void TranslateCountSample()
        {
            var uri = new Uri($"http://localhost/User?$count={HttpUtility.UrlEncode("true")}&$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT COUNT(1) FROM {TABLE_NAME} WHERE \"englishName\" = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateRoundSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("round(DoubleValue) eq 0")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE ROUND(\"DoubleValue\",0) = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is(0m);
        }

        [TestMethod]
        public void TranslateFloorSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("floor(DoubleValue) eq 0")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE FLOOR(\"DoubleValue\") = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is(0m);
        }

        [TestMethod]
        public void TranslateCeilingSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("ceiling(DoubleValue) eq 1")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE CEILING(\"DoubleValue\") = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is(1m);
        }


        [TestMethod]
        public void TranslateGeoDistanceSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.distance(location, geography'POINT(31.9 -4.8)') lt 100")}");
            var ex = AssertEx.Throws<NotSupportedException>(() => Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters));
            ex.Message.Is("geo.distance");

        }

        [TestMethod]
        public void TranslateGeoIntersectsSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.intersects(area, geography'POLYGON((31.8 -5, 32 -5, 32 -4.7, 31.8 -4.7, 31.8 -5))')")}");
            var ex = AssertEx.Throws<NotSupportedException>(() => Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters));
            ex.Message.Is("geo.intersects");
        }

        [TestMethod]
        public void TranslateAnySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("companies/any(p: p/id eq 'abc' or p/name eq 'blaat')")}");
            var ex = AssertEx.Throws<NotSupportedException>(() => Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE, null, null, out var parameters));
            ex.Message.Is("any");
        }

        [TestMethod]
        public void TranslateFilterByNull()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("value eq null")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE \"value\" IS null", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateFilterByNullLeft()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("null eq value")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("WHERE \"value\" IS null", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateEscape()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("englishName eq 'Alice''s car' and concat(englishName, '''') eq ''")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE \"englishName\" = @o_param1 AND CONCAT(\"englishName\",@o_param2) = @o_param3", sqlQuery);
            parameters.Count.Is(3);
            parameters["@o_param1"].Is("Alice's car");
            parameters["@o_param2"].Is("'");
            parameters["@o_param3"].Is("");
        }

        [TestMethod]
        public void TranslateNotWithProperty()
        {
            var validator = new Mock<IODataFilterValidator>();
            validator.Setup(x => x.ValidateAndFormat(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.IsBooleanProperty(It.IsAny<string>())).Returns(true);

            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("not IsActivated")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, null, validator, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE not((isnull(\"IsActivated\",0)=1))", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void TranslateNotWithFunction()
        {
            var validator = new Mock<IODataFilterValidator>();
            validator.Setup(x => x.ValidateAndFormat(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.IsBooleanProperty(It.IsAny<string>())).Returns(true);

            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("not endswith(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, null, validator, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE not(\"englishName\" LIKE '%' + @o_param1)", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateNotWithExpression()
        {
            var validator = new Mock<IODataFilterValidator>();
            validator.Setup(x => x.ValidateAndFormat(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.IsBooleanProperty(It.IsAny<string>())).Returns(true);

            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("not (IsActivated and endswith(englishName, 'Microsoft') or not IsEnabled)")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, null, validator, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE not((isnull(\"IsActivated\",0)=1) AND \"englishName\" LIKE '%' + @o_param1 OR not((isnull(\"IsEnabled\",0)=1)))", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("Microsoft");
        }

        [TestMethod]
        public void TranslateOnlyBoolProperty()
        {
            var validator = new Mock<IODataFilterValidator>();
            validator.Setup(x => x.ValidateAndFormat(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.IsBooleanProperty(It.IsAny<string>())).Returns(true);

            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("IsActivated")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, null, validator, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE (isnull(\"IsActivated\",0)=1)", sqlQuery);
            parameters.Count.Is(0);
        }

        [TestMethod]
        public void FilterValidation()
        {
            var validator = new Mock<IODataFilterValidator>();
            validator.Setup(x => x.ValidateAndFormat(It.Is<string>(y => y == "value1"), It.Is<object>(y => y is string && (string)y == "aaa"))).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.ValidateAndFormat(It.Is<string>(y => y == "value2"), It.Is<object>(y => y is decimal && (decimal)y == 123m))).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.ValidateAndFormat(It.Is<string>(y => y == "value3"), It.IsAny<object>())).Returns<string, object>((x, y) => y);
            validator.Setup(x => x.ValidateAndFormat(It.Is<string>(y => y == "value4"), It.IsAny<object>())).Returns<string, object>((x, y) => y);

            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("(value1 eq 'aaa' or value2 gt 123 or value3 eq null) and round(value4) eq 0")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, validator, out var parameters);
            Assert.AreEqual("WHERE (\"value1\" = @o_param1 OR \"value2\" > @o_param2 OR \"value3\" IS null) AND ROUND(\"value4\",0) = @o_param3", sqlQuery);
            parameters.Count.Is(3);
            parameters["@o_param1"].Is("aaa");
            parameters["@o_param2"].Is(123m);
            parameters["@o_param3"].Is(0m);

            validator.Verify(x => x.ValidateAndFormat(It.Is<string>(y => y == "value1"), It.Is<object>(y => y is string && (string)y == "aaa")), Times.Exactly(1));
            validator.Verify(x => x.ValidateAndFormat(It.Is<string>(y => y == "value2"), It.Is<object>(y => y is decimal && (decimal)y == 123m)), Times.Exactly(1));
            validator.Verify(x => x.ValidateAndFormat(It.Is<string>(y => y == "value3"), It.IsAny<object>()), Times.Exactly(0));
            validator.Verify(x => x.ValidateAndFormat(It.Is<string>(y => y == "value4"), It.IsAny<object>()), Times.Exactly(0));
        }

        [TestMethod]
        public void UnescapeFilterValue()
        {
            var validator = new Mock<IODataFilterValidator>();
            validator.Setup(x => x.ValidateAndFormat(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((x, y) => y);
            validator.SetupGet(x => x.IsFilterValueUnescapeEnabled).Returns(true);

            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("value1 eq 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 \\u0021\"\\u0023\\u0024%\\u0026\\u0027\\u0028\\u0029\\u002a\\u002b\\u002c-.\\u002f\\u003a\\u003b<\\u003d>\\u003f\\u0040\\u005b\\u005c\\u005d^_`{|}~あいうえお'")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, null, validator, out var parameters);
            Assert.AreEqual("WHERE \"value1\" = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~あいうえお");
        }

        [TestMethod]
        public void TranslateFilterAsciiSymbols()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("key eq '! \"#$%&''()*+,-./:;<=>?@[\\]^_`{|}~'")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, null, null, out var parameters);
            Assert.AreEqual("SELECT * FROM {TABLE_NAME} WHERE \"key\" = @o_param1", sqlQuery);
            parameters.Count.Is(1);
            parameters["@o_param1"].Is("! \"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~");
        }


        private string Translate(Uri uri, TranslateOptions options, string additionalWhereClause, Mock<IODataFilterValidator> validator, out Dictionary<string, object> parameters)
        {
            HttpContext.Request.Method = "GET";
            HttpContext.Request.Host = new HostString(uri.Host, uri.Port);
            HttpContext.Request.Path = uri.LocalPath;
            HttpContext.Request.QueryString = new QueryString(uri.Query);
            var oDataQueryOptions = new ODataQueryOptions(oDataQueryContext, HttpContext.Request);

            var formatter = validator == null ? new SQLQueryFormatter() : new SQLQueryFormatter(validator.Object);
            var oDataToSqlTranslator = new ODataToSqlTranslator(formatter);
            return oDataToSqlTranslator.Translate(oDataQueryOptions, options, additionalWhereClause, out parameters);
        }
    }
}
