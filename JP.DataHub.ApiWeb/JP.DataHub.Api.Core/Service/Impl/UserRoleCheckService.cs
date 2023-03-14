using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Repository;

namespace JP.DataHub.Api.Core.Service.Impl
{
    internal class UserRoleCheckService : IUserRoleCheckService
    {
        private Lazy<IUserRoleCheckRepository> _lazyLoggingRepository => new Lazy<IUserRoleCheckRepository>(() => UnityCore.Resolve<IUserRoleCheckRepository>());
        private IUserRoleCheckRepository _loggingRepository { get => _lazyLoggingRepository.Value; }
        private Lazy<IApiCoreSystemRepository> _lazySystemRepository => new Lazy<IApiCoreSystemRepository>(() => UnityCore.Resolve<IApiCoreSystemRepository>());
        private IApiCoreSystemRepository _systemRepository { get => _lazySystemRepository.Value; }
        

        public bool CheckModel(string vendorId, string systemId, bool isVendorAccess, object model)
        {
            if (isVendorAccess == true)
            {
                vendorId = vendorId?.ToLower();
                systemId = systemId?.ToLower();
                bool isValidVendorId = false;
                bool isValidSystemid = false;
                var vendorIds = new List<string>();
                var systemds = new List<string>();
                return CheckObjectModel(vendorId, systemId, model, ref isValidVendorId, ref isValidSystemid, vendorIds, systemds);
            }
            return true;
        }

        public bool Check(string openId, string vendorId, string systemId, string functionName, UserRoleAccessType access, bool isVendorAccess, IDictionary<string, object?> arguments)
        {
            // functionName,accessのチェック
            var list = _loggingRepository.GetAllAccessRights(openId);
            if (list?.Where(x => x.FunctionName == functionName && ((access == UserRoleAccessType.Read && x.IsRead == true) || (access == UserRoleAccessType.Write && x.IsWrite == true))).FirstOrDefault() == null)
            {
                return false;
            }

            // isVendorAccessのチェック
            if (isVendorAccess == true)
            {
                vendorId = vendorId?.ToLower();
                systemId = systemId?.ToLower();
                bool isValidVendorId = false;
                bool isValidSystemId = false;
                var vendorIds = new List<string>();
                var systemds = new List<string>();
                foreach (var argument in arguments)
                {
                    // パラメータの場合
                    if (isValidVendorId == false && argument.Key.ToLower() == "vendorid" && argument.Value.ToString().ToLower() == vendorId)
                    {
                        isValidVendorId = true;
                    }
                    if (isValidVendorId == false && argument.Key.ToLower() == "systemid" && argument.Value.ToString().ToLower() == systemId)
                    {
                        isValidSystemId = true;
                    }
                    // オブジェクトの場合
                    CheckObjectModel(vendorId, systemId, argument.Value, ref isValidVendorId, ref isValidSystemId, vendorIds, systemds);
                }
                // ベンダーIDが見つかった場合は、それは正しい
                if (isValidVendorId == true)
                {
                    return true;
                }
                // システムIDが見つかった場合は、そのシステムIDからベンダーIDを取得する。取得したベンダーIDが合致していれば問題ない
                else if (isValidSystemId == true)
                {
                    if (_systemRepository.SystemIdToVendorId(systemId) == vendorId)
                    {
                        return true;
                    }
                }
                // システムIDを拾っていたら、そのベンダーIDが正しいか？
                // ※すべて正しくないとダメ
                foreach (var x in systemds)
                {
                    if (_systemRepository.SystemIdToVendorId(x) != vendorId)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private bool CheckObjectModel(string vendorId, string systemId, object model, ref bool isValidVendorId, ref bool isValidSystemid, IList<string> vendorIds, IList<string> systemIds)
        {
            if (model != null)
            {
                var props = model.GetType().GetProperties().ToList();
                if (isValidVendorId == false)
                {
                    var hitVendorId = props.Where(x => x.Name.ToLower() == "vendorid").FirstOrDefault();
                    if (hitVendorId != null)
                    {
                        var val = hitVendorId.GetValue(model)?.ToString().ToLower();
                        vendorIds.Add(val);
                        if (val == vendorId)
                        {
                            isValidVendorId = true;
                        }
                    }
                }
                if (isValidSystemid == false)
                {
                    var hitSystemId = props.Where(x => x.Name.ToLower() == "systemid").FirstOrDefault();
                    if (hitSystemId != null)
                    {
                        var val = hitSystemId.GetValue(model)?.ToString().ToLower();
                        systemIds.Add(val);
                        if (val == systemId)
                        {
                            isValidSystemid = true;
                        }
                    }
                }
            }
            return isValidVendorId && isValidSystemid;
        }
    }
}
