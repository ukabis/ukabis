﻿using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/CompanyMaster/V3/Private/CompanyCertified", typeof(CompanyCertifiedModel))]
    public interface ICompanyCertifiedApi : ICommonResource<CompanyCertifiedModel>
    {
        [WebApiPost("RegisterCertifiedApplication")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResultModel> RegisterCertifiedApplication(CompanyCertifiedModel model);
    }
}