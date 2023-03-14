using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;

namespace UnitTest.JP.DataHub.Com.WebApi
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

    [WebApiResource("/API/Master/BrandName", typeof(BrandNameModel))]
    public class BrandNameApi_Inheritence : CommonResourceInheritence<BrandNameModel>, IBrandNameApi
    {
        public BrandNameApi_Inheritence()
            : base()
        {
        }
        public BrandNameApi_Inheritence(string serverUrl)
            : base(serverUrl)
        {
        }
        public BrandNameApi_Inheritence(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }
    }

    public class BrandNameApi : CommonResource<BrandNameModel>, IBrandNameApi
    {
        public BrandNameApi()
            : base()
        {
        }
        public BrandNameApi(string serverUrl)
            : base(serverUrl)
        {
        }
        public BrandNameApi(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }
    }
}
