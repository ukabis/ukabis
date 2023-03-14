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
    public interface IInformationService
    {
        List<InformationModel> GetList(int? getInformationCount, bool isVisible, bool isVisibleAdmin);

        InformationModel Get(string informationId);
        InformationModel Registration(InformationModel information);

        InformationModel Update(InformationModel information);

        void Delete(string informationId);
    }
}
