using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class TestService : AbstractService, ITestService
    {
        private Lazy<ITestRepository> _lazyTestRepository = new Lazy<ITestRepository>(() => UnityCore.Resolve<ITestRepository>());
        private ITestRepository _testRepository { get => _lazyTestRepository.Value; }

        public TestModel Get(string id)
            => _testRepository.Get(id);

        public IEnumerable<TestModel> GetList()
            => _testRepository.GetList();

        public void Register(TestModel model)
            => _testRepository.Register(model);

        public void Delete(string id)
        {
            var model = _testRepository.Get(id);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            _testRepository.Delete(id);
        }
    }
}
