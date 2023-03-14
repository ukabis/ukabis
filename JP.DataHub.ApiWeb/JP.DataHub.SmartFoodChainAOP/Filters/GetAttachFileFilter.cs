using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class GetAttachFileFilter : AbstractApiFilter
    {
        private static readonly string QUERYSTRING_FILE_ID = "FileId";
        
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            var urlparam = param.QueryStringDic.Validate<Guid>(QUERYSTRING_FILE_ID, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E105400/*FileIdがありません*/));
            var fileId = param.QueryStringDic.GetOrDefault(QUERYSTRING_FILE_ID);

            //添付ファイル取得
            if (param.IsInternalCall)
            {
                param.IsOverPartition = true;
            }
            return null;
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg) => msg;
    }
}
