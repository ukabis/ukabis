using System.Globalization;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.Models;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AccessTokenExpiretionAttribute : ValidationAttribute
    {
        private class ViewModelErrorMessage
        {
            internal const string Required = "有効期限は必須項目です";
            internal const string AccessTokenExpirationRangeOutside = "有効期限は00:01～24:00の範囲で入力してください";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var vm = (ClientModel)validationContext.ObjectInstance;

            var accessToken = value?.ToString().Trim();

            if (vm.IsActive)
            {
                // 必須
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new ValidationResult(ViewModelErrorMessage.Required, new List<string>() { validationContext.DisplayName });
                }

                // 00:00はNG かつ 24：00 または 00:01～23:59の範囲
                TimeSpan ts;
                if (!TimeSpan.TryParseExact(accessToken, @"hh\:mm", CultureInfo.CurrentCulture, out ts))
                {
                    // Parseに失敗するが、24:00はOK
                    if (accessToken != "24:00")
                    {
                        return new ValidationResult(ViewModelErrorMessage.AccessTokenExpirationRangeOutside, new List<string>() { validationContext.DisplayName });
                    }
                }
                else if ((ts.Hours == 0 && ts.Minutes == 0))
                {
                    // 00:00はNG
                    return new ValidationResult(ViewModelErrorMessage.AccessTokenExpirationRangeOutside, new List<string>() { validationContext.DisplayName });
                }
            }

            return ValidationResult.Success;
        }
   }
}
