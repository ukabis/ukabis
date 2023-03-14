using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Misc
{
    public static class CallStack
    {
        public static StackFrame FindStackFrame<T>(int start = 1)
        {
            for (int i = start; i < 10; i++)
            {
                var caller = new StackFrame(i);
                var method = caller.GetMethod();
                if (method.IsPublic == false)
                {
                    continue;
                }
                var type = method.DeclaringType;
                var interfaces = type.GetPropertyValue("ImplementedInterfaces") as Type[];
                if (interfaces.ToList().Where(x => x == typeof(T)).FirstOrDefault() != null)
                {
                    return caller;
                }
            }
            return null;
        }
    }
}
