using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    internal interface IMailTemplateApplicationService
    {
        MailTemplateModel Get(string mailTemplateId);
        IList<MailTemplateModel> GetList(string vendorId);
        MailTemplateModel Register(MailTemplateModel model);
        void Delete(string mailTemplateId);
        ApiMailTemplateModel GetApiMailTemplate(string apiMailTemplateId);
        IList<ApiMailTemplateModel> GetApiMailTemplateList(string apiId, string vendorId);
        ApiMailTemplateModel RegisterApiMailTemplate(ApiMailTemplateModel model);
        ApiMailTemplateModel UpdateApiMailTemplate(ApiMailTemplateModel model);
        void DeleteApiMailTemplate(string apiMailTemplateId);
    }
}
