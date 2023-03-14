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
    public class CropModel
    {
        public string CropCode { get; set; }
        public string CropName { get; set; }
    }

    [WebApiResource("/API/Master/BrandName", typeof(CropModel))]
    public interface ICropApi : ICommonResource<CropModel>
    {
    }
}
