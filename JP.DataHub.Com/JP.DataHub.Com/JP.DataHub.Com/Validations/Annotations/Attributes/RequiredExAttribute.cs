using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    public class RequiredExAttribute : RequiredAttribute, IValidationAttributeEx
    {
        public Type ExceptionType { get; }

        public RequiredExAttribute() { }

        public RequiredExAttribute(Type exceptionType) => ExceptionType = exceptionType;
    }
}
