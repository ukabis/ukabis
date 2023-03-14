using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Service.Impl
{
    internal class RevokeService : AbstractService, IRevokeService
    {
        private Lazy<IRevokeRepository> _lazyRevokeRepository = new Lazy<IRevokeRepository>(() => UnityCore.Resolve<IRevokeRepository>());
        private IRevokeRepository _revokeRepository { get => _lazyRevokeRepository.Value; }

        public UserRevokeModel Start(string user_terms_id, string open_id)
            => _revokeRepository.Start(user_terms_id, open_id);

        public void Stop(string user_revoke_id, string open_id)
            => _revokeRepository.Stop(user_revoke_id, open_id);

        public RemoveHistoryModel RemoveResourceStart(string user_revoke_id, string controller_id, string open_id)
            => _revokeRepository.RemoveResourceStart(user_revoke_id, controller_id, open_id);

        public void RemoveResourceStop(string revoke_history_id, string open_id)
            => _revokeRepository.RemoveResourceStop(revoke_history_id, open_id);
    }
}
