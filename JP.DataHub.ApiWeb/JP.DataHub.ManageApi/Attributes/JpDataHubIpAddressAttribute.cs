using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JP.DataHub.ManageApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JpDataHubIpAddressAttribute : ValidationAttribute
    {
        const string FormatErrorIpAddressMessage = "IPアドレス形式ではありません。";
        const string IpAddressPattern = @"^(([1-9]?[0-9]|1[[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([1-9]?[0-9]|1[[0-9]{2}|2[0-4][0-9]|25[0-5])\/([1-9]|1[0-9]|2[0-9]|3[0-2])$";

        public JpDataHubIpAddressAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var ipAddressString = Convert.ToString(value);
            if (string.IsNullOrEmpty(ipAddressString))
            {
                return ValidationResult.Success;
            }

            var rg = new Regex(IpAddressPattern, RegexOptions.IgnoreCase);
            if (rg.IsMatch(ipAddressString))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage());
            }

        }

        public override string FormatErrorMessage(string exceptionMessage = null)
        {
            var message = string.IsNullOrEmpty(ErrorMessage) ? FormatErrorIpAddressMessage : ErrorMessage;
            return $"{message} {exceptionMessage}";
        }
    }
}
