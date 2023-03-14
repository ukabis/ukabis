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
    public class SensorDeviceRegisterExFilterTest : ItTestCaseBase
    {
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
        public void SensorDeviceRegisterExFilter_NormalScenario()
        {
            // SensorDevice削除
            var client = new IntegratedTestClient("test1");
            var sensorDeviceApi = UnityCore.Resolve<ISensorDeviceApi>();
            client.GetWebApiResponseResult(sensorDeviceApi.ODataDelete($"description eq '__IntegratedTest_description'")).Assert(DeleteExpectStatusCodes);
            // Thing削除
            var thingsApi = UnityCore.Resolve<IThingsApi>();
            client.GetWebApiResponseResult(thingsApi.ODataDelete($"description eq '__IntegratedTest_Thing_description'")).Assert(DeleteExpectStatusCodes);
            // Datastream削除
            var datastreamsApi = UnityCore.Resolve<IDatastreamsApi>();
            client.GetWebApiResponseResult(datastreamsApi.ODataDelete($"startswith(description, '__IntegratedTest') and not contains(key, '__IntegratedTestDatastream')")).Assert(DeleteExpectStatusCodes);
            
            // センサーに紐づくデータストリームを基に(isDefaultDataStream=true)登録
            var data = this.CreateRequestData(true, true);
            var registerResult = client.GetWebApiResponseResult(sensorDeviceApi.RegisterEx(data)).Assert(RegisterSuccessExpectStatusCode);
            var id = registerResult.Result.id;

            // SensorDevice確認
            var sensorDeviceResult = client.GetWebApiResponseResult(sensorDeviceApi.OData(id)).Assert(GetSuccessExpectStatusCode);
            var sensorDevice = sensorDeviceResult.Result.Single();
            sensorDevice.uuid.Is(data.sensorUuid);
            sensorDevice.name.Is(data.name);
            // Thing確認
            var thingResult = client.GetWebApiResponseResult(thingsApi.Get(sensorDevice.thingsId)).Assert(GetSuccessExpectStatusCode);
            var thing = thingResult.Result;
            thing.name.Is(data.thing.name);
            // Datastream確認
            var datastreamResult = client.GetWebApiResponseResult(datastreamsApi.OData(sensorDevice.thingsId)).Assert(GetSuccessExpectStatusCode);
            var datastreams = datastreamResult.Result.OrderBy(datastream => datastream.name).ToList();
            datastreams[0].name.Contains("__IntegratedTest_Thing-Datastream-").IsTrue();
            
            // 指定したデータストリームを基に(isDefaultDataStream=false)、名前未指定で登録
            data = this.CreateRequestData(false, false);
            registerResult = client.GetWebApiResponseResult(sensorDeviceApi.RegisterEx(data)).Assert(RegisterSuccessExpectStatusCode);
            id = registerResult.Result.id;

            // SensorDevice確認
            sensorDeviceResult = client.GetWebApiResponseResult(sensorDeviceApi.OData(id)).Assert(GetSuccessExpectStatusCode);
            sensorDevice = sensorDeviceResult.Result.Single();
            sensorDevice.uuid.Is(data.sensorUuid);
            sensorDevice.name.Is($"INTEL-GWS-CSCG Tag-{data.sensorSerialNumber}");
            // Thing確認
            thingResult = client.GetWebApiResponseResult(thingsApi.Get(sensorDevice.thingsId)).Assert(GetSuccessExpectStatusCode);
            thing = thingResult.Result;
            thing.name.Is($"{sensorDevice.name}-Thing");
            // Datastream確認
            datastreamResult = client.GetWebApiResponseResult(datastreamsApi.OData(sensorDevice.thingsId)).Assert(GetSuccessExpectStatusCode);
            datastreams = datastreamResult.Result;
            datastreams.ForEach(stream => stream.name.Is($"{thing.name}-Datastream"));

            // 登録済みデバイスを登録(BadRequest)
            client.GetWebApiResponseResult(sensorDeviceApi.RegisterEx(data)).Assert(BadRequestStatusCode);

            // 未登録センサーのデバイスを登録(BadRequest)
            data = this.CreateRequestData(true, true);
            data.sensorId = "test";
            client.GetWebApiResponseResult(sensorDeviceApi.RegisterEx(data)).Assert(BadRequestStatusCode);

            // SensorDevice削除
            client.GetWebApiResponseResult(sensorDeviceApi.ODataDelete($"description eq '__IntegratedTest_description'")).Assert(DeleteExpectStatusCodes);
            // Thing削除
            client.GetWebApiResponseResult(thingsApi.ODataDelete($"description eq '__IntegratedTest_Thing_description'")).Assert(DeleteExpectStatusCodes);
            // Datastream削除
            client.GetWebApiResponseResult(datastreamsApi.ODataDelete($"startswith(description, '__IntegratedTest') and not contains(key, '__IntegratedTestDatastream')")).Assert(DeleteExpectStatusCodes);
        }

        private SensorDeviceRegisterExModel CreateRequestData(bool isDefaultDataStream, bool shouldSetName)
        {
            return new SensorDeviceRegisterExModel()
            {
                name = shouldSetName ? "__IntegratedTest_SensorDevice" : null,
                description = "__IntegratedTest_description",
                sensorId = "21DEAA37-5567-45E8-AFD5-27F606502FA1",
                sensorSerialNumber = "__IntegratedTest_sensorSerialNumber",
                sensorUuid = $"__IntegratedTest_sensorUuid{Guid.NewGuid().ToString()}",
                isDefaultDataStream = isDefaultDataStream,
                thing = new SensorDeviceRegisterExThingModel()
                {
                    name = shouldSetName ? "__IntegratedTest_Thing" : null,
                    description = "__IntegratedTest_Thing_description",
                },
                dataStreams = new List<SensorDeviceRegisterExDataStreamModel>()
                {
                    new SensorDeviceRegisterExDataStreamModel()
                    {
                        name = shouldSetName ? "__IntegratedTest_DataStream1" : null,
                        description = "__IntegratedTest_DataStream1_description",
                        measurementUnit = "18E5AA34-FDE6-411F-A455-EEA588CAB34B",
                        observedProperty = "BC6A4FE1-4211-478A-A85E-EF1F3CE34B54",
                        observedArea = new DatastreamsModel.PolygonModel()
                        {
                            type = "Polygon",
                            coordinates = new List<List<List<double>>>()
                            {
                                new List<List<double>>()
                                {
                                    new List<double>()
                                    {
                                        1, 2
                                    },
                                    new List<double>()
                                    {
                                        3, 4
                                    },
                                    new List<double>()
                                    {
                                        5, 6
                                    },
                                    new List<double>()
                                    {
                                        7, 8
                                    }
                                }
                            }
                        },
                        observationConditionId = null
                    },
                    new SensorDeviceRegisterExDataStreamModel()
                    {
                        name = shouldSetName ? "__IntegratedTest_DataStream2" : null,
                        description = "__IntegratedTest_DataStream2_description",
                        measurementUnit = "18E5AA34-FDE6-411F-A455-EEA588CAB34B",
                        observedProperty = "BC6A4FE1-4211-478A-A85E-EF1F3CE34B54",
                        observedArea = new DatastreamsModel.PolygonModel()
                        {
                            type = "Polygon",
                            coordinates = new List<List<List<double>>>()
                            {
                                new List<List<double>>()
                                {
                                    new List<double>()
                                    {
                                        1, 2
                                    },
                                    new List<double>()
                                    {
                                        3, 4
                                    },
                                    new List<double>()
                                    {
                                        5, 6
                                    },
                                    new List<double>()
                                    {
                                        7, 8
                                    }
                                }
                            }
                        },
                        observationConditionId = null
                    }
                }
            };
        }
    }
}
