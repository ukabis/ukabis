﻿using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ReferenceNotifyODataFirst", typeof(ReferenceNotifyFirstModel))]
    public interface IReferenceNotifyODataFirstApi : ICommonResource<ReferenceNotifyFirstModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(ReferenceNotifyFirstModel model);
    }
}
