using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MaxLengthExAttribute : MaxLengthAttribute
    {
        public int MaxLength { get; set; } = -1;

        public string ErrorMessageFormat { get; set; } = "{0}文字以内で入力して下さい。";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (MaxLength != -1 && value != null)
            {
                if (value is string)
                {
                    var str = value as string;
                    if (str.Length > MaxLength)
                    {
                        return new ValidationResult(FormatErrorMessage(), new List<string>() { validationContext.DisplayName });
                    }
                }
            }
            return ValidationResult.Success;
        }

        private string FormatErrorMessage()
        {
            return string.Format(ErrorMessageFormat, MaxLength);
        }

    }
}
