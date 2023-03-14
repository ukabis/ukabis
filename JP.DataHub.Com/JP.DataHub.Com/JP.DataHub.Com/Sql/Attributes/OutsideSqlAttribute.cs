using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace JP.DataHub.Com.SQL.Attributes
{
    //[DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class OutsideSqlAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new OutsideSqlHandler();
        }

        //[DebuggerStepThrough]
        public class OutsideSqlHandler : ICallHandler
        {
            //[DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                if (input.Target is AbstractOutsideSQL sql)
                {
                    return input.CreateMethodReturn(sql.GetSql(input.Target.GetType(), input.MethodBase.Name));
                }
                return input.CreateMethodReturn(null);
            }

            public int Order { get => 1; set { } }
        }
    }
}
