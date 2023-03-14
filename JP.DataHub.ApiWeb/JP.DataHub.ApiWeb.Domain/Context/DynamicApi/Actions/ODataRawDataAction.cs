using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class ODataRawDataAction : ODataAction
    {
        public override HttpResponseMessage ExecuteAction()
        {
            // 運用管理ベンダー以外の呼び出しはForbbiden(暫定)
            // 将来的にはAPIの所有ベンダーであれば許可したい
            if (!PerRequestDataContainer.IsOperatingVendorUser)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10441, RelativeUri?.Value);
            }

            // データ移行用透過APIのため強制的に領域越え+管理項目ありとする
            IsOverPartition = new IsOverPartition(true);
            XGetInnerAllField = new XGetInnerField(true);
            PerRequestDataContainer.XgetInternalAllField = true;

            return base.ExecuteAction();
        }
    }
}
