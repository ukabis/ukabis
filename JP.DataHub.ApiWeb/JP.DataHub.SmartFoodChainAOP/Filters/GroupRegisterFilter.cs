using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class GroupRegisterFilter : AbstractApiFilter
    {
        private readonly int RetryCount
            = int.TryParse(ConfigurationManager.AppSettings["SmartFoodChainGetReservedAccountRetryCount"], out var value) ? value : 3;
        private readonly int MinAccountCount
            = int.TryParse(ConfigurationManager.AppSettings["SmartFoodChainMinReservedAccountCount"], out var value) ? value : 100;

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // RequestBody取出し
            if (param.ContentsStream == null)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
            }
            GroupModel model;
            try
            {
                try
                {
                    using (var reader = new StreamReader(param.ContentsStream, Encoding.UTF8, true, 1024, true))
                    {
                        model = JsonConvert.DeserializeObject<GroupModel>(reader.ReadToEnd());

                        // 内部呼び出し(=このフィルターからの登録処理)や更新処理の場合は基盤側でreadするのでストリームを先頭に戻す
                        param.ContentsStream.Position = 0;
                    }
                }
                catch (JsonException)
                {
                    param.ContentsStream.Close();
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
                }
                if (model == null)
                {
                    param.ContentsStream.Close();
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
                }

                if (param.IsInternalCall)
                {
                    // 内部呼び出しの場合はこのフィルターからの登録処理と判断し、OpenIdを代表者のOpenIdに差し替えてそのまま登録する
                    param.OpenId = model.representativeMember.openId;
                    return null;
                }

                if (!string.IsNullOrEmpty(model.groupId) && this.IsUpdate(param, model.groupId, out var existingGroup))
                {
                    // グループ更新可能なユーザでない場合はエラー
                    if (!this.IsWritable(param, existingGroup)) {
                        throw param.MakeRfc7807Response(ErrorCodeMessage.Code.E100410);
                    }

                    // グループ更新の場合、代表者の変更はエラー
                    if (model.representativeMember?.Equals(existingGroup.representativeMember) == false)
                    {
                        throw param.MakeRfc7807Response(ErrorCodeMessage.Code.E100408);
                    }

                    // 管理者の指定が不正な場合はエラー
                    ValidateManager(param, model);

                    // グループ所有者のOpenIdに差し替える
                    // (新仕様グループはグループ代表者、旧仕様グループはグループ作成者の個人領域に存在するため)
                    param.OpenId = existingGroup._Owner_Id;

                    // そのまま更新
                    return null;
                }
                else
                {
                    // 管理者の指定が不正な場合はエラー
                    ValidateManager(param, model);

                    // 新規登録
                    model.groupId = Guid.NewGuid().ToString();
                }

                param.ContentsStream.Close();
            }
            catch
            {
                param.ContentsStream.Close();
                throw;
            }

            // 予約枠アカウントの残数チェック
            this.LogReservedAccountCount(param);

            // 予約枠アカウントを取得
            var reservedAccount = this.GetReservedAccount(param);

            // 予約枠アカウントで全規約に同意
            var terms = param.TermsHelper.TermsGetList();
            terms.ForEach(x => param.TermsHelper.Agreement(reservedAccount.Account.OpenId, x.TermsId));

            // Groupを作成
            var result = this.RegisterGroup(param, model, reservedAccount);

            // GroupMap作成
            var groupMap = new GroupMapModel()
            {
                groupMapId = Guid.NewGuid().ToString(),
                groupId = model.groupId,
                ReservedAccountId = reservedAccount.ReservedAccountId,
                Account = reservedAccount.Account
            };
            param.ApiHelper.ExecutePostApi("/API/Global/Public/GroupMap/Register", groupMap.ToJsonString())
                        .ToWebApiResponseResult<RegisterResultModel>()
                        .ThrowRfc7807();

            return new HttpResponseMessage() { StatusCode = result.StatusCode, Content = new StringContent(JsonConvert.SerializeObject(result.Result), Encoding.UTF8, "application/json") };
        }

        /// <summary>
        /// リクエストがグループ更新かどうかを判定する。
        /// </summary>
        /// <param name="ex">APIフィルターのパラメータ。</param>
        /// <param name="groupId">グループID。</param>
        /// <param name="group">更新である場合、更新対象のグループが返される。</param>
        /// <returns>リクエストがグループ更新である場合はtrue、それ以外はfalse。</returns>
        private bool IsUpdate(IApiFilterActionParam param, string groupId, out GroupWithOwnerIdModel group)
        {
            var headers = new Dictionary<string, List<string>>()
            {
                { "X-GetInternalAllField", new List<string>() { "true" } }
            };
            var existingGroup = param.ApiHelper.ExecuteGetApi(GroupFilter.GetGroupApiUrl(groupId, param.OpenId), null, null, headers)
                                            .ToWebApiResponseResult<List<GroupWithOwnerIdModel>>()
                                            .ThrowRfc7807()
                                            .Result
                                            ?.SingleOrDefault();

            // 既に登録済みのグループである場合は更新
            if (existingGroup != null)
            {
                group = existingGroup;
                return true;
            }

            group = null;
            return false;
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

        /// <summary>
        /// 予約枠アカウントの残数が少ない場合にログを出力する。
        /// </summary>
        /// <param name="param">APIフィルターのパラメータ。</param>
        private void LogReservedAccountCount(IApiFilterActionParam param)
        {
            param.ApiHelper.ExecuteGetApi("/API/Global/Public/ReservedAccount/GetUnusedCount")
                .ToWebApiResponseResult<int>()
                .ThrowRfc7807()
                .Action(response => response.Result < MinAccountCount, response =>
                {
                    Logger.Warn($"There’s only a limited ReservedAccount left. {response.Result} accounts left.");
                });
        }

        /// <summary>
        /// 予約枠アカウントを取得する。
        /// </summary>
        /// <param name="param">APIフィルターのパラメータ。</param>
        /// <returns>予約枠アカウント。</returns>
        private ReservedAccountModel GetReservedAccount(IApiFilterActionParam param)
        {
            bool ignoreRetry = false;
            return Policy.Handle<AopResponseException>(e => !ignoreRetry)
                         .Retry(RetryCount)
                         .Execute<ReservedAccountModel>(() =>
                         {
                             // 予約枠アカウント取得
                             var account = param.ApiHelper.ExecuteGetApi("/API/Global/Public/ReservedAccount/OData?$filter=IsUsed eq false&$top=1")
                                                       .ToWebApiResponseResult<List<ReservedAccountModel>>()
                                                       .ThrowMessage(response => response.StatusCode == HttpStatusCode.NotFound, response =>
                                                       {
                                                           // アカウントが1つも残っていない場合はエラー
                                                           ignoreRetry = true;
                                                           return param.MakeRfc7807Response(ErrorCodeMessage.Code.E100406);
                                                       })
                                                       .ThrowRfc7807()
                                                       .Result
                                                       .Single();

                             // 予約済みアカウントを使用済みに更新
                             // (GroupとGroupMapを作成後に更新したいが、409になった場合のロールバックが大変なので先に確保する)
                             account.IsUsed = true;
                             param.ApiHelper.ExecutePostApi("/API/Global/Public/ReservedAccount/Register", account.ToJsonString())
                                         .ToWebApiResponseResult<RegisterResultModel>()
                                         .ThrowMessage(response => response.StatusCode == HttpStatusCode.Conflict, response =>
                                         {
                                             // 既に使用済みになっていた場合はエラー(リトライ)
                                             return param.MakeRfc7807Response(ErrorCodeMessage.Code.E100407);
                                         })
                                         .ThrowRfc7807();

                             return account;
                         });
        }

        /// <summary>
        /// グループを作成する。
        /// </summary>
        /// <param name="ex">APIフィルターのパラメータ。</param>
        /// <param name="group">登録するグループ。</param>
        /// <param name="reservedAccount">予約枠アカウント。</param>
        /// <remarks>
        /// グループの代表者は予約枠アカウントとなり、予約枠アカウントの個人領域に登録する。
        /// また、リクエストで設定されていた代表者はメンバーとして登録する。
        /// </remarks>
        /// <returns>グループ作成リクエストに対するレスポンス。</returns>
        private WebApiResponseResult<RegisterResultModel> RegisterGroup(IApiFilterActionParam param, GroupModel group, ReservedAccountModel reservedAccount)
        {
            // グループ代表者が指定されていた場合はメンバーに追加
            if (group.representativeMember != null)
            {
                if (group.member == null)
                {
                    group.member = new List<MemberModel>();
                }
                group.member.Add(group.representativeMember);
            }

            // 予約枠アカウントを代表者に設定
            group.representativeMember = new MemberModel()
            {
                openId = reservedAccount.Account.OpenId,
                mailAddress = reservedAccount.Account.Account,
                accessControl = new List<string>()
                {
                    "ReadWrite"
                }
            };

            // 予約枠アカウントのデータとしてGroupを登録
            // ここでは内部呼び出しのOpenIdを差し替えられないため、内部呼び出しでAPIフィルターを通過した時に差し替える
            var orgOpenId = param.OpenId;
            var result = param.ApiHelper.ExecutePostApi("/API/Global/Private/Groups/Register", group.ToJsonString())
                                     .ToWebApiResponseResult<RegisterResultModel>()
                                     .ThrowRfc7807();

            // OpenIdを戻す
            param.OpenId = orgOpenId;

            return result;
        }

        /// <summary>
        /// グループ管理者の共通バリデーション
        /// </summary>
        /// <param name="group"></param>
        private void ValidateManager(IApiFilterActionParam param, GroupModel group)
        {
            // 管理者が未設定または代表者/メンバーに含まれない場合はエラー
            if (group.manager?.Any() != true)
            {
                throw param.MakeRfc7807Response(ErrorCodeMessage.Code.E100411);
            }
            
            if (!group.manager.All(x => 
                x.ToLower() == group.representativeMember?.openId.ToLower() || 
                group.member?.Any(y => x.ToLower() == y.openId.ToLower()) == true))
            {
                throw param.MakeRfc7807Response(ErrorCodeMessage.Code.E100412);
            }
        }
    }
}
