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
    internal class UserResourceShareService : AbstractService, IUserResourceShareService
    {
        private Lazy<IUserResourceShareRepository> _lazyUserResourceShareRepository = new Lazy<IUserResourceShareRepository>(() => UnityCore.Resolve<IUserResourceShareRepository>());
        private IUserResourceShareRepository _userResourceShareRepository { get => _lazyUserResourceShareRepository.Value; }

        public IList<UserResourceShareModel> GetList(string open_id)
            => _userResourceShareRepository.GetList(open_id);

        public string Register(UserResourceShareModel model)
        {
            if (string.IsNullOrEmpty(model.UserResourceGroupId))
            {
                model.UserResourceGroupId = Guid.NewGuid().ToString();
            }
            model.OpenId = PerRequestDataContainer.OpenId;
            return _userResourceShareRepository.Register(model);
        }

        public void Delete(string user_resource_group_id)
            => _userResourceShareRepository.Delete(user_resource_group_id);
    }
}
