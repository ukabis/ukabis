#if (false) // このAOPは不要になったため
using JP.DataHub.Aop;
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
    public class ArrivalRegisterFilter : AbstractApiFilter
    {
        // API
        private static readonly string ShipmentSensorApi = "/API/SmartFoodChain/V2/Private/ShipmentSensor/ODataFullAccess?$Filter=ShipmentId eq '{0}'";
        private static readonly string ShipmentSensorUpdateApi = "/API/SmartFoodChain/V2/Private/ShipmentSensor/UpdateFullAccess/{0}";

        // メソッド名
        private static readonly string registerApi = "Register";
        private static readonly string registerListApi = "RegisterList";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // RequestBody取出し
            string contents = "";

            if (param.ContentsStream == null)
            {
                return null;
            }

            var ms = new MemoryStream();
            param.ContentsStream.CopyTo(ms);
            param.ContentsStream.Position = 0;
            ms.Position = 0;
            using (var sr = new StreamReader(ms))
            {
                contents = sr.ReadToEnd();
                if (string.IsNullOrEmpty(contents))
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102402, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
                }
            }

            if (param.ApiUrl.StartsWith(registerListApi))
            {
                // 複数登録
                List<ArrivalModel> contentsJson;
                try
                {
                    contentsJson = JsonConvert.DeserializeObject<List<ArrivalModel>>(contents);
                }
                catch (JsonReaderException)
                {
                    return null;
                }

                foreach (var arrival in contentsJson)
                {
                    Main(param, arrival);
                }
            }
            else
            {
                // 1件登録
                ArrivalModel contentsJson;
                try
                {
                    contentsJson = JsonConvert.DeserializeObject<ArrivalModel>(contents);
                }
                catch (JsonReaderException)
                {
                    return null;
                }

                Main(param, contentsJson);
            }
            return null;
        }

        private void Main(IApiFilterActionParam param, ArrivalModel arrival)
        {
            // ShipmentIdがあるか
            if (string.IsNullOrEmpty(arrival.ShipmentId))
            {
                return;
            }
            var shipmentId = arrival.ShipmentId;
            
            // 出荷のセンサー付帯を取得
            var shipmentSensors = param.ApiHelper.ExecuteGetApi(string.Format(ShipmentSensorApi, shipmentId))
                    .ToWebApiResponseResult<List<ShipmentSensorModel>>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x=>!x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound,m=> param.MakeRfc7807Response(ErrorCodeMessage.Code.E106501))
                    .Result;

            if(shipmentSensors == null || !shipmentSensors.Any()) return; 

            // 終了日時を更新
            foreach (var shipmentSensor in shipmentSensors)
            {
                var isUpdate = false;
                if (shipmentSensor.MeasurementStartDateTime != null)
                {
                    shipmentSensor.MeasurementEndDateTime = arrival.ArrivalDate;
                    isUpdate = true;
                }

                foreach (var package in shipmentSensor.ProductShipmentPackage)
                {
                    if (package.ProductCode.Count == 1)
                    {
                        // センサーに紐付いている商品コードが1件のときだけ更新（暫定）

                        foreach (var arrivalProduct in arrival.ArrivalProduct)
                        {
                            if (arrivalProduct.ProductCode == package.ProductCode[0])
                            {
                                if (package.MeasurementStartDateTime != null)
                                {
                                    package.MeasurementEndDateTime = arrival.ArrivalDate;
                                    isUpdate = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.Info($"ShipmentSensor.ProductShipmentPackageProductCode.Count > 1");
                    }
                }

                if (isUpdate)
                {
                    param.ApiHelper.ExecutePatchApi(string.Format(ShipmentSensorUpdateApi, shipmentSensor.ShipmentSensorId), JsonConvert.SerializeObject(shipmentSensor))
                        .ToWebApiResponseResult<VoidModel>()
                        .ThrowRfc7807();
                }
            }
        }
    }
}
#endif