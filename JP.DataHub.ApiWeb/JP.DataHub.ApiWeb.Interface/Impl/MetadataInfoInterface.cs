using AutoMapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.MetadataInfo;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo;

namespace JP.DataHub.ApiWeb.Interface
{
    /// <summary>
    /// APIのメタデータを取得します。
    /// </summary>
    public class MetadataInfoInterface : IMetadataInfoInterface
    {
        /// <summary>
        /// AutoMapperの設定
        /// </summary>
        private static Lazy<IMapper> s_mapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApiDescriptionModel, ApiDescription>().ReverseMap();
                cfg.CreateMap<MethodDescriptionModel, MethodDescription>().ReverseMap();
                cfg.CreateMap<SchemaDescriptionModel, SchemaDescription>().ReverseMap();
                cfg.CreateMap<CategoryModel, Category>().ReverseMap();
                cfg.CreateMap<FieldModel, Field>().ReverseMap();
                cfg.CreateMap<TagModel, Tag>().ReverseMap();
                cfg.CreateMap<SampleCodeModel, SampleCode>().ReverseMap();
                cfg.CreateMap<MethodLinkModel, MethodLink>().ReverseMap();
            });

            return config.CreateMapper();
        });

        private static IMapper Mapper => s_mapper.Value;


        private IMetadataInfoApplicationService ApplicationService = UnityCore.Resolve<IMetadataInfoApplicationService>();


        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="culture">カルチャ(ロケール)</param>
        /// <param name="isActiveOnly">削除フラグの検索条件</param>
        /// <param name="isEnableOnly">有効フラグの検索条件</param>
        /// <param name="isNotHiddenOnly">非公開フラグの検索条件</param>
        /// <returns>API情報の一覧</returns>
        public IEnumerable<ApiDescriptionModel> GetApiDescription(bool noChildren, string culture = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false)
            => Mapper.Map<IEnumerable<ApiDescriptionModel>>(ApplicationService.GetApiDescription(noChildren, culture, isActiveOnly, isEnableOnly, isNotHiddenOnly));

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <param name="culture">カルチャ(ロケール)</param>
        /// <returns>スキーマ情報の一覧</returns>
        public IEnumerable<SchemaDescriptionModel> GetSchemaDescription(string culture = null)
            => Mapper.Map<IEnumerable<SchemaDescriptionModel>>(ApplicationService.GetSchemaDescription(culture));

        /// <summary>
        /// メタデータを作成します。
        /// </summary>
        /// <param name="apis">API情報</param>
        /// <param name="schemas">スキーマ情報</param>
        /// <param name="urlSchemas">URLスキーマ情報</param>
        /// <returns></returns>
        public string CreateMetadata(List<ApiDescriptionModel> apis, List<SchemaDescriptionModel> schemas, List<SchemaDescriptionModel> urlSchemas)
            => ApplicationService.CreateMetadata(Mapper.Map<List<ApiDescription>>(apis), Mapper.Map<List<SchemaDescription>>(schemas), Mapper.Map<List<SchemaDescription>>(urlSchemas));
    }
}
