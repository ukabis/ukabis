using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    public class BrandNameModel
    {
        public string BrandNameCode { get; set; }
        public string BrandName { get; set; }
    }

    [WebApiResource("/API/Master/BrandName", typeof(BrandNameModel))]
    public interface IBrandNameApi : ICommonResource<BrandNameModel>
    {
    }
}
