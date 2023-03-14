using System.Net;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class GetShipmentAndArrivalFilterTest : ItTestCaseBase
    {        
        private const string ShipmentType = "shp";
        private const string ArrivalType = "arv";

        private const string CompanyId1 = "49b53b27-ec37-44e2-b3a0-76a92547e076";
        private const string CompanyId2 = "4dde1ad6-6d2b-4ddc-b2c0-8ce6c8726735";
        private const string CompanyId3 = "f2f59baa-67e7-4768-9938-d54b7b2367a6";
        private const string CompanyId4 = "aea25faf-c4af-4ac3-9073-cfe84bc6ee24";
        private const string CompanyId5 = "46f71d12-136a-46a2-a5fc-cca81541bf1e";

        private const string OfficeId1 = "aaab1ce2-febd-4768-8d4d-d5fc5b3c5311";
        private const string OfficeId2 = "73b8e5c9-50bf-4804-8467-73b2e1d138b8";
        private const string OfficeId3 = "9d4b7a3c-274e-4c4c-a90c-8d342ef2bed8";
        private const string OfficeId4 = "fd0adc1e-3106-4b79-99b2-32ea1cd638e4";
        private const string OfficeId5 = "cafee774-708e-4403-be21-ab0348875042";

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
        /// 対応パターン
        /// ・分荷
        /// ・シリアル変更
        /// </summary>
        [TestMethod]
        public void GetShipmentAndArrivalFilterTest_NormalScenario()
        {
            var client = new IntegratedTestClient("test1");
            var getShipmentAndArrival = UnityCore.Resolve<IGetShipmentAndArrivalApi>();
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();
            var companyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productDetail = UnityCore.Resolve<IProductDetailApi>();
            var company = UnityCore.Resolve<ICompanyApi>();
            var office = UnityCore.Resolve<IOfficeApi>();

            // データ削除
            DeleteTestData();

            // データ登録
            // 事業者
            client.GetWebApiResponseResult(company.RegisterList(CreateCompanyModel())).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(office.RegisterList(CreateOfficeModel())).Assert(RegisterSuccessExpectStatusCode);

            // 商品
            client.GetWebApiResponseResult(companyProduct.Register(new CompanyProductModel
            {
                GtinCode = "01ITGtin",
                CodeType = "GS1-128",
                CompanyId = CompanyId1,
                IsOrganic = false,
                ProductName = "FilterITProduct",
                RegistrationDate = "2021-11-12",
                Profile = new ProductProfileModel() { CropCode = "01010010", CropTypeCode = "headlettuce" }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(productDetail.RegisterList(new List<ProductDetailModel>
            {
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product1",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                },
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product2",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                }
            })).Assert(RegisterSuccessExpectStatusCode);

            // 入出荷パターン
            // 出荷1⇒入荷1⇒出荷2(商品コード変更あり)⇒入荷2
            //             ⇒出荷3(商品コード変更あり)⇒入荷3
            //             ⇒出荷4(商品コード変更なし)⇒入荷4

            // 出荷1
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                ShipmentId = null,
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId1,
                    ShipmentOfficeId = OfficeId1,
                    ShipmentGln = "FilterITPartyOfficeGln1"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId2,
                    ShippingOfficeId = OfficeId2,
                    ShippingGln = "FilterITPartyOfficeGln2"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId3,
                    DeliveryOfficeId = OfficeId3,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
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
                Message = "FilterIT",
            })).Assert(RegisterSuccessExpectStatusCode);
            var shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId1 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId1).ShipmentId;

            // 入荷1
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ArrivalId = null,
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 2),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId1,
                ShipmentCompanyId = CompanyId1,
                ShipmentOfficeId = OfficeId1,
                ShipmentGln = "FilterITPartyOfficeGln1",
                ArrivalCompanyId = CompanyId2,
                ArrivalOfficeId = OfficeId2,
                ArrivalGln = "FilterITPartyOfficeGln2",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            var arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId1 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId2).ArrivalId;

            // 出荷2
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId3,
                    ShippingOfficeId = OfficeId3,
                    ShippingGln = "FilterITPartyOfficeGln3"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId3,
                    DeliveryOfficeId = OfficeId3,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId1,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 3),
                DeliveryDate = new DateTime(2021, 12, 3),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string> { shipmentId1 },
                PreviousShipmentId = new List<string> { shipmentId1 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId2 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId2).ShipmentId;

            // 入荷2
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 4),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId2,
                ShipmentCompanyId = CompanyId2,
                ShipmentOfficeId = OfficeId2,
                ShipmentGln = "FilterITPartyOfficeGln2",
                ArrivalCompanyId = CompanyId3,
                ArrivalOfficeId = OfficeId3,
                ArrivalGln = "FilterITPartyOfficeGln3",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId2 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId3).ArrivalId;

            // 出荷3（別経路なので取れないことを確認）
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId4,
                    ShippingOfficeId = OfficeId4,
                    ShippingGln = "FilterITPartyOfficeGln4"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId3,
                    DeliveryOfficeId = OfficeId3,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId1,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 5),
                DeliveryDate = new DateTime(2021, 12, 5),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string> { shipmentId1 },
                PreviousShipmentId = new List<string> { shipmentId1 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId3 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId4).ShipmentId;

            // 入荷3（別経路なので取れないことを確認）
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "MKT",
                ArrivalDate = new DateTime(2021, 12, 6),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId3,
                ShipmentCompanyId = CompanyId2,
                ShipmentOfficeId = OfficeId2,
                ShipmentGln = "FilterITPartyOfficeGln2",
                ArrivalCompanyId = CompanyId4,
                ArrivalOfficeId = OfficeId4,
                ArrivalGln = "FilterITPartyOfficeGln4",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId3 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId4).ArrivalId;

            // GetShipmentAndArrivalを取得
            var result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product2", arrivalId2)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(4);
            // 出荷1
            result.Result[0].ProductCode.Is("01ITGtin21Product1");
            result.Result[0].DateTime.Is(new DateTime(2021, 12, 1));
            result.Result[0].Type.Is(ShipmentType);
            result.Result[0].Company.CompanyId.Is(CompanyId1);
            result.Result[0].Company.CompanyName.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test1");
            result.Result[0].Company.OfficeId.Is(OfficeId1);
            result.Result[0].Company.OfficeName.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice1");
            result.Result[0].Company.GlnCode.Is("FilterITPartyOfficeGln1");
            result.Result[0].ShipmentId.Is(shipmentId1);
            result.Result[0].ArrivalId.IsNull();
            result.Result[0].PreviousShipmentId.IsNull();
            result.Result[0].PackageQuantity.Is(100);
            // 入荷1
            result.Result[1].ProductCode.Is("01ITGtin21Product1");
            result.Result[1].DateTime.Is(new DateTime(2021, 12, 2));
            result.Result[1].Type.Is(ArrivalType);
            result.Result[1].Company.CompanyId.Is(CompanyId2);
            result.Result[1].Company.CompanyName.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[1].Company.OfficeId.Is(OfficeId2);
            result.Result[1].Company.OfficeName.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[1].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[1].ShipmentId.IsNull();
            result.Result[1].ArrivalId.Is(arrivalId1);
            result.Result[1].PreviousShipmentId.Is(shipmentId1);
            result.Result[1].PackageQuantity.Is(100);
            // 出荷2
            result.Result[2].ProductCode.Is("01ITGtin21Product2");
            result.Result[2].DateTime.Is(new DateTime(2021, 12, 3));
            result.Result[2].Type.Is(ShipmentType);
            result.Result[2].Company.CompanyId.Is(CompanyId2);
            result.Result[2].Company.CompanyName.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[2].Company.OfficeId.Is(OfficeId2);
            result.Result[2].Company.OfficeName.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[2].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[2].ShipmentId.Is(shipmentId2);
            result.Result[2].ArrivalId.IsNull();
            result.Result[2].PreviousShipmentId.Is(shipmentId1);
            result.Result[2].PackageQuantity.Is(100);
            // 入荷2
            result.Result[3].ProductCode.Is("01ITGtin21Product2");
            result.Result[3].DateTime.Is(new DateTime(2021, 12, 4));
            result.Result[3].Type.Is(ArrivalType);
            result.Result[3].Company.CompanyId.Is(CompanyId3);
            result.Result[3].Company.CompanyName.Is("テスト用事業者3");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[3].Company.OfficeId.Is(OfficeId3);
            result.Result[3].Company.OfficeName.Is("テスト用事業所3");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[3].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[3].ShipmentId.IsNull();
            result.Result[3].ArrivalId.Is(arrivalId2);
            result.Result[3].PreviousShipmentId.Is(shipmentId2);
            result.Result[3].PackageQuantity.Is(100);

            // 出荷4（商品コード不変でも取得できることを確認）
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId5,
                    ShippingOfficeId = OfficeId5,
                    ShippingGln = "FilterITPartyOfficeGln5"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId1,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 5),
                DeliveryDate = new DateTime(2021, 12, 5),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string> { shipmentId1 },
                PreviousShipmentId = new List<string> { shipmentId1 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId4 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId5).ShipmentId;

            // 入荷4（商品コード不変でも取得できることを確認）
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "MKT",
                ArrivalDate = new DateTime(2021, 12, 6),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId4,
                ShipmentCompanyId = CompanyId2,
                ShipmentOfficeId = OfficeId2,
                ShipmentGln = "FilterITPartyOfficeGln2",
                ArrivalCompanyId = CompanyId5,
                ArrivalOfficeId = OfficeId5,
                ArrivalGln = "FilterITPartyOfficeGln5",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId4 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId5).ArrivalId;

            // 入出荷(出荷1、入荷1、出荷4、入荷4)を取得
            // 同じ商品コードの入出荷(別経路)を取得することで、入荷取得と出荷取得にキャッシュが利用されていないことを確認する
            result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product1", arrivalId4)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(4);

            // 商品コードの誤り
            result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival(Guid.NewGuid().ToString(), arrivalId4)).AssertErrorCode(NotFoundStatusCode, "E104403");

            // データ削除
            DeleteTestData();
        }

        /// <summary>
        /// 対応パターン
        /// ・合流
        /// </summary>
        [TestMethod]
        public void GetShipmentAndArrivalFilterTest_AggregationScenario()
        {
            var client = new IntegratedTestClient("test1");
            var getShipmentAndArrival = UnityCore.Resolve<IGetShipmentAndArrivalApi>();
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();
            var companyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productDetail = UnityCore.Resolve<IProductDetailApi>();
            var company = UnityCore.Resolve<ICompanyApi>();
            var office = UnityCore.Resolve<IOfficeApi>();

            // データ削除
            DeleteTestData();

            // データ登録
            // 事業者
            client.GetWebApiResponseResult(company.RegisterList(CreateCompanyModel())).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(office.RegisterList(CreateOfficeModel())).Assert(RegisterSuccessExpectStatusCode);

            // 商品
            client.GetWebApiResponseResult(companyProduct.Register(new CompanyProductModel
            {
                GtinCode = "01ITGtin",
                CodeType = "GS1-128",
                CompanyId = CompanyId1,
                IsOrganic = false,
                ProductName = "FilterITProduct",
                RegistrationDate = "2021-11-12",
                Profile = new ProductProfileModel() { CropCode = "01010010", CropTypeCode = "headlettuce" }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(productDetail.RegisterList(new List<ProductDetailModel>
            {
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product1",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                },
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product2",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                }
            })).Assert(RegisterSuccessExpectStatusCode);

            // 入出荷パターン
            // 出荷1⇒入荷1⇒出荷3⇒入荷3
            // 出荷2⇒入荷2

            // 出荷1(Company1⇒Company3:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                ShipmentId = null,
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId1,
                    ShipmentOfficeId = OfficeId1,
                    ShipmentGln = "FilterITPartyOfficeGln1"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId3,
                    ShippingOfficeId = OfficeId3,
                    ShippingGln = "FilterITPartyOfficeGln3"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
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
                Message = "FilterIT",
            })).Assert(RegisterSuccessExpectStatusCode);
            var shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId1 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId1).ShipmentId;

            // 入荷1(Company1⇒Company3:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ArrivalId = null,
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 2),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId1,
                ShipmentCompanyId = CompanyId1,
                ShipmentOfficeId = OfficeId1,
                ShipmentGln = "FilterITPartyOfficeGln1",
                ArrivalCompanyId = CompanyId3,
                ArrivalOfficeId = OfficeId3,
                ArrivalGln = "FilterITPartyOfficeGln3",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            var arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId1 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId3 && x.ShipmentCompanyId == CompanyId1).ArrivalId;

            // 出荷2(Company2⇒Company3:Product2)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId3,
                    ShippingOfficeId = OfficeId3,
                    ShippingGln = "FilterITPartyOfficeGln3"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
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
                ShipmentDate = new DateTime(2021, 12, 3),
                DeliveryDate = new DateTime(2021, 12, 3),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT"
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId2 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId2).ShipmentId;

            // 入荷2(Company2⇒Company3:Product2)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 4),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId2,
                ShipmentCompanyId = CompanyId2,
                ShipmentOfficeId = OfficeId2,
                ShipmentGln = "FilterITPartyOfficeGln2",
                ArrivalCompanyId = CompanyId3,
                ArrivalOfficeId = OfficeId3,
                ArrivalGln = "FilterITPartyOfficeGln3",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId2 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId3 && x.ShipmentCompanyId == CompanyId2).ArrivalId;

            // 出荷3(Company3⇒Company4:Product1,Product2)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId3,
                    ShipmentOfficeId = OfficeId3,
                    ShipmentGln = "FilterITPartyOfficeGln3"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId4,
                    ShippingOfficeId = OfficeId4,
                    ShippingGln = "FilterITPartyOfficeGln4"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId1,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        },
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId2,
                                    ArrivalProductCode = "01ITGtin21Product2"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 5),
                DeliveryDate = new DateTime(2021, 12, 5),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string> { shipmentId1, shipmentId2 },
                PreviousShipmentId = new List<string> { shipmentId1, shipmentId2 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId3 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId4).ShipmentId;

            // 入荷3(Company3⇒Company4:Product1,Product2)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "MKT",
                ArrivalDate = new DateTime(2021, 12, 6),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId3,
                ShipmentCompanyId = CompanyId3,
                ShipmentOfficeId = OfficeId3,
                ShipmentGln = "FilterITPartyOfficeGln3",
                ArrivalCompanyId = CompanyId4,
                ArrivalOfficeId = OfficeId4,
                ArrivalGln = "FilterITPartyOfficeGln4",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        },
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product2",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId3 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId4).ArrivalId;

            // 指定した商品コードの経路のみが取得されること
            // GetShipmentAndArrivalを取得(01ITGtin21Product1)
            var result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product1", arrivalId3)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(4);
            // 出荷1
            result.Result[0].ProductCode.Is("01ITGtin21Product1");
            result.Result[0].DateTime.Is(new DateTime(2021, 12, 1));
            result.Result[0].Type.Is(ShipmentType);
            result.Result[0].Company.CompanyId.Is(CompanyId1);
            result.Result[0].Company.CompanyName.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test1");
            result.Result[0].Company.OfficeId.Is(OfficeId1);
            result.Result[0].Company.OfficeName.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice1");
            result.Result[0].Company.GlnCode.Is("FilterITPartyOfficeGln1");
            result.Result[0].ShipmentId.Is(shipmentId1);
            result.Result[0].ArrivalId.IsNull();
            result.Result[0].PreviousShipmentId.IsNull();
            result.Result[0].PackageQuantity.Is(100);
            // 入荷1
            result.Result[1].ProductCode.Is("01ITGtin21Product1");
            result.Result[1].DateTime.Is(new DateTime(2021, 12, 2));
            result.Result[1].Type.Is(ArrivalType);
            result.Result[1].Company.CompanyId.Is(CompanyId3);
            result.Result[1].Company.CompanyName.Is("テスト用事業者3");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[1].Company.OfficeId.Is(OfficeId3);
            result.Result[1].Company.OfficeName.Is("テスト用事業所3");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[1].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[1].ShipmentId.IsNull();
            result.Result[1].ArrivalId.Is(arrivalId1);
            result.Result[1].PreviousShipmentId.Is(shipmentId1);
            result.Result[1].PackageQuantity.Is(100);
            // 出荷3
            result.Result[2].ProductCode.Is("01ITGtin21Product1");
            result.Result[2].DateTime.Is(new DateTime(2021, 12, 5));
            result.Result[2].Type.Is(ShipmentType);
            result.Result[2].Company.CompanyId.Is(CompanyId3);
            result.Result[2].Company.CompanyName.Is("テスト用事業者3");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[2].Company.OfficeId.Is(OfficeId3);
            result.Result[2].Company.OfficeName.Is("テスト用事業所3");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[2].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[2].ShipmentId.Is(shipmentId3);
            result.Result[2].ArrivalId.IsNull();
            result.Result[2].PreviousShipmentId.Is(shipmentId1);
            result.Result[2].PackageQuantity.Is(100);
            // 入荷3
            result.Result[3].ProductCode.Is("01ITGtin21Product1");
            result.Result[3].DateTime.Is(new DateTime(2021, 12, 6));
            result.Result[3].Type.Is(ArrivalType);
            result.Result[3].Company.CompanyId.Is(CompanyId4);
            result.Result[3].Company.CompanyName.Is("テスト用事業者4");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者4");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test4");
            result.Result[3].Company.OfficeId.Is(OfficeId4);
            result.Result[3].Company.OfficeName.Is("テスト用事業所4");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所4");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice4");
            result.Result[3].Company.GlnCode.Is("FilterITPartyOfficeGln4");
            result.Result[3].ShipmentId.IsNull();
            result.Result[3].ArrivalId.Is(arrivalId3);
            result.Result[3].PreviousShipmentId.Is(shipmentId3);
            result.Result[3].PackageQuantity.Is(100);

            // GetShipmentAndArrivalを取得(01ITGtin21Product2)
            result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product2", arrivalId3)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(4);
            // 出荷2
            result.Result[0].ProductCode.Is("01ITGtin21Product2");
            result.Result[0].DateTime.Is(new DateTime(2021, 12, 3));
            result.Result[0].Type.Is(ShipmentType);
            result.Result[0].Company.CompanyId.Is(CompanyId2);
            result.Result[0].Company.CompanyName.Is("テスト用事業者2");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[0].Company.OfficeId.Is(OfficeId2);
            result.Result[0].Company.OfficeName.Is("テスト用事業所2");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[0].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[0].ShipmentId.Is(shipmentId2);
            result.Result[0].ArrivalId.IsNull();
            result.Result[0].PreviousShipmentId.IsNull();
            result.Result[0].PackageQuantity.Is(100);
            // 入荷2
            result.Result[1].ProductCode.Is("01ITGtin21Product2");
            result.Result[1].DateTime.Is(new DateTime(2021, 12, 4));
            result.Result[1].Type.Is(ArrivalType);
            result.Result[1].Company.CompanyId.Is(CompanyId3);
            result.Result[1].Company.CompanyName.Is("テスト用事業者3");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[1].Company.OfficeId.Is(OfficeId3);
            result.Result[1].Company.OfficeName.Is("テスト用事業所3");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[1].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[1].ShipmentId.IsNull();
            result.Result[1].ArrivalId.Is(arrivalId2);
            result.Result[1].PreviousShipmentId.Is(shipmentId2);
            result.Result[1].PackageQuantity.Is(100);
            // 出荷3
            result.Result[2].ProductCode.Is("01ITGtin21Product2");
            result.Result[2].DateTime.Is(new DateTime(2021, 12, 5));
            result.Result[2].Type.Is(ShipmentType);
            result.Result[2].Company.CompanyId.Is(CompanyId3);
            result.Result[2].Company.CompanyName.Is("テスト用事業者3");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[2].Company.OfficeId.Is(OfficeId3);
            result.Result[2].Company.OfficeName.Is("テスト用事業所3");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[2].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[2].ShipmentId.Is(shipmentId3);
            result.Result[2].ArrivalId.IsNull();
            result.Result[2].PreviousShipmentId.Is(shipmentId1);
            result.Result[2].PackageQuantity.Is(100);
            // 入荷3
            result.Result[3].ProductCode.Is("01ITGtin21Product2");
            result.Result[3].DateTime.Is(new DateTime(2021, 12, 6));
            result.Result[3].Type.Is(ArrivalType);
            result.Result[3].Company.CompanyId.Is(CompanyId4);
            result.Result[3].Company.CompanyName.Is("テスト用事業者4");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者4");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test4");
            result.Result[3].Company.OfficeId.Is(OfficeId4);
            result.Result[3].Company.OfficeName.Is("テスト用事業所4");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所4");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice4");
            result.Result[3].Company.GlnCode.Is("FilterITPartyOfficeGln4");
            result.Result[3].ShipmentId.IsNull();
            result.Result[3].ArrivalId.Is(arrivalId3);
            result.Result[3].PreviousShipmentId.Is(shipmentId3);
            result.Result[3].PackageQuantity.Is(100);

            // データ削除
            DeleteTestData();
        }

        /// <summary>
        /// 対応パターン
        /// ・出荷の入荷元設定漏れ
        /// </summary>
        [TestMethod]
        public void GetShipmentAndArrivalFilterTest_ImplicitTraceFromShipmentScenario()
        {
            var client = new IntegratedTestClient("test1");
            var getShipmentAndArrival = UnityCore.Resolve<IGetShipmentAndArrivalApi>();
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();
            var companyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productDetail = UnityCore.Resolve<IProductDetailApi>();
            var company = UnityCore.Resolve<ICompanyApi>();
            var office = UnityCore.Resolve<IOfficeApi>();

            // データ削除
            DeleteTestData();

            // データ登録
            // 事業者
            client.GetWebApiResponseResult(company.RegisterList(CreateCompanyModel())).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(office.RegisterList(CreateOfficeModel())).Assert(RegisterSuccessExpectStatusCode);

            // 商品
            client.GetWebApiResponseResult(companyProduct.Register(new CompanyProductModel
            {
                GtinCode = "01ITGtin",
                CodeType = "GS1-128",
                CompanyId = CompanyId1,
                IsOrganic = false,
                ProductName = "FilterITProduct",
                RegistrationDate = "2021-11-12",
                Profile = new ProductProfileModel() { CropCode = "01010010", CropTypeCode = "headlettuce" }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(productDetail.RegisterList(new List<ProductDetailModel>
            {
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product1",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                }
            })).Assert(RegisterSuccessExpectStatusCode);

            // 入出荷パターン
            // 出荷1⇒入荷1⇒出荷2(入荷1との紐付漏れ)⇒入荷2

            // 出荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                ShipmentId = null,
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId1,
                    ShipmentOfficeId = OfficeId1,
                    ShipmentGln = "FilterITPartyOfficeGln1"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId2,
                    ShippingOfficeId = OfficeId2,
                    ShippingGln = "FilterITPartyOfficeGln2"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
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
                Message = "FilterIT",
            })).Assert(RegisterSuccessExpectStatusCode);
            var shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId1 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId1).ShipmentId;

            // 入荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ArrivalId = null,
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 2),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId1,
                ShipmentCompanyId = CompanyId1,
                ShipmentOfficeId = OfficeId1,
                ShipmentGln = "FilterITPartyOfficeGln1",
                ArrivalCompanyId = CompanyId2,
                ArrivalOfficeId = OfficeId2,
                ArrivalGln = "FilterITPartyOfficeGln2",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            var arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId1 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId2).ArrivalId;

            // 出荷2(Company2⇒Company3:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId3,
                    ShippingOfficeId = OfficeId3,
                    ShippingGln = "FilterITPartyOfficeGln3"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
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
                ShipmentDate = new DateTime(2021, 12, 3),
                DeliveryDate = new DateTime(2021, 12, 3),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT"
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId2 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId3).ShipmentId;

            // 入荷2(Company2⇒Company3:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "MKT",
                ArrivalDate = new DateTime(2021, 12, 4),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId2,
                ShipmentCompanyId = CompanyId2,
                ShipmentOfficeId = OfficeId2,
                ShipmentGln = "FilterITPartyOfficeGln2",
                ArrivalCompanyId = CompanyId3,
                ArrivalOfficeId = OfficeId3,
                ArrivalGln = "FilterITPartyOfficeGln3",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId2 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId3).ArrivalId;

            // 紐付漏れがあっても経路が取得されること
            // GetShipmentAndArrivalを取得
            var result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product1", arrivalId2)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(4);
            // 出荷1
            result.Result[0].ProductCode.Is("01ITGtin21Product1");
            result.Result[0].DateTime.Is(new DateTime(2021, 12, 1));
            result.Result[0].Type.Is(ShipmentType);
            result.Result[0].Company.CompanyId.Is(CompanyId1);
            result.Result[0].Company.CompanyName.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test1");
            result.Result[0].Company.OfficeId.Is(OfficeId1);
            result.Result[0].Company.OfficeName.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice1");
            result.Result[0].Company.GlnCode.Is("FilterITPartyOfficeGln1");
            result.Result[0].ShipmentId.Is(shipmentId1);
            result.Result[0].ArrivalId.IsNull();
            result.Result[0].PreviousShipmentId.IsNull();
            result.Result[0].PackageQuantity.Is(100);
            // 入荷1
            result.Result[1].ProductCode.Is("01ITGtin21Product1");
            result.Result[1].DateTime.Is(new DateTime(2021, 12, 2));
            result.Result[1].Type.Is(ArrivalType);
            result.Result[1].Company.CompanyId.Is(CompanyId2);
            result.Result[1].Company.CompanyName.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[1].Company.OfficeId.Is(OfficeId2);
            result.Result[1].Company.OfficeName.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[1].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[1].ShipmentId.IsNull();
            result.Result[1].ArrivalId.Is(arrivalId1);
            result.Result[1].PreviousShipmentId.Is(shipmentId1);
            result.Result[1].PackageQuantity.Is(100);
            // 出荷2
            result.Result[2].ProductCode.Is("01ITGtin21Product1");
            result.Result[2].DateTime.Is(new DateTime(2021, 12, 3));
            result.Result[2].Type.Is(ShipmentType);
            result.Result[2].Company.CompanyId.Is(CompanyId2);
            result.Result[2].Company.CompanyName.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[2].Company.OfficeId.Is(OfficeId2);
            result.Result[2].Company.OfficeName.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[2].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[2].ShipmentId.Is(shipmentId2);
            result.Result[2].ArrivalId.IsNull();
            result.Result[2].PreviousShipmentId.IsNull();
            result.Result[2].PackageQuantity.Is(100);
            // 入荷2
            result.Result[3].ProductCode.Is("01ITGtin21Product1");
            result.Result[3].DateTime.Is(new DateTime(2021, 12, 4));
            result.Result[3].Type.Is(ArrivalType);
            result.Result[3].Company.CompanyId.Is(CompanyId3);
            result.Result[3].Company.CompanyName.Is("テスト用事業者3");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[3].Company.OfficeId.Is(OfficeId3);
            result.Result[3].Company.OfficeName.Is("テスト用事業所3");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[3].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[3].ShipmentId.IsNull();
            result.Result[3].ArrivalId.Is(arrivalId2);
            result.Result[3].PreviousShipmentId.Is(shipmentId2);
            result.Result[3].PackageQuantity.Is(100);

            // データ削除
            DeleteTestData();
        }

        /// <summary>
        /// 対応パターン
        /// ・入荷の出荷元紐付漏れ
        /// </summary>
        [TestMethod]
        public void GetShipmentAndArrivalFilterTest_ImplicitTraceFromArrivalScenario()
        {
            var client = new IntegratedTestClient("test1");
            var getShipmentAndArrival = UnityCore.Resolve<IGetShipmentAndArrivalApi>();
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();
            var companyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productDetail = UnityCore.Resolve<IProductDetailApi>();
            var company = UnityCore.Resolve<ICompanyApi>();
            var office = UnityCore.Resolve<IOfficeApi>();

            // データ削除
            DeleteTestData();

            // データ登録
            // 事業者
            client.GetWebApiResponseResult(company.RegisterList(CreateCompanyModel())).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(office.RegisterList(CreateOfficeModel())).Assert(RegisterSuccessExpectStatusCode);

            // 商品
            client.GetWebApiResponseResult(companyProduct.Register(new CompanyProductModel
            {
                GtinCode = "01ITGtin",
                CodeType = "GS1-128",
                CompanyId = CompanyId1,
                IsOrganic = false,
                ProductName = "FilterITProduct",
                RegistrationDate = "2021-11-12",
                Profile = new ProductProfileModel() { CropCode = "01010010", CropTypeCode = "headlettuce" }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(productDetail.RegisterList(new List<ProductDetailModel>
            {
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product1",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                }
            })).Assert(RegisterSuccessExpectStatusCode);

            // 入出荷パターン
            // 出荷1⇒入荷1⇒出荷2⇒入荷2(出荷2との紐付漏れ)⇒出荷3⇒入荷3

            // 出荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                ShipmentId = null,
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId1,
                    ShipmentOfficeId = OfficeId1,
                    ShipmentGln = "FilterITPartyOfficeGln1"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId2,
                    ShippingOfficeId = OfficeId2,
                    ShippingGln = "FilterITPartyOfficeGln2"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
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
                Message = "FilterIT",
            })).Assert(RegisterSuccessExpectStatusCode);
            var shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId1 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId1).ShipmentId;

            // 入荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ArrivalId = null,
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 2),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId1,
                ShipmentCompanyId = CompanyId1,
                ShipmentOfficeId = OfficeId1,
                ShipmentGln = "FilterITPartyOfficeGln1",
                ArrivalCompanyId = CompanyId2,
                ArrivalOfficeId = OfficeId2,
                ArrivalGln = "FilterITPartyOfficeGln2",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            var arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId1 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId2).ArrivalId;

            // 出荷2(Company2⇒Company3:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId3,
                    ShippingOfficeId = OfficeId3,
                    ShippingGln = "FilterITPartyOfficeGln3"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId1,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 3),
                DeliveryDate = new DateTime(2021, 12, 3),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string>() { shipmentId1 },
                PreviousShipmentId = new List<string>() { shipmentId1 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId2 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId3).ShipmentId;

            // 入荷2(Company2⇒Company3:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "MKT",
                ArrivalDate = new DateTime(2021, 12, 4),
                InvoiceCode = "FilterIT",
                ShipmentId = null,
                ShipmentCompanyId = null,
                ShipmentOfficeId = null,
                ShipmentGln = null,
                ArrivalCompanyId = CompanyId3,
                ArrivalOfficeId = OfficeId3,
                ArrivalGln = "FilterITPartyOfficeGln3",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId2 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId3).ArrivalId;

            // 出荷3(Company3⇒Company4:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId3,
                    ShipmentOfficeId = OfficeId3,
                    ShipmentGln = "FilterITPartyOfficeGln3"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId4,
                    ShippingOfficeId = OfficeId4,
                    ShippingGln = "FilterITPartyOfficeGln4"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId2,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 5),
                DeliveryDate = new DateTime(2021, 12, 5),
                ShipmentTypeCode = "MKT",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string>() { shipmentId1 },
                PreviousShipmentId = new List<string>() { shipmentId2 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId3 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId4).ShipmentId;

            // 入荷3(Company3⇒Company4:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "RTL",
                ArrivalDate = new DateTime(2021, 12, 6),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId3,
                ShipmentCompanyId = CompanyId3,
                ShipmentOfficeId = OfficeId3,
                ShipmentGln = "FilterITPartyOfficeGln3",
                ArrivalCompanyId = CompanyId4,
                ArrivalOfficeId = OfficeId4,
                ArrivalGln = "FilterITPartyOfficeGln4",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId3 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId4).ArrivalId;

            // 紐付漏れがあっても経路が取得されること
            // GetShipmentAndArrivalを取得
            var result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product1", arrivalId3)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(6);
            // 出荷1
            result.Result[0].ProductCode.Is("01ITGtin21Product1");
            result.Result[0].DateTime.Is(new DateTime(2021, 12, 1));
            result.Result[0].Type.Is(ShipmentType);
            result.Result[0].Company.CompanyId.Is(CompanyId1);
            result.Result[0].Company.CompanyName.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test1");
            result.Result[0].Company.OfficeId.Is(OfficeId1);
            result.Result[0].Company.OfficeName.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice1");
            result.Result[0].Company.GlnCode.Is("FilterITPartyOfficeGln1");
            result.Result[0].ShipmentId.Is(shipmentId1);
            result.Result[0].ArrivalId.IsNull();
            result.Result[0].PreviousShipmentId.IsNull();
            result.Result[0].PackageQuantity.Is(100);
            // 入荷1
            result.Result[1].ProductCode.Is("01ITGtin21Product1");
            result.Result[1].DateTime.Is(new DateTime(2021, 12, 2));
            result.Result[1].Type.Is(ArrivalType);
            result.Result[1].Company.CompanyId.Is(CompanyId2);
            result.Result[1].Company.CompanyName.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[1].Company.OfficeId.Is(OfficeId2);
            result.Result[1].Company.OfficeName.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[1].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[1].ShipmentId.IsNull();
            result.Result[1].ArrivalId.Is(arrivalId1);
            result.Result[1].PreviousShipmentId.Is(shipmentId1);
            result.Result[1].PackageQuantity.Is(100);
            // 出荷2
            result.Result[2].ProductCode.Is("01ITGtin21Product1");
            result.Result[2].DateTime.Is(new DateTime(2021, 12, 3));
            result.Result[2].Type.Is(ShipmentType);
            result.Result[2].Company.CompanyId.Is(CompanyId2);
            result.Result[2].Company.CompanyName.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[2].Company.OfficeId.Is(OfficeId2);
            result.Result[2].Company.OfficeName.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[2].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[2].ShipmentId.Is(shipmentId2);
            result.Result[2].ArrivalId.IsNull();
            result.Result[2].PreviousShipmentId.Is(shipmentId1);
            result.Result[2].PackageQuantity.Is(100);
            // 入荷2
            result.Result[3].ProductCode.Is("01ITGtin21Product1");
            result.Result[3].DateTime.Is(new DateTime(2021, 12, 4));
            result.Result[3].Type.Is(ArrivalType);
            result.Result[3].Company.CompanyId.Is(CompanyId3);
            result.Result[3].Company.CompanyName.Is("テスト用事業者3");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[3].Company.OfficeId.Is(OfficeId3);
            result.Result[3].Company.OfficeName.Is("テスト用事業所3");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[3].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[3].ShipmentId.IsNull();
            result.Result[3].ArrivalId.Is(arrivalId2);
            result.Result[3].PreviousShipmentId.IsNull();
            result.Result[3].PackageQuantity.Is(100);
            // 出荷3
            result.Result[4].ProductCode.Is("01ITGtin21Product1");
            result.Result[4].DateTime.Is(new DateTime(2021, 12, 5));
            result.Result[4].Type.Is(ShipmentType);
            result.Result[4].Company.CompanyId.Is(CompanyId3);
            result.Result[4].Company.CompanyName.Is("テスト用事業者3");
            result.Result[4].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者3");
            result.Result[4].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test3");
            result.Result[4].Company.OfficeId.Is(OfficeId3);
            result.Result[4].Company.OfficeName.Is("テスト用事業所3");
            result.Result[4].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所3");
            result.Result[4].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice3");
            result.Result[4].Company.GlnCode.Is("FilterITPartyOfficeGln3");
            result.Result[4].ShipmentId.Is(shipmentId3);
            result.Result[4].ArrivalId.IsNull();
            result.Result[4].PreviousShipmentId.Is(shipmentId2);
            result.Result[4].PackageQuantity.Is(100);
            // 入荷3
            result.Result[5].ProductCode.Is("01ITGtin21Product1");
            result.Result[5].DateTime.Is(new DateTime(2021, 12, 6));
            result.Result[5].Type.Is(ArrivalType);
            result.Result[5].Company.CompanyId.Is(CompanyId4);
            result.Result[5].Company.CompanyName.Is("テスト用事業者4");
            result.Result[5].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者4");
            result.Result[5].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test4");
            result.Result[5].Company.OfficeId.Is(OfficeId4);
            result.Result[5].Company.OfficeName.Is("テスト用事業所4");
            result.Result[5].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所4");
            result.Result[5].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice4");
            result.Result[5].Company.GlnCode.Is("FilterITPartyOfficeGln4");
            result.Result[5].ShipmentId.IsNull();
            result.Result[5].ArrivalId.Is(arrivalId3);
            result.Result[5].PreviousShipmentId.Is(shipmentId3);
            result.Result[5].PackageQuantity.Is(100);

            // データ削除
            DeleteTestData();
        }

        /// <summary>
        /// 対応パターン
        /// ・中抜け
        /// </summary>
        [TestMethod]
        public void GetShipmentAndArrivalFilterTest_MissingLinkScenario()
        {
            var client = new IntegratedTestClient("test1");
            var getShipmentAndArrival = UnityCore.Resolve<IGetShipmentAndArrivalApi>();
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();
            var companyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productDetail = UnityCore.Resolve<IProductDetailApi>();
            var company = UnityCore.Resolve<ICompanyApi>();
            var office = UnityCore.Resolve<IOfficeApi>();

            // データ削除
            DeleteTestData();

            // データ登録
            // 事業者
            client.GetWebApiResponseResult(company.RegisterList(CreateCompanyModel())).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(office.RegisterList(CreateOfficeModel())).Assert(RegisterSuccessExpectStatusCode);

            // 商品
            client.GetWebApiResponseResult(companyProduct.Register(new CompanyProductModel
            {
                GtinCode = "01ITGtin",
                CodeType = "GS1-128",
                CompanyId = CompanyId1,
                IsOrganic = false,
                ProductName = "FilterITProduct",
                RegistrationDate = "2021-11-12",
                Profile = new ProductProfileModel() { CropCode = "01010010", CropTypeCode = "headlettuce" }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(productDetail.RegisterList(new List<ProductDetailModel>
            {
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product1",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                }
            })).Assert(RegisterSuccessExpectStatusCode);

            // 入出荷パターン
            // 出荷1⇒入荷1⇒出荷2⇒入荷2(データなし)⇒出荷3(データなし)⇒入荷3

            // 出荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                ShipmentId = null,
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId1,
                    ShipmentOfficeId = OfficeId1,
                    ShipmentGln = "FilterITPartyOfficeGln1"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId2,
                    ShippingOfficeId = OfficeId2,
                    ShippingGln = "FilterITPartyOfficeGln2"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
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
                Message = "FilterIT",
            })).Assert(RegisterSuccessExpectStatusCode);
            var shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId1 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId1).ShipmentId;

            // 入荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ArrivalId = null,
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 2),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId1,
                ShipmentCompanyId = CompanyId1,
                ShipmentOfficeId = OfficeId1,
                ShipmentGln = "FilterITPartyOfficeGln1",
                ArrivalCompanyId = CompanyId2,
                ArrivalOfficeId = OfficeId2,
                ArrivalGln = "FilterITPartyOfficeGln2",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            var arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId1 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId2).ArrivalId;

            // 出荷2(Company2⇒Company3:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId3,
                    ShippingOfficeId = OfficeId3,
                    ShippingGln = "FilterITPartyOfficeGln3"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId1,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 3),
                DeliveryDate = new DateTime(2021, 12, 3),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string>() { shipmentId1 },
                PreviousShipmentId = new List<string>() { shipmentId1 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId2 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId3).ShipmentId;

            // 入荷3(Company3⇒Company4:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ShipmentTypeCode = "RTL",
                ArrivalDate = new DateTime(2021, 12, 6),
                InvoiceCode = "FilterIT",
                ShipmentId = null,
                ShipmentCompanyId = null,
                ShipmentOfficeId = null,
                ShipmentGln = null,
                ArrivalCompanyId = CompanyId4,
                ArrivalOfficeId = OfficeId4,
                ArrivalGln = "FilterITPartyOfficeGln4",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0,
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId3 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId4).ArrivalId;

            // 紐付漏れがあっても経路が取得されること
            // GetShipmentAndArrivalを取得
            var result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product1", arrivalId3)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(4);
            // 出荷1
            result.Result[0].ProductCode.Is("01ITGtin21Product1");
            result.Result[0].DateTime.Is(new DateTime(2021, 12, 1));
            result.Result[0].Type.Is(ShipmentType);
            result.Result[0].Company.CompanyId.Is(CompanyId1);
            result.Result[0].Company.CompanyName.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test1");
            result.Result[0].Company.OfficeId.Is(OfficeId1);
            result.Result[0].Company.OfficeName.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice1");
            result.Result[0].Company.GlnCode.Is("FilterITPartyOfficeGln1");
            result.Result[0].ShipmentId.Is(shipmentId1);
            result.Result[0].ArrivalId.IsNull();
            result.Result[0].PreviousShipmentId.IsNull();
            result.Result[0].PackageQuantity.Is(100);
            // 入荷1
            result.Result[1].ProductCode.Is("01ITGtin21Product1");
            result.Result[1].DateTime.Is(new DateTime(2021, 12, 2));
            result.Result[1].Type.Is(ArrivalType);
            result.Result[1].Company.CompanyId.Is(CompanyId2);
            result.Result[1].Company.CompanyName.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[1].Company.OfficeId.Is(OfficeId2);
            result.Result[1].Company.OfficeName.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[1].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[1].ShipmentId.IsNull();
            result.Result[1].ArrivalId.Is(arrivalId1);
            result.Result[1].PreviousShipmentId.Is(shipmentId1);
            result.Result[1].PackageQuantity.Is(100);
            // 出荷2
            result.Result[2].ProductCode.Is("01ITGtin21Product1");
            result.Result[2].DateTime.Is(new DateTime(2021, 12, 3));
            result.Result[2].Type.Is(ShipmentType);
            result.Result[2].Company.CompanyId.Is(CompanyId2);
            result.Result[2].Company.CompanyName.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[2].Company.OfficeId.Is(OfficeId2);
            result.Result[2].Company.OfficeName.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[2].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[2].ShipmentId.Is(shipmentId2);
            result.Result[2].ArrivalId.IsNull();
            result.Result[2].PreviousShipmentId.Is(shipmentId1);
            result.Result[2].PackageQuantity.Is(100);
            // 入荷3
            result.Result[3].ProductCode.Is("01ITGtin21Product1");
            result.Result[3].DateTime.Is(new DateTime(2021, 12, 6));
            result.Result[3].Type.Is(ArrivalType);
            result.Result[3].Company.CompanyId.Is(CompanyId4);
            result.Result[3].Company.CompanyName.Is("テスト用事業者4");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者4");
            result.Result[3].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test4");
            result.Result[3].Company.OfficeId.Is(OfficeId4);
            result.Result[3].Company.OfficeName.Is("テスト用事業所4");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所4");
            result.Result[3].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice4");
            result.Result[3].Company.GlnCode.Is("FilterITPartyOfficeGln4");
            result.Result[3].ShipmentId.IsNull();
            result.Result[3].ArrivalId.Is(arrivalId3);
            result.Result[3].PreviousShipmentId.IsNull();
            result.Result[3].PackageQuantity.Is(100);

            // データ削除
            DeleteTestData();
        }

        /// <summary>
        /// 対応パターン
        /// ・入荷データなし
        /// </summary>
        [TestMethod]
        public void GetShipmentAndArrivalFilterTest_NoArrival()
        {
            var client = new IntegratedTestClient("test1");
            var getShipmentAndArrival = UnityCore.Resolve<IGetShipmentAndArrivalApi>();
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();
            var companyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productDetail = UnityCore.Resolve<IProductDetailApi>();
            var company = UnityCore.Resolve<ICompanyApi>();
            var office = UnityCore.Resolve<IOfficeApi>();

            // データ削除
            DeleteTestData();

            // データ登録
            // 事業者
            client.GetWebApiResponseResult(company.RegisterList(CreateCompanyModel())).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(office.RegisterList(CreateOfficeModel())).Assert(RegisterSuccessExpectStatusCode);

            // 商品
            client.GetWebApiResponseResult(companyProduct.Register(new CompanyProductModel
            {
                GtinCode = "01ITGtin",
                CodeType = "GS1-128",
                CompanyId = CompanyId1,
                IsOrganic = false,
                ProductName = "FilterITProduct",
                RegistrationDate = "2021-11-12",
                Profile = new ProductProfileModel() { CropCode = "01010010", CropTypeCode = "headlettuce" }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(productDetail.RegisterList(new List<ProductDetailModel>
            {
                new ProductDetailModel
                {
                    GtinCode = "01ITGtin",
                    ProductCode = "01ITGtin21Product1",
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel {FoodRegistrationNo = "1"}
                }
            })).Assert(RegisterSuccessExpectStatusCode);

            // 入出荷パターン
            // 出荷1⇒入荷1⇒出荷2⇒入荷2(データなし)

            // 出荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                ShipmentId = null,
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId1,
                    ShipmentOfficeId = OfficeId1,
                    ShipmentGln = "FilterITPartyOfficeGln1"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId2,
                    ShippingOfficeId = OfficeId2,
                    ShippingGln = "FilterITPartyOfficeGln2"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
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
                Message = "FilterIT",
            })).Assert(RegisterSuccessExpectStatusCode);
            var shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId1 = shipmentResult.Result.First(x => x.Shipment.ShipmentCompanyId == CompanyId1).ShipmentId;

            // 入荷1(Company1⇒Company2:Product1)
            client.GetWebApiResponseResult(arrival.Register(new ArrivalModel()
            {
                ArrivalId = null,
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 2),
                InvoiceCode = "FilterIT",
                ShipmentId = shipmentId1,
                ShipmentCompanyId = CompanyId1,
                ShipmentOfficeId = OfficeId1,
                ShipmentGln = "FilterITPartyOfficeGln1",
                ArrivalCompanyId = CompanyId2,
                ArrivalOfficeId = OfficeId2,
                ArrivalGln = "FilterITPartyOfficeGln2",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 100,
                            DamagePackageQuantity = 0
                        }
                    },
            })).Assert(RegisterSuccessExpectStatusCode);
            var arrivalResult = client.GetWebApiResponseResult(arrival.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var arrivalId1 = arrivalResult.Result.First(x => x.ArrivalCompanyId == CompanyId2).ArrivalId;

            // 出荷2(Company2⇒Company3:Product1)
            client.GetWebApiResponseResult(shipment.Register(new ShipmentModel()
            {
                Shipment = new ShipmentCompanyModel()
                {
                    ShipmentCompanyId = CompanyId2,
                    ShipmentOfficeId = OfficeId2,
                    ShipmentGln = "FilterITPartyOfficeGln2"
                },
                Shipping = new ShippingModel()
                {
                    ShippingCompanyId = CompanyId3,
                    ShippingOfficeId = OfficeId3,
                    ShippingGln = "FilterITPartyOfficeGln3"
                },
                Delivery = new DeliveryModel()
                {
                    DeliveryCompanyId = CompanyId5,
                    DeliveryOfficeId = OfficeId5,
                    ShipmentMethodCode = "LTS"
                },
                IsInHouseDelivery = false,
                ShipmentProducts = new List<ShipmentProductModel>()
                    {
                        new ShipmentProductModel()
                        {
                            ProductCode = "01ITGtin21Product1",
                            InvoiceCode = "hoge",
                            Quantity = 1,
                            PackageQuantity = 100,
                            SinglePackageWeight = 100,
                            CapacityUnitCode = "KG",
                            PackagingId = null,
                            Message = "hoge",
                            ArrivalProductMap = new List<ShipmentArrivalProductMap>()
                            {
                                new ShipmentArrivalProductMap()
                                {
                                    ArrivalId = arrivalId1,
                                    ArrivalProductCode = "01ITGtin21Product1"
                                }
                            },
                        }
                    },
                InvoiceCode = "hoge",
                ShipmentDate = new DateTime(2021, 12, 3),
                DeliveryDate = new DateTime(2021, 12, 3),
                ShipmentTypeCode = "PUP",
                ProducingAreaCode = "011002",
                Message = "FilterIT",
                FirstShipmentId = new List<string>() { shipmentId1 },
                PreviousShipmentId = new List<string>() { shipmentId1 }
            })).Assert(RegisterSuccessExpectStatusCode);
            shipmentResult = client.GetWebApiResponseResult(shipment.OData("FilterIT")).Assert(GetSuccessExpectStatusCode);
            var shipmentId2 = shipmentResult.Result.First(x => x.Shipping.ShippingCompanyId == CompanyId3).ShipmentId;

            // 入荷ID指定なしでも経路が取得されること
            // GetShipmentAndArrivalを取得
            var result = client.GetWebApiResponseResult(getShipmentAndArrival.GetShipmentAndArrival("01ITGtin21Product1", null)).Assert(GetSuccessExpectStatusCode);
            // 取得した中身の確認
            result.Result.Count.Is(3);
            // 出荷1
            result.Result[0].ProductCode.Is("01ITGtin21Product1");
            result.Result[0].DateTime.Is(new DateTime(2021, 12, 1));
            result.Result[0].Type.Is(ShipmentType);
            result.Result[0].Company.CompanyId.Is(CompanyId1);
            result.Result[0].Company.CompanyName.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者1");
            result.Result[0].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test1");
            result.Result[0].Company.OfficeId.Is(OfficeId1);
            result.Result[0].Company.OfficeName.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所1");
            result.Result[0].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice1");
            result.Result[0].Company.GlnCode.Is("FilterITPartyOfficeGln1");
            result.Result[0].ShipmentId.Is(shipmentId1);
            result.Result[0].ArrivalId.IsNull();
            result.Result[0].PreviousShipmentId.IsNull();
            result.Result[0].PackageQuantity.Is(100);
            // 入荷1
            result.Result[1].ProductCode.Is("01ITGtin21Product1");
            result.Result[1].DateTime.Is(new DateTime(2021, 12, 2));
            result.Result[1].Type.Is(ArrivalType);
            result.Result[1].Company.CompanyId.Is(CompanyId2);
            result.Result[1].Company.CompanyName.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[1].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[1].Company.OfficeId.Is(OfficeId2);
            result.Result[1].Company.OfficeName.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[1].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[1].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[1].ShipmentId.IsNull();
            result.Result[1].ArrivalId.Is(arrivalId1);
            result.Result[1].PreviousShipmentId.Is(shipmentId1);
            result.Result[1].PackageQuantity.Is(100);
            // 出荷2
            result.Result[2].ProductCode.Is("01ITGtin21Product1");
            result.Result[2].DateTime.Is(new DateTime(2021, 12, 3));
            result.Result[2].Type.Is(ShipmentType);
            result.Result[2].Company.CompanyId.Is(CompanyId2);
            result.Result[2].Company.CompanyName.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業者2");
            result.Result[2].Company.CompanyNameLang.First(x => x.LocaleCode == "en-us").Name.Is("Test2");
            result.Result[2].Company.OfficeId.Is(OfficeId2);
            result.Result[2].Company.OfficeName.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "ja-jp").Name.Is("テスト用事業所2");
            result.Result[2].Company.OfficeNameLang.First(x => x.LocaleCode == "en-us").Name.Is("TestOffice2");
            result.Result[2].Company.GlnCode.Is("FilterITPartyOfficeGln2");
            result.Result[2].ShipmentId.Is(shipmentId2);
            result.Result[2].ArrivalId.IsNull();
            result.Result[2].PreviousShipmentId.Is(shipmentId1);
            result.Result[2].PackageQuantity.Is(100);

            // データ削除
            DeleteTestData();
        }

        private void DeleteTestData()
        {
            var client = new IntegratedTestClient("test1");
            var shipment = UnityCore.Resolve<IShipmentApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();

            client.GetWebApiResponseResult(shipment.ODataDelete("FilterIT")).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(arrival.ODataDelete("FilterIT")).Assert(DeleteExpectStatusCodes);
        }

        /// <summary>
        /// 事業者のテストデータを作成する
        /// </summary>
        /// <returns></returns>
        private List<CompanyModel> CreateCompanyModel()
        {
            return new List<CompanyModel>
            {
                new CompanyModel()
                {
                    CompanyId = CompanyId1,
                    CompanyName = "テスト用事業者1",
                    CompanyNameLang = new List<NameLangModel>
                    {
                        new NameLangModel {LocaleCode = "ja-jp", Name = "テスト用事業者1"},
                        new NameLangModel {LocaleCode = "en-us", Name = "Test1"}
                    },
                    IndustoryTypeCode = "wholesaler",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Fax = "1234567890",
                    Images = new List<ImageModel>
                    {
                        new ImageModel
                        {
                            DefaultImageFlag = true, ImageDescription = "hoge",
                            ImagePath = "hoge"
                        }
                    },
                    MailAddress = "hoge@example.com",
                    ZipCode = "1234567",
                    Tel = "1234567890",
                    CountryCode = "JP",
                    GlnCode = CompanyId1,
                    GS1CompanyCode = "hoge"
                },
                new CompanyModel()
                {
                    CompanyId = CompanyId2,
                    CompanyName = "テスト用事業者2",
                    CompanyNameLang = new List<NameLangModel>
                    {
                        new NameLangModel {LocaleCode = "ja-jp", Name = "テスト用事業者2"},
                        new NameLangModel {LocaleCode = "en-us", Name = "Test2"}
                    },
                    IndustoryTypeCode = "wholesaler",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Fax = "1234567890",
                    Images = new List<ImageModel>
                    {
                        new ImageModel
                        {
                            DefaultImageFlag = true, ImageDescription = "hoge",
                            ImagePath = "hoge"
                        }
                    },
                    MailAddress = "hoge@example.com",
                    ZipCode = "1234567",
                    Tel = "1234567890",
                    CountryCode = "JP",
                    GlnCode = CompanyId2,
                    GS1CompanyCode = "hoge"
                },
                new CompanyModel()
                {
                    CompanyId = CompanyId3,
                    CompanyName = "テスト用事業者3",
                    CompanyNameLang = new List<NameLangModel>
                    {
                        new NameLangModel {LocaleCode = "ja-jp", Name = "テスト用事業者3"},
                        new NameLangModel {LocaleCode = "en-us", Name = "Test3"}
                    },
                    IndustoryTypeCode = "wholesaler",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Fax = "1234567890",
                    Images = new List<ImageModel>
                    {
                        new ImageModel
                        {
                            DefaultImageFlag = true, ImageDescription = "hoge",
                            ImagePath = "hoge"
                        }
                    },
                    MailAddress = "hoge@example.com",
                    ZipCode = "1234567",
                    Tel = "1234567890",
                    CountryCode = "JP",
                    GlnCode = CompanyId3,
                    GS1CompanyCode = "hoge"
                },
                new CompanyModel()
                {
                    CompanyId = CompanyId4,
                    CompanyName = "テスト用事業者4",
                    CompanyNameLang = new List<NameLangModel>
                    {
                        new NameLangModel {LocaleCode = "ja-jp", Name = "テスト用事業者4"},
                        new NameLangModel {LocaleCode = "en-us", Name = "Test4"}
                    },
                    IndustoryTypeCode = "wholesaler",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Fax = "1234567890",
                    Images = new List<ImageModel>
                    {
                        new ImageModel
                        {
                            DefaultImageFlag = true, ImageDescription = "hoge",
                            ImagePath = "hoge"
                        }
                    },
                    MailAddress = "hoge@example.com",
                    ZipCode = "1234567",
                    Tel = "1234567890",
                    CountryCode = "JP",
                    GlnCode = CompanyId3,
                    GS1CompanyCode = "hoge"
                },
                new CompanyModel()
                {
                    CompanyId = CompanyId5,
                    CompanyName = "テスト用事業者5",
                    CompanyNameLang = new List<NameLangModel>
                    {
                        new NameLangModel {LocaleCode = "ja-jp", Name = "テスト用事業者5"},
                        new NameLangModel {LocaleCode = "en-us", Name = "Test5"}
                    },
                    IndustoryTypeCode = "wholesaler",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Fax = "1234567890",
                    Images = new List<ImageModel>
                    {
                        new ImageModel
                        {
                            DefaultImageFlag = true, ImageDescription = "hoge",
                            ImagePath = "hoge"
                        }
                    },
                    MailAddress = "hoge@example.com",
                    ZipCode = "1234567",
                    Tel = "1234567890",
                    CountryCode = "JP",
                    GlnCode = CompanyId5,
                    GS1CompanyCode = "hoge"
                }
            };
        }
        
        /// <summary>
        /// 事業所のテストデータを作成する
        /// </summary>
        /// <returns></returns>
        private List<OfficeModel> CreateOfficeModel()
        {
            return new List<OfficeModel>()
            {
                new OfficeModel()
                {
                    OfficeId = OfficeId1,
                    CompanyId = CompanyId1,
                    OfficeName = "テスト用事業所1",
                    OfficeNameKana = "hoge",
                    ZipCode = "1234567",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Tel = "1234567890",
                    GlnCode = "FilterITPartyOfficeGln1",
                    Address1Lang = new List<AddressLangModel>()
                    {
                        new AddressLangModel() { LocaleCode = "ja-jp", Address = "hoge" },
                        new AddressLangModel() { LocaleCode = "en-us", Address = "huga" }
                    },
                    OfficeNameLang = new List<NameLangModel>()
                    {
                        new NameLangModel { LocaleCode = "ja-jp", Name = "テスト用事業所1" },
                        new NameLangModel { LocaleCode = "en-us", Name = "TestOffice1" }
                    },
                },
                new OfficeModel()
                {
                    OfficeId = OfficeId2,
                    CompanyId = CompanyId2,
                    OfficeName = "テスト用事業所2",
                    OfficeNameKana = "hoge",
                    ZipCode = "1234567",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Tel = "1234567890",
                    GlnCode = "FilterITPartyOfficeGln2",
                    Address1Lang = new List<AddressLangModel>()
                    {
                        new AddressLangModel() { LocaleCode = "ja-jp", Address = "hoge" },
                        new AddressLangModel() { LocaleCode = "en-us", Address = "huga" }
                    },
                    OfficeNameLang = new List<NameLangModel>()
                    {
                        new NameLangModel { LocaleCode = "ja-jp", Name = "テスト用事業所2" },
                        new NameLangModel { LocaleCode = "en-us", Name = "TestOffice2" }
                    }
                },
                new OfficeModel()
                {
                    OfficeId = OfficeId3,
                    CompanyId = CompanyId3,
                    OfficeName = "テスト用事業所3",
                    OfficeNameKana = "hoge",
                    ZipCode = "1234567",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Tel = "1234567890",
                    GlnCode = "FilterITPartyOfficeGln3",
                    Address1Lang = new List<AddressLangModel>()
                    {
                        new AddressLangModel() { LocaleCode = "ja-jp", Address = "hoge" },
                        new AddressLangModel() { LocaleCode = "en-us", Address = "huga" }
                    },
                    OfficeNameLang = new List<NameLangModel>()
                    {
                        new NameLangModel { LocaleCode = "ja-jp", Name = "テスト用事業所3" },
                        new NameLangModel { LocaleCode = "en-us", Name = "TestOffice3" }
                    }
                },
                new OfficeModel()
                {
                    OfficeId = OfficeId4,
                    CompanyId = CompanyId4,
                    OfficeName = "テスト用事業所4",
                    OfficeNameKana = "hoge",
                    ZipCode = "1234567",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Tel = "1234567890",
                    GlnCode = "FilterITPartyOfficeGln4",
                    Address1Lang = new List<AddressLangModel>()
                    {
                        new AddressLangModel() { LocaleCode = "ja-jp", Address = "hoge" },
                        new AddressLangModel() { LocaleCode = "en-us", Address = "huga" }
                    },
                    OfficeNameLang = new List<NameLangModel>()
                    {
                        new NameLangModel { LocaleCode = "ja-jp", Name = "テスト用事業所4" },
                        new NameLangModel { LocaleCode = "en-us", Name = "TestOffice4" }
                    }
                },
                new OfficeModel()
                {
                    OfficeId = OfficeId5,
                    CompanyId = CompanyId5,
                    OfficeName = "テスト用事業所5",
                    OfficeNameKana = "hoge",
                    ZipCode = "1234567",
                    Address1 = "hoge",
                    Address2 = "hoge",
                    Address3 = "hoge",
                    Tel = "1234567890",
                    GlnCode = "FilterITPartyOfficeGln5",
                    Address1Lang = new List<AddressLangModel>()
                    {
                        new AddressLangModel() {LocaleCode = "ja-jp", Address = "hoge"},
                        new AddressLangModel() {LocaleCode = "en-us", Address = "huga"}
                    },
                    OfficeNameLang = new List<NameLangModel>()
                    {
                        new NameLangModel { LocaleCode = "ja-jp", Name = "テスト用事業所5" },
                        new NameLangModel { LocaleCode = "en-us", Name = "TestOffice5" }
                    }
                }
            };
        }
    }
}
