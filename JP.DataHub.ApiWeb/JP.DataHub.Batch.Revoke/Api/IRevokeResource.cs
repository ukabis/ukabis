using JP.DataHub.Batch.Revoke.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke.Api
{
    [WebApiResource("/Manage/Revoke", typeof(UserRevokeResponseModel))]
    public interface IRevokeResource : ICommonResource<UserRevokeResponseModel>
    {
        [WebApiGet("Start?user_terms_id={user_terms_id}&open_id={open_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<UserRevokeResponseModel> Start(string user_terms_id,string open_id);

        [WebApiGet("Stop?user_revoke_id={user_revoke_id}&open_id={open_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Stop(string user_revoke_id, string open_id);

        [WebApiGet("RemoveResourceStart?user_revoke_id={user_revoke_id}&resource_id={resource_id}&open_id={open_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RevokeHistoryModel> RemoveResourceStart(string user_revoke_id, string resource_id, string open_id);

        [WebApiGet("RemoveResourceStop?revoke_history_id={revoke_history_id}&open_id={open_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel RemoveResourceStop(string revoke_history_id, string open_id);
    }
}
