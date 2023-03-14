using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;

namespace JP.DataHub.Com.Net.Http
{
    public interface IDynamicApiClientSelector
    {
        string Name { get; }
        Dictionary<string,object> Param { get; set; }
    }

    public interface ICommon : IDynamicApiClientSelector { }

    public interface ILoginUser : IDynamicApiClientSelector { }

    public class CommonDynamicApiClientSelector : ICommon
    {
        public string Name { get => CommonDynamicApiConst.CommonKey; }
        public Dictionary<string, object> Param { get; set; } = new Dictionary<string, object>();
    }

    public class LoginUserDynamicApiClientSelector : ILoginUser
    {
        public string Name { get => CommonDynamicApiConst.LoginUserKey; }
        public Dictionary<string, object> Param { get; set; } = new Dictionary<string, object>();
    }
}
