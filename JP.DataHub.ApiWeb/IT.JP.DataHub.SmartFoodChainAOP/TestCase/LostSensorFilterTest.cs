using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class LostSensorFilterTest : ItTestCaseBase
    {
        public const string ShipmentId = "57029213-411d-4ae6-bdfa-aa00ebf9539b";
        public const string ShipmentSensorId = "878ce661-2056-4625-bdcc-af4851ac4d4c";


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


        [TestMethod]
        public void LostSensorFilterTest_NormalSenario()
        {
            var shipper = new IntegratedTestClient("test1");
            var receiver = new IntegratedTestClient("test2");

            var shipment = UnityCore.Resolve<IShipmentApi>();
            var shipmentSensor = UnityCore.Resolve<IShipmentSensorApi>();

            // データ削除
            DeleteTestData(shipper, receiver);

            // データ登録
            // 出荷
            shipper.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                ShipmentId = ShipmentId,
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = Guid.NewGuid().ToString(),
                    ShipmentOfficeId = Guid.NewGuid().ToString(),
                    ShipmentGln = Guid.NewGuid().ToString()
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = Guid.NewGuid().ToString(),
                    ShippingOfficeId = Guid.NewGuid().ToString(),
                    ShippingGln = Guid.NewGuid().ToString()
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = Guid.NewGuid().ToString(),
                    DeliveryOfficeId = Guid.NewGuid().ToString(),
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = Guid.NewGuid().ToString(),
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = null
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 1),
                DeliveryDate = new DateTime(2021, 12, 1),
                ShipmentTypeCode = "PRD",
                ProducingAreaCode = "011002",
                Message = "LostSensorFilterIT",
            })).Assert(RegisterSuccessExpectStatusCode);

            // 出荷のセンサー付帯
            shipper.GetWebApiResponseResult(shipmentSensor.Register(new ShipmentSensorModel()
            {
                ShipmentSensorId = ShipmentSensorId,
                ShipmentId = ShipmentId,
                SensorDeviceId = Guid.NewGuid().ToString(),
                ProductCode = new List<string>() { Guid.NewGuid().ToString() },
                MeasurementStartDateTime = null,
                MeasurementEndDateTime = null,
                LastArrivalId = null,
                IsLostSensor = false
            })).Assert(RegisterSuccessExpectStatusCode);

            // ShipmentSensorIdなし
            receiver.GetWebApiResponseResult(shipmentSensor.Lost(null)).AssertErrorCode(BadRequestStatusCode, "E106403");

            // 存在しないShipmentSensorId
            receiver.GetWebApiResponseResult(shipmentSensor.Lost(Guid.NewGuid().ToString())).AssertErrorCode(BadRequestStatusCode, "E106405");

            // センサーロスト
            receiver.GetWebApiResponseResult(shipmentSensor.Lost(ShipmentSensorId)).Assert(DeleteSuccessStatusCode);

            // ロストフラグがtrueになっている
            var updatedShipmentSensor = shipper.GetWebApiResponseResult(shipmentSensor.Get(ShipmentSensorId)).Assert(GetSuccessExpectStatusCode);
            updatedShipmentSensor.Result.IsLostSensor.Value.IsTrue();

            // データ削除
            DeleteTestData(shipper, receiver);
        }

        private void DeleteTestData(IDynamicApiClient shipper, IDynamicApiClient receiver)
        {
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var shipmentSensor = UnityCore.Resolve<IShipmentSensorApi>();

            shipper.GetWebApiResponseResult(shipment.Delete(ShipmentId)).Assert(DeleteExpectStatusCodes);
            shipper.GetWebApiResponseResult(shipmentSensor.Delete(ShipmentSensorId)).Assert(DeleteExpectStatusCodes);
        }
    }
}
