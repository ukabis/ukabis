using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Service
{
    internal interface ICertifiedApplicationService
    {
        IList<CertifiedApplicationModel> GetList();
        CertifiedApplicationModel Get(string certified_application_id);
        string Register(CertifiedApplicationModel model);
        void Delete(string certified_application_id);
    }
}
