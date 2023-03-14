using System.Net;
using AutoMapper;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Models.ScriptRuntimeLog;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class ScriptRuntimeLogMetaDataRepository : AbstractRepository, IScriptRuntimeLogMetaDataRepository
    {
        private const string MEDIATYPE_JSON = "application/json";
        private static string s_registerUrl = UnityCore.Resolve<string>("ScriptRuntimeLogDynamicApiUrlRegist");

        private IDynamicApiInterface _dynamicApiInterface => _lazyDynamicApiInterface.Value;
        private Lazy<IDynamicApiInterface> _lazyDynamicApiInterface = new Lazy<IDynamicApiInterface>(() => UnityCore.Resolve<IDynamicApiInterface>());

        private IMapper _mapper => s_lazyMapper.Value;
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ScriptRuntimeLogMetaData, ScriptRuntimeLogMetaDataModel>()
                    .ForMember(d => d.ScriptRuntimeLogId, opt => opt.MapFrom(s => s.ScriptRuntimeLogId))
                    .ForMember(d => d.ApiId, opt => opt.MapFrom(s => s.ApiId))
                    .ForMember(d => d.ExecStartDate, opt => opt.MapFrom(s => s.ExecStartDate.Value))
                    .ForMember(d => d.ExecDurationMsec, opt => opt.MapFrom(s => s.ExecDurationMsec.Value))
                    .ForMember(d => d.IsError, opt => opt.MapFrom(s => s.IsError.Value))
                    .ForMember(d => d.RegUserName, opt => opt.MapFrom(s => s.RegUserName))
                    .ForMember(d => d.RegDate, opt => opt.MapFrom(s => s.RegDate.Value))
                    ;
            });
            return mappingConfig.CreateMapper();
        });


        public HttpResponseMessage Create(ScriptRuntimeLogMetaData data)
        {
            var model = _mapper.Map<ScriptRuntimeLogMetaDataModel>(data);
            var registModel = new DynamicApiRequestModel
            {
                HttpMethod = "POST",
                RelativeUri = s_registerUrl,
                Contents = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                MediaType = MEDIATYPE_JSON
            };

            var res = this._dynamicApiInterface.Request(registModel, true);
            if (res.StatusCode != HttpStatusCode.Created)
            {
                throw new ApiException(res.ToHttpResponseMessage());
            }
            return res.ToHttpResponseMessage();
        }
    }
}