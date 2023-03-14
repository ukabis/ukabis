﻿using Unity;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class CallCountHandler : ICallHandler
    {
        private int callCount;
        private int order = 0;

        [InjectionConstructor]
        public CallCountHandler()
        {
        }

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
            }
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            ++callCount;
            return getNext()(input, getNext);
        }

        public int CallCount
        {
            get { return callCount; }
        }
    }
}
