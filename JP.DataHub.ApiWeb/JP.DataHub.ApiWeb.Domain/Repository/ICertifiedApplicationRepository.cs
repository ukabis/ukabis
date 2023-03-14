using JP.DataHub.ApiWeb.Domain.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    internal interface ICertifiedApplicationRepository
    {
        IList<CertifiedApplicationModel> GetList();
        CertifiedApplicationModel Get(string certified_application_id);
        string Register(CertifiedApplicationModel model);
        void Delete(string certified_application_id);
    }
}
