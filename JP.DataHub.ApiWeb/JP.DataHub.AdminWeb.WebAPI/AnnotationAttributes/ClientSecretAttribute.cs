using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JP.DataHub.AdminWeb.WebAPI.Models;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ClientSecretAttribute : ValidationAttribute
    {
        private class ViewModelErrorMessage
        {
            internal const string ClientSecretRequired = "クライアントシークレットは、新規登録する場合は必須項目です";
            internal const string ClientSecretLengthError = "クライアントシークレットは8文字以上64文字以内で入力してください";
            internal const string ClientSecretFormatError = "クライアントシークレットは大文字、小文字、数字、記号のいずれか3つを含めた半角で入力してください";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var vm = (ClientModel)validationContext.ObjectInstance;

            if (vm.IsActive)
            {
                // 新規登録時は必須
                if (vm.ClientId == null && string.IsNullOrEmpty(value as string))
                {
                    return new ValidationResult(ViewModelErrorMessage.ClientSecretRequired, new List<string>() { validationContext.DisplayName });
                }
                else if (vm.ClientId != null && string.IsNullOrEmpty(value as string))
                {
                    return ValidationResult.Success;
                }

                string target = value.ToString();

                // 桁数チェック
                if (target.Length < 8 || target.Length > 64)
                {
                    return new ValidationResult(ViewModelErrorMessage.ClientSecretLengthError, new List<string>() { validationContext.DisplayName });
                }

                // 半角英数字と記号かどうか
                if (!new Regex("^[\x20-\x7E]+$").IsMatch(target))
                {
                    return new ValidationResult(ViewModelErrorMessage.ClientSecretFormatError, new List<string>() { validationContext.DisplayName });
                }

                bool checkFormat = false;

                Dictionary<string, bool> existCheckList = new Dictionary<string, bool>()
            {
                { "Num", false },
                { "Lower", false },
                { "Upper", false },
                { "Symbol", false }
            };

                foreach (char c in target)
                {
                    // 数字
                    if (char.IsDigit(c))
                    {
                        existCheckList["Num"] = true;
                    }
                    // 小文字
                    else if (char.IsLower(c))
                    {
                        existCheckList["Lower"] = true;
                    }
                    // 大文字
                    else if (char.IsUpper(c))
                    {
                        existCheckList["Upper"] = true;
                    }
                    // 記号(上記以外)
                    else
                    {
                        existCheckList["Symbol"] = true;
                    }

                    if (existCheckList.Where(x => x.Value).Count() >= 3)
                    {
                        checkFormat = true;
                        break;
                    }
                }

                // 大文字、小文字、記号、数字のいずれ3つを含んでいない
                if (!checkFormat)
                {
                    return new ValidationResult(ViewModelErrorMessage.ClientSecretFormatError, new List<string>() { validationContext.DisplayName });
                }
            }

            return ValidationResult.Success;
        }
    }
}
