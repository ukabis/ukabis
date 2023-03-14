using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    /// <summary>
    /// 型指定のValidation属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class TypeAttribute : ValidationAttribute
    {
        private Type PropertyType { get; set; }

        private readonly string DefaultErrorMessage = "{0} is invalid type.The type must be {1}.";

        public TypeAttribute(Type type)
        {
            PropertyType = type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value?.IsConvert(PropertyType) != true)
            {
                return new ValidationResult(string.Format(DefaultErrorMessage, validationContext.DisplayName, PropertyType));
            }
            return null;
        }
    }

    public class TypeExAttributeEx : TypeAttribute, IValidationAttributeEx
    {
        public Type ExceptionType { get; }

        public TypeExAttributeEx(Type type, Type exceptionType)
            : base(type)
        {
            ExceptionType = exceptionType;
        }
    }
}
