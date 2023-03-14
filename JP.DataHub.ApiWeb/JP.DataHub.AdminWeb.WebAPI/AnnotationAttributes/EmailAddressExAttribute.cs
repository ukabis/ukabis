using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class EmailAddressExAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var emailAddressAttribute = new EmailAddressAttribute();
            if (value != null && !string.IsNullOrEmpty(value as string))
            {
                var array = value.ToString().Split(',');
                foreach (var i in array)
                {
                    if (!emailAddressAttribute.IsValid(i))
                    {
                        return new ValidationResult("メールアドレスの形式で入力してください。", new List<string>() { validationContext.DisplayName });
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
