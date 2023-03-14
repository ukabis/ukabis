using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using Unity.Interception.Utilities;
using JP.DataHub.Com.Exceptions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class ApiDescriptionService : IApiDescriptionService
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper _mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IDynamicApiRepository> _lazyDynamicApiRepository = new Lazy<IDynamicApiRepository>(() => UnityCore.Resolve<IDynamicApiRepository>());
        private IDynamicApiRepository DynamicApiRepository { get => _lazyDynamicApiRepository.Value; }

        private Lazy<IDynamicApiService> _lazyDynamicApiService = new Lazy<IDynamicApiService>(() => UnityCore.Resolve<IDynamicApiService>());
        private IDynamicApiService DynamicApiService { get => _lazyDynamicApiService.Value; }

        /// <summary>
        /// ベンダーのリンク情報の一覧を取得します。
        /// </summary>
        /// <returns>ベンダーのリンク情報</returns>
        public IEnumerable<VendorLinkModel> GetVendorLink()
            => DynamicApiRepository.GetVendorLink();

        /// <summary>
        /// システムのリンク情報の一覧を取得します。
        /// </summary>
        /// <returns>システムのリンク情報</returns>
        public IEnumerable<SystemLinkModel> GetSystemLink()
            => DynamicApiRepository.GetSystemLink();

        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">削除フラグの検索条件</param>
        /// <param name="isEnableOnly">有効フラグの検索条件</param>
        /// <param name="isNotHiddenOnly">非公開フラグの検索条件</param>
        /// <returns>API情報の一覧</returns>
        public IEnumerable<ApiDescriptionModel> GetApiDescription(bool noChildren, string localeCode = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false)
            => DynamicApiRepository.GetApiDescription(noChildren, localeCode, isActiveOnly, isEnableOnly, isNotHiddenOnly);

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <returns>スキーマ情報の一覧</returns>
        public IEnumerable<SchemaDescriptionModel> GetSchemaDescription(string localeCode = null)
            => DynamicApiRepository.GetSchemaDescription(localeCode);

        /// <summary>
        /// カテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>カテゴリーの一覧</returns>
        public IEnumerable<CategoryModel> GetCategoryList(string localeCode = null)
            => DynamicApiRepository.GetCategoryList(localeCode);

        /// <summary>
        /// 指定されたAPI IDのStaticApiを登録します。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public RegisterStaticApiResponseModel RegisterStaticApi(StaticApiModel requestModel)
        {
            var staticApiDb = DynamicApiRepository.GetStaticApiList(requestModel.Controller.ControllerId);
            var result = RefreshStaticApiResourceAndApi(new List<StaticApiModel>() { requestModel }, staticApiDb).FirstOrDefault();
            if(result == null)
            {
                throw new NotFoundException($"No API is registered, updated or deleted with API ID {requestModel.Controller.ControllerId}.");
            }
            return result;
        }

        /// <summary>
        /// StaticApiを一括登録する。
        /// </summary>
        public IEnumerable<RegisterStaticApiResponseModel> RegisterStaticApi(IEnumerable<StaticApiModel> staticApiList)
        {
            var staticApiDb = DynamicApiRepository.GetStaticApiList();
            return RefreshStaticApiResourceAndApi(staticApiList, staticApiDb);
        }

        /// <summary>
        /// StaticApiのResource及びApiを登録・更新・削除する。
        /// </summary>
        private IEnumerable<RegisterStaticApiResponseModel> RefreshStaticApiResourceAndApi(IEnumerable<StaticApiModel> sourceList, IEnumerable<StaticApiId> dbList)
        {
            var results = new List<RegisterStaticApiResponseModel>();

            sourceList.ForEach(source =>
            {
                if (source.DetectedInSourceCode)
                {
                    DynamicApiService.RegistOrUpdateController(source.Controller);
                    RefreshStaticApiApi(source.ApiList, dbList.Where(db => db.ControllerId.ToUpper() == source.Controller.ControllerId.ToUpper()));
                    results.Add(new RegisterStaticApiResponseModel() { ApiId = source.Controller.ControllerId, Url = source.Controller.Url, Message = "Registered or Updated" });
                }
                else if (dbList.Any(db => db.ControllerId.ToUpper() == source.Controller.ControllerId.ToUpper()))
                {
                    DynamicApiService.DeleteController(source.Controller.ControllerId);
                    results.Add(new RegisterStaticApiResponseModel() { ApiId = source.Controller.ControllerId, Message = "Deleted" });
                }
            });

            dbList.Select(x => x.ControllerId).Distinct().ForEach(dbControllerId =>
            {
                if (sourceList.All(source => source.Controller.ControllerId.ToUpper() != dbControllerId.ToUpper()))
                {
                    DynamicApiService.DeleteController(dbControllerId);
                    results.Add(new RegisterStaticApiResponseModel() { ApiId = dbControllerId, Message = "Deleted" });
                }
            });

            return results;
        }


        /// <summary>
        /// StaticApiのApiを登録・更新・削除する。
        /// </summary>
        private void RefreshStaticApiApi(IEnumerable<ApiInformationModel> sourceList, IEnumerable<StaticApiId> dbList)
        {
            sourceList.ForEach(source =>
            {
               DynamicApiService.RegistOrUpdateApi(source);
            });

            dbList.ForEach(db =>
            {
                if (!sourceList.Any(source => source.ApiId.ToUpper() == db.ApiId.ToUpper()))
                {
                    DynamicApiService.DeleteApi(db.ApiId);
                }
            });
        }
    }
}
