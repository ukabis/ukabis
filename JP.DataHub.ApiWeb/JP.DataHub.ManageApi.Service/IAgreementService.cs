using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    public interface IAgreementService
    {
        List<AgreementModel> GetAgreementList(string? vendorId = null);

        AgreementModel GetAgreement(string agreementId);

        AgreementModel RegistAgreement(AgreementModel agreement);

        AgreementModel UpdateAgreement(AgreementModel agreement);

        void DeleteAgreement(string agreementId);
    }
}
