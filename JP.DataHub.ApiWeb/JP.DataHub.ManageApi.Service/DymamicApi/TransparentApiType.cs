using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    internal class TransparentApiType
    {
        public TransparentApiType(HttpMethodType.MethodType methodType, string methodName, string description)
        {
            MethodType = methodType;
            MethodName = methodName;
            Description = description;
        }

        public HttpMethodType.MethodType MethodType { get; }
        public string MethodName { get; }
        public string Description { get; }
    }
}
