using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    /// <summary>
    /// 文字数を指定するValidation属性
    /// StringLengthとの違いは、型は何でもよい（stringでなくても）
    /// その型がstringに変換（ToString）したあとの文字数をチェックする
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class ToStringLengthAttribute : ValidationAttribute
    {
        private int MinLength { get; set; }
        private int MaxLength { get; set; }
        private readonly string DefaultErrorMessage = "{0} must be {1} to {2} characters.";

        public ToStringLengthAttribute(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = value?.ToString();
            if (value == null || val.Length < MinLength || val.Length > MaxLength)
            {
                    return new ValidationResult(string.Format(DefaultErrorMessage, validationContext.DisplayName, MinLength, MaxLength));
            }
            return null;
        }
    }

    public class LengthAttributeEx : ToStringLengthAttribute, IValidationAttributeEx
    {
        public Type ExceptionType { get; }

        public LengthAttributeEx(int minLength, int maxLength, Type exceptionType)
            : base(minLength, maxLength)
        {
            ExceptionType = exceptionType;
        }
    }
}