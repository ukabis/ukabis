using AutoMapper;
using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class SensorDeviceRegisterExFilter : AbstractApiFilter
    {
        // センサーデバイスのキャッシュキーの接頭辞
        private static readonly string SensorDeviceCacheKeyPrefix = "SensorDeviceRegisterExFilter_SensorDeviceCache";

        // センサーのキャッシュキーの接頭辞
        private static readonly string SensorCacheKeyPrefix = "SensorDeviceRegisterExFilter_SensorCache";

        // 測定項目のキャッシュキーの接頭辞
        private static readonly string ObservationPropertyCacheKeyPrefix = "SensorDeviceRegisterExFilter_ObservationPropertyCache";

        // 測定単位のキャッシュキーの接頭辞
        private static readonly string MeasurementUnitCacheKeyPrefix = "SensorDeviceRegisterExFilter_MeasurementUnitCache";
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // RequestBody取出し
            SensorDeviceRegisterExModel model;
            try
            {
                using (var reader = new StreamReader(param.ContentsStream))
                {
                    model = JsonConvert.DeserializeObject<SensorDeviceRegisterExModel>(reader.ReadToEnd());
                }
            }
            catch (JsonReaderException)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103400);
            }

            // リクエストされたデータストリームを登録する場合は、センサーIDとセンサー群の指定必須(未指定の場合はエラー)
            if (!model.isDefaultDataStream && model.dataStreams?.Any() == true && (string.IsNullOrEmpty(model.sensorId) || model.thing == null))
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103409);
            }

            // 同じセンサーデバイスが登録済みかを確認する(登録済みの場合はエラー)
            var sensorDeviceCacheKey = $"{SensorDeviceCacheKeyPrefix}.{param.OpenId}.{model.sensorId}.{model.sensorUuid}";
            var devices = CacheHelper.GetOrAdd<List<SensorDeviceModel>>(sensorDeviceCacheKey, () =>
            {
                return param.ApiHelper.ExecuteGetApi($"/API/Sensing/V3/Private/SensorDevice/OData?$filter=sensorId eq '{model.sensorId}' and uuid eq '{model.sensorUuid}'")
                    .ToWebApiResponseResult<List<SensorDeviceModel>>()
                    .ThrowRfc7807()
                    .ThrowMessage(x => x.StatusCode != HttpStatusCode.NotFound, x => param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103401))
                    .Result;
            });

            // センサーが指定されている場合は存在するかを確認する(存在しない場合はエラー)
            SensorModel sensor = null;
            if (!string.IsNullOrEmpty(model.sensorId)) {
                sensor = CacheHelper.GetOrAdd<SensorModel>($"{SensorCacheKeyPrefix}.{param.OpenId}.{model.sensorId}", () =>
                {
                    return param.ApiHelper.ExecuteGetApi($"/API/Sensing/V3/Master/Sensors/Get/{model.sensorId}")
                        .ToWebApiResponseResult<SensorModel>()
                        .ThrowRfc7807()
                        .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, x => param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103402))
                        .Result;
                });
            }

            // 登録データの欠損プロパティを修正
            model.FixProperties(sensor);

            // センサー群を登録
            ThingModel thing = null;
            if (model.thing != null)
            {
                thing = model.thing.ToThing();
                var registerResult = param.ApiHelper.ExecutePostApi("/API/Sensing/V3/Private/Things/Register", thing.ToJsonString(true))
                    .ToWebApiResponseResult<RegisterResultModel>()
                    .ThrowRfc7807()
                    .Result;
            }

            // データストリームを登録
            var datastreams = this.CraeteDataStreamModels(param, model, sensor, thing);
            List<string> datastreamIds = null;
            if (datastreams?.Any() == true)
            {
                var registerResults = param.ApiHelper.ExecutePostApi("/API/Sensing/V3/Private/Datastreams/RegisterList", datastreams.ToJsonString(true))
                    .ToWebApiResponseResult<List<RegisterResultModel>>()
                    .ThrowRfc7807()
                    .Result;
                datastreamIds = datastreams.Select(datastream => datastream.key).ToList();
            }

            // センサーデバイスを登録
            var result = param.ApiHelper.ExecutePostApi("/API/Sensing/V3/Private/SensorDevice/Register", model.ToSensorDevice(param.OpenId, sensor?.key, thing?.key, datastreamIds).ToJsonString(true))
                .ToWebApiResponseResult<RegisterResultModel>()
                .ThrowRfc7807()
                .Result;

            // センサーデバイス登録前の取得結果(null)がキャッシュされているので、キャッシュを削除
            CacheHelper.Remove(sensorDeviceCacheKey);
            
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// データストリームのリクエストモデルを作成する。
        /// </summary>
        /// <param name="requestModel">RegisterExのリクエストモデル。</param>
        /// <param name="sensorId">センサーID。</param>
        /// <param name="thingId">センサー群ID。</param>
        /// <returns>データストリームのリクエストモデル。</returns>
        private List<DatastreamModel> CraeteDataStreamModels(IApiFilterActionParam param, SensorDeviceRegisterExModel requestModel, SensorModel sensor, ThingModel thing)
        {
            if (requestModel.isDefaultDataStream)
            {
                if (sensor?.key == null)
                {
                    return null;
                }
                // センサーの測定項目、測定単位それぞれのデータストリームを登録する
                List<DatastreamModel> datastreamList = new List<DatastreamModel>();
                sensor.observations?.ForEach(x =>
                {
                    // データストリームの名称用にデータ取得
                    var prop = GetObservedProperty(param, x.observedPropertyId);
                    x.measurementUnitId?.ForEach(measurementId =>
                    {
                        // データストリームの名称用にデータ取得
                        var measurementUnit = GetMeasurementUnit(param, measurementId);
                        datastreamList.Add(new DatastreamModel
                        {
                            key = Guid.NewGuid().ToString(),
                            name = $"{thing.name}-Datastream-{prop.name}-{measurementUnit.name}",
                            sensorId = sensor.key,
                            thingId = thing.key,
                            observedPropertyId = x.observedPropertyId,
                            unitOfMeasurementId = measurementId,
                        });
                    });
                });
                return datastreamList;
            }
            else
            {
                return requestModel.dataStreams?.Select(stream => stream.ToDataStream(sensor?.key, thing?.key)).ToList();
            }
        }

        /// <summary>
        /// 測定項目の値を取得
        /// </summary>
        /// <param name="param"></param>
        /// <param name="observedPropertyId"></param>
        /// <returns></returns>
        private ObservationPropertiesModel GetObservedProperty(IApiFilterActionParam param, string observedPropertyId)
        {
            var observedProperties = CacheHelper.GetOrAdd<List<ObservationPropertiesModel>>($"{ObservationPropertyCacheKeyPrefix}", () =>
            {
                return param.ApiHelper.ExecuteGetApi($"/API/Sensing/V3/Master/ObservedProperties/OData?$select=key,name")
                    .ToWebApiResponseResult<List<ObservationPropertiesModel>>()
                    .ThrowRfc7807()
                    .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, x => param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103412))
                    .Result;
            });
            return observedProperties?.FirstOrDefault(x => x.key == observedPropertyId);
        }

        /// <summary>
        /// 測定単位の値を取得
        /// </summary>
        /// <param name="param"></param>
        /// <param name="measurementUnitId"></param>
        /// <returns></returns>
        private MeasurementUnitsModel GetMeasurementUnit(IApiFilterActionParam param, string measurementUnitId)
        {
            var measurementUnits = CacheHelper.GetOrAdd<List<MeasurementUnitsModel>>($"{MeasurementUnitCacheKeyPrefix}", () =>
            {
                return param.ApiHelper.ExecuteGetApi($"/API/Sensing/V3/Master/MeasurementUnits/OData?$select=key,name")
                    .ToWebApiResponseResult<List<MeasurementUnitsModel>>()
                    .ThrowRfc7807()
                    .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, x => param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103413))
                    .Result;
            });
            return measurementUnits?.FirstOrDefault(x => x.key == measurementUnitId);
        }
    }
}
