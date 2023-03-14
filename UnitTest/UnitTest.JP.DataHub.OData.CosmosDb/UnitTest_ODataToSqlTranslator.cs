using System;
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
using JP.DataHub.OData.CosmosDb.ODataToSqlTranslator;
using UnitTest.JP.DataHub.OData.CosmosDb.MockDataModel;

namespace UnitTest.JP.DataHub.OData.CosmosDb
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
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            Assert.AreEqual("SELECT * FROM c ", sqlQuery);
        }

        [TestMethod]
        public void TranslateSelectSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("englishName, id, add/item1")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            Assert.AreEqual("SELECT c.englishName, c.id, c.add.item1 FROM c ", sqlQuery);
        }

        [TestMethod]
        public void TranslateSelectWithKeywordSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("Value, Asc, As/item1")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            Assert.AreEqual("SELECT c[\"Value\"], c[\"Asc\"], c.As.item1 FROM c ", sqlQuery);
        }

        [TestMethod]
        public void TranslateSelectWithEnumSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("enumNumber, id")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            Assert.AreEqual("SELECT c.enumNumber, c.id FROM c ", sqlQuery);
        }
        [TestMethod]
        public void TranslateAnySampleNested()
        {
            // '|'は内部的にJOINの構文解析に使用しており、フィルタの値に含まれても問題ないことの確認のため入れている。
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("prop1/companies/any(p: p/id eq 'abc|')")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT DISTINCT  VALUE c FROM c JOIN d IN c.prop1.companies WHERE (d.id = 'abc|')", sqlQuery);
        }
        [TestMethod]
        public void TranslateAnySample()
        {
            // '|'は内部的にJOINの構文解析に使用しており、フィルタの値に含まれても問題ないことの確認のため入れている。
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("companies /any(p: p/id eq '|abc' or p/name eq '||blaat')")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT DISTINCT  VALUE c FROM c JOIN d IN c.companies WHERE (d.id = '|abc' OR d.name = '||blaat')", sqlQuery);
        }
        [TestMethod]
        public void TranslateAnySampleWithMultipleClauses()
        {
            // '|'は内部的にJOINの構文解析に使用しており、フィルタの値に含まれても問題ないことの確認のため入れている。
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("(companies/any(p: p/id eq '|abc|' or p/name eq '|blaat|')) and customers/any(x: x/customer_name eq '|jaap|')")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT DISTINCT  VALUE c FROM c JOIN d IN c.companies JOIN e IN c.customers WHERE (d.id = '|abc|' OR d.name = '|blaat|') AND (e.customer_name = '|jaap|')", sqlQuery);
        }

        [TestMethod]
        public void TranslateAnySampleWithKeyword()
        {
            // '|'は内部的にJOINの構文解析に使用しており、フィルタの値に含まれても問題ないことの確認のため入れている。
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("(where/any(p: p/order eq 'a|b|c' or p/by eq 'b|a|a|t')) and join/any(x: x/with eq 'j|a|a|p')")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT DISTINCT  VALUE c FROM c JOIN d IN c[\"where\"] JOIN e IN c[\"join\"] WHERE (d[\"order\"] = 'a|b|c' OR d[\"by\"] = 'b|a|a|t') AND (e[\"with\"] = 'j|a|a|p')", sqlQuery);
        }

        [TestMethod]
        public void TranslateSelectAllTopSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId DESC,id ASC")}&$top={HttpUtility.UrlEncode("15")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT TOP 15 * FROM c ", sqlQuery);
        }

        [TestMethod]
        public void TranslateSelectTopSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("p1, p2, p3")}&$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId DESC,id ASC")}&$top={HttpUtility.UrlEncode("15")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT TOP 15 c.p1, c.p2, c.p3 FROM c ", sqlQuery);
        }

        [TestMethod]
        public void TranslateWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft' and intField le 5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("WHERE c.englishName = 'Microsoft' AND c.intField <= 5", sqlQuery);
        }

        [TestMethod]
        public void TranslateWhereWithKeywordSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("Value eq 'Microsoft' and ASC le 5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("WHERE c[\"Value\"] = 'Microsoft' AND c[\"ASC\"] <= 5", sqlQuery);
        }

        [TestMethod]
        public void TranslateWhereSampleWithGUID()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("hoge eq 2ED27DF5-F505-4A06-B168-7321C6B4AD0C")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("WHERE c.hoge = '2ed27df5-f505-4a06-b168-7321c6b4ad0c'", sqlQuery);
        }

        [TestMethod]
        public void TranslateWhereWithEnumSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("enumNumber eq UnitTest.JP.DataHub.OData.CosmosDb.MockDataModel.MockEnum'ONE' and intField le 5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("WHERE c.enumNumber = 'ONE' AND c.intField <= 5", sqlQuery);
        }

        [TestMethod]
        public void TranslateWhereWithNextedFieldsSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("parent /child eq 'childValue' and intField le 5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("WHERE c.parent.child = 'childValue' AND c.intField <= 5", sqlQuery);
        }

        [TestMethod]
        public void TranslateWhereSampleIn()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("id in ('A','B')")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("WHERE c.id in ('A','B')", sqlQuery);
        }

        [TestMethod]
        public void TranslateAdditionalWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft' and intField le 5")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE, "c.dataType = 'MockOpenType'");
            Assert.AreEqual("WHERE (c.dataType = 'MockOpenType') AND (c.englishName = 'Microsoft' AND c.intField <= 5)", sqlQuery);
        }

        [TestMethod]
        public void TranslateSelectWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE, "c.dataType = 'MockOpenType'");
            Assert.AreEqual("SELECT * FROM c WHERE (c.dataType = 'MockOpenType') AND (c.englishName = 'Microsoft')", sqlQuery);
        }

        [TestMethod]
        public void TranslateOrderBySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId desc,id asc")}");
            var sqlQuery = Translate(uri, TranslateOptions.ORDERBY_CLAUSE);
            Assert.AreEqual("ORDER BY c.companyId DESC, c.id ASC ", sqlQuery);
        }

        [TestMethod]
        public void TranslateOrderByKeywordSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("Value ne 'str1'")}&$orderby={HttpUtility.UrlEncode("Value desc,Asc asc")}");
            var sqlQuery = Translate(uri, TranslateOptions.ORDERBY_CLAUSE);
            Assert.AreEqual("ORDER BY c[\"Value\"] DESC, c[\"Asc\"] ASC ", sqlQuery);
        }

        [TestMethod]
        public void TranslateSelectOrderBySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId desc,id asc")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE c.property != 'str1' ORDER BY c.companyId DESC, c.id ASC ", sqlQuery);
        }

        [TestMethod]
        public void TranslateContainsSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("contains(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE CONTAINS(c.englishName,'Microsoft')", sqlQuery);
        }

        [TestMethod]
        public void TranslateStartswithSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("startswith(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE STARTSWITH(c.englishName,'Microsoft')", sqlQuery);
        }

        [TestMethod]
        public void TranslateEndswithSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("endswith(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE ENDSWITH(c.englishName,'Microsoft')", sqlQuery);
        }

        [TestMethod]
        public void TranslateUpperAndLowerSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("toupper(englishName) eq 'MICROSOFT' or tolower(englishName) eq 'microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE UPPER(c.englishName) = 'MICROSOFT' OR LOWER(c.englishName) = 'microsoft'", sqlQuery);
        }

        [TestMethod]
        public void TranslateLengthSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("length(englishName) ge 10 and length(englishName) lt 15")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE LENGTH(c.englishName) >= 10 AND LENGTH(c.englishName) < 15", sqlQuery);
        }

        [TestMethod]
        public void TranslateIndexOfSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("indexof(englishName,'soft') eq 4")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE INDEX_OF(c.englishName,'soft') = 4", sqlQuery);
        }

        [TestMethod]
        public void TranslateSubstring2ParamSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("substring(englishName, 1) eq 'icrosoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual($"SELECT * FROM c WHERE SUBSTRING(c.englishName,1,{int.MaxValue}) = 'icrosoft'", sqlQuery);
        }

        [TestMethod]
        public void TranslateSubstring3ParamSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("substring(englishName, 1, length(englishName)) eq 'icrosoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE SUBSTRING(c.englishName,1,LENGTH(c.englishName)) = 'icrosoft'", sqlQuery);
        }

        [TestMethod]
        public void TranslateTrimSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("trim(englishName) eq 'Microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE LTRIM(RTRIM(c.englishName)) = 'Microsoft'", sqlQuery);
        }

        [TestMethod]
        public void TranslateConcatSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("concat(englishName, ' Canada') eq 'Microsoft Canada'")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE CONCAT(c.englishName,' Canada') = 'Microsoft Canada'", sqlQuery);
        }

        [TestMethod]
        public void TranslateMasterSample()
        {
            var uri = new Uri($"http://localhost/Post?$select={HttpUtility.UrlEncode("id, englishName")}&$filter={HttpUtility.UrlEncode("title eq 'title1' and property/field ne 'val' or viewedCount ge 5 and (likedCount ne 3 or enumNumber eq UnitTest.JP.DataHub.OData.CosmosDb.MockDataModel.MockEnum'TWO')")}&$orderby={HttpUtility.UrlEncode("_lastClientEditedDateTime asc, createdDateTime desc")}&$top={HttpUtility.UrlEncode("30")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL, "c._t = 'dataType'");
            Assert.AreEqual("SELECT TOP 30 c.id, c.englishName FROM c WHERE (c._t = 'dataType') AND (c.title = 'title1' AND c.property.field != 'val' OR c.viewedCount >= 5 AND (c.likedCount != 3 OR c.enumNumber = 'TWO')) ORDER BY c._lastClientEditedDateTime ASC, c.createdDateTime DESC ", sqlQuery);
        }

        [TestMethod]
        public void TranslateCountSample()
        {
            var uri = new Uri($"http://localhost/User?$count={HttpUtility.UrlEncode("true")}&$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft'")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT VALUE COUNT(1) FROM c WHERE c.englishName = 'Microsoft'", sqlQuery);
        }
        [TestMethod]
        public void TranslateCountSample_Any()
        {
            var uri = new Uri($"http://localhost/User?$count={HttpUtility.UrlEncode("true")}&$filter={HttpUtility.UrlEncode("MetaList/any(o: o/MetaKey eq 'TestKeyA')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL | TranslateOptions.COUNT_INCLUDE_ANY);
            Assert.AreEqual("SELECT VALUE COUNT(1) FROM (SELECT DISTINCT  VALUE c FROM c JOIN d IN c.MetaList WHERE (d.MetaKey = 'TestKeyA'))", sqlQuery);
        }

        [TestMethod]
        public void TranslateCountSample_Any2()
        {
            var uri = new Uri($"http://localhost/User?$count={HttpUtility.UrlEncode("true")}&$filter={HttpUtility.UrlEncode("MetaList/any(o: o/MetaKey eq 'TestKeyA' and o/MetaValue eq 'value2')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL | TranslateOptions.COUNT_INCLUDE_ANY);
            Assert.AreEqual("SELECT VALUE COUNT(1) FROM (SELECT DISTINCT  VALUE c FROM c JOIN d IN c.MetaList WHERE (d.MetaKey = 'TestKeyA' AND d.MetaValue = 'value2'))", sqlQuery);
        }

        [TestMethod]
        public void TranslateCountSample_MultiAny()
        {
            var uri = new Uri($"http://localhost/User?$count={HttpUtility.UrlEncode("true")}&$filter={HttpUtility.UrlEncode("MetaList/any(o: o/MetaKey eq 'TestKeyB') and MetaList/any(o: o/MetaValue eq 'value2') and MetaList/any(o: o/MetaKey ne 'TestKeyA')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL | TranslateOptions.COUNT_INCLUDE_ANY);
            Assert.AreEqual("SELECT VALUE COUNT(1) FROM (SELECT DISTINCT  VALUE c FROM c JOIN d IN c.MetaList JOIN e IN c.MetaList JOIN f IN c.MetaList WHERE (d.MetaKey = 'TestKeyB') AND (e.MetaValue = 'value2') AND (f.MetaKey != 'TestKeyA'))", sqlQuery);
        }

        [TestMethod]
        public void TranslateGeoDistanceSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.distance(location, geography'POINT(31.9 -4.8)') lt 100")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE ST_DISTANCE(c.location,{\"type\":\"Point\",\"coordinates\":[31.9,-4.8]}) < 100", sqlQuery);
        }

        [TestMethod]
        public void TranslateGeoIntersectsSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.intersects(area, geography'POLYGON((31.8 -5, 32 -5, 32 -4.7, 31.8 -4.7, 31.8 -5))')")}");
            var sqlQuery = Translate(uri, TranslateOptions.ALL & ~TranslateOptions.TOP_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE ST_INTERSECTS(c.area,{\"type\":\"Polygon\",\"coordinates\":[[[31.8,-5.0],[32.0,-5.0],[32.0,-4.7],[31.8,-4.7],[31.8,-5.0]]]})", sqlQuery);
        }

        [TestMethod]
        public void TranslateEscape()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("englishName eq 'Alice''s car' and concat(englishName, '''') eq ''")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE c.englishName = 'Alice\\'s car' AND CONCAT(c.englishName,'\\'') = ''", sqlQuery);
        }

        [TestMethod]
        public void TranslateNotWithProperty()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("not IsActivated")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE not(c.IsActivated)", sqlQuery);
        }

        [TestMethod]
        public void TranslateNotWithFunction()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("not endswith(englishName, 'Microsoft')")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE not(ENDSWITH(c.englishName,'Microsoft'))", sqlQuery);
        }

        [TestMethod]
        public void TranslateNotWithExpression()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("not (IsActivated and endswith(englishName, 'Microsoft') or not IsEnabled)")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE not(c.IsActivated AND ENDSWITH(c.englishName,'Microsoft') OR not(c.IsEnabled))", sqlQuery);
        }

        [TestMethod]
        public void TranslateOnlyBoolProperty()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("IsActivated")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE c.IsActivated", sqlQuery);
        }

        [TestMethod]
        public void TranslateFilterAsciiSymbols()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("key eq '! \"#$%&''()*+,-./:;<=>?@[\\]^_`{|}~'")}");
            var sqlQuery = Translate(uri, TranslateOptions.SELECT_CLAUSE | TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual("SELECT * FROM c WHERE c.key = '! \\\"#$%&\\'()*+,-./:;<=>?@[\\\\]^_`{|}~'", sqlQuery);
        }


        private string Translate(Uri uri, TranslateOptions options, string additionalWhereClause = null)
        {
            HttpContext.Request.Method = "GET";
            HttpContext.Request.Host = new HostString(uri.Host, uri.Port);
            HttpContext.Request.Path = uri.LocalPath;
            HttpContext.Request.QueryString = new QueryString(uri.Query);
            var oDataQueryOptions = new ODataQueryOptions(oDataQueryContext, HttpContext.Request);

            var oDataToSqlTranslator = new ODataToSqlTranslator(new SQLQueryFormatter());
            return oDataToSqlTranslator.Translate(oDataQueryOptions, options, additionalWhereClause);
        }
    }
}
