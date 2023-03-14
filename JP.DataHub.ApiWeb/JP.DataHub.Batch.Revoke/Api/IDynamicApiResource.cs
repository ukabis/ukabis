using JP.DataHub.Batch.Revoke.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke.Api
{
    [WebApiResource("", typeof(UserRevokeResponseModel))]
    public interface IDynamicApiResource : ICommonResource<UserRevokeResponseModel>
    {
        [WebApiDelete("ODataDeleteOverPartition?$filter=_Owner_Id eq '{owner_id}'")]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataDeleteOverPartition(string owner_id);

    }
}
