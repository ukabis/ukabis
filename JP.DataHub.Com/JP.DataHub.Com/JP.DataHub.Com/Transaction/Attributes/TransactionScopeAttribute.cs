using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Reflection;
using Unity;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.PolicyInjection.Pipeline;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Configuration;
using Unity.Interception.Utilities;

namespace JP.DataHub.Com.Transaction.Attributes
{
    //[DebuggerStepThrough]
    public class TransactionScopeWrapper : IDisposable
    {
        private TransactionScope scope = null;

        public TransactionScopeWrapper(bool isEnable, TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
        {
            if (isEnable == true)
            {
                scope = new TransactionScope(scopeOption, scopeTimeout);
            }
        }

        public void Dispose()
        {
            if (scope != null)
            {
                scope.Dispose();
            }
        }

        public void Complete()
        {
            if (scope != null)
            {
                scope.Complete();
            }
        }
    }

    //[DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class TransactionScopeAttribute : HandlerAttribute
    {
        /// <summary>Transaction Scope Option</summary>
        public TransactionScopeOption Option { get; set; } = TransactionScopeOption.Required;

        /// <summary>Transaction Timeout</summary>
        public int Timeout { get; set; }

        public override ICallHandler CreateHandler(IUnityContainer container) => new TransactionScopeHandler();

        public class TransactionScopeHandler : ICallHandler
        {
            [DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var classAttr = input.MethodBase.DeclaringType.GetCustomAttribute<TransactionScopeAttribute>();
                var methodAttr = input.MethodBase.GetCustomAttribute<TransactionScopeAttribute>();

                var option = methodAttr?.Option ?? classAttr?.Option ?? TransactionScopeOption.Required;
                var timeout = methodAttr?.Timeout ?? classAttr?.Timeout ?? 0;
                var connectionStrings = UnityCore.Resolve<IConnectionStrings>();

                IMethodReturn result = null;
                try
                {
                    if (classAttr != null || methodAttr != null)
                    {
                        using (var tran = new TransactionScopeWrapper(connectionStrings.IsTransactionScope, option, TimeSpan.FromSeconds(timeout)))
                        {
                            result = getNext()(input, getNext);

                            var tranman = UnityCore.Resolve<IJPDataHubTransactionManager>();
                            if (connectionStrings.IsTransactionScope == false)
                            {
                                if (result.Exception != null)
                                {
                                    tranman.ForEach(x => x.Rollback());
                                }
                            }
                            tranman.ForEach(x => x.Close());

                            if (result.Exception == null)
                            {
                                tran.Complete();
                            }
                        }
                    }
                    else
                    {
                        result = getNext()(input, getNext);
                    }
                }
                catch (Exception)
                {
                    //new JPDataHubLogger(input.MethodBase.DeclaringType.FullName).Error(e);
                    throw;
                }
                return result;
            }

            public int Order { get => 1; set { } }
        }
    }
}
