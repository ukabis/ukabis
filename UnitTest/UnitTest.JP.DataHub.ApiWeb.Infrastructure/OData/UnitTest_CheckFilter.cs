using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Infrastructure.OData;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.OData
{
    /// <summary>
    /// UnitTest_CheckFilter の概要の説明
    /// </summary>
    [TestClass]
    public class UnitTest_CheckFilter : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public override void TestInitialize()
        {
            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterInstance("InvalidODataColums", Configuration.GetSection("AppConfig:InvalidODataColums").Get<string[]>());
        }

        [TestMethod]
        [TestCaseSource("OkCase")]
        public void CheckFilter_CheckOk()
        {
            TestContext.Run((string filter, bool result, string caseName) =>
            {
                var target = CreateInstance($"$filter={filter}");
                target.Filter.FilterClause.Expression.CheckODataFilter().Is(result, $"{caseName}  is Error ");
            });
        }

        [TestMethod]
        [TestCaseSource("NgCase")]
        public void CheckFilter_CheckNg()
        {
            TestContext.Run((string filter, bool result, string caseName) =>
            {
                var target = CreateInstance($"$filter={filter}");
                target.Filter.FilterClause.Expression.CheckODataFilter().Is(result, $"{caseName}  is Error ");
            });
        }


        public static object[] OkCase = new[]
        {
             new object[] {"AreaUnitCode eq 'aaaaa'",false, "NomalCase"},
             new object[] {"AreaUnitCode eq 'aaaaa' and test eq 'aaa'" ,false, "And Case"},
             new object[] {"AreaUnitCode eq 'aaaaa' or test eq 'aaa'" ,false, "or Case"},
             new object[] {"AreaUnitCode eq 'aaaaa' and test eq 'aaa' and bbb eq 'ccc' " ,false, "and triple Case"},
             new object[] {"contains(AreaUnitCode,'aaaaa') ", false, "function Contain Singele"},
             new object[] {"aaaaa eq 'aaa' and contains(AreaUnitCode,'aaaaa') ", false, "function contain Double"},
             new object[] { "endswith(AreaUnitCode,'aaaaa') ", false, "function endswith Singele"},
             new object[] { "aaaa eq 'aaa' and endswith(AreaUnitCode,'aaaaa') ", false, "function endswith Double"},
             new object[] { "startswith(AreaUnitCode,'aaaaa') ", false, "function startswith Singele"},
             new object[] { "aaaa eq 'aaa' and startswith(AreaUnitCode,'aaaaa') ", false, "function startswith Double"},
             new object[] { "length(AreaUnitCode) eq 9", false, "function length Singele"},
             new object[] { "aaaa eq 'aaa' and length(AreaUnitCode) eq 9", false, "function length Double"},
             new object[] { "indexof(AreaUnitCode,'aaaaa') eq 9", false, "function indexof Singele"},
             new object[] { "aaaa eq 'aaa' and indexof(AreaUnitCode,'aaaaa') eq 9", false, "function indexof Double"},
             new object[] { "substring(AreaUnitCode,2) eq '9'", false, "function substring Singele"},
             new object[] { "aaaa eq 'aaa' and substring(AreaUnitCode,2) eq '9'", false, "function substring Double"},
             new object[] { "tolower(AreaUnitCode) eq '9'", false, "function tolower Singele"},
             new object[] { "aaaa eq 'aaa' and tolower(AreaUnitCode) eq '9'", false, "function tolower Double"},
             new object[] { "toupper(AreaUnitCode) eq '9'", false, "function toupper Singele"},
             new object[] { "aaaa eq 'aaa' and toupper(AreaUnitCode) eq '9'", false, "function toupper Double"},
             new object[] { "trim(AreaUnitCode) eq '9'", false, "function trim Singele"},
             new object[] { "aaaa eq 'aaa' and trim(AreaUnitCode) eq '9'", false, "function trim Double"},
             new object[] { "trim(AreaUnitCode) eq '9'", false, "function trim Singele"},
             new object[] { "aaaa eq 'aaa' and trim(AreaUnitCode) eq '9'", false, "function trim Double"},
             new object[] { "concat(AreaUnitCode,'aaaa') eq '9'", false, "function concat Singele"},
             new object[] { "aaaa eq 'aaa' and concat(AreaUnitCode,'aaaa') eq '9'", false, "function concat Double"},
             new object[] { "round(AreaUnitCode) eq 9", false, "function round Singele"},
             new object[] { "aaaa eq 'aaa' and round(AreaUnitCode) eq 9", false, "function round Double"},
             new object[] { "floor(AreaUnitCode) eq 9", false, "function floor Singele"},
             new object[] { "aaaa eq 'aaa' and floor(AreaUnitCode) eq 9", false, "function floor Double"},
             new object[] { "ceiling(AreaUnitCode) eq 9", false, "function floor Singele"},
             new object[] { "aaaa eq 'aaa' and ceiling(AreaUnitCode) eq 9", false, "function ceiling Double"},
             new object[] { "ceiling(AreaUnitCode) eq 9", false, "function floor Singele"},
             new object[] { "aaaa eq 'aaa' and ceiling(AreaUnitCode) eq 9", false, "function ceiling Double"},
             new object[] { "geo.distance(AreaUnitCode,geography'POINT(-127.89734578345 45.234534534)') eq 9", false, "function geo.distance Singele"},
             new object[] { "aaaa eq 'aaa' and geo.distance(AreaUnitCode,geography'POINT(-127.89734578345 45.234534534)') eq 9", false, "function geo.distance Double"},
             new object[] { "geo.intersects(AreaUnitCode,geography'POINT(-127.89734578345 45.234534534)')", false, "function geo.intersects Singele"},
             new object[] { "aaaa eq 'aaa' and geo.intersects(AreaUnitCode,geography'POINT(-127.89734578345 45.234534534)') ", false, "function geo.intersects Double"},
         };

        public static object[] NgCase = new[]
        {
             new object[] {"_Regdate eq 'aaaaa'",true, "NomalCase"},
             new object[] {"_Regdate eq 'aaaaa' and test eq 'aaa'" ,true, "And Case"},
             new object[] {"_Regdate eq 'aaaaa' or test eq 'aaa'" ,true, "or Case"},
             new object[] {"_Regdate eq 'aaaaa' and test eq 'aaa' and bbb eq 'ccc' " ,true, "and triple Case"},
             new object[] {"contains(_Regdate,'aaaaa') ", true, "function Contain Singele"},
             new object[] {"aaaaa eq 'aaa' and contains(_Regdate,'aaaaa') ", true, "function contain Double"},
             new object[] { "endswith(_Regdate,'aaaaa') ", true, "function endswith Singele"},
             new object[] { "aaaa eq 'aaa' and endswith(_Regdate,'aaaaa') ", true, "function endswith Double"},
             new object[] { "startswith(_Regdate,'aaaaa') ", true, "function startswith Singele"},
             new object[] { "aaaa eq 'aaa' and startswith(_Regdate,'aaaaa') ", true, "function startswith Double"},
             new object[] { "length(_Regdate) eq 9", true, "function length Singele"},
             new object[] { "aaaa eq 'aaa' and length(_Regdate) eq 9", true, "function length Double"},
             new object[] { "indexof(_Regdate,'aaaaa') eq 9", true, "function indexof Singele"},
             new object[] { "aaaa eq 'aaa' and indexof(_Regdate,'aaaaa') eq 9", true, "function indexof Double"},
             new object[] { "substring(_Regdate,2) eq '9'", true, "function substring Singele"},
             new object[] { "aaaa eq 'aaa' and substring(_Regdate,2) eq '9'", true, "function substring Double"},
             new object[] { "tolower(_Regdate) eq '9'", true, "function tolower Singele"},
             new object[] { "aaaa eq 'aaa' and tolower(_Regdate) eq '9'", true, "function tolower Double"},
             new object[] { "toupper(_Regdate) eq '9'", true, "function toupper Singele"},
             new object[] { "aaaa eq 'aaa' and toupper(_Regdate) eq '9'", true, "function toupper Double"},
             new object[] { "trim(_Regdate) eq '9'", true, "function trim Singele"},
             new object[] { "aaaa eq 'aaa' and trim(_Regdate) eq '9'", true, "function trim Double"},
             new object[] { "trim(_Regdate) eq '9'", true, "function trim Singele"},
             new object[] { "aaaa eq 'aaa' and trim(_Regdate) eq '9'", true, "function trim Double"},
             new object[] { "concat(_Regdate,'aaaa') eq '9'", true, "function concat Singele"},
             new object[] { "aaaa eq 'aaa' and concat(_Regdate,'aaaa') eq '9'", true, "function concat Double"},
             new object[] { "round(_Regdate) eq 9", true, "function round Singele"},
             new object[] { "aaaa eq 'aaa' and round(_Regdate) eq 9", true, "function round Double"},
             new object[] { "floor(_Regdate) eq 9", true, "function floor Singele"},
             new object[] { "aaaa eq 'aaa' and floor(_Regdate) eq 9", true, "function floor Double"},
             new object[] { "ceiling(_Regdate) eq 9", true, "function floor Singele"},
             new object[] { "aaaa eq 'aaa' and ceiling(_Regdate) eq 9", true, "function ceiling Double"},
             new object[] { "ceiling(_Regdate) eq 9", true, "function floor Singele"},
             new object[] { "aaaa eq 'aaa' and ceiling(_Regdate) eq 9", true, "function ceiling Double"},
             new object[] { "geo.distance(_Regdate,geography'POINT(-127.89734578345 45.234534534)') eq 9", true, "function geo.distance Singele"},
             new object[] { "aaaa eq 'aaa' and geo.distance(_Regdate,geography'POINT(-127.89734578345 45.234534534)') eq 9", true, "function geo.distance Double"},
             new object[] { "geo.intersects(_Regdate,geography'POINT(-127.89734578345 45.234534534)')", true, "function geo.intersects Singele"},
             new object[] { "aaaa eq 'aaa' and geo.intersects(_Regdate,geography'POINT(-127.89734578345 45.234534534)') ", true, "function geo.intersects Double"},
         };


        private ODataQueryOptions CreateInstance(string filter)
        {
            var entityType = new EdmEntityType("ns", "ns", null, false, true);
            var edmModel = new EdmModel();
            edmModel.AddElement(entityType);

            var context = new ODataQueryContext(edmModel, entityType, new ODataPath());

            var uri = new Uri("https://localhost/?" + filter);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "GET";
            httpContext.Request.Host = new HostString(uri.Host, uri.Port);
            httpContext.Request.Path = uri.LocalPath;
            httpContext.Request.QueryString = new QueryString(uri.Query);

            return new ODataQueryOptions(context, httpContext.Request);
        }
    }
}
