using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class GroupMemberFilter : AbstractApiFilter
    {
        // 更新対象のグループにアクセスできるように、OpenIdをグループ所有者のものに差し替える
        // (リクエストユーザがWrite権限を持つメンバーである場合のみ)
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            if (param.QueryStringDic.IsNullOrEmpty("groupId"))
            {
                // groupIdが指定されていない場合は何もしない
                return null;
            }
            var groupId = param.QueryStringDic.GetOrDefault("groupId");

            var existingGroup = param.ApiHelper.ExecuteGetApi(GroupFilter.GetGroupApiUrl(groupId, param.OpenId))
                                            .ToWebApiResponseResult<List<GroupWithOwnerIdModel>>()
                                            .ThrowRfc7807()
                                            .Result
                                            ?.SingleOrDefault();

            // 対象グループがない場合は何もしない
            if (existingGroup != null)
            {
                if (!this.IsWritable(param, existingGroup))
                {
                    // グループ更新可能なユーザでない場合はエラー
                    throw param.MakeRfc7807Response(ErrorCodeMessage.Code.E100410);
                }

                // リクエストユーザにWrite権限がある場合はOpenIdを差し替え
                param.OpenId = existingGroup._Owner_Id;
            }

            return null;
        }

        /// <summary>
        /// リクエストユーザがグループ更新可能かどうかを判定する。
        /// </summary>
        /// <param name="param">APIフィルターのパラメータ。</param>
        /// <param name="group">更新対象のグループ。</param>
        /// <returns>リクエストユーザがグループ更新可能な場合はtrue、それ以外はfalse。</returns>
        private bool IsWritable(IApiFilterActionParam param, GroupModel group)
        {
            // グループの管理者かどうか
            return (group.manager?.Contains(param.OpenId) == true);
        }
    }
}
