using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface ITestRepository
    {
        TestModel Get(string id);
        IList<TestModel> GetList();
        TestModel Register(TestModel model);
        void Update(TestModel model);
        void Delete(string id);
    }
}
