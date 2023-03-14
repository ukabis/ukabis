using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Log;

namespace JP.DataHub.Aop
{
    // .NET6
    public interface IApiFilter
    {
        IApiHelper Apihelper { get; set; }
        IAopCacheHelper CacheHelper { get; set; }
        ITermsHelper TermsHelper { get; set; }
        ISimpleLogWriter Logger { get; set; }
        string Param { get; set; }

        HttpResponseMessage BeforeAction(IApiFilterActionParam param);

        HttpResponseMessage AfterAction(HttpResponseMessage msg);
    }
}
