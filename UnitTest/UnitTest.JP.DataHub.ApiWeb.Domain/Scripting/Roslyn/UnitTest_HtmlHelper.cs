using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.ApiWeb.Domain.MockClass;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass()]
    public class UnitTest_HtmlHelper : UnitTestBase
    {
        [TestMethod()]
        public void SelectHtmlNodesByClassNameTest()
        {
            //urlとして初期化するためにセットしている アクセスはしていないし使用していない
            var testUrl = "https://www.google.com";
            var testHtml = @"
<html>
<body>
<div id=""dupleid"" class = ""hogehoge""></div>
<div id=""dupleid"" class = ""hogehoge""></div>
<div id=""uniqueid"" class = ""hogehoge""></div>
<div id=""dupleid"" class = ""hogehoge""></div>
</body>
</html>
";
            var htmlHelper = new HtmlHelper(new HttpClient(new MoqHttpClient(testHtml)));
            var resultList = htmlHelper.SelectHtmlNodesByClassName(testUrl, "hogehoge");
            Assert.AreEqual(resultList.Count(), 4);
            foreach (var result in resultList)
            {
                Assert.IsTrue(result.Contains(@"class=""hogehoge"""));
            }
        }
        [TestMethod()]
        public void SelectHtmlNodesByIdTest()
        {
            var testHtml = @"
<html>
<body>
<div id=""dupleid""></div>
<div id=""dupleid""></div>
<div id=""uniqueid""></div>
<div id=""dupleid""></div>
</body>
</html>
";
            //urlとして初期化するためにセットしている アクセスはしていないし使用していない
            var testUrl = "https://www.google.com";
            var htmlHelper = new HtmlHelper(new HttpClient(new MoqHttpClient(testHtml)));
            var resultList = htmlHelper.SelectHtmlNodesById(testUrl, "dupleid");
            Assert.AreEqual(resultList.Count(), 3);
            foreach (var result in resultList)
            {
                Assert.IsTrue(result.Contains(@"id=""dupleid"""));
            }
        }

        [TestMethod()]
        public void SelectHtmlNodesByXPathTest()
        {
            //urlとして初期化するためにセットしている アクセスはしていないし使用していない
            var testUrl = "https://www.google.com";
            var testHtml = @"
<html>
<body>
<div id=""dupleid"" class = ""hogehoge""></div>
<div id=""dupleid"" class = ""hogehoge""></div>
<div id=""uniqueid"" class = ""hogehoge""></div>
<div id=""dupleid"" class = ""hogehoge""></div>
</body>
</html>
";
            var htmlHelper = new HtmlHelper(new HttpClient(new MoqHttpClient(testHtml)));
            var resultList = htmlHelper.SelectHtmlNodesByQuery(testUrl, @"//*/div[2]");
            Assert.AreEqual(resultList.Count(), 1);
        }


        [TestMethod()]
        public void SelectHtmlNodesByXPathTest_異常系_存在しないXpath()
        {
            //urlとして初期化するためにセットしている アクセスはしていないし使用していない
            var testUrl = "https://www.google.com";
            var testHtml = @"
<html>
<body>
<div id=""dupleid"" class = ""hogehoge""></div>
<div id=""dupleid"" class = ""hogehoge""></div>
<div id=""uniqueid"" class = ""hogehoge""></div>
<div id=""dupleid"" class = ""hogehoge""></div>
</body>
</html>
";
            var htmlHelper = new HtmlHelper(new HttpClient(new MoqHttpClient(testHtml)));
            Assert.IsNull(htmlHelper.SelectHtmlNodesByQuery(testUrl, @"//*[@id=""fugafuga""]/div[2]/ul/li/a"));
        }
    }
}
