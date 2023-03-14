using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;

namespace JP.DataHub.ApiWeb.Domain.Service.Impl
{
    internal class UserTermsService : AbstractService, IUserTermsService
    {
        private Lazy<IUserTermsRepository> _lazyUserTermsRepository = new Lazy<IUserTermsRepository>(() => UnityCore.Resolve<IUserTermsRepository>());
        private IUserTermsRepository _userTermsRepository { get => _lazyUserTermsRepository.Value; }

        public IList<UserTermsModel> GetList(string open_id)
            => _userTermsRepository.GetList(open_id);

        public UserTermsModel Get(string open_id, string user_terms_id)
            => _userTermsRepository.Get(open_id, user_terms_id);
    }
}
