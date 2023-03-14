using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class GroupDeleteExFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            var groupId= param.QueryStringDic.GetOrDefault("groupId");

            // 指定されたグループを削除する(領域越えだが、管理者のみ利用可能)
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

            if (group == null)
            {
                // 指定されたグループがなければエラー(NotFound)
                return param.MakeRfc7807Response(ErrorCode.ErrorCodeMessage.Code.E100403).Response;
            }
            else if (group.manager?.Contains(param.OpenId) != true)
            {
                // リクエストユーザが管理者ではない場合はエラー(Forbidden)
                return param.MakeRfc7807Response(ErrorCode.ErrorCodeMessage.Code.E100404).Response;
            }

            return null;
        }
    }
}
