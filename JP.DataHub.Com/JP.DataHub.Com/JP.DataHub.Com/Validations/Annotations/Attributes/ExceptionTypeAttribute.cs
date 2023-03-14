using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class ExceptionTypeAttribute : Attribute
    {
        public Type ExceptionType { get; }

        public ExceptionTypeAttribute(Type exceptionType)
        {
            ExceptionType = exceptionType;
        }
    }
}
