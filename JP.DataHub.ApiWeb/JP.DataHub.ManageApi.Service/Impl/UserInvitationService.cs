using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class UserInvitationService : AbstractService, IUserInvitationService
    {
        private Lazy<IUserInvitationRepository> _lazyUserInvitationRepository = new Lazy<IUserInvitationRepository>(() => UnityCore.Resolve<IUserInvitationRepository>());
        private IUserInvitationRepository _userInvitationRepository { get => _lazyUserInvitationRepository.Value; }

        private Lazy<IVendorRepository> _lazyVendorRepository = new Lazy<IVendorRepository>(() => UnityCore.Resolve<IVendorRepository>());
        private IVendorRepository _vendorRepository { get => _lazyVendorRepository.Value; }

        private Lazy<IRoleRepository> _lazyRoleRepository = new Lazy<IRoleRepository>(() => UnityCore.Resolve<IRoleRepository>());
        private IRoleRepository _roleRepository { get => _lazyRoleRepository.Value; }

        private Lazy<ISendMailRepository> _lazySendMailRepository = new Lazy<ISendMailRepository>(() => UnityCore.Resolve<ISendMailRepository>());
        private ISendMailRepository _sendMailRepository { get => _lazySendMailRepository.Value; }

        public string RegisterUserInvitation(SendUserInvitationModel model)
        {
            // RoleIdの存在チェック
            if (_roleRepository.IsExstis(model.RoleId) == false)
            {
                throw new NotFoundException($"RoleId({model.RoleId}) not found.");
            }

            // VendorIdが存在しているかのチェック
            var vendor = _vendorRepository.Get(model.VendorId);
            if (vendor == null)
            {
                throw new NotFoundException($"VendorId({model.VendorId}) not found.");
            }

            // RegisterAccountが存在しているかのチェック
            if (_vendorRepository.IsExistsStaffByAccount(model.RegistAccount) == false)
            {
                throw new NotFoundException($"RegistAccount({model.RegistAccount}) not found.");
            }

            var codeMailTemplate = AppConfig.GetValue<string>("SendMailTamplateCdDocUserInvitation");
            var userInvitationSiteUrl = AppConfig.GetValue<string>("UserInvitationSiteUrl");
            var signUpSiteUrl = AppConfig.GetValue<string>("SignUpSiteUrl");

            // 登録
            var userInvitation = _userInvitationRepository.Get(_userInvitationRepository.RegisterUserInvitation(model));

            // メール送信
            if (string.IsNullOrEmpty(codeMailTemplate) == false && string.IsNullOrEmpty(userInvitationSiteUrl) == false)
            {
                var siteUrl = string.Format(userInvitationSiteUrl, userInvitation.UserInvitationId);
                var parameters = new Dictionary<string, object>()
                {
                    { "vendorName", vendor.VendorName },
                    { "doc_invitationuser_mail", model.MailAddress },
                    { "expireDate", userInvitation.ExpireDate },
                    { "siteUrl", siteUrl },
                    { "signupUrl", signUpSiteUrl },
                };
                var tmp = _sendMailRepository.SendMailAsync(codeMailTemplate, parameters, new Dictionary<string, string>() { { "UserInvitationId", userInvitation.UserInvitationId } }).Result;
            }

            return userInvitation.UserInvitationId;
        }

        public VendorStaffModel RegistUserInvitationInfoToStaff(string userInvitationId, string openId, string email)
        {
            var userInvitation = _userInvitationRepository.Get(userInvitationId);
            if (DateTime.Parse(userInvitation.ExpireDate) < DateTime.UtcNow)
            {
                throw new ExpirationException("招待期限が経過しました。再度招待を実行してください。");
            }
            else if (userInvitation.IsUsed == true)
            {
                // 既に登録済み
                throw new RegisteredException("招待はすでに実行しています。");
            }

            // 削除済みのスタッフの場合はStaffIdを使いまわす
            var deletedStaff = _vendorRepository.GetStaffByAccount(openId, false);
            string staffId = null;
            if (deletedStaff != null)
            {
                staffId = deletedStaff.StaffId;
            }

            // ユーザーとロールを登録する
            var user = _vendorRepository.AddStaff(userInvitation.VendorId, openId, email, staffId);
            var staffroleId = _vendorRepository.AddStaffRole(user.StaffId, userInvitation.RoleId);

            // 招待済に変更
            _userInvitationRepository.Invited(userInvitation.UserInvitationId);

            return new VendorStaffModel() { IsActive = true, RoleId = userInvitation.RoleId, StaffId = user.StaffId, Account = user.StaffId, StaffRoleId = staffroleId.To<Guid>() };
        }
    }
}
