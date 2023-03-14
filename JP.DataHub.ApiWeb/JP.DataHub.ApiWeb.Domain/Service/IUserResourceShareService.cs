using JP.DataHub.ApiWeb.Domain.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Service
{
    internal interface IUserResourceShareService
    {
        IList<UserResourceShareModel> GetList(string open_id);
        string Register(UserResourceShareModel model);
        void Delete(string user_resource_group_id);
    }
}
