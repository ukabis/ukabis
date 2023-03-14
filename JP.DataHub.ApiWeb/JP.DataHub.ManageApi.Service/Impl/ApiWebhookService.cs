using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Exceptions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class ApiWebhookService : AbstractService, IApiWebhookService
    {
        private IApiWebhookRepository ApiWebhookRepository => _apiWebhookRepository.Value;
        private Lazy<IApiWebhookRepository> _apiWebhookRepository = new(() => UnityCore.Resolve<IApiWebhookRepository>());

        private IVendorRepository VendorRepository => _vendorRepository.Value;
        private Lazy<IVendorRepository> _vendorRepository = new(() => UnityCore.Resolve<IVendorRepository>());

        private IDynamicApiRepository DynamicApiRepository => _dynamicApiAdminRepository.Value;
        private Lazy<IDynamicApiRepository> _dynamicApiAdminRepository = new(() => UnityCore.Resolve<IDynamicApiRepository>());

        /// <summary>
        /// 指定されたApiWebhookIdのApiWebhookを取得します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        /// <returns>Webhook</returns>
        public ApiWebhookModel Get(string apiWebhookId)
            => ApiWebhookRepository.Get(apiWebhookId);

        /// <summary>
        /// Webhookを登録します。
        /// </summary>
        /// <param name="model">Webhook</param>
        /// <returns>登録結果</returns>
        public ApiWebhookModel Register(ApiWebhookModel model)
        {
            if (ApiWebhookRepository.IsExists(model.ApiId, model.VendorId))
                throw new AlreadyExistsException("Webhook for this ApiId already exists.");
            if (!VendorRepository.IsExists(model.VendorId))
                throw new ForeignKeyException("This VendorId does not exist.");
            if (!DynamicApiRepository.IsExists(model.ApiId, model.VendorId))
                throw new NotFoundException("This ApiId does not exist.");

            // ApiWebhookIdが無い場合は新規登録
            if (string.IsNullOrEmpty(model.ApiWebhookId))
            {
                model.ApiWebhookId = Guid.NewGuid().ToString();
                ApiWebhookRepository.Register(model);
            }
            // ApiWebhookIdがある場合は更新
            else ApiWebhookRepository.Update(model);

            return model;
        }

        /// <summary>
        /// Webhookを更新します。
        /// </summary>
        /// <param name="model">Webhook</param>
        /// <returns>更新結果</returns>
        public ApiWebhookModel Update(ApiWebhookModel model)
        {
            if (!ApiWebhookRepository.IsExists(model.ApiId, model.VendorId))
                throw new NotFoundException("Webhook for this ApiId does not exist.");
            if (!VendorRepository.IsExists(model.VendorId))
                throw new ForeignKeyException("This VendorId does not exist.");
            if (!DynamicApiRepository.IsExists(model.ApiId, model.VendorId))
                throw new NotFoundException($"ApiWebhook Dose Not Exist. ApiWebhookId:{model.ApiWebhookId}");

            ApiWebhookRepository.Update(model);

            return model;
        }

        /// <summary>
        /// 指定されたベンダーIDのApiWebhook一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ApiWebhookId</param>
        /// <returns>Webhook一覧</returns>
        public IList<ApiWebhookModel> GetList(string vendorId)
        {
            if (string.IsNullOrEmpty(vendorId))
            {
                throw new ArgumentNullException();
            }

            var result = ApiWebhookRepository.GetList(vendorId);
            if (result.Count == 0)
                throw new NotFoundException();
            return result;
        }

        /// <summary>
        /// Webhookを削除します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        public void Delete(string apiWebhookId)
        {
            // Getが結果のnullチェックを実施
            var model = Get(apiWebhookId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            ApiWebhookRepository.Delete(apiWebhookId);
        }
    }
}