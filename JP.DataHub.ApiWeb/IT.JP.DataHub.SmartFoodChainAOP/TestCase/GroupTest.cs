using System.Net;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    /// <summary>
    /// グループAOPがDEBUGビルドの場合、"X-GroupFilterTest"を指定すると対象APIを実行せずメンバーとグループのOpenIDが返されるようになっている。
    /// 本テストでは"X-GroupFilterTest"を指定したリクエストにより、各APIにグループAPIが適用されていることを確認する。
    /// グループAOPがRELEASEビルドの場合は失敗するため、RELEASEの環境では実行しないこと。
    /// </summary>
    [TestClass]
    public class GroupTest : ItTestCaseBase
    {
        private string TestGroupId = "__IntegratedTestGroupId";

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        /// <summary>
        /// JASv3の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/Traceability/JasFreshnessManagement/V3/Public/JasPrint", "Get", "GetPrintableCount?product=hoge&gln=hoge&groupId={0}")]
        [DataRow("/API/Traceability/JasFreshnessManagement/V3/Public/JasPrint", "Post", "RegisterPrintLog?groupId={0}")]
        [DataRow("/API/Traceability/JasFreshnessManagement/V3/Public/JasPrint", "Post", "RegisterRePrintLog?groupId={0}")]
        //[DataRow("/API/Traceability/JasFreshnessManagement/V3/Private/JasPrintLog")] ※内部呼び出し専用
        public void GroupTest_JasV3(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// センサーv3の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/Sensing/V3/Private/Observations", "Get", "Get?ThingsId=hoge&DatastreamId=hoge&StartDatetime=hoge&EndDatetime=hoge&groupId={0}")]
        [DataRow("/API/Sensing/V3/Private/CaptureMap")]
        [DataRow("/API/Sensing/V3/Private/Datastreams")]
        [DataRow("/API/Sensing/V3/Private/FeaturesOfInterest")]
        [DataRow("/API/Sensing/V3/Private/Locations")]
        [DataRow("/API/Sensing/V3/Private/ObservationConditions")]
        [DataRow("/API/Sensing/V3/Private/SensorDevice")]
        [DataRow("/API/Sensing/V3/Private/SensorDeviceCapture")]
        [DataRow("/API/Sensing/V3/Private/Things")]
        public void GroupTest_SensorV3(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// SmartFoodChainv2の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/SmartFoodChain/V2/Master/CropBrand")]
        [DataRow("/API/SmartFoodChain/V2/Master/ProductSize")]
        [DataRow("/API/SmartFoodChain/V2/Private/Arrival")]
        [DataRow("/API/SmartFoodChain/V2/Private/AuthenticationPartyOffice")]
        [DataRow("/API/SmartFoodChain/V2/Private/Customer")]
        [DataRow("/API/SmartFoodChain/V2/Private/DeliveryDetail")]
        [DataRow("/API/SmartFoodChain/V2/Private/FarmerCertification")]
        [DataRow("/API/SmartFoodChain/V2/Private/Fmis/Farmer")]
        [DataRow("/API/SmartFoodChain/V2/Private/Grade")]
        [DataRow("/API/SmartFoodChain/V2/Private/Packaging")]
        [DataRow("/API/SmartFoodChain/V2/Private/PartyPlace")]
        [DataRow("/API/SmartFoodChain/V2/Private/PartyProduct")]
        [DataRow("/API/SmartFoodChain/V2/Private/POS")]
        [DataRow("/API/SmartFoodChain/V2/Private/ProductAppeal")]
        [DataRow("/API/SmartFoodChain/V2/Private/ProductCodeDetail")]
        [DataRow("/API/SmartFoodChain/V2/Private/ProductComment")]
        [DataRow("/API/SmartFoodChain/V2/Private/PurchaseOrder")]
        [DataRow("/API/SmartFoodChain/V2/Private/PurchaseOrderMap")]
        [DataRow("/API/SmartFoodChain/V2/Private/SalesReport")]
        [DataRow("/API/SmartFoodChain/V2/Private/Shipment")]
        [DataRow("/API/SmartFoodChain/V2/Private/ShipmentReference")]
        [DataRow("/API/SmartFoodChain/V2/Private/ShipmentSensor")]
        [DataRow("/API/SmartFoodChain/V2/Private/ShippingForecast")]
        [DataRow("/API/SmartFoodChain/V2/Private/StorageLocation")]
        [DataRow("/API/SmartFoodChain/V2/Private/StorageLocationSensors")]
        public void GroupTest_SmartFoodChainV2(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// SmartFoodChainv3の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/Traceability/V3/Private/CompanyProductEx", "Get", "GetProductCount/hoge?fb=hoge&fv=hoge&groupId={0}")]
        [DataRow("/API/Traceability/V3/Private/CompanyProductEx", "Get", "GetProductList/hoge?oid=hoge&fb=hoge&fv=hoge&ob=hoge&ds=hoge&of=hoge&ft=hoge&groupId={0}")]
        [DataRow("/API/Traceability/V3/Private/CompanyProductEx", "Get", "GetProduct/hoge&groupId={0}")]
        [DataRow("/API/Traceability/V3/Master/CropBrand")]
        [DataRow("/API/Traceability/V3/Master/Grade")]
        [DataRow("/API/Traceability/V3/Master/ProductSize")]
        [DataRow("/API/Traceability/V3/Private/Arrival")]
        [DataRow("/API/Traceability/V3/Private/CompanyPlace")]
        [DataRow("/API/Traceability/V3/Private/CompanyProduct")]
        [DataRow("/API/Traceability/V3/Private/Customer")]
        [DataRow("/API/Traceability/V3/Private/DeliveryDetail")]
        [DataRow("/API/Traceability/V3/Private/Fmis/Cultivation")]
        [DataRow("/API/Traceability/V3/Private/Fmis/Farmer")]
        [DataRow("/API/Traceability/V3/Private/Fmis/Field")]
        [DataRow("/API/Traceability/V3/Private/Fmis/Planting")]
        [DataRow("/API/Traceability/V3/Private/Fmis/Tree")]
        [DataRow("/API/Traceability/V3/Private/Packaging")]
        [DataRow("/API/Traceability/V3/Private/POS")]
        [DataRow("/API/Traceability/V3/Private/ProductAppeal")]
        [DataRow("/API/Traceability/V3/Private/ProductComment")]
        [DataRow("/API/Traceability/V3/Private/ProductDetail")]
        [DataRow("/API/Traceability/V3/Private/ProductDetailSensorMap")]
        [DataRow("/API/Traceability/V3/Private/PurchaseOrder")]
        [DataRow("/API/Traceability/V3/Private/PurchaseOrderMap")]
        [DataRow("/API/Traceability/V3/Private/SalesReport")]
        [DataRow("/API/Traceability/V3/Private/Shipment")]
        [DataRow("/API/Traceability/V3/Private/ShipmentReference")]
        [DataRow("/API/Traceability/V3/Private/ShipmentSensor")]
        [DataRow("/API/Traceability/V3/Private/ShippingForecast")]
        [DataRow("/API/Traceability/V3/Private/StorageLocation")]
        [DataRow("/API/Traceability/V3/Private/StorageLocationSensors")]
        [DataRow("/API/Traceability/V3/Private/Trace")]
        [DataRow("/API/Traceability/V3/Private/TraceManage")]
        [DataRow("/API/Traceability/V3/Private/ProductLotDetail")]
        public void GroupTest_SmartFoodChainV3(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// 認証ポータルv1の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/CertificationPortal/Private/Accreditation")]
        [DataRow("/API/CertificationPortal/Private/AccreditationCertificationMap")]
        [DataRow("/API/CertificationPortal/Private/ApplicantCompany")]
        [DataRow("/API/CertificationPortal/Private/ApplicantOffice")]
        [DataRow("/API/CertificationPortal/Private/CertificationApply")]
        [DataRow("/API/CertificationPortal/Private/CertificationApplyAdditionalItem")]
        [DataRow("/API/CertificationPortal/Private/CertificationQuestionnaire")]
        [DataRow("/API/CertificationPortal/Private/CertificationQuestionnaireClassification")]
        [DataRow("/API/CertificationPortal/Private/CertificationQuestionnaireQuestion")]
        [DataRow("/API/CertificationPortal/Private/CorrectAnswer")]
        [DataRow("/API/CertificationPortal/Private/TemporaryCertification")]
        [DataRow("/API/CertificationPortal/Private/TemporaryCertificationData")]
        [DataRow("/API/CertificationPortal/Private/CertificationIssueHistory")]
        public void GroupTest_CertificationPortalV1(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// 認証ポータルv2の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/CertificationPortal/V2/Private/Accreditation")]
        [DataRow("/API/CertificationPortal/V2/Private/AccreditationCertificationMap")]
        [DataRow("/API/CertificationPortal/V2/Private/CertificationApply")]
        [DataRow("/API/CertificationPortal/V2/Private/CertificationApplyAdditionalItem")]
        [DataRow("/API/CertificationPortal/V2/Private/CertificationQuestionnaire")]
        [DataRow("/API/CertificationPortal/V2/Private/CertificationQuestionnaireClassification")]
        [DataRow("/API/CertificationPortal/V2/Private/CertificationQuestionnaireQuestion")]
        [DataRow("/API/CertificationPortal/V2/Private/CorrectAnswer")]
        [DataRow("/API/CertificationPortal/V2/Private/TemporaryCertification")]
        [DataRow("/API/CertificationPortal/V2/Private/TemporaryCertificationData")]
        [DataRow("/API/CertificationPortal/V2/Private/CertificationIssueHistory")]
        public void GroupTest_CertificationPortalV2(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// 種苗v2の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/Private/SeedsManagement/V2/Application")]
        [DataRow("/API/Private/SeedsManagement/V2/Approve")]
        [DataRow("/API/Private/SeedsManagement/V2/ApproveAgency")]
        [DataRow("/API/Private/SeedsManagement/V2/ApproveAgencyStaff")]
        [DataRow("/API/Private/SeedsManagement/V2/Authorization")]
        [DataRow("/API/Private/SeedsManagement/V2/AuthorizationAgency")]
        [DataRow("/API/Private/SeedsManagement/V2/AuthorizationAgencyStaff")]
        public void GroupTest_SeedsManagementV2(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// 事業社v3の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/CompanyMaster/V3/Private/Company")]
        [DataRow("/API/CompanyMaster/V3/Private/CompanyUrl")]
        [DataRow("/API/CompanyMaster/V3/Private/Office")]
        [DataRow("/API/CompanyMaster/V3/Private/Staff")]
        [DataRow("/API/CompanyMaster/V3/Private/StaffRole")]
        [DataRow("/API/CompanyMaster/V3/Private/CompanyCertified")]
        [DataRow("/API/CompanyMaster/V3/Private/OfficeCertified")]
        public void GroupTest_CompanyMasterV3(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// こども食堂v2の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/Donation/V2/Private/Donors")]
        [DataRow("/API/Donation/V2/Private/Foods")]
        public void GroupTest_DonationV2(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        /// <summary>
        /// 共同物流v2の各APIにグループAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        [DataRow("/API/JointDelivery/V2/Private/Office")]
        [DataRow("/API/JointDelivery/V2/Private/Product")]
        [DataRow("/API/JointDelivery/V2/Private/ShippingRequest")]
        [DataRow("/API/JointDelivery/V2/Private/ShippingRequestConfirm")]
        [DataRow("/API/JointDelivery/V2/Private/ShippingRequestTruck")]
        public void GroupTest_JointDeliveryV2(string apiUrl, string method = null, string methodUrl = null)
        {
            Test(apiUrl, method, methodUrl);
        }

        public void Test(string apiUrl, string method = null, string methodUrl = null)
        {
            // グループメンバーでAPI実行
            var member = new IntegratedTestClient("test2");
            var api = new GroupTargetResource(UnityCore.Resolve<IServerEnvironment>());
            api.ResourceUrl = apiUrl;
            api.AddHeaders.Add("X-GroupFilterTest", "true");
            var request = api.GetList(TestGroupId);

            if (!string.IsNullOrEmpty(methodUrl))
            {
                request.HttpMethod = new HttpMethod(method);
                request.Action = string.Format(methodUrl, TestGroupId);
            }

            var response = member.Request(request).ToWebApiResponseResult<GroupTargetResponseModel>();
            response.StatusCode.Is(HttpStatusCode.OK);
            response.Result.GroupOpenId.IsNot(response.Result.MemberOpenId);
        }
    }
}
