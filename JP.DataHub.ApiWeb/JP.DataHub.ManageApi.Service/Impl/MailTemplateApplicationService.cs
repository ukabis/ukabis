using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class MailTemplateApplicationService : AbstractService, IMailTemplateApplicationService
    {
        private Lazy<IMailTemplateRepository> _lazyMailTemplateRepository = new Lazy<IMailTemplateRepository>(() => UnityCore.Resolve<IMailTemplateRepository>());
        private Lazy<IApiMailTemplateRepository> _lazyApiMailTemplateRepository = new Lazy<IApiMailTemplateRepository>(() => UnityCore.Resolve<IApiMailTemplateRepository>());
        private IMailTemplateRepository MailTemplateRepository { get => _lazyMailTemplateRepository.Value; }
        private IApiMailTemplateRepository ApiMailTemplateRepository { get => _lazyApiMailTemplateRepository.Value; }

        public ApiMailTemplateModel GetApiMailTemplate(string apiMailTemplateId)
            => ApiMailTemplateRepository.Get(apiMailTemplateId);

        public IList<ApiMailTemplateModel> GetApiMailTemplateList(string apiId, string vendorId)
            => ApiMailTemplateRepository.GetList(apiId, vendorId);

        public ApiMailTemplateModel RegisterApiMailTemplate(ApiMailTemplateModel model)
        {
            if (ApiMailTemplateRepository.IsExistsApiMailTemplate(model.ApiId, model.VendorId, model.MailTemplateId))
            {
                throw new AlreadyExistsException($"指定されたメールテンプレートは既に使用されています。{model.ApiId},{model.VendorId},{model.MailTemplateId}");
            }
            // 指定したベンダーがメールテンプレートを保持していない場合
            if (MailTemplateRepository.IsExistsVendorMailTemplate(model.VendorId, model.MailTemplateId) == false)
            {
                throw new NotFoundException($"vendor does not have a MailTemplate.{model.MailTemplateId}");
            }
            // 登録なのでActiveにする
            model.IsActive = true;
            ApiMailTemplateRepository.Register(model);
            return model;
        }

        public ApiMailTemplateModel UpdateApiMailTemplate(ApiMailTemplateModel model)
        {
            if (ApiMailTemplateRepository.IsExistsApiMailTemplate(model.ApiId, model.VendorId, model.MailTemplateId, model.ApiMailTemplateId))
            {
                throw new AlreadyExistsException($"指定されたメールテンプレートは既に使用されています。{ model.ApiId },{ model.VendorId},{ model.MailTemplateId}");
            }
            // 指定したベンダーがメールテンプレートを保持していない場合
            if (MailTemplateRepository.IsExistsVendorMailTemplate(model.VendorId, model.MailTemplateId) == false)
            {
                throw new NotFoundException($"vendor does not have a MailTemplate.{model.MailTemplateId}");
            }
            ApiMailTemplateRepository.Update(model);
            return model;
        }

        public void DeleteApiMailTemplate(string apiMailTemplateId)
        {
            var model = ApiMailTemplateRepository.Get(apiMailTemplateId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            ApiMailTemplateRepository.Delete(apiMailTemplateId);
        }

        public MailTemplateModel Get(string mailTemplateId)
            => MailTemplateRepository.Get(mailTemplateId);


        public IList<MailTemplateModel> GetList(string vendorId)
            => MailTemplateRepository.GetList(vendorId);

        public MailTemplateModel Register(MailTemplateModel model)
        {
            // 同じテンプレート名があるかチェック
            if (MailTemplateRepository.IsExistsMailTemplateName(model.MailTemplateName, model.MailTemplateId))
            {
                throw new AlreadyExistsException($"指定されたメールテンプレート名は既に存在します。{ model.MailTemplateId }");
            }

            // MailTemplateIdが無い場合は新規登録
            if (string.IsNullOrEmpty(model.MailTemplateId))
            {
                // 登録なのでActiveにする
                model.IsActive = true;
                model.MailTemplateId = Guid.NewGuid().ToString();
                MailTemplateRepository.Register(model);
            }
            else
            {
                MailTemplateRepository.Update(model);
            }
            return model;
        }

        public void Delete(string mailTemplateId)
        {
            // 使用されている場合は削除不可
            if (!ApiMailTemplateRepository.CheckUsedMailTemplate(mailTemplateId))
            {
                throw new InUseException("このメールテンプレートは使用されているため削除できません。");
            }
            var model = MailTemplateRepository.Get(mailTemplateId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            MailTemplateRepository.Delete(mailTemplateId);
        }
    }
}
