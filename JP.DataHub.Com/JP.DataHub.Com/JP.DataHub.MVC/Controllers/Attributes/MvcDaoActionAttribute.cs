using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;

namespace JP.DataHub.MVC.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MvcDaoActionAttribute : HandlerAttribute
    {
        public string ActionName { get; private set; }

        public override ICallHandler CreateHandler(IUnityContainer container) => new MvcActionHandler(ActionName);

        public MvcDaoActionAttribute(string actionName = null)
        {
            ActionName = actionName;
        }

        internal class MvcActionHandler : ICallHandler
        {
            public int Order { get => 1; set { } }

            public string ActionName { get; private set; }

            public MvcActionHandler(string actionName = null)
            {
                ActionName = actionName;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                return result;
            }
        }
    }
}
