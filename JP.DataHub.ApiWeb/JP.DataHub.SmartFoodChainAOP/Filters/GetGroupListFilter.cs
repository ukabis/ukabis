using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
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
    public class GetGroupListFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // リクエストユーザが所属するグループのリストを返す
            var scope = param.QueryStringDic?.GetOrDefault("scope");
            var groupApiUrl = GroupFilter.GetGroupApiUrl("", param.OpenId, scope);
            return param.ApiHelper.ExecuteGetApi(groupApiUrl);
        }
    }
}
