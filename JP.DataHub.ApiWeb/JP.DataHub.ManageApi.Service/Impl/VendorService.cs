using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using AutoMapper;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class VendorService : AbstractService, IVendorService
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VendorLinkModel, RegisterVendorLinkModel>();
                cfg.CreateMap<OpenIdCaModel, RegisterOpenIdCaModel>();
                cfg.CreateMap<StaffModel, StaffRoleModel>();
                cfg.CreateMap<OpenIdCaModel, VendorOpenIdCaModel>();
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }


        private Lazy<IVendorRepository> _lazyVendorRepository = new Lazy<IVendorRepository>(() => UnityCore.Resolve<IVendorRepository>());
        private IVendorRepository _vendorRepository { get => _lazyVendorRepository.Value; }

        private Lazy<IAttachFileRepository> _lazyAttachFileRepository = new Lazy<IAttachFileRepository>(() => UnityCore.Resolve<IAttachFileRepository>());
        private IAttachFileRepository _attachFileRepository { get => _lazyAttachFileRepository.Value; }

        private static Lazy<string> s_lazyAccountValidation = new Lazy<string>(() => AppConfig.GetValue<string>("AccountValidation"));
        private static string s_accountValidation { get => s_lazyAccountValidation.Value; }

        private Lazy<IRepositoryGroupRepository> _lazyIRepositoryGroupRepository = new Lazy<IRepositoryGroupRepository>(() => UnityCore.Resolve<IRepositoryGroupRepository>());
        private IRepositoryGroupRepository _repositoryGroupRepository { get => _lazyIRepositoryGroupRepository.Value; }

        public IList<VendorModel> GetEnableVendorSystemNameList(string vendorId = null)
            => _vendorRepository.GetVendorSystemNameList(enableOnly: true, vendorId: vendorId);

        public IList<VendorModel> GetVendorList()
            => _vendorRepository.GetVendorSystemNameList(false, false, true);

        public VendorModel Register(VendorModel model)
        {
            var ret = _vendorRepository.Register(model);
            _repositoryGroupRepository.RegistDefaultVendorRepositoryGroup(model.VendorId);
            return ret;
        }

        public VendorModel Update(VendorModel model)
            => _vendorRepository.Update(model);

        public void Delete(string vendorId)
        {
            IList<StaffRoleModel> staffList = null;
            try
            {
                staffList = _vendorRepository.GetStaffList(vendorId);
            }
            catch (NotFoundException)
            {
                //スルー
            }

            _vendorRepository.Delete(vendorId);

            if (staffList?.Any() == true)
            {
                staffList.ForEach(staff =>
                {
                    _vendorRepository.DeleteStaff(staff.StaffId);
                    _vendorRepository.DeleteStaffRole(staff.StaffRoleId);
                });
            }
        }

        public VendorModel Get(string vendorId)
        {
            var vendor = _vendorRepository.Get(vendorId);
            if (vendor != null)
            {
                vendor.AttachFileStorage = _attachFileRepository.GetAttachFileStorageList();
                try
                {
                    vendor.AttachFileStorageId = _attachFileRepository.GetAttachFileStorageIdByVendorId(vendorId);
                }
                catch (NotFoundException)
                {
                    //スルー
                }

                try
                {
                    vendor.StaffList = _vendorRepository.GetStaffList(vendorId);
                }
                catch (NotFoundException)
                {
                    //スルー
                }
                vendor.OpenIdCaList = _vendorRepository.GetVendorOpenIdCaListByVendorId(vendorId);
                try
                {
                    vendor.VendorLinkList = _vendorRepository.GetVendorLinkList(vendorId);
                }
                catch (NotFoundException)
                {
                    //スルー
                }
            }
            return vendor;
        }

        public VendorModel GetByOpenId(string openId)
            => _vendorRepository.GetByOpenId(openId);

        public StaffModel AddStaff(StaffModel model)
        {
            if (_vendorRepository.IsExists(model.VendorId) == false)
            {
                throw new NotFoundException("not found vendorId.");
            }
            ValidateAccount(model.Account);

            // 削除済みのスタッフの場合はStaffIdを使いまわす
            var deletedStaff = _vendorRepository.GetStaffByAccount(model.Account, false);
            if (deletedStaff != null) 
            {
                model.StaffId = deletedStaff.StaffId;
            }

            var stf = _vendorRepository.AddStaff(model.VendorId, model.Account, model.EmailAddress, model.StaffId);
            _vendorRepository.AddStaffRole(stf.StaffId, model.RoleId, model.StaffRoleId);
            model.StaffId = stf.StaffId;
            return model;
        }

        public StaffModel UpdateStaff(StaffModel model)
        {
            ValidateAccount(model.Account);
            return _vendorRepository.UpdateStaff(model);
        }

        public string GetVendorIdByStaffAccount(string account)
            => _vendorRepository.GetVendorIdByStaffAccount(account);

        public StaffModel GetStaff(string staff_id)
            => _vendorRepository.GetStaff(staff_id);

        public void DeleteStaff(string staff_id)
        {
            var model = _vendorRepository.GetStaff(staff_id);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            _vendorRepository.DeleteStaff(staff_id);
            _vendorRepository.DeleteStaffRole(model.StaffRoleId);
        }

        public IList<StaffModel> GetStaffListByVendorId(string vendor_id)
        {
            var result = _vendorRepository.GetStaffListByVendorId(vendor_id);
            if (result?.Count == 0)
            {
                throw new NotFoundException();
            }
            return result;
        }

        private void ValidateAccount(string account)
        {
            // AccountValidationが指定されていない場合、検証OK
            if (string.IsNullOrEmpty(s_accountValidation))
            {
                return;
            }

            bool isType = Regex.IsMatch(s_accountValidation, "^Type\\(.+?\\)$");
            bool isRegex = Regex.IsMatch(s_accountValidation, "^Regex\\(.+?\\)$");

            // Accountが指定された型に変換できるか検証する
            if (isType)
            {
                string typeName = Regex.Replace(s_accountValidation, "^Type\\(|\\)$", "");
                // AccountValidationに指定されたTypeを取得
                Type type = Type.GetType($"System.{typeName}", true);
                if (type == null)
                {
                    throw new InvalidAccountFormat("AccountValidationに存在しない型が指定されています。", new Exception(typeName));
                }
                try
                {
                    // Accountを指定されたTypeでキャストする
                    TypeDescriptor.GetConverter(type).ConvertFromInvariantString(account);
                }
                catch (Exception exception)
                {
                    if (exception is FormatException || exception.InnerException is FormatException)
                    {
                        throw new InvalidAccountFormat();
                    }
                }
            }
            // Accountが正規表現にマッチするか検証する
            else if (isRegex)
            {
                string regexPattern = Regex.Replace(s_accountValidation, "^Regex\\(|\\)$", "");
                if (!Regex.IsMatch(account, regexPattern))
                {
                    throw new InvalidAccountFormat();
                }
            }
        }

        public IList<VendorLinkModel> RegisterVendorLink(IList<RegisterVendorLinkModel> model)
            => _vendorRepository.RegisterVendorLink(model);

        public void DeleteVendorLink(string vendorLinkId)
        {
            var model = _vendorRepository.GetVendorLink(vendorLinkId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            _vendorRepository.DeleteVendorLink(vendorLinkId);
        }

        public IList<VendorLinkModel> GetVendorLinkList(string vendorId)
            => _vendorRepository.GetVendorLinkList(vendorId);

        public VendorLinkModel GetVendorLink(string vendorLinkId)
            => _vendorRepository.GetVendorLink(vendorLinkId);

        public IList<VendorOpenIdCaModel> GetVendorOpenIdCaList(string vendorId)
        {
            var result = s_mapper.Map<IList<VendorOpenIdCaModel>>(_vendorRepository.GetVendorOpenIdCaListByVendorId(vendorId));
            result?.ForEach(x => x.VendorId = vendorId.To<Guid>());
            return result;
        }

        public IList<RegisterOpenIdCaModel> RegisterVendorOpenIdCaList(IList<RegisterOpenIdCaModel> model)
        {
            var error = ValidateRegisterOpenIdCa(model);
            if (string.IsNullOrEmpty(error) == false)
            {
                throw new Rfc7807Exception(new RFC7807ProblemDetail() { Status = (int)HttpStatusCode.BadRequest, Title = error });
            }
            return _vendorRepository.RegisterVendorOpenIdCaList(model);
        }

        public void DeleteVendorOpenIdCa(string vendorId, string vendorOpenidCaId)
        {
            if (PerRequestDataContainer.VendorCheck(new { Vendorid = vendorId }) == false)
            {
                throw new AccessDeniedException();
            }
            _vendorRepository.DeleteVendorOpenIdCa(vendorId, vendorOpenidCaId);
        }

        /// <summary>
        /// ベンダーに登録するOpenId認証局のValidation
        /// </summary>
        /// <param name="regModelList">登録するモデル</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>エラーがある場合はステータスを返し、無い場合はnullを返す</returns>
        private string ValidateRegisterOpenIdCa(IList<RegisterOpenIdCaModel> model)
        {
            // 次のチェックを行う（ここのプロパティはmodel引数の中身を言っている）
            // 1. vendorId = nullのものは共通のものなのでここでは変更できない
            // 2. vendorIdが指定されている場合は、既存のものの更新として処理する
            // 3. vendorIdが指定されていて、既存に存在しないものは新規登録として処理する
            // ということは、結論として、1のみをチェックする
            // 理由はVendorIdが指定されているってことは、Vendor専用のものであるから、新規・更新は自由に行えることだから、VendorId指定していない（共通のもの）の更新処理
            // のみチェックをするべきとなる

            if (model?.Where(x => x.VendorId == null).Any() == true)
            {
                return $"共通のOpenIdCAは更新できません。VendorIdを指定したOpenIdのみ登録・更新が可能になります";
            }

            if (model.Select(x => x.VendorId).Distinct().Count() > 1)
            {
                return $"登録または更新するOpenIdCAは同一のベンダーのみになります";
            }

            return null;
        }
    }
}
