using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Aop;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.ApiFilter;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter
{
    [TestClass]
    public class UnitTest_FilterManager : UnitTestBase
    {
        private IFilterManager _filterManager;


        [TestInitialize]
        public override void TestInitialize()
        {
            var container = new UnityContainer();
            var config = Options.Create(new List<ApiFilterConfig>()
            {
                new() { Level=1, ResourceUrl="/API/Public/Hoge", ApiUrl="*", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.IgnoreInterfaceApiFilter" },

                new() { Level=10, ResourceUrl="/API/Public/Resource1", ApiUrl="*", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=11, ResourceUrl="/API/Public/Resource2", ApiUrl="*", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=100, ResourceUrl="/API/Public/RESOURCE-T.*", ApiUrl="*", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=101, ResourceUrl="/API/Public/RESOURCE-X.*", ApiUrl="*", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=20, ResourceUrl="/API/Public/Api", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=21, ResourceUrl="/API/Public/Api", ApiUrl="Register", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=200, ResourceUrl="/API/Public/ApiR", ApiUrl="Get|Regist", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=201, ResourceUrl="/API/Public/ApiR", ApiUrl="Delete", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=30, ResourceUrl="/API/Public/Method", ApiUrl="*", HttpMethod="Get", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=31, ResourceUrl="/API/Public/Method", ApiUrl="*", HttpMethod="Post", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=300, ResourceUrl="/API/Public/MethodR", ApiUrl="Get", HttpMethod="G.*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=301, ResourceUrl="/API/Public/MethodR", ApiUrl="Get", HttpMethod="D.*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=40, ResourceUrl="/API/Public/Action", ApiUrl="*", HttpMethod="*", Action="Regist", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=41, ResourceUrl="/API/Public/Action", ApiUrl="*", HttpMethod="*", Action="Delete", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=400, ResourceUrl="/API/Public/ActionR", ApiUrl="Get", HttpMethod="*", Action="Q.*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=401, ResourceUrl="/API/Public/ActionR", ApiUrl="Get", HttpMethod="*", Action="D.*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },

                new() { Level=410, ResourceUrl="/API/Public/VendorSystem/Action", ApiUrl="Get", HttpMethod="*", Action="*", RequestVendorId="VendorA", RequestSystemId="SystemA", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=412, ResourceUrl="/API/Public/System/Action", ApiUrl="Get", HttpMethod="*", Action="*", RequestSystemId="SystemA", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },
                new() { Level=413, ResourceUrl="/API/Public/Vendor/Action", ApiUrl="Get", HttpMethod="*", Action="*", RequestVendorId="VendorB", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter3" },

                new() { Level=500, Seq=1, ResourceUrl="/API/Public/OrderedActionA", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter3" },
                new() { Level=500, Seq=3, ResourceUrl="/API/Public/OrderedActionA", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },
                new() { Level=500, Seq=2, ResourceUrl="/API/Public/OrderedActionA", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=501, Seq=1, ResourceUrl="/API/Public/OrderedActionB", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter3" },
                new() { Level=501, ResourceUrl="/API/Public/OrderedActionB", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter3" },
                new() { Level=501, Seq=100, ResourceUrl="/API/Public/OrderedActionB", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=501, Seq=4, ResourceUrl="/API/Public/OrderedActionB", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=502, Seq=1, ResourceUrl="/API/Public/OrderedActionC", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter2" },
                new() { Level=502, Seq=2, ResourceUrl="/API/Public/OrderedActionB", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },
                new() { Level=502, Seq=3, ResourceUrl="/API/Public/OrderedActionC", ApiUrl="Get", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.ApiFilter1" },

                new() { Level=998, ResourceUrl="/API/Public/Hoge", ApiUrl="*", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.HighPriorityApiFilter" },
                new() { Level=999, ResourceUrl="/API/Public/Hoge", ApiUrl="*", HttpMethod="*", Action="*", Assembly="UnitTest.JP.DataHub.ApiWeb.Domain.dll", Type="UnitTest.JP.DataHub.ApiWeb.Domain.ApiFilter.LowPriorityApiFilter" },
            });
            container.RegisterInstance<IOptions<List<ApiFilterConfig>>>(config);

            UnityCore.UnityContainer = container;

            _filterManager = new FilterManager(null, null, null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void Priority()
        {
            // 次の観点でテストする
            // ・優先度が高いものが返ってくるか？ => 998のものが返ってくればよい
            // ・ApiFilterクラスとして定義しているクラスが、ちゃんとinterfaceを実装しているものを選択しているか？
            //      => IgnoreInterfaceApiFilterを選んでいないということは、これも守れている
            // ・存在しないクラスを選んでいないか？ => app.configに定義があった場合、エラーをthrowする
            // ・存在しないアセンブリのものを選んでいないか？ => app.configに定義があった場合、エラーをthrowする
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Hoge", ApiUrl = "Get/123", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(HighPriorityApiFilter));
        }

        [TestMethod]
        public void FindResource()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Resource1", ApiUrl = "Get/123", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Resource2", ApiUrl = "Get/123", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter2));
        }

        [TestMethod]
        public void FindApi()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Api", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Api", ApiUrl = "Register", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter2));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Hoge", ApiUrl = "Hoge", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(HighPriorityApiFilter));
        }

        [TestMethod]
        public void FindApiRegex()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/ApiR", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/ApiR", ApiUrl = "Regist", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/ApiR", ApiUrl = "Delete", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter2));
        }

        [TestMethod]
        public void FindHttpMethod()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Method", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Method", ApiUrl = "Get", HttpMethodType = "Post", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter2));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Hoge", ApiUrl = "Get", HttpMethodType = "Delete", Action = "Query" });
            x.Single().GetType().Is(typeof(HighPriorityApiFilter));
        }

        [TestMethod]
        public void FindHttpMethodRegex()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/MethodR", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/MethodR", ApiUrl = "Get", HttpMethodType = "Delete", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter2));
        }

        [TestMethod]
        public void FindAction()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Regist" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Delete" });
            x.Single().GetType().Is(typeof(ApiFilter2));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Hoge", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(HighPriorityApiFilter));
        }

        [TestMethod]
        public void FindActionRegex()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/ActionR", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/ActionR", ApiUrl = "Get", HttpMethodType = "Get", Action = "Delete" });
            x.Single().GetType().Is(typeof(ApiFilter2));
        }

        [TestMethod]
        public void FindActionRequestVendorSystem()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/VendorSystem/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query", RequestVendorId = "VendorA", RequestSystemId = "SystemA" });
            x.Single().GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/VendorSystem/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query", RequestVendorId = "VendorA", RequestSystemId = "SystemB" });
            x.IsNull();

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/System/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query", RequestVendorId = "VendorA", RequestSystemId = "SystemA" });
            x.Single().GetType().Is(typeof(ApiFilter2));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/System/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query", RequestVendorId = "VendorA", RequestSystemId = "SystemB" });
            x.IsNull();

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Vendor/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query", RequestVendorId = "VendorB", RequestSystemId = "SystemA" });
            x.Single().GetType().Is(typeof(ApiFilter3));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/Vendor/Action", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query", RequestVendorId = "VendorA", RequestSystemId = "SystemA" });
            x.IsNull();
        }


        [TestMethod]
        public void OrderedFilter_正常系()
        {
            var x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/OrderedActionA", ApiUrl = "Get", HttpMethodType = "Get", Action = "Query" });
            x.Count().Is(3);
            x[0].GetType().Is(typeof(ApiFilter3));
            x[1].GetType().Is(typeof(ApiFilter1));
            x[2].GetType().Is(typeof(ApiFilter2));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/OrderedActionB", ApiUrl = "Get", HttpMethodType = "Get", Action = "Delete" });
            x.Count().Is(4);
            x[0].GetType().Is(typeof(ApiFilter3));
            x[1].GetType().Is(typeof(ApiFilter3));
            x[2].GetType().Is(typeof(ApiFilter1));
            x[3].GetType().Is(typeof(ApiFilter1));

            x = _filterManager.GetApiFilter(new ApiFilterActionParam() { ResourceUrl = "/API/Public/OrderedActionC", ApiUrl = "Get", HttpMethodType = "Get", Action = "Delete" });
            x.Count().Is(2);
            x[0].GetType().Is(typeof(ApiFilter2));
            x[1].GetType().Is(typeof(ApiFilter1));
        }
    }
}
