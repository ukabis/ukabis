using JP.DataHub.ApiWeb.Domain.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    internal interface ITermsRepository
    {
        IList<TermsGroupModel> GroupGetList();
        string GroupRegister(TermsGroupModel model);
        void GroupDelete(string terms_group_id);

        TermsModel TermsGet(string terms_id);
        IList<TermsModel> TermsGetList();
        IList<TermsModel> TermsGetListByTermGroupCode(string terms_group_code);
        string TermsRegister(TermsModel model);
        void TermsDelete(string terms_id);
        void Agreement(string open_id, string terms_id);
        void Revoke(string open_id, string terms_id);
    }
}
