using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Api.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class ValidateAccessTokenExpirationTimeSpanAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(ErrorMessageConst.Required);
            }

            var accessToken = value?.ToString().Trim().FormatWith(@"hh\:mm").PadLeft(5, '0');

            // 00:00はNG かつ 24：00 または 00:01～23:59の範囲
            TimeSpan ts;
            if (!TimeSpan.TryParseExact(accessToken, @"hh\:mm", CultureInfo.CurrentCulture, out ts))
            {
                // Parseに失敗するが、24:00はOK
                if (accessToken != "24:00")
                {
                    return new ValidationResult(ErrorMessageConst.AccessTokenExpirationRangeOutside);
                }
            }
            else if ((ts.Hours == 0 && ts.Minutes == 0))
            {
                // 00:00はNG
                return new ValidationResult(ErrorMessageConst.AccessTokenExpirationRangeOutside);
            }

            return ValidationResult.Success;
        }
    }
}
