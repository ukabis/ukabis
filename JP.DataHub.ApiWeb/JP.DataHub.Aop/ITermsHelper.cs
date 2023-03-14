using JP.DataHub.Aop.Models;

namespace JP.DataHub.Aop
{
    public interface ITermsHelper
    {
        List<TermsModel> TermsGetList();
        void Agreement(string openId, string terms_id);
    }
}
