﻿using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/RdbmsTableJoinTestJoin", typeof(RdbmsTableJoinModel))]
    public interface IRdbmsTableJoinTestJoinApi : ICommonResource<RdbmsTableJoinModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RdbmsTableJoinModel>> GetAll();

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<RdbmsTableJoinModel> model);
    }
}
