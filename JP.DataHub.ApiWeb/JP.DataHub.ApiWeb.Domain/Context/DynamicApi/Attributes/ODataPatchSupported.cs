using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class ODataPatchSupportedAttribute : Attribute
    {
        public ODataPatchSupport ODataPatchSupport { get; }

        public ODataPatchSupportedAttribute(ODataPatchSupport oDataPatchSupport)
        {
            ODataPatchSupport = oDataPatchSupport;
        }
    }
}
