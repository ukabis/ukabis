using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Controllers.Attributes
{
    public class MvcDaoAttribute : Attribute
    {
        public string RepositoryName { get; private set; }
        public string RepositoryModelName { get; private set; }
        public Type ResultModel { get; private set; }

        public MvcDaoAttribute(string repositoryName, string modelName, Type resultModel = null)
        {
            RepositoryName = repositoryName;
            RepositoryModelName = modelName;
            ResultModel = resultModel;
        }

        public MvcDaoAttribute(Type resource, Type model, Type resultModel = null)
        {
            RepositoryName = resource.Name;
            RepositoryModelName = model.Name;
            ResultModel = resultModel ?? model;
        }
    }
}
