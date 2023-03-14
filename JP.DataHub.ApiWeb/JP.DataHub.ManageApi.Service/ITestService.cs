using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    internal interface ITestService
    {
        TestModel Get(string id);
        IEnumerable<TestModel> GetList();
        void Register(TestModel model);
        void Delete(string id);
    }
}
