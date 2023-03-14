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
using JP.DataHub.OData.MongoDb.ODataToSqlTranslator;
using UnitTest.JP.DataHub.OData.MongoDb.MockDataModel;

namespace UnitTest.JP.DataHub.OData.MongoDb
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
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("");
        }

        [TestMethod]
        public void TranslateSelectSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("englishName, hoge")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("{ englishName: 1, hoge: 1 }");
        }

        [TestMethod]
        public void TranslateSelectWithEnumSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("enumNumber, hoge")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("{ enumNumber: 1, hoge: 1 }");
        }

        [TestMethod]
        public void TranslateAnySampleNested()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("prop1/companies/any(p: p/id eq 'abc')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { 'prop1.companies' : { '$elemMatch' :  { 'id' : 'abc' }  } } ] }");
        }

        [TestMethod]
        public void TranslateAnySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("companies/any(p: p/id eq 'abc' or p/name eq 'blaat')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { 'companies' : { '$elemMatch' : { '$or' : [  { 'id' : 'abc' } ,  { 'name' : 'blaat' }  ] } } } ] }");
        }

        [TestMethod]
        public void TranslateAnySampleWithMultipleClauses()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("(companies/any(p: p/id eq 'abc' or p/name eq 'blaat')) and customers/any(x: x/customer_name eq 'jaap')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$and' : [ { 'companies' : { '$elemMatch' : { '$or' : [  { 'id' : 'abc' } ,  { 'name' : 'blaat' }  ] } } }, { 'customers' : { '$elemMatch' :  { 'customer_name' : 'jaap' }  } } ] } ] }");
        }

        [TestMethod]
        public void TranslateSelectAllTopSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId DESC,id ASC")}&$top={HttpUtility.UrlEncode("15")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("");
            var top = Translate(uri, TranslateOptions.TOP_CLAUSE);
            top.Is("15");
        }

        [TestMethod]
        public void TranslateSelectTopSample()
        {
            var uri = new Uri($"http://localhost/User?$select={HttpUtility.UrlEncode("p1, p2, p3")}&$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId DESC,id ASC")}&$top={HttpUtility.UrlEncode("15")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("{ p1: 1, p2: 1, p3: 1 }");
            var top = Translate(uri, TranslateOptions.TOP_CLAUSE);
            top.Is("15");
        }

        [TestMethod]
        public void TranslateWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("a eq 'XYZ' and b le 5")}");
            var query = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            query.Is(" { '$and' : [ { '$and' : [  { 'a' : 'XYZ' } ,  { 'b' : { '$lte' : 5 } }  ] } ] }");
        }

        [TestMethod]
        public void TranslateWhereSampleWithGUID()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("hoge eq 2ED27DF5-F505-4A06-B168-7321C6B4AD0C")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [  { 'hoge' : '2ed27df5-f505-4a06-b168-7321c6b4ad0c' }  ] }");
        }

        [TestMethod]
        public void TranslateWhereWithNextedFieldsSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("parent/child eq 'childValue' and intField le 5")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$and' : [  { 'parent.child' : 'childValue' } ,  { 'intField' : { '$lte' : 5 } }  ] } ] }");
        }

        [TestMethod]
        public void TranslateAdditionalWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft' and intField le 5")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE, "{ '$and' : [ { 'dataType' : 'MockOpenType' } ] }");
            where.Is(@" { '$and' : [ { ""$and"": [
  {
    ""dataType"": ""MockOpenType""
  }
] },{ '$and' : [ { '$and' : [  { 'englishName' : 'Microsoft' } ,  { 'intField' : { '$lte' : 5 } }  ] } ] } ] }");
        }

        [TestMethod]
        public void TranslateWhereSampleIn()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("id in ('A','B')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { 'id' : { '$in' : ['A','B'] } } ] }");
        }

        [TestMethod]
        public void TranslateSelectWhereSample()
        {
            var uri = new Uri($"http://localhost?$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft'")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE, "{ '$and' : [ { 'dataType' : 'MockOpenType' } ] }");
            where.Is(@" { '$and' : [ { ""$and"": [
  {
    ""dataType"": ""MockOpenType""
  }
] },{ '$and' : [  { 'englishName' : 'Microsoft' }  ] } ] }");
        }

        [TestMethod]
        public void TranslateOrderBySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId desc,hoge asc")}");
            var orderby = Translate(uri, TranslateOptions.ORDERBY_CLAUSE);
            orderby.Is("{ companyId : -1, hoge : 1 }");
        }

        [TestMethod]
        public void TranslateSelectOrderBySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("property ne 'str1'")}&$orderby={HttpUtility.UrlEncode("companyId desc,hoge asc")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [  { 'property' : { '$ne' : 'str1' } }  ] }");
            var orderby = Translate(uri, TranslateOptions.ORDERBY_CLAUSE);
            orderby.Is("{ companyId : -1, hoge : 1 }");
        }

        [TestMethod]
        public void TranslateContainsSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("contains(englishName, 'Microsoft')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { englishName : /Microsoft/ } ] }");
        }

        [TestMethod]
        public void TranslateStartswithSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("startswith(englishName, 'Microsoft')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { englishName : /^Microsoft/ } ] }");
        }

        [TestMethod]
        public void TranslateEndswithSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("endswith(englishName, 'Microsoft')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { englishName : /Microsoft$/ } ] }");
        }

        [TestMethod]
        public void TranslateUpperAndLowerSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("toupper(englishName) eq 'MICROSOFT' or tolower(englishName) eq 'microsoft'")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$or' : [ { '$expr' : { '$eq' : [ { '$toUpper' : '$englishName' } , 'MICROSOFT' ] } }, { '$expr' : { '$eq' : [ { '$toLower' : '$englishName' } , 'microsoft' ] } } ] } ] }");
        }

        [TestMethod]
        public void TranslateLengthSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("length(englishName) ge 10 and length(englishName) lt 15")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$and' : [ { '$expr' : { '$gte' : [ { '$strLenCP' : { '$ifNull': [ '$englishName', '' ] } } , 10 ] } }, { '$expr' : { '$lt' : [ { '$strLenCP' : { '$ifNull': [ '$englishName', '' ] } } , 15 ] } } ] } ] }");
        }

        [TestMethod]
        public void TranslateIndexOfSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("indexof(englishName,'soft') eq 4")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$indexOfCP' : [ '$englishName' , 'soft' ] } , 4 ] } } ] }");
        }

        [TestMethod]
        public void TranslateSubstring2ParamSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("substring(englishName, 1) eq 'icrosoft'")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is($" {{ '$and' : [ {{ '$expr' : {{ '$eq' : [ {{ '$substrCP' : [ '$englishName' , 1 , {int.MaxValue} ] }} , 'icrosoft' ] }} }} ] }}");
        }

        [TestMethod]
        public void TranslateSubstring3ParamSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("substring(englishName, 1, length(englishName)) eq 'icrosoft'")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$substrCP' : [ '$englishName' , 1 , { '$strLenCP' : { '$ifNull': [ '$englishName', '' ] } } ] } , 'icrosoft' ] } } ] }");
        }

        [TestMethod]
        public void TranslateTrimSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("trim(englishName) eq 'Microsoft'")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$trim' : { input : '$englishName' } } , 'Microsoft' ] } } ] }");
        }

        [TestMethod]
        public void TranslateConcatSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("concat(englishName, ' Canada') eq 'Microsoft Canada'")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$concat' : [ '$englishName' , ' Canada' ] } , 'Microsoft Canada' ] } } ] }");
        }

        [TestMethod]
        public void TranslateMasterSample()
        {
            var uri = new Uri($"http://localhost/Post?$select={HttpUtility.UrlEncode("id, englishName")}&$filter={HttpUtility.UrlEncode("title eq 'title1' and property/field ne 'val' or viewedCount ge 5 and (likedCount ne 3 or enumNumber eq UnitTest.JP.DataHub.OData.MongoDb.MockDataModel.MockEnum'TWO')")}&$orderby={HttpUtility.UrlEncode("_lastClientEditedDateTime asc, createdDateTime desc")}&$top={HttpUtility.UrlEncode("30")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("{ id: 1, englishName: 1 }");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$or' : [ { '$and' : [  { 'title' : 'title1' } ,  { 'property.field' : { '$ne' : 'val' } }  ] }, { '$and' : [  { 'viewedCount' : { '$gte' : 5 } } , ({ '$or' : [  { 'likedCount' : { '$ne' : 3 } } ,  { 'enumNumber' : 'TWO' }  ] }) ] } ] } ] }");
            var orderby = Translate(uri, TranslateOptions.ORDERBY_CLAUSE);
            orderby.Is("{ _lastClientEditedDateTime : 1, createdDateTime : -1 }");
        }

        [TestMethod]
        public void TranslateCountSample()
        {
            var uri = new Uri($"http://localhost/User?$count={HttpUtility.UrlEncode("true")}&$filter={HttpUtility.UrlEncode("englishName eq 'Microsoft'")}");
            var select = Translate(uri, TranslateOptions.SELECT_CLAUSE);
            select.Is("{ '$count' : 1 }");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [  { 'englishName' : 'Microsoft' }  ] }");
        }

        [TestMethod]
        public void TranslateGeoDistanceLessThanOrEqualSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.distance(location, geography'POINT(31.9 -4.8)') le 100")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { location : { '$near' : { '$geometry' : {\"type\":\"Point\",\"coordinates\":[31.9,-4.8]} , '$maxDistance' : 100 } } } ] }");
        }

        [TestMethod]
        public void TranslateGeoDistanceGreaterThanOrEqualSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.distance(location, geography'POINT(31.9 -4.8)') ge 200")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { location : { '$near' : { '$geometry' : {\"type\":\"Point\",\"coordinates\":[31.9,-4.8]} , '$minDistance' : 200 } } } ] }");
        }

        [TestMethod]
        public void TranslateGeoDistanceEqualSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.distance(location, geography'POINT(31.9 -4.8)') eq 300")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { location : { '$near' : { '$geometry' : {\"type\":\"Point\",\"coordinates\":[31.9,-4.8]} , '$minDistance' : 300 , '$maxDistance' : 300 } } } ] }");
        }

        [TestMethod]
        public void TranslateGeoIntersectsSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("geo.intersects(area, geography'POLYGON((31.8 -5, 32 -5, 32 -4.7, 31.8 -4.7, 31.8 -5))')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { area : { '$geoIntersects' : { '$geometry' : {\"type\":\"Polygon\",\"coordinates\":[[[31.8,-5.0],[32.0,-5.0],[32.0,-4.7],[31.8,-4.7],[31.8,-5.0]]]} } } } ] }");
        }

        [TestMethod]
        public void TranslateRoundSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("round(amount) eq 10")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$round' : '$amount' } , 10 ] } } ] }");
        }

        [TestMethod]
        public void TranslateFloorSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("floor(amount) eq 20")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$floor' : '$amount' } , 20 ] } } ] }");
        }

        [TestMethod]
        public void TranslateCeilingSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("ceiling(amount) eq 30")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$ceil' : '$amount' } , 30 ] } } ] }");
        }

        [TestMethod]
        public void TranslateConcatMultiSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("concat(concat(concat(englishName, ' Canada'), englishName), ' America') eq 'Microsoft CanadaMicrosoft America'")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$concat' : [ { '$concat' : [ { '$concat' : [ '$englishName' , ' Canada' ] } , '$englishName' ] } , ' America' ] } , 'Microsoft CanadaMicrosoft America' ] } } ] }");
        }

        [TestMethod]
        public void TranslateMultiNestedSample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("concat(concat(englishName, ' Office'), ' Canada') eq concat('Microsoft', ' Office Canada') and length(englishName) ge 9 and endswith(englishName, 'Microsoft') and (toupper(englishName) eq 'MICROSOFT' or tolower(englishName) eq 'microsoft')")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$and' : [ { '$and' : [ { '$and' : [ { '$expr' : { '$eq' : [ { '$concat' : [ { '$concat' : [ '$englishName' , ' Office' ] } , ' Canada' ] } , { '$concat' : [ 'Microsoft' , ' Office Canada' ] } ] } }, { '$expr' : { '$gte' : [ { '$strLenCP' : { '$ifNull': [ '$englishName', '' ] } } , 9 ] } } ] } , { englishName : /Microsoft$/ } ] }, ({ '$or' : [ { '$expr' : { '$eq' : [ { '$toUpper' : '$englishName' } , 'MICROSOFT' ] } }, { '$expr' : { '$eq' : [ { '$toLower' : '$englishName' } , 'microsoft' ] } } ] }) ] } ] }");
        }

        [TestMethod]
        public void TranslateBinaryOperatorLeftNodeIsPropertySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("value2 eq ceiling(value1)")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ '$value2' , { '$ceil' : '$value1' } ] } } ] }");
        }

        [TestMethod]
        public void TranslateBinaryOperatorRightNodeIsPropertySample()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("ceiling(value1) eq value2")}");
            var where = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            where.Is(" { '$and' : [ { '$expr' : { '$eq' : [ { '$ceil' : '$value1' } , '$value2' ] } } ] }");
        }

        [TestMethod]
        public void TranslateEscape()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("englishName eq 'Alice''s car' and concat(englishName, '''') eq ''")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual(" { '$and' : [ { '$and' : [  { 'englishName' : 'Alice\\'s car' } , { '$expr' : { '$eq' : [ { '$concat' : [ '$englishName' , '\\'' ] } , '' ] } } ] } ] }", sqlQuery);
        }

        [TestMethod]
        public void TranslateFilterAsciiSymbols()
        {
            var uri = new Uri($"http://localhost/User?$filter={HttpUtility.UrlEncode("key eq '! \"#$%&''()*+,-./:;<=>?@[\\]^_`{|}~'")}");
            var sqlQuery = Translate(uri, TranslateOptions.WHERE_CLAUSE);
            Assert.AreEqual(" { '$and' : [  { 'key' : '! \\\"#$%&\\'()*+,-./:;<=>?@[\\\\]^_`{|}~' }  ] }", sqlQuery);
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
