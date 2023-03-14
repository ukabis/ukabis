using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    public class HttpMethodType
    {
        public enum MethodType
        {
            Get,
            Post,
            Put,
            Delete,
            Head,
            Method,
            Options,
            Trace,
            Patch,
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMethodType" /> class.
        /// </summary>
        public HttpMethodType(string methodType)
        {
            if (Enum.TryParse(methodType, true, out MethodType enumMethodType))
            {
                Value = enumMethodType;
            }
        }


        public MethodType Value { get; }
    }
}
