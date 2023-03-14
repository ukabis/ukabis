using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredGUIDAttribute : RequiredAttribute
    {
        public string ErrorMessageWhenNull { get; set; }
        public string ErrorMessageWhenNotGuid { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(value as string))
                return new ValidationResult(ErrorMessageWhenNull, new List<string>() { validationContext.DisplayName });

            if (!Guid.TryParse(value as string, out var _))
                return new ValidationResult(ErrorMessageWhenNotGuid, new List<string>() { validationContext.DisplayName });

            return ValidationResult.Success;
        }
    }
}
