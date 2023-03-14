﻿using System;
using System.Collections.Generic;
using Unity;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class CallCountInterceptionBehavior : IInterceptionBehavior
    {
        private int callCount;

        [InjectionConstructor]
        public CallCountInterceptionBehavior()
        {
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            ++callCount;
            return getNext()(input, getNext);
        }

        public int CallCount
        {
            get { return callCount; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
