using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Primitives;
using Unity;
using Unity.Interception.Utilities;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Interface;

namespace JP.DataHub.ApiWeb.Controllers
{
#if DEBUG
    [EnableCors("corspolicy")]
#endif
    public abstract class AbstractController : UnityAutoInjectionController
    {
        protected const string MEDIATYPE_JSON = "application/json";

        protected IPerRequestDataContainer PerRequestDataContainer => _perRequestDataContainer.Value;
        private Lazy<IPerRequestDataContainer> _perRequestDataContainer = new Lazy<IPerRequestDataContainer>(() => UnityCore.Resolve<IPerRequestDataContainer>());


        [Dependency]
        public IDynamicApiInterface Api { get; set; }


        public AbstractController()
            : base()
        {
        }


        protected IActionResult ToActionResult(DynamicApiResponse dynamicApiResponse)
        {
            dynamicApiResponse.Headers.Keys
                .Where(x => x != HeaderConst.TransferEncoding) // HACK GatewayでChunkDL時にTransfer-Encodingを付与するとクライアントでエラーになる
                .ForEach(key => Response.Headers.TryAdd(key, new StringValues(dynamicApiResponse.Headers[key].ToArray())));
            dynamicApiResponse.Contents?.HttpHeader?.ToList().ForEach(x => Response.Headers.Add(x.Key, new StringValues(x.Value.ToArray())));

            if (dynamicApiResponse.Contents == null)
            {
                return new StatusCodeResult((int)dynamicApiResponse.StatusCode);
            }
            else if (dynamicApiResponse.Contents?.IsStreamContents == true)
            {
                return new ObjectResult(dynamicApiResponse.Contents.Stream) { StatusCode = (int)dynamicApiResponse.StatusCode, ContentTypes = new() { dynamicApiResponse.Contents.ContentType } };
            }
            else if (!string.IsNullOrEmpty(dynamicApiResponse.Contents?.StringContents))
            {
                return new ContentResult() { StatusCode = (int)dynamicApiResponse.StatusCode, Content = dynamicApiResponse.Contents?.StringContents, ContentType = dynamicApiResponse.Contents.ContentType };
            }
            else
            {
                return new StatusCodeResult((int)dynamicApiResponse.StatusCode);
            }
        }
    }
}