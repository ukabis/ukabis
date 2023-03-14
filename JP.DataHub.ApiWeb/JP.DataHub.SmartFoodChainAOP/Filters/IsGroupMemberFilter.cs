using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class IsGroupMemberFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            var groupId = param.QueryStringDic["groupId"];
            var group = CacheHelper.GetOrAdd<GroupModel>(GroupFilter.GetGroupCacheKey(param.OpenId, groupId), () =>
            {
                var groupApiUrl = GroupFilter.GetGroupApiUrl(groupId, param.OpenId);
                var result = param.ApiHelper.ExecuteGetApi(groupApiUrl)
                    .ToWebApiResponseResult<List<GroupModel>>()
                    .Action(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, response =>
                    {
                        Logger.Error($"Failed at {groupApiUrl}. Response: {response.RawContentString}");
                    })
                    .ThrowRfc7807();
                return result.Result?.SingleOrDefault();
            });

            var isGroupMember = false;
            if (group != null)
            {
                var scope = param.QueryStringDic["scope"];
                if (group.scope?.Contains(scope) == true)
                {
                    // scopeが一致するグループが存在する場合はtrue
                    isGroupMember = true;
                }
                else if (group.scope?.Contains(GroupModel.Scope.All.ToString()) == true && Enum.TryParse<GroupModel.Scope>(scope, out var result))
                {
                    // グループのscopeがAllかつ指定されていたscopeが妥当ならtrue
                    isGroupMember = true;
                }
            }
            return HttpStatusCode.OK.ToHttpResponseMessage(new IsGroupMemberResult() { isGroupMember = isGroupMember });
        }
    }
}
