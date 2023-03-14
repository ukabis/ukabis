using JP.DataHub.AdminWeb.Service.Interface;
using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Resources;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Service.Core.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Service.Impl
{
    public class AdminRepositoryGroupService : CommonCrudService, IAdminRepositoryGroupService
    {
        #region RepositoryGroup
        public WebApiResponseResult<RepositoryGroupModel> GetRepositoryGroup(string repositoryGroupId)
            => BaseRepository.Get<RepositoryGroupModel>(GetDaoS<ILoginUser, IRepositoryGroupResource>(), repositoryGroupId);
        public Task<WebApiResponseResult<RepositoryGroupModel>> GetRepositoryGroupAsync(string repositoryGroupId)
            => Task.Run(() => GetRepositoryGroup(repositoryGroupId));

        public WebApiResponseResult<List<RepositoryGroupModel>> GetRepositoryGroupList(string queryString = null)
            => BaseRepository.GetList<RepositoryGroupModel>(GetDaoS<ILoginUser, IRepositoryGroupResource>(), queryString);
        public Task<WebApiResponseResult<List<RepositoryGroupModel>>> GetRepositoryGroupListAsync(string queryString = null)
            => Task.Run(() => GetRepositoryGroupList(queryString));

        public WebApiResponseResult<List<RepositoryTypeModel>> GetRepositoryGroupTypeList(string queryString = null)
            => BaseRepository.GetList<RepositoryTypeModel>(GetDaoS<ILoginUser, IRepositoryGroupResource, RepositoryTypeModel>(), queryString);

        public Task<WebApiResponseResult<List<RepositoryTypeModel>>> GetRepositoryGroupTypeListAsync(string queryString = null)
            => Task.Run(() => GetRepositoryGroupTypeList(queryString));

        public WebApiResponseResult DeleteRepositoryGroup(string key, string queryString = null)
            => BaseRepository.Delete(GetDaoS<ILoginUser, IRepositoryGroupResource>(), null, $"repositoryGroupId={key}");

        public WebApiResponseResult<RepositoryGroupModel> RegisterRepositoryGroup(object data, string queryString = null)
            => BaseRepository.Access<RepositoryGroupModel>(GetDaoS<ILoginUser, IRepositoryGroupResource>(), data, queryString);
        #endregion

        #region VendorRepositoryGroup
        public WebApiResponseResult<List<VendorRepositoryGroupListModel>> GetVendorRepositoryGroupList(string queryString = null)
            => BaseRepository.GetList<VendorRepositoryGroupListModel>(GetDaoS<ILoginUser, IVendorRepositoryGroupResource>(), queryString);

        public Task<WebApiResponseResult<List<VendorRepositoryGroupListModel>>> GetVendorRepositoryGroupListAsync(string queryString = null)
            => Task.Run(() => GetVendorRepositoryGroupList(queryString));

        public WebApiResponseResult<List<ActivateVendorRepositoryGroupModel>> ActivateVendorRepositoryGroupList(object data, string queryString = null)
            => BaseRepository.AccessList<ActivateVendorRepositoryGroupModel>(GetDaoS<ILoginUser, IActivateVendorRepositoryGroupResource>(), data, queryString);

        public Task<WebApiResponseResult<List<ActivateVendorRepositoryGroupModel>>> ActivateVendorRepositoryGroupListAsync(object data, string queryString = null)
            => Task.Run(() => ActivateVendorRepositoryGroupList(data, queryString));
        #endregion
    }
}
