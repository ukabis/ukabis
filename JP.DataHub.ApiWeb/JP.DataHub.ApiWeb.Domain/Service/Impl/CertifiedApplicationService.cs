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
    internal class CertifiedApplicationService : AbstractService, ICertifiedApplicationService
    {
        private Lazy<ICertifiedApplicationRepository> _lazyCertifiedApplicationRepository = new Lazy<ICertifiedApplicationRepository>(() => UnityCore.Resolve<ICertifiedApplicationRepository>());
        private ICertifiedApplicationRepository _certifiedApplicationRepository { get => _lazyCertifiedApplicationRepository.Value; }

        public IList<CertifiedApplicationModel> GetList()
            => _certifiedApplicationRepository.GetList();

        public CertifiedApplicationModel Get(string certified_application_id)
            => _certifiedApplicationRepository.Get(certified_application_id);

        public string Register(CertifiedApplicationModel model)
        {
            if (string.IsNullOrEmpty(model.CertifiedApplicationId))
            {
                model.CertifiedApplicationId = Guid.NewGuid().ToString();
            }
            return _certifiedApplicationRepository.Register(model);
        }

        public void Delete(string certified_application_id)
            => _certifiedApplicationRepository.Delete(certified_application_id);
    }
}
