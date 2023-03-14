using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// ベンダーとシステムのIsEnable・IsActiveの値によるApiの実行可否をテストする。
    /// </summary>
    [TestClass]
    public class VenderOrSystemDeletingOrDisabledTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// ベンダーとシステムのIsEnable・IsActiveが全てTrueの場合Apiを実行可能
        /// </summary>
        [TestMethod]
        public void NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IVendorOrSystemDeletingOrDisabledApi>();

            // ベンダーのIsEnable：True
            // ベンダーのIsActive：True
            // システムのIsEnable：True
            // システムのIsActive：True
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetErrorExpectStatusCode);
        }

        /// <summary>
        /// ベンダーとシステムのIsEnable・IsActiveのいずれかがFalseの場合実行不可(501 Not Implemented)
        /// </summary>
        [TestMethod]
        public void NotImplementedSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);

            // ベンダーのIsEnable：True
            // ベンダーのIsActive：True
            // システムのIsEnable：False
            // システムのIsActive：True
            var systemDisabled = UnityCore.Resolve<IVendorOrSystemDeletingOrDisabledSystemDisabledApi>();
            client.GetWebApiResponseResult(systemDisabled.GetAll()).Assert(NotImplementedExpectStatusCode);

            // ベンダーのIsEnable：True
            // ベンダーのIsActive：True
            // システムのIsEnable：True
            // システムのIsActive：False
            var systemDeleted = UnityCore.Resolve<IVendorOrSystemDeletingOrDisabledSystemDeletedApi>();
            client.GetWebApiResponseResult(systemDeleted.GetAll()).Assert(NotImplementedExpectStatusCode);

            // ベンダーのIsEnable：False
            // ベンダーのIsActive：True
            // システムのIsEnable：True
            // システムのIsActive：True
            var vendorDisabled = UnityCore.Resolve<IVendorOrSystemDeletingOrDisabledVendorDisabledApi>();
            client.GetWebApiResponseResult(vendorDisabled.GetAll()).Assert(NotImplementedExpectStatusCode);

            // ベンダーのIsEnable：True
            // ベンダーのIsActive：False
            // システムのIsEnable：True
            // システムのIsActive：True
            var vendorDeleted = UnityCore.Resolve<IVendorOrSystemDeletingOrDisabledVendorDeletedApi>();
            client.GetWebApiResponseResult(vendorDeleted.GetAll()).Assert(NotImplementedExpectStatusCode);
        }
    }
}
