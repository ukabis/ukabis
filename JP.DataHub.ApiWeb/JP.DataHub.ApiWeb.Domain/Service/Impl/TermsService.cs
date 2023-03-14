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

namespace JP.DataHub.ApiWeb.Domain.Service.Impl
{
    internal class TermsService : AbstractService, ITermsService
    {
        private Lazy<ITermsRepository> _lazyTermsRepository = new Lazy<ITermsRepository>(() => UnityCore.Resolve<ITermsRepository>());
        private ITermsRepository _termsRepository { get => _lazyTermsRepository.Value; }

        public IList<TermsGroupModel> GroupGetList()
            => _termsRepository.GroupGetList();

        public string GroupRegister(TermsGroupModel model)
             => _termsRepository.GroupRegister(model);

        public void GroupDelete(string terms_group_code)
             => _termsRepository.GroupDelete(terms_group_code);

        public TermsModel TermsGet(string terms_id)
             => _termsRepository.TermsGet(terms_id);

        public IList<TermsModel> TermsGetList()
            => _termsRepository.TermsGetList();

        public IList<TermsModel> TermsGetListByTermGroupCode(string terms_group_code)
            => _termsRepository.TermsGetListByTermGroupCode(terms_group_code);

        public string TermsRegister(TermsModel model)
        {
            if (string.IsNullOrEmpty(model.TermsId))
            {
                model.TermsId = Guid.NewGuid().ToString();
            }
            return _termsRepository.TermsRegister(model);
        }

        public void TermsDelete(string terms_id)
             => _termsRepository.TermsDelete(terms_id);

        public void Agreement(string open_id, string terms_id)
             => _termsRepository.Agreement(open_id, terms_id);

        public void Revoke(string open_id, string terms_id)
             => _termsRepository.Revoke(open_id, terms_id);
    }
}
