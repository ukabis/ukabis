using JP.DataHub.ApiWeb.Domain.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Service
{
    internal interface IUserGroupService
    {
        IList<UserGroupModel> GetList(string open_id);
        string Register(UserGroupModel model);
        void Delete(string user_group_id);
    }
}
