using AutoMapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.Interface.Model;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    class ScriptRuntimeLogApplicationService : IScriptRuntimeLogApplicationService
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ScriptRuntimeLogGetFile, ScriptRuntimeLogFileModel>()
                    .ForMember(d => d.ScriptRuntimeLogId, opt => opt.MapFrom(s => s.ScriptRuntimeLogId))
                    .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name.Value))
                    .ForMember(d => d.FilePath, opt => opt.MapFrom(s => s.FilePath.Value))
                    .ForMember(d => d.ContentType, opt => opt.MapFrom(s => s.ContentType.Value))
                    .ForMember(d => d.Content, opt => opt.MapFrom(s => s.Content));
            });
            return mappingConfig.CreateMapper();
        });
        private IMapper _mapper { get { return s_lazyMapper.Value; } }

        private IScriptRuntimeLogFileRepository _repository = UnityCore.Resolve<IScriptRuntimeLogFileRepository>();


        public ScriptRuntimeLogFileModel Get(Guid logId, Guid vendorId) => _mapper.Map<ScriptRuntimeLogFileModel>(_repository.Get(logId, vendorId));

        public bool Delete(Guid logId, Guid vendorId) => _repository.Delete(logId, vendorId);
    }
}
