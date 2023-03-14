using AutoMapper;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;
using System.Text;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    internal class DynamicApiInterface : IDynamicApiInterface
    {
        [Dependency]
        public IDynamicApiApplicationService Api { get; set; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg => cfg.CreateMap<AsyncApiResult, AsyncApiResultModel>()
                .ForMember(dst => dst.Status, ops => ops.MapFrom(src => src.Status.ToString()))
            );
            return mappingConfig.CreateMapper();

        });
        private static IMapper s_mapper => s_lazyMapper.Value;


        public DynamicApiInterface()
        {
            this.AutoInjection();
        }

        public HttpResponseMessage Get(DynamicApiRequestModel request, bool notAuthentication = false)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Post(DynamicApiRequestModel request, bool notAuthentication = false)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Put(DynamicApiRequestModel request, bool notAuthentication = false)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Delete(DynamicApiRequestModel request, bool notAuthentication = false)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Patch(DynamicApiRequestModel request, bool notAuthentication = false)
        {
            throw new NotImplementedException();
        }

        public DynamicApiResponse Request(DynamicApiRequestModel request, bool notAuthentication = false)
        {
            var result = Api.Request(
                request.HttpMethod.ToHttpMethodType(),
                request.RelativeUri.ToRequestRelativeUri(),
                new Contents(request.GetContentsStream()),
                request.QueryString.ToQueryString(),
                request.Header.ToHttpHeaderValueObject(),
                request.MediaType.ToMediaType(),
                request.Accept.ToAccept(),
                request.ContentRange.ToContentRange(),
                request.ContentType.ToContentType(),
                request.ContentLength.ToContentLength(),
                new NotAuthentication(notAuthentication)
            );
            return result;
        }


        public AsyncApiResultModel GetStatus(string requestId)
        {
            return s_mapper.Map<AsyncApiResultModel>(Api.GetStatus(new AsyncRequestId(requestId)));
        }

        public DynamicApiResponse GetResult(string requestId)
        {
            return Api.GetResult(new AsyncRequestId(requestId));
        }

        public bool SetResult(Stream content, string blobPath, string accept = null)
        {
            if (string.IsNullOrEmpty(blobPath))
            {
                return false;
            }
            if (string.IsNullOrEmpty(accept) || accept == "*/*")
            {
                accept = "application/json";
            }
            return Api.SetResult(content, blobPath, accept);
        }
        public bool SetResult(string content, string blobPath, string accept = null)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(blobPath))
            {
                return false;
            }
            if (string.IsNullOrEmpty(accept) || accept == "*/*")
            {
                accept = "application/json";
            }

            return Api.SetResult(new MemoryStream(Encoding.UTF8.GetBytes(content)), blobPath, accept);
        }
        public bool SetResultOverwrite(Stream content, string blobPath, string accept = null)
        {
            if (string.IsNullOrEmpty(blobPath))
            {
                return false;
            }
            if (string.IsNullOrEmpty(accept) || accept == "*/*")
            {
                accept = "application/json";
            }
            return Api.SetResultOverwrite(content, blobPath, accept);
        }

        public bool SetResultOverwrite(string content, string blobPath, string accept = null)
        {
            if (string.IsNullOrEmpty(blobPath))
            {
                return false;
            }
            if (string.IsNullOrEmpty(accept) || accept == "*/*")
            {
                accept = "application/json";
            }
            return Api.SetResultOverwrite(content, blobPath, accept);
        }

        /// <summary>
        /// リソースURLからデータモデル(文字列)を返す
        /// </summary>
        /// <param name="url">リソースURL</param>
        /// <returns>データモデル文字列</returns>
        public string GetControllerSchemaByUrl(string url) => Api.GetControllerSchemaByUrl(url);

        /// <summary>
        /// モデル名からデータモデル（文字列）を返す
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetSchemaModelByName(string name) => Api.GetSchemaModelByName(name);
    }
}
