using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface ISystemRepository
    {
        List<FunctionNode> GetFunctionTree();
        void RegistFunc(FuncInfomation funcInfo);

        void UpdateFunc(FuncInfomation funcInfo);

        void RegistSystemFunc(SystemFuncInfomation systemFuncInfo);

        void UpdateSystemFunc(SystemFuncInfomation systemFuncInfo);

        /// <summary>
        /// SystemFuncから指定されたデータを物理削除します。
        /// </summary>
        /// <param name="systemFuncInfo">SystemFuncデータ</param>
        void DeleteSystemFunc(SystemFuncInfomation systemFuncInfo);

        List<SystemEntity> GetSystemList(VendorId vendorId);
        SystemEntity Get(SystemId systemId);
        SystemEntity Registration(SystemEntity system);
        SystemEntity Update(SystemEntity system);
        void Delete(SystemId systemId);
        List<FunctionNode> GetAuthorizeFuncList(SystemId systemId);
        SystemEntity Restore(SystemId systemId);
        (List<FuncInfomation> FuncList, List<SystemFuncInfomation> SystemFuncList) GetRegisteredFuncData(SystemId systemId);

        SystemAdmin GetSystemAdmin(SystemId systemId);
        SystemAdmin UpdateSystemAdmin(SystemId systemId, SystemAdmin systemAdmin);

        /// <summary>
        /// 指定されたベンダーIDに紐づくClientIdの一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>ClientIdの一覧</returns>
        IEnumerable<ClientId> GetClientIdByVendorId(VendorId vendorId);
    }
}
