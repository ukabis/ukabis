using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class ShipmentSensorRawDataGetRawDataSensorFilter : AbstractApiFilter
    {
        private static readonly string ManagementApi = "/API/Traceability/V3/Public/SensorTraceability/Summary/GetLatest?ProductCode={0}&ArrivalId={1}&Time=0";

        private IConfiguration _configuration { get; } = UnityCore.Resolve<IConfiguration>();

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // パラメータ取得
            if (string.IsNullOrEmpty(param.QueryString))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103403, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            var productCode = param.QueryStringDic.GetOrDefault("ProductCode");
            var arrivalId = param.QueryStringDic.GetOrDefault("ArrivalId");

            // パメータ検証
            if (string.IsNullOrEmpty(productCode))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103404, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
            if (string.IsNullOrEmpty(arrivalId))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103405, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            // 管理用データ取得
            var result = param.ApiHelper.ExecuteGetApi(string.Format(ManagementApi, productCode, arrivalId))
                            .ToWebApiResponseResult<SensorTraceabilitySummaryModel>()
                            .ThrowRfc7807(HttpStatusCode.BadRequest)
                            .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E103406))
                            .ThrowMessage(x => !x.IsSuccessStatusCode, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E103501))
                            .Result;

            // BLOB取得
            try
            {
                var connectionString = _configuration.GetSection("ConnectionStrings:SmartFoodChain:SensorDataStorageConnectionStrings").Get<string>();
                var container = new BlobContainerClient(connectionString, result.blobContainer);
                var blob = container.GetBlobClient(result.blobFileName);
                var properties = blob.GetProperties();
                if (properties.Value.ContentLength == 0)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103407, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(blob.DownloadStreaming().Value.Content)
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    return response;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Unexpected error occured on download blob. (Message={e.Message})");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103502, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
        }
    }
}