using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Service.Impl;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;

namespace JP.DataHub.ApiWeb.Domain.Service
{
    internal interface IUserTermsService
    {
        IList<UserTermsModel> GetList(string open_id);
        UserTermsModel Get(string open_id, string user_terms_id);
    }
}
