﻿using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Public/ShipmentSensorRawData", typeof(ShipmentSensorRawDataResultModel))]
    public interface IShipmentSensorRawDataApi
    {
        [WebApiGet("GetRawDataSensor?ProductCode={ProductCode}&ArrivalId={ArrivalId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<ShipmentSensorRawDataResultModel>> GetRawDataSensor(string ProductCode, string ArrivalId);
    }
}
