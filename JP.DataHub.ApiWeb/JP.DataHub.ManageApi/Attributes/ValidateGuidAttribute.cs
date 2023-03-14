using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace JP.DataHub.ManageApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false)]

    public class ValidateGuidAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            return Guid.TryParse(value.ToString(),out _) ? ValidationResult.Success : new ValidationResult("Invalid GUID String.");
        }
    }
}
