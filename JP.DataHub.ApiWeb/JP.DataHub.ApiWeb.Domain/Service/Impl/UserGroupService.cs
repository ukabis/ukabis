using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Domain.Service.Impl
{
    internal class UserGroupService : AbstractService, IUserGroupService
    {
        private Lazy<IUserGroupRepository> _lazyUserGroupRepository = new Lazy<IUserGroupRepository>(() => UnityCore.Resolve<IUserGroupRepository>());
        private IUserGroupRepository _userGroupRepository { get => _lazyUserGroupRepository.Value; }

        public IList<UserGroupModel> GetList(string open_id)
            => _userGroupRepository.GetList(open_id);

        public string Register(UserGroupModel model)
        {
            if (string.IsNullOrEmpty(model.UserGroupId))
            {
                model.UserGroupId = Guid.NewGuid().ToString();
            }
            model.OpenId = PerRequestDataContainer.OpenId;
            return _userGroupRepository.Register(model);
        }

        public void Delete(string user_group_id)
            => _userGroupRepository.Delete(user_group_id);
    }
}
