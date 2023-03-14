using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IMailTemplateRepository
    {
        IList<MailTemplateModel> GetList(string vendorId);
        MailTemplateModel Get(string mailTemplateId);
        void Register(MailTemplateModel model);
        MailTemplateModel Update(MailTemplateModel model);
        void Delete(string mailTemplateId);
        bool IsExistsVendorMailTemplate(string vendorId, string mailTemplateId);
        bool IsExistsMailTemplateName(string mailTemplateIdstring, string excludeMailTemplateId);
    }
}
