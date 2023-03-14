using JP.DataHub.AdminWeb.WebAPI.Models.Api;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Resources
{
    /// <summary>
    /// DynamicAPIをリクエストするためのリソース。
    /// 他の***Resourceとは異なり、各メソッドでリソースのURLを指定して呼び出す。
    /// </summary>
    [WebApiResource("", typeof(ApiResourceModel))]
    public interface IDynamicApiResource : IResource
    {
        [WebApiPost("AdaptResourceSchema", true)]
        WebApiRequestModel AdaptResourceSchema(string resourceUrl);
    }

    [DomainUrl(DomainType.DynamicApi)]
    public class DynamicApiResource : Resource, IDynamicApiResource
    {
        public DynamicApiResource()
        {
        }

        public DynamicApiResource(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        public DynamicApiResource(IServerEnvironment serverEnvironment)
        {
            ServerEnvironment = serverEnvironment;
            ServerUrl = GetDomainUrl();
        }

        public WebApiRequestModel AdaptResourceSchema(string resourceUrl) {
            var requestModel = MakeApiRequestModel<WebApiRequestModel>();
            // 任意のリソースをリクエストできるようにするため、AutoGenerateReturnModelは使わずにResourceUrlを設定
            requestModel.ResourceUrl = resourceUrl;
            return requestModel;
        }
    }
}
