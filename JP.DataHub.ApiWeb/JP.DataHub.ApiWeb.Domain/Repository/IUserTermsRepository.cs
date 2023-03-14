using JP.DataHub.ApiWeb.Domain.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    internal interface IUserTermsRepository
    {
        IList<UserTermsModel> GetList(string open_id);
        UserTermsModel Get(string open_id, string user_terms_id);
    }
}
