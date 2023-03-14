using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Aop;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;

namespace JP.DataHub.SmartFoodChainAOP.Helper
{
    internal static class JasPrintHelper
    {
        // キャッシュキーの接頭辞
        private static readonly string JudgmentHistoryPrefix = "JasPrint.JudgmentHistoryCache";
        private static readonly string ProductCodeDetailCacheKeyPrefix = "JasPrint.ProductCodeDetailCache";
        private static readonly string CropFreshnessJudgmentCacheKeyPrefix = "JasPrint.CropFreshnessJudgment";
        private static readonly string ProductCacheKeyPrefix = "JasPrint.ProductCache";
        private static readonly string ShipmentSensorRowDataCacheKeyPrefix = "JasPrint.ShipmentSensorRowData";
        private static readonly string ObservationPropertiesCacheKeyPrefix = "Sensor.ObservationProperties";
        private static readonly string MeasurementUnitsCacheKeyPrefix = "Sensor.MeasurementUnits";
        private static readonly string ShipmentAndArrivalCacheKeyPrefix = "ShipmentAndArrival";

        // APIのURL
        private static readonly string ProductCodeDetailApi = "/API/Traceability/V3/Private/ProductDetail/ODataOtherAccessible?$filter=ProductCode eq '{0}'";
        private static readonly string ProductApi = "/API/Traceability/V3/Private/CompanyProduct/ODataOtherAccessible?$filter=GtinCode eq '{0}'";
        //private static readonly string JasPrintLogApi = "/API/Traceability/V3/Private/JasPrintLog/?$filter=ProductCode eq '{0}'";
        private static readonly string ShipmentSensorRowData = "/API/Traceability/V3/Public/ShipmentSensorRawData/GetRawDataSensor?product={0}&gln={1}";
        private static readonly string GetShipmentAndArrival = "/API/Traceability/V3/Public/Traceability/GetShipmentAndArrival?ProductCode={0}&ArrivalId={1}";
        private static readonly string ObservationProperties = "/API/Sensing/V3/Master/ObservedProperties";
        private static readonly string MeasurementUnits = "/API/Sensing/V3/Master/MeasurementUnits";
        private static readonly string CropFreshnessJudgment = "/API/Traceability/JasFreshnessManagement/V3/Public/CropFreshnessManagement/OData?$filter=CropCode eq '{0}'";
        private static readonly string JudgmentHistoryApi = "/API/Traceability/JasFreshnessManagement/V3/Public/Judgment/GetHistory?product={0}&gln={1}&arrivalId={2}";
        private static readonly string JasCertificationHistory = "/API/Traceability/JasFreshnessManagement/V3/Public/JasCertificationHistory";
        private static readonly string JudgmentHistory = "/API/Traceability/JasFreshnessManagement/V3/Public/JudgmentHistory";

        internal static Func<List<JudgmentModel>> GetJudgmentHistoryFunc(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode, string gtinCode, string arrivalId)
        {
            Func<List<JudgmentModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<JudgmentModel>>($"{JudgmentHistoryPrefix}.{productCode}.{gtinCode}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(JudgmentHistoryApi, Regex.Escape(productCode), Regex.Escape(gtinCode), Regex.Escape(arrivalId)))
                        .ToWebApiResponseResult<List<JudgmentModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return func;
        }

        internal static Task<List<JudgmentModel>> GetJudgmentHistoryAsync(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode, string gtinCode, string arrivalId)
        {
            Func<List<JudgmentModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<JudgmentModel>>($"{JudgmentHistoryPrefix}.{productCode}.{gtinCode}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(JudgmentHistoryApi, Regex.Escape(productCode), Regex.Escape(gtinCode), Regex.Escape(arrivalId)))
                        .ToWebApiResponseResult<List<JudgmentModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return Task.Run(() => func());
        }


        internal static Func<List<ProductCodeDetailModel>> GetProductDetailFunc(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode)
        {
            Func<List<ProductCodeDetailModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<ProductCodeDetailModel>>($"{ProductCodeDetailCacheKeyPrefix}.{productCode}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(ProductCodeDetailApi, Regex.Escape(productCode)))
                        .ToWebApiResponseResult<List<ProductCodeDetailModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return func;
        }

        internal static Task<List<ProductCodeDetailModel>> GetProductCodeDetailAsync(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode)
        {
            Func<List<ProductCodeDetailModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<ProductCodeDetailModel>>($"{ProductCodeDetailCacheKeyPrefix}.{productCode}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(ProductCodeDetailApi, Regex.Escape(productCode)))
                        .ToWebApiResponseResult<List<ProductCodeDetailModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return Task.Run(() => func());
        }

        internal static Task<TraceabilityModel> GetShipmentAndArrivalAsync(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode, string arrivalId)
        {
            Func<TraceabilityModel> func = () =>
                filter.CacheHelper.GetOrAdd<TraceabilityModel>($"{ShipmentAndArrivalCacheKeyPrefix}.{productCode}.{arrivalId}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(GetShipmentAndArrival, Regex.Escape(productCode), Regex.Escape(arrivalId)))
                        .ToWebApiResponseResult<TraceabilityModel>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return Task.Run(() => func());
        }

        internal static Func<List<ProductModel>> GetProductFunc(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode)
        {
            Func<List<ProductModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<ProductModel>>($"{ProductCacheKeyPrefix}.{productCode}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(ProductApi, Regex.Escape(productCode)))
                        .ToWebApiResponseResult<List<ProductModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return func;
        }

        internal static Func<List<CropFreshnessJudgmentModel>> GetFreshnessManagementFunc(this AbstractApiFilter filter, IApiFilterActionParam param, string cropCode)
        {
            Func<List<CropFreshnessJudgmentModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<CropFreshnessJudgmentModel>>($"{CropFreshnessJudgmentCacheKeyPrefix}.{cropCode}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(CropFreshnessJudgment, Regex.Escape(cropCode)))
                        .ToWebApiResponseResult<List<CropFreshnessJudgmentModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return func;
        }

        internal static Func<List<ShipmentSensorRawDataModel>> GetShipmentSensorRowDataFunnc(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode, string glnCode)
        {
            Func<List<ShipmentSensorRawDataModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<ShipmentSensorRawDataModel>>($"{ShipmentSensorRowDataCacheKeyPrefix}.{productCode}.{glnCode}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(ShipmentSensorRowData, Regex.Escape(productCode), Regex.Escape(glnCode)))
                        .ToWebApiResponseResult<List<ShipmentSensorRawDataModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return func;
        }

        internal static Func<List<ObservationPropertiesModel>> GetObservationPropertiesFunc(this AbstractApiFilter filter, IApiFilterActionParam param)
        {
            Func<List<ObservationPropertiesModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<ObservationPropertiesModel>>($"{ObservationPropertiesCacheKeyPrefix}", () =>
                    param.ApiHelper.ExecuteGetApi(ObservationProperties)
                        .ToWebApiResponseResult<List<ObservationPropertiesModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return func;
        }

        internal static Task<List<ObservationPropertiesModel>> GetObservationPropertiesAsync(this AbstractApiFilter filter, IApiFilterActionParam param)
        {
            Func<List<ObservationPropertiesModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<ObservationPropertiesModel>>($"{ObservationPropertiesCacheKeyPrefix}", () =>
                    param.ApiHelper.ExecuteGetApi(ObservationProperties)
                        .ToWebApiResponseResult<List<ObservationPropertiesModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return Task.Run(() => func());
        }


        internal static Func<List<MeasurementUnitsModel>> GetMeasurementUnitsFunc(this AbstractApiFilter filter, IApiFilterActionParam param)
        {
            Func<List<MeasurementUnitsModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<MeasurementUnitsModel>>($"{MeasurementUnitsCacheKeyPrefix}", () =>
                    param.ApiHelper.ExecuteGetApi(MeasurementUnits)
                        .ToWebApiResponseResult<List<MeasurementUnitsModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return func;
        }

        internal static Task<List<MeasurementUnitsModel>> GetMeasurementUnitsAsync(this AbstractApiFilter filter, IApiFilterActionParam param)
        {
            Func<List<MeasurementUnitsModel>> func = () =>
                filter.CacheHelper.GetOrAdd<List<MeasurementUnitsModel>>($"{MeasurementUnitsCacheKeyPrefix}", () =>
                    param.ApiHelper.ExecuteGetApi(MeasurementUnits)
                        .ToWebApiResponseResult<List<MeasurementUnitsModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                        .Result);
            return Task.Run(() => func());
        }


        internal static Action RegisterJudgmentHistoryFunc(this AbstractApiFilter filter, IApiFilterActionParam param, JudgmentModel result)
        {
            Action action = () => param.ApiHelper.ExecutePostApi($"{JudgmentHistory}/Register", JsonConvert.SerializeObject(result))
                    .ToWebApiResponseResult<RegisterResultModel>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x => !x.IsSuccessStatusCode, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102414));
            return action;
        }

        internal static Action RegisterJasCertificationHistoryFunc(this AbstractApiFilter filter, IApiFilterActionParam param, string productCode, string glnCode, string jasCertificationCode)
        {
            var contentsJson = new JasCertificationHistoryModel() { ProductCode = productCode, GlnCode = glnCode, JasCertificationCode = new List<string>() { jasCertificationCode } };
            Action action = () => param.ApiHelper.ExecutePostApi($"{JasCertificationHistory}/Register", JsonConvert.SerializeObject(contentsJson))
                    .ToWebApiResponseResult<RegisterResultModel>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x => !x.IsSuccessStatusCode, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102414));
            return action;
        }
    }
}
