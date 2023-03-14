using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JP.DataHub.Service.Core.Repository
{
    public class RepositoryDaoInfo
    {
        public string ClassName { get; set; }
        public string ModelName { get; set; }
        public string ActionName { get; set; }

        public Type ClassType { get; set; }
        public Type ModelType { get; set; }
        public MethodInfo ActionMethod { get; set; }
        public bool IsArray { get; set; }
        public Type ResultType { get; set; }

        public Dictionary<string, string[]> Headers { get; set; }

        public Type DynamicApiClientSelector { get; set; }

        public Dictionary<string,object> Param { get; set; }
    }

    internal class RepositoryDaoInfoInternal : RepositoryDaoInfo
    {
        public bool IsNoConvert { get; set; }
        public bool IsConvertRequestModel { get; set; }
    }
}
