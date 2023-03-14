using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    /// <summary>
    /// Base64エンコードされた文字列か検証します。
    /// </summary>
    public class Base64StringAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            string str = value.ToString().Trim();
            return (str.Length % 4 == 0) && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None) ?
                ValidationResult.Success : new ValidationResult("Invalid Base64 String.");
        }
    }
}