using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    public class RegularExpressionExAttribute : RegularExpressionAttribute, IValidationAttributeEx
    {
        public Type ExceptionType { get; }

        public RegularExpressionExAttribute(string pattern) : base(pattern) { }

        public RegularExpressionExAttribute(string pattern, Type exceptionType) : base(pattern) => ExceptionType = exceptionType;
    }
}
