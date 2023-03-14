using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Service
{
    public interface IRevokeService
    {
        UserRevokeModel Start(string user_terms_id, string open_id);
        void Stop(string user_revoke_id, string open_id);
        RemoveHistoryModel RemoveResourceStart(string user_revoke_id, string controller_id, string open_id);
        void RemoveResourceStop(string revoke_history_id, string open_id);
    }
}
