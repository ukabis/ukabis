using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IAgreementRepository
    {
        List<AgreementModel> GetAgreementList(string? vendorId = null);

        AgreementModel GetAgreement(string agreementId);

        AgreementModel RegistAgreement(AgreementModel agreement);

        AgreementModel UpdateAgreement(AgreementModel agreement);

        void DeleteAgreement(string agreementId);
    }
}
