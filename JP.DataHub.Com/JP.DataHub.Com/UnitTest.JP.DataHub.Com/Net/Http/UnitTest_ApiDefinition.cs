using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Net.Http;
using UnitTest.JP.DataHub.Com.WebApi;

namespace UnitTest.JP.DataHub.Com.Net.Http
{
    [TestClass]
    public class UnitTest_ApiDefinition : ComUnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        public void ApiGenerate()
        {
            var brandname = UnityCore.Resolve<IBrandNameApi>();
            var getlist = brandname.GetList();
            getlist.HttpMethod.Is(HttpMethod.Get);
            getlist.Action.Is("GetList?");
            getlist.ResourceUrl.Is("/API/Master/BrandName");

            var register = brandname.Register(new BrandNameModel());
            register.HttpMethod.Is(HttpMethod.Post);
            register.Action.Is("Register?");
            register.ResourceUrl.Is("/API/Master/BrandName");

            var delete = brandname.DeleteAll();
            delete.HttpMethod.Is(HttpMethod.Delete);
            delete.Action.Is("DeleteAll?");
            delete.ResourceUrl.Is("/API/Master/BrandName");

            var update = brandname.Update(null, null);
            update.HttpMethod.Is(HttpMethod.Patch);
            update.Action.Is("Update/?");
            update.ResourceUrl.Is("/API/Master/BrandName");
        }

        [TestMethod]
        public void ApiGenerate_NoImpliment()
        {
            UnityCore.UnityContainer.BuildupApiDifinition();
            var brandname = UnityCore.Resolve<INoImplBrandName>();

            var getlist = brandname.GetList();
            getlist.HttpMethod.Is(HttpMethod.Get);
            getlist.Action.Is("GetList?");
            getlist.ResourceUrl.Is("/API/Master/BrandName");

            var register = brandname.Register(new NoImplBrandNameModel());
            register.HttpMethod.Is(HttpMethod.Post);
            register.Action.Is("Register?");
            register.ResourceUrl.Is("/API/Master/BrandName");

            var delete = brandname.DeleteAll();
            delete.HttpMethod.Is(HttpMethod.Delete);
            delete.Action.Is("DeleteAll?");
            delete.ResourceUrl.Is("/API/Master/BrandName");

            var update = brandname.Update(null, null);
            update.HttpMethod.Is(HttpMethod.Patch);
            update.Action.Is("Update/?");
            update.ResourceUrl.Is("/API/Master/BrandName");
        }
    }
}