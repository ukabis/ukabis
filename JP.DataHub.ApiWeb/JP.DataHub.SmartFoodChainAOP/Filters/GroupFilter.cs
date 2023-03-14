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
using JP.DataHub.SmartFoodChainAOP.Shared;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class GroupFilter : AbstractApiFilter
    {
        // グループのキャッシュキーの接頭辞
        private static readonly string GroupFilterCacheKeyPrefix = "GroupFilterCache"; 

        // グループ取得API(領域越えのAPI)
        private static readonly string GroupApi = "/API/Global/Private/Groups/GetBy";

        /// <summary>
        /// グループIDとOpenIdでグループを取得するAPIのURLを返す。
        /// </summary>
        /// <param name="groupId">グループID。</param>
        /// <param name="openId">OpenId。</param>
        /// <returns>APIのURL。</returns>
        public static string GetGroupApiUrl(string groupId, string openId, string scope = "")
        {
            return $"{GroupApi}?groupId={groupId}&scope={scope}&member={openId}";
        }

        /// <summary>
        /// グループのキャッシュキーを取得する。
        /// </summary>
        /// <param name="openId">リクエストユーザのOpenID。</param>
        /// <param name="groupId">グループID。</param>
        /// <returns>グループのキャッシュキー。</returns>
        public static string GetGroupCacheKey(string openId, string groupId = null)
        {
            return $"{GroupFilterCacheKeyPrefix}.{openId}{(string.IsNullOrEmpty(groupId) ? "" : $".{groupId}")}";
        }

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // リクエストしたメンバーが属するグループの代表者のOpenIdでリクエストが実行されるようにする
            if (param?.QueryStringDic?.IsNullOrEmpty("groupId") != false)
            {
                // グループID未指定なら何もしない
                return null;
            }
            var groupId = param.QueryStringDic["groupId"];
            var group = CacheHelper.GetOrAdd<GroupModel>(GetGroupCacheKey(param.OpenId, groupId), () =>
            {
                var groupApiUrl = GetGroupApiUrl(groupId, param.OpenId);
                var result = param.ApiHelper.ExecuteGetApi(groupApiUrl)
                    .ToWebApiResponseResult<List<GroupModel>>()
                    .Action(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, response =>
                    {
                        Logger.Error($"Failed at {groupApiUrl}. Response: {response.RawContentString}");
                    })
                    .ThrowRfc7807()
                    .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m =>
                    {
                        // 指定されたグループが取得できなかった場合はエラー
                        Logger.Error($"Group({groupId}) is not found. Requested by OpenID:{param.OpenId}.");
                        return param.MakeRfc7807Response(ErrorCodeMessage.Code.E100409);
                    });
                return result.Result.SingleOrDefault();
            });

            if (group.representativeMember == null)
            {
                // グループに代表者が設定されていない場合はエラー
                Logger.Error($"{param.OpenId} is a member of group({group.groupId}). But the group does not have representative.");
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100401);
            }

#if DEBUG
            // GroupFilter適用確認用(GroupTestで使用)
            if (param.Headers.ContainsKey("X-GroupFilterTest"))
            {
                var response = new
                {
                    MemberOpenId = param.OpenId,
                    GroupOpenId = group.representativeMember.openId
                };
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json") };
            }
#endif

            // グループ代表者のOpenIdに差し替え
            param.OpenId = group.representativeMember.openId;

            // groupIdをクエリ文字列から削除
            // (削除しないと、SQL ServerリポジトリのリソースでgroupIdがテーブルに存在しないためエラーになる)
            param.QueryStringDic.Remove("groupId");
            param.QueryString = param.QueryStringDic.ToQueryString();

            return null;
        }
    }
}