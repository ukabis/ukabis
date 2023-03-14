using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredIpAddressAttribute : RequiredAttribute
    {
        const string IpAddressPattern = @"^(([1-9]?[0-9]|1[[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([1-9]?[0-9]|1[[0-9]{2}|2[0-4][0-9]|25[0-5])\/([1-9]|1[0-9]|2[0-9]|3[0-2])$";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var data = value as string;
            if (string.IsNullOrEmpty(data))
            {
                return new ValidationResult("IPアドレスは入力必須です。", new List<string>() { validationContext.DisplayName });
            }
            else
            {
                var rg = new Regex(IpAddressPattern, RegexOptions.IgnoreCase);
                if (!rg.IsMatch(data))
                {
                    return new ValidationResult("IPアドレスは 0.0.0.0/0 の形式で入力して下さい。", new List<string>() { validationContext.DisplayName });
                }
            }
            return ValidationResult.Success;
        }
    }
}
