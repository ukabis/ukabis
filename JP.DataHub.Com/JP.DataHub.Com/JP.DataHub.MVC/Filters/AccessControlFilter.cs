using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Session;

namespace JP.DataHub.MVC.Filters
{
    public class AccessControlFilter : ActionFilterAttribute
    {
        public const string ApplicationId = "72e8e85e-331c-46a3-977a-a601831873e0";
        public const string AdminFunctionId = "2d59c3f5-0953-44e3-8879-cb079b474a57";
        public const string OperatorFunctionId = "476b879d-80fd-490b-bdd0-60f36910d06c";

        public enum Scope
        {
            All,
            Sensor,
            SmartFoodChain,
            Admin,
            representative
        }

        public enum ScopeSet
        {
            SensorVendor,
            SmartFoodChainVendorMaster,
            Group
        }

        private static Dictionary<ScopeSet, Scope[]> ScopeSetDif = new Dictionary<ScopeSet, Scope[]>()
        {
            { ScopeSet.SensorVendor, new Scope[] { Scope.Sensor, Scope.All, Scope.Admin } },
            { ScopeSet.SmartFoodChainVendorMaster, new Scope[] { Scope.SmartFoodChain, Scope.All, Scope.Admin } },
            { ScopeSet.Group, new Scope[] { Scope.representative, Scope.Admin } }
        };


        private Scope[] _scopes = null;
        private string[] _functionIds = null;


        public AccessControlFilter(params Scope[] scopes)
        {
            _scopes = scopes ?? Array.Empty<Scope>();
        }

        public AccessControlFilter(ScopeSet scopeSet)
        {
            _scopes = ScopeSetDif[scopeSet];
        }

        public AccessControlFilter(params string[] functionIds)
        {
            _functionIds = functionIds ?? Array.Empty<string>();
        }

        public AccessControlFilter(ScopeSet scopeSet, string functionId)
        {
            _scopes = ScopeSetDif[scopeSet];
            _functionIds = string.IsNullOrEmpty(functionId) ? Array.Empty<string>() : new[] { functionId };
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_scopes != null || _functionIds != null)
            {
                // スコープ検証
                var isValidScope = false;
                if (_scopes != null)
                {
                    var groupSessionManager = UnityCore.Resolve<IGroupSessionManager>();
                    var group = groupSessionManager.Get();
                    isValidScope = group?.scope != null && _scopes.Any(x => group.scope.Contains(x.ToString()));
                }

                // 権限検証
                var isValidAuth = false;
                if (_functionIds != null)
                {
                    var authSessionManager = UnityCore.Resolve<IAuthorizationSessionManager>();
                    var auth = authSessionManager.Get();
                    isValidAuth = auth?.FunctionList != null && _functionIds.Any(x => auth.FunctionList.Contains(x));
                }

                // いずれの条件も満たさない場合は403
                if (!isValidScope && !isValidAuth)
                {
                    filterContext.Result = new StatusCodeResult(403);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
