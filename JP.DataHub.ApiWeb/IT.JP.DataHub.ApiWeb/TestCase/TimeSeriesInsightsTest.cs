using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("TimeSeriesInsights")]
    public class TimeSeriesInsightsTest : ApiWebItTestCase
    {
        #region TestData

        private class TimeSeriesInsightsTestData : TestDataBase
        {
            public ObservationModel Data1 = new ObservationModel()
            {
                key = "a1a30ec2-d33a-4621-a45f-4bcb4cd9e31e",
                phenomenonTime = "2020-01-02T01:02:03Z",
                phenomenonEndTime = "2020-01-02T01:02:03Z",
                resultTime = "2020-01-02T01:02:03Z",
                result = "1"
            };

            public List<ObservationModel> Data2 = new List<ObservationModel>()
            {
                new ObservationModel()
                {
                    key = "c94c14d5-349e-4b62-a001-d9a08d252396",
                    phenomenonTime = "2020-01-02T01:03:03Z",
                    phenomenonEndTime = "2020-01-02T01:03:03Z",
                    resultTime = "2020-01-02T01:03:03Z",
                    result = "3"
                },
                new ObservationModel()
                {
                    key = "8da3d945-3d31-45b1-b47b-69ecee692e3f",
                    phenomenonTime = "2020-01-02T01:12:03Z",
                    phenomenonEndTime = "2020-01-02T01:12:03Z",
                    resultTime = "2020-01-02T01:12:03Z",
                    result = "4"
                },
                new ObservationModel()
                {
                    key = "57d80b25-c362-45f7-9d19-09eef30e17c4",
                    phenomenonTime = "2020-01-02T01:13:03Z",
                    phenomenonEndTime = "2020-01-02T01:13:03Z",
                    resultTime = "2020-01-02T01:13:03Z",
                    result = "6"
                }
            };

            public List<ObservationModel> DataGetEventsExpected = new List<ObservationModel>()
            {
                new ObservationModel()
                {
                    key = "57d80b25-c362-45f7-9d19-09eef30e17c4",
                    phenomenonTime = "2020-01-02T01:13:03Z",
                    phenomenonEndTime = "2020-01-02T01:13:03Z",
                    resultTime = "2020-01-02T01:13:03Z",
                    result = "6",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "8da3d945-3d31-45b1-b47b-69ecee692e3f",
                    phenomenonTime = "2020-01-02T01:12:03Z",
                    phenomenonEndTime = "2020-01-02T01:12:03Z",
                    resultTime = "2020-01-02T01:12:03Z",
                    result = "4",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "a1a30ec2-d33a-4621-a45f-4bcb4cd9e31e",
                    phenomenonTime = "2020-01-02T01:02:03Z",
                    phenomenonEndTime = "2020-01-02T01:02:03Z",
                    resultTime = "2020-01-02T01:02:03Z",
                    result = "1",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "c94c14d5-349e-4b62-a001-d9a08d252396",
                    phenomenonTime = "2020-01-02T01:03:03Z",
                    phenomenonEndTime = "2020-01-02T01:03:03Z",
                    resultTime = "2020-01-02T01:03:03Z",
                    result = "3",
                    _Owner_Id = WILDCARD
                }
            };

            public List<ObservationModel> DataGetEventsFullExpected = new List<ObservationModel>()
            {
                new ObservationModel()
                {
                    key = "57d80b25-c362-45f7-9d19-09eef30e17c4",
                    phenomenonTime = "2020-01-02T01:13:03Z",
                    phenomenonEndTime = "2020-01-02T01:13:03Z",
                    resultTime = "2020-01-02T01:13:03Z",
                    result = "6",
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "8da3d945-3d31-45b1-b47b-69ecee692e3f",
                    phenomenonTime = "2020-01-02T01:12:03Z",
                    phenomenonEndTime = "2020-01-02T01:12:03Z",
                    resultTime = "2020-01-02T01:12:03Z",
                    result = "4",
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "a1a30ec2-d33a-4621-a45f-4bcb4cd9e31e",
                    phenomenonTime = "2020-01-02T01:02:03Z",
                    phenomenonEndTime = "2020-01-02T01:02:03Z",
                    resultTime = "2020-01-02T01:02:03Z",
                    result = "1",
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "c94c14d5-349e-4b62-a001-d9a08d252396",
                    phenomenonTime = "2020-01-02T01:03:03Z",
                    phenomenonEndTime = "2020-01-02T01:03:03Z",
                    resultTime = "2020-01-02T01:03:03Z",
                    result = "3",
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Owner_Id = WILDCARD
                }
            };

            public List<TsiAggregateSeriesModel> DataAggregateSeriesExpected = new List<TsiAggregateSeriesModel>()
            {
                new TsiAggregateSeriesModel()
                {
                    timestamp = "2020-01-02T01:00:00Z",
                    Count = 2,
                    Average = 2,
                    Min = 1,
                    Max = 3
                },
                new TsiAggregateSeriesModel()
                {
                    timestamp = "2020-01-02T01:10:00Z",
                    Count = 2,
                    Average = 5,
                    Min = 4,
                    Max = 6
                }
            };

            public List<ObservationModel> Data1_A = new List<ObservationModel>()
            {
                new ObservationModel()
                {
                    key = "a1a30ec2-d33a-4621-a45f-4bcb4cd9e31e",
                    phenomenonTime = "2020-01-02T01:02:03Z",
                    phenomenonEndTime = "2020-01-02T01:02:03Z",
                    resultTime = "2020-01-02T01:02:03Z",
                    result = "10"
                },
                new ObservationModel()
                {
                    key = "c94c14d5-349e-4b62-a001-d9a08d252396",
                    phenomenonTime = "2020-01-02T01:03:03Z",
                    phenomenonEndTime = "2020-01-02T01:03:03Z",
                    resultTime = "2020-01-02T01:03:03Z",
                    result = "30"
                },
                new ObservationModel()
                {
                    key = "8da3d945-3d31-45b1-b47b-69ecee692e3f",
                    phenomenonTime = "2020-01-02T01:12:03Z",
                    phenomenonEndTime = "2020-01-02T01:12:03Z",
                    resultTime = "2020-01-02T01:12:03Z",
                    result = "40"
                },
                new ObservationModel()
                {
                    key = "57d80b25-c362-45f7-9d19-09eef30e17c4",
                    phenomenonTime = "2020-01-02T01:13:03Z",
                    phenomenonEndTime = "2020-01-02T01:13:03Z",
                    resultTime = "2020-01-02T01:13:03Z",
                    result = "60"
                }
            };
            public List<ObservationModel> DataGetEventsExpected_A = new List<ObservationModel>()
            {
                new ObservationModel()
                {
                    key = "57d80b25-c362-45f7-9d19-09eef30e17c4",
                    phenomenonTime = "2020-01-02T01:13:03Z",
                    phenomenonEndTime = "2020-01-02T01:13:03Z",
                    resultTime = "2020-01-02T01:13:03Z",
                    result = "60",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "8da3d945-3d31-45b1-b47b-69ecee692e3f",
                    phenomenonTime = "2020-01-02T01:12:03Z",
                    phenomenonEndTime = "2020-01-02T01:12:03Z",
                    resultTime = "2020-01-02T01:12:03Z",
                    result = "40",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "a1a30ec2-d33a-4621-a45f-4bcb4cd9e31e",
                    phenomenonTime = "2020-01-02T01:02:03Z",
                    phenomenonEndTime = "2020-01-02T01:02:03Z",
                    resultTime = "2020-01-02T01:02:03Z",
                    result = "10",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "c94c14d5-349e-4b62-a001-d9a08d252396",
                    phenomenonTime = "2020-01-02T01:03:03Z",
                    phenomenonEndTime = "2020-01-02T01:03:03Z",
                    resultTime = "2020-01-02T01:03:03Z",
                    result = "30",
                    _Owner_Id = WILDCARD
                }
            };
            public List<TsiAggregateSeriesModel> DataAggregateSeriesExpected_A = new List<TsiAggregateSeriesModel>()
            {
                new TsiAggregateSeriesModel()
                {
                    timestamp = "2020-01-02T01:00:00Z",
                    Count = 2,
                    Average = 20,
                    Min = 10,
                    Max = 30
                },
                new TsiAggregateSeriesModel()
                {
                    timestamp = "2020-01-02T01:10:00Z",
                    Count = 2,
                    Average = 50,
                    Min = 40,
                    Max = 60
                }
            };

            public List<ObservationModel> Data1_B = new List<ObservationModel>()
            {
                new ObservationModel()
                {
                    key = "a1a30ec2-d33a-4621-a45f-4bcb4cd9e31e",
                    phenomenonTime = "2020-01-02T01:02:03Z",
                    phenomenonEndTime = "2020-01-02T01:02:03Z",
                    resultTime = "2020-01-02T01:02:03Z",
                    result = "100"
                },
                new ObservationModel()
                {
                    key = "c94c14d5-349e-4b62-a001-d9a08d252396",
                    phenomenonTime = "2020-01-02T01:03:03Z",
                    phenomenonEndTime = "2020-01-02T01:03:03Z",
                    resultTime = "2020-01-02T01:03:03Z",
                    result = "300"
                },
                new ObservationModel()
                {
                    key = "8da3d945-3d31-45b1-b47b-69ecee692e3f",
                    phenomenonTime = "2020-01-02T01:12:03Z",
                    phenomenonEndTime = "2020-01-02T01:12:03Z",
                    resultTime = "2020-01-02T01:12:03Z",
                    result = "400"
                },
                new ObservationModel()
                {
                    key = "57d80b25-c362-45f7-9d19-09eef30e17c4",
                    phenomenonTime = "2020-01-02T01:13:03Z",
                    phenomenonEndTime = "2020-01-02T01:13:03Z",
                    resultTime = "2020-01-02T01:13:03Z",
                    result = "600"
                }
            };
            public List<ObservationModel> DataGetEventsExpected_B = new List<ObservationModel>()
            {
                new ObservationModel()
                {
                    key = "57d80b25-c362-45f7-9d19-09eef30e17c4",
                    phenomenonTime = "2020-01-02T01:13:03Z",
                    phenomenonEndTime = "2020-01-02T01:13:03Z",
                    resultTime = "2020-01-02T01:13:03Z",
                    result = "600",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "8da3d945-3d31-45b1-b47b-69ecee692e3f",
                    phenomenonTime = "2020-01-02T01:12:03Z",
                    phenomenonEndTime = "2020-01-02T01:12:03Z",
                    resultTime = "2020-01-02T01:12:03Z",
                    result = "400",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "a1a30ec2-d33a-4621-a45f-4bcb4cd9e31e",
                    phenomenonTime = "2020-01-02T01:02:03Z",
                    phenomenonEndTime = "2020-01-02T01:02:03Z",
                    resultTime = "2020-01-02T01:02:03Z",
                    result = "100",
                    _Owner_Id = WILDCARD
                },
                new ObservationModel()
                {
                    key = "c94c14d5-349e-4b62-a001-d9a08d252396",
                    phenomenonTime = "2020-01-02T01:03:03Z",
                    phenomenonEndTime = "2020-01-02T01:03:03Z",
                    resultTime = "2020-01-02T01:03:03Z",
                    result = "300",
                    _Owner_Id = WILDCARD
                }
            };
            public List<TsiAggregateSeriesModel> DataAggregateSeriesExpected_B = new List<TsiAggregateSeriesModel>()
            {
                new TsiAggregateSeriesModel()
                {
                    timestamp = "2020-01-02T01:00:00Z",
                    Count = 2,
                    Average = 200,
                    Min = 100,
                    Max = 300
                },
                new TsiAggregateSeriesModel()
                {
                    timestamp = "2020-01-02T01:10:00Z",
                    Count = 2,
                    Average = 500,
                    Min = 400,
                    Max = 600
                }
            };

            public TimeSeriesInsightsTestData(string resourceUrl, bool isVendor, bool isPerson, IntegratedTestClient client)
                : base(Repository.Default, resourceUrl, isVendor, isPerson, client) {}
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [DataTestMethod]
        public void TimeSeriesInsights_NormalSenario()
        {
            var thingId = Guid.NewGuid().ToString();
            var datastreamId = Guid.NewGuid().ToString();

            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ITimeSeriesInsightsApi>();
            var testData = new TimeSeriesInsightsTestData(api.ResourceUrl, false, false, client);

            // データ登録(1件)
            client.GetWebApiResponseResult(api.Register(PrepareModel(testData.Data1, thingId, datastreamId))).Assert(RegisterSuccessExpectStatusCode);

            // データ登録(配列)
            client.GetWebApiResponseResult(api.RegisterList(PrepareModel(testData.Data2, thingId, datastreamId))).Assert(RegisterSuccessExpectStatusCode);

            // OData(BadRequest)
            client.GetWebApiResponseResult(api.OData()).Assert(BadRequestStatusCode);

            // APIクエリなし(BadRequest)
            client.GetWebApiResponseResult(api.GetAll()).Assert(BadRequestStatusCode);

            // GetEvents
            // TSIへの反映までタイムラグあり
            var i = 0;
            var policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).WaitAndRetry(9, i => TimeSpan.FromSeconds(20));
            var result = client.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
            while (result.Count < 4 && i < 3)
            {
                result = client.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
                i++;
            }
            result.OrderBy(x => x.key).ToList().IsStructuralEqual(PrepareModel(testData.DataGetEventsExpected, thingId, datastreamId));

            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            if (IsIgnoreGetInternalAllField)
            {
                result = client.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId)).Assert(GetSuccessExpectStatusCode).Result;
                result.OrderBy(x => x.key).ToList().IsStructuralEqual(PrepareModel(testData.DataGetEventsExpected, thingId, datastreamId));
            }
            else
            {
                result = client.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId)).Assert(GetSuccessExpectStatusCode).Result;
                result.OrderBy(x => x.key).ToList().IsStructuralEqual(PrepareModel(testData.DataGetEventsFullExpected, thingId, datastreamId));
            }
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // AggregateSeries
            client.GetWebApiResponseResult(api.AggregateSeries(thingId, datastreamId)).Assert(GetSuccessExpectStatusCode, testData.DataAggregateSeriesExpected);
        }

        [DataTestMethod]
        public void TimeSeriesInsights_VendorPrivate()
        {
            var thingId = Guid.NewGuid().ToString();
            var datastreamId = Guid.NewGuid().ToString();

            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin");
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal");
            var api = UnityCore.Resolve<ITimeSeriesInsightsVendorPrivateApi>();
            var testData = new TimeSeriesInsightsTestData(api.ResourceUrl, true, false, clientA);

            // データ登録(配列)
            clientA.GetWebApiResponseResult(api.RegisterList(PrepareModel(testData.Data1_A, thingId, datastreamId))).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegisterList(PrepareModel(testData.Data1_B, thingId, datastreamId))).Assert(RegisterSuccessExpectStatusCode);

            // GetEvents
            // TSIへの反映までタイムラグあり
            var i = 0;
            var policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).WaitAndRetry(9, i => TimeSpan.FromSeconds(20));
            var result = clientA.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
            while (result.Count < 4 && i < 3)
            {
                result = clientA.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
                i++;
            }
            result.OrderBy(x => x.key).ToList().IsStructuralEqual(PrepareModel(testData.DataGetEventsExpected_A, thingId, datastreamId));

            result = clientB.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
            while (result.Count < 4 && i < 3)
            {
                result = clientB.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
                i++;
            }
            result.OrderBy(x => x.key).ToList().IsStructuralEqual(PrepareModel(testData.DataGetEventsExpected_B, thingId, datastreamId));

            // AggregateSeries
            clientA.GetWebApiResponseResult(api.AggregateSeries(thingId, datastreamId)).Assert(GetSuccessExpectStatusCode, testData.DataAggregateSeriesExpected_A);
            clientB.GetWebApiResponseResult(api.AggregateSeries(thingId, datastreamId)).Assert(GetSuccessExpectStatusCode, testData.DataAggregateSeriesExpected_B);
        }

        [DataTestMethod]
        public void TimeSeriesInsights_PersonPrivate()
        {
            var thingId = Guid.NewGuid().ToString();
            var datastreamId = Guid.NewGuid().ToString();

            var clientA = new IntegratedTestClient("test1");
            var clientB = new IntegratedTestClient("test2");
            var api = UnityCore.Resolve<ITimeSeriesInsightsPersonPrivateApi>();
            var testData = new TimeSeriesInsightsTestData(api.ResourceUrl, false, true, clientA);

            // データ登録(配列)
            clientA.GetWebApiResponseResult(api.RegisterList(PrepareModel(testData.Data1_A, thingId, datastreamId))).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegisterList(PrepareModel(testData.Data1_B, thingId, datastreamId))).Assert(RegisterSuccessExpectStatusCode);

            // GetEvents
            // TSIへの反映までタイムラグあり
            var i = 0;
            var policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).WaitAndRetry(9, i => TimeSpan.FromSeconds(20));
            var result = clientA.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
            while (result.Count < 4 && i < 3)
            {
                result = clientA.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
                i++;
            }
            result.OrderBy(x => x.key).ToList().IsStructuralEqual(PrepareModel(testData.DataGetEventsExpected_A, thingId, datastreamId));

            result = clientB.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
            while (result.Count < 4 && i < 3)
            {
                result = clientB.GetWebApiResponseResult(api.GetEvents(thingId, datastreamId), policy).Assert(GetSuccessExpectStatusCode).Result;
                i++;
            }
            result.OrderBy(x => x.key).ToList().IsStructuralEqual(PrepareModel(testData.DataGetEventsExpected_B, thingId, datastreamId));

            // AggregateSeries
            clientA.GetWebApiResponseResult(api.AggregateSeries(thingId, datastreamId)).Assert(GetSuccessExpectStatusCode, testData.DataAggregateSeriesExpected_A);
            clientB.GetWebApiResponseResult(api.AggregateSeries(thingId, datastreamId)).Assert(GetSuccessExpectStatusCode, testData.DataAggregateSeriesExpected_B);
        }


        /// <remarks>
        /// ページングは大量データ取得時(33000件程度,take指定あり)にしか発生しない。
        /// 事前に40000件以上データを登録してデータに合わせたthingId,datastreamId,spanFrom,spanToで手動実行すること。
        /// </remarks>
        [Ignore]
        [DataTestMethod]
        public void TimeSeriesInsights_PagingSenario()
        {
            var thingId = "26f7c055-5c89-48c6-882e-68769e43d81e";
            var datastreamId = "64d965d6-dbc9-4ca8-b913-b5f64fa86857";
            var spanFrom = "2021-11-01T06:58:00Z";
            var spanTo = "2022-01-15T07:00:00Z";

            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ITimeSeriesInsightsApi>();
            var testData = new TimeSeriesInsightsTestData(api.ResourceUrl, false, false, client);

            var events = client.GetWebApiResponseResult(api.GetEventsTakeMax(thingId, datastreamId, spanFrom, spanTo)).Assert(GetSuccessExpectStatusCode).Result;
            var count = events.Count;
            (count >= 40000).IsTrue();
            count.Is(events.Select(x => x.key).Distinct().Count());
        }


        private ObservationModel PrepareModel(ObservationModel model, string thingId, string datastreamId)
        {
            model.thingId = thingId;
            model.datastreamId = datastreamId;
            return model;
        }

        private List<ObservationModel> PrepareModel(List<ObservationModel> models, string thingId, string datastreamId)
        {
            foreach (var model in models)
            {
                model.thingId = thingId;
                model.datastreamId = datastreamId;
            }
            return models;
        }
    }
}
