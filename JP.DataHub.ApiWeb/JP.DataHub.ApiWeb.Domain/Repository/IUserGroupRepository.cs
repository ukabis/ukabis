using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    internal interface IUserGroupRepository
    {
        IList<UserGroupModel> GetList(string open_id);
        string Register(UserGroupModel model);
        void Delete(string user_group_id);
    }
}
