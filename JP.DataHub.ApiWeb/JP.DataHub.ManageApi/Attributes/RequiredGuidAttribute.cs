using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace JP.DataHub.ManageApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    public class RequiredGuidAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("必須項目です。");
            }
            if (!Guid.TryParse(value.ToString(), out _))
            {
                return new ValidationResult("Invalid GUID String.");
            }
            return ValidationResult.Success;
        }
    }
}
