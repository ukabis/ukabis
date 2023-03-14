using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Log;

namespace JP.DataHub.Aop
{
    public class AbstractApiFilter : IApiFilter
    {
        public IApiHelper Apihelper { get; set; }
        public IAopCacheHelper CacheHelper { get; set; }
        public ITermsHelper TermsHelper { get; set; }
        public ISimpleLogWriter Logger { get; set; }
        public string Param { get; set; }

        public virtual HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            return null;
        }

        public virtual HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            return null;
        }
    }
}
