using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IApiMailTemplateRepository
    {
        IList<ApiMailTemplateModel> GetList(string apiId, string vendorId);
        ApiMailTemplateModel Get(string apiMailTemplateId);
        void Register(ApiMailTemplateModel model);
        ApiMailTemplateModel Update(ApiMailTemplateModel model);
        void Delete(string apiMailTemplateId);
        bool IsExistsApiMailTemplate(string apiId, string vendorId, string mailTemplateId, string? excludeApiMailTemplateId = null);
        bool CheckUsedMailTemplate(string mailTemplateId);
    }
}
