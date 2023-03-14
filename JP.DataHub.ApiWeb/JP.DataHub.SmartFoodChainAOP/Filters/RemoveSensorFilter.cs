﻿using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class RemoveSensorFilter : AbstractApiFilter
    {
        // API
        private static readonly string ShipmentSensorApi = "/API/Traceability/V3/Private/ShipmentSensor/ODataCertifiedApplication?$filter=ShipmentSensorId eq '{0}'";
        private static readonly string ArrivalApi = "/API/Traceability/V3/Private/Arrival/ODataCertifiedApplication?$filter=ArrivalId eq '{0}'";
        private static readonly string ShipmentSensorUpdateApi = "/API/Traceability/V3/Private/ShipmentSensor/UpdateInternal/{0}";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // パラメータ取得
            if (string.IsNullOrEmpty(param.QueryString))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E106402, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            // パラメータ検証
            var shipmentSensorId = param.QueryStringDic.GetOrDefault("ShipmentSensorId");
            var lastArrivalId = param.QueryStringDic.GetOrDefault("LastArrivalId");
            if (string.IsNullOrEmpty(shipmentSensorId))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E106403, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
            if (string.IsNullOrEmpty(lastArrivalId))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E106404, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            // 出荷のセンサー付帯を取得
            var shipmentSensor = param.ApiHelper.ExecuteGetApi(string.Format(ShipmentSensorApi, shipmentSensorId))
                    .ToWebApiResponseResult<List<ShipmentSensorModel>>()
                    .ThrowRfc7807()
                    .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E106501))
                    .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E106405))
                    .Result
                    .Single();

            // 入荷を取得
            var arrival = param.ApiHelper.ExecuteGetApi(string.Format(ArrivalApi, lastArrivalId))
                    .ToWebApiResponseResult<List<ArrivalModel>>()
                    .ThrowRfc7807()
                    .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E106501))
                    .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E106406))
                    .Result
                    .Single();

            // 終了日時を更新
            if (shipmentSensor.MeasurementStartDateTime != null &&
                shipmentSensor.MeasurementEndDateTime == null)
            {
                shipmentSensor.MeasurementEndDateTime = arrival.ArrivalDate;
                shipmentSensor.LastArrivalId = lastArrivalId;

                param.ApiHelper.ExecutePatchApi(string.Format(ShipmentSensorUpdateApi, shipmentSensor.ShipmentSensorId), JsonConvert.SerializeObject(shipmentSensor))
                    .ToWebApiResponseResult<VoidModel>()
                    .ThrowRfc7807()
                    .ThrowMessage(x => !x.IsSuccessStatusCode, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E106507));
            }
            else
            {
                // 測定開始していない or 測定終了済の場合はBadRequest
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E106408, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            return new HttpResponseMessage() { StatusCode = HttpStatusCode.NoContent };
        }
    }
}