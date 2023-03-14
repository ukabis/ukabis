using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/MailTemplate", typeof(MailTemplateModel))]
    public interface IMailTemplateApi
    {
        [WebApi("GetMailTemplate?mailTemplateId={mailTemplateId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<MailTemplateModel> GetMailTemplate(string mailTemplateId);

        [WebApi("GetMailTemplateList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<MailTemplateModel>> GetMailTemplateList(string vendorId);

        [WebApiPost("RegisterMailTemplate")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<MailTemplateModel> RegisterMailTemplate(MailTemplateModel model);

        [WebApiPost("UpdateMailTemplate")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<MailTemplateModel> UpdateMailTemplate(MailTemplateModel model);

        [WebApiDelete("DeleteMailTemplate?mailTemplateId={mailTemplateId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteMailTemplate(string mailTemplateId);
    }
}
