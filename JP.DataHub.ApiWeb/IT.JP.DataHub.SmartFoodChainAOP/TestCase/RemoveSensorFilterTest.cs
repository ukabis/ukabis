using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class RemoveSensorFilterTest : ItTestCaseBase
    {
        public const string ShipmentId = "0cec8b38-ac6d-4714-afce-b5cd1581a55f";
        public const string ShipmentSensorId = "3b05ebfe-e5e1-42be-81e5-4591a1c8c5fc";
        public const string ArrivalId = "7efb8688-71b2-4309-b294-8de45cfb6915";


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
        public void RemoveSensorFilterTest_NormalSenario()
        {
            var shipper = new IntegratedTestClient("test1");
            var receiver = new IntegratedTestClient("test2");

            var shipment = UnityCore.Resolve<IShipmentApi>();
            var shipmentSensor = UnityCore.Resolve<IShipmentSensorApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();

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
                Message = "RemoveSensorFilterIT",
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

            // 入荷
            var arrivalDate = new DateTime(2021, 12, 2);
            receiver.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ArrivalId = ArrivalId,
                ShipmentTypeCode = "PUP",
                ArrivalDate = arrivalDate,
                InvoiceCode = "RemoveSensorFilterIT",
                ShipmentId = ShipmentId,
                ShipmentCompanyId = Guid.NewGuid().ToString(),
                ShipmentOfficeId = Guid.NewGuid().ToString(),
                ShipmentGln = Guid.NewGuid().ToString(),
                ArrivalCompanyId = Guid.NewGuid().ToString(),
                ArrivalOfficeId = Guid.NewGuid().ToString(),
                ArrivalGln = Guid.NewGuid().ToString(),
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = Guid.NewGuid().ToString(),
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);

            // ShipmentSensorIdなし
            receiver.GetWebApiResponseResult(shipmentSensor.RemoveSensor(null, ArrivalId)).AssertErrorCode(BadRequestStatusCode, "E106403");

            // LastArrivalIdなし
            receiver.GetWebApiResponseResult(shipmentSensor.RemoveSensor(ShipmentSensorId, null)).AssertErrorCode(BadRequestStatusCode, "E106404");

            // 存在しないShipmentSensorId
            receiver.GetWebApiResponseResult(shipmentSensor.RemoveSensor(Guid.NewGuid().ToString(), ArrivalId)).AssertErrorCode(BadRequestStatusCode, "E106405");

            // 存在しないLastArrivalId
            receiver.GetWebApiResponseResult(shipmentSensor.RemoveSensor(ShipmentSensorId, Guid.NewGuid().ToString())).AssertErrorCode(BadRequestStatusCode, "E106406");

            // 測定開始前
            receiver.GetWebApiResponseResult(shipmentSensor.RemoveSensor(ShipmentSensorId, ArrivalId)).AssertErrorCode(BadRequestStatusCode, "E106408");

            // 測定終了日時、最終入荷IDが設定されていない
            var updatedShipmentSensor = shipper.GetWebApiResponseResult(shipmentSensor.Get(ShipmentSensorId)).Assert(GetSuccessExpectStatusCode);
            updatedShipmentSensor.Result.MeasurementEndDateTime.IsNull();
            updatedShipmentSensor.Result.LastArrivalId.IsNull();

            // 測定開始
            shipper.GetWebApiResponseResult(shipmentSensor.Register(new ShipmentSensorModel()
            {
                ShipmentSensorId = ShipmentSensorId,
                ShipmentId = ShipmentId,
                SensorDeviceId = Guid.NewGuid().ToString(),
                ProductCode = new List<string>() { Guid.NewGuid().ToString() },
                MeasurementStartDateTime = DateTime.UtcNow,
                MeasurementEndDateTime = null,
                LastArrivalId = null,
                IsLostSensor = false
            })).Assert(RegisterSuccessExpectStatusCode);

            // 測定終了
            receiver.GetWebApiResponseResult(shipmentSensor.RemoveSensor(ShipmentSensorId, ArrivalId)).Assert(DeleteSuccessStatusCode);

            // 測定終了日時、最終入荷IDが設定されている
            updatedShipmentSensor = shipper.GetWebApiResponseResult(shipmentSensor.Get(ShipmentSensorId)).Assert(GetSuccessExpectStatusCode);
            updatedShipmentSensor.Result.MeasurementEndDateTime.Is(arrivalDate);
            updatedShipmentSensor.Result.LastArrivalId.Is(ArrivalId);

            // 測定終了済
            receiver.GetWebApiResponseResult(shipmentSensor.RemoveSensor(ShipmentSensorId, ArrivalId)).AssertErrorCode(BadRequestStatusCode, "E106408");

            // 測定終了日時、最終入荷IDに変更なし
            updatedShipmentSensor = shipper.GetWebApiResponseResult(shipmentSensor.Get(ShipmentSensorId)).Assert(GetSuccessExpectStatusCode);
            updatedShipmentSensor.Result.MeasurementEndDateTime.Is(arrivalDate);
            updatedShipmentSensor.Result.LastArrivalId.Is(ArrivalId);

            // データ削除
            DeleteTestData(shipper, receiver);
        }

        private void DeleteTestData(IDynamicApiClient shipper, IDynamicApiClient receiver)
        {
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var shipmentSensor = UnityCore.Resolve<IShipmentSensorApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();

            shipper.GetWebApiResponseResult(shipment.Delete(ShipmentId)).Assert(DeleteExpectStatusCodes);
            shipper.GetWebApiResponseResult(shipmentSensor.Delete(ShipmentSensorId)).Assert(DeleteExpectStatusCodes);

            receiver.GetWebApiResponseResult(arrival.Delete(ArrivalId)).Assert(DeleteExpectStatusCodes);
        }
    }
}
