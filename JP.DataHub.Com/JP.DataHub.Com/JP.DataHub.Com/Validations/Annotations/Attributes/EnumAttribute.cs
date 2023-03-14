using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    /// <summary>
    /// 値がenumのメンバーであること
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class EnumAttribute : ValidationAttribute
    {
        private Type EnumType { get; set; }

        internal readonly string DefaultErrorMessage = "{0} is not a member of {1}.";

        public EnumAttribute(Type enumType)
        {
            EnumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var str = value?.ToString();
            object enumval;
            if (str != null && str.TryParse(EnumType, out enumval) == false)
            {
                return new ValidationResult(string.Format(DefaultErrorMessage, validationContext.DisplayName, EnumType));
            }
            return null;
        }
    }

    public class EnumAttributeEx : EnumAttribute, IValidationAttributeEx
    {
        public Type ExceptionType { get; }

        public EnumAttributeEx(Type enumType, Type exceptionType)
            : base(enumType)
        {
            ExceptionType = exceptionType;
        }
    }
}
