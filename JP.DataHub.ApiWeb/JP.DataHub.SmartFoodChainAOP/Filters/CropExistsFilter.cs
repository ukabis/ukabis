using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Aop;
using JP.DataHub.Com.Consts;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.Com.Net.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net;
using Newtonsoft.Json;
using NLog.Filters;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class CropFilter
    {
        public static StaticCacheControl<List<CropModel>> scCropExists = new StaticCacheControl<List<CropModel>>();
    }

    public class CropExistsFilter : AbstractApiFilter
    {
        private const string NG = "NG";

        // "Exists/{CropCode}"
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            var cropCode = param.QueryStringDic.GetOrDefault("CropCode");
            string exists = CacheHelper.GetOrAdd($"CropExists.{cropCode}", () => CropExits(param, cropCode));
            if (exists != null && exists != NG)
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(exists, Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("", Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }
        }

        private string CropExits(IApiFilterActionParam param, string cropCode)
        {
            List<CropModel> list = null;
            if (!CropFilter.scCropExists.TryGet(out list))
            {
                CropFilter.scCropExists.Refresh(() =>
                    param.ApiHelper.ExecuteGetApi(string.Format("/API/Master/Crop/GetList"))
                            .ToWebApiResponseResult<List<CropModel>>()
                            .ThrowRfc7807(HttpStatusCode.BadRequest)
                            .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => "")
                            .Result);
                CropFilter.scCropExists.TryGet(out list);
            }
            return FindCropIdByCropCode(list, cropCode);
        }


        private string FindCropIdByCropCode(List<CropModel> list, string cropCode)
        {
            if (list == null)
            {
                return NG;
            }
            var hit = list?.FirstOrDefault(x => x.CropCode == cropCode);
            if (hit != null)
            {
                return $"[ {{ \"id\" : \"API~Master~Crop~1~{hit.CropCode}\" }} ]";
            }
            foreach (var x in list)
            {
                var hitdl = FindCropIdByCropCode(x.DownLevelCrop, cropCode);
                if (hitdl != null)
                {
                    return hitdl;
                }
            }
            return NG;
        }
    }
}