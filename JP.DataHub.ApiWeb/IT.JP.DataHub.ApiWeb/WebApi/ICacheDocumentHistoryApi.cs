﻿using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/CacheDocumentHistory", typeof(AreaUnitModelEx))]
    public interface ICacheDocumentHistoryApi : ICommonResource<AreaUnitModelEx>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModelEx>> GetAll();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModelEx model);
    }
}
