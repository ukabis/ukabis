using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class ValidateClientSecretAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string target = value?.ToString();

            if (string.IsNullOrEmpty(target))
            {
                return ValidationResult.Success;
            }

            // 桁数チェック
            if (target.Length < 8 || target.Length > 64)
            {
                return new ValidationResult(ErrorMessageConst.ClientSecretLengthError);
            }

            // 半角英数字と記号かどうか
            if (!new Regex("^[\x20-\x7E]+$").IsMatch(target))
            {
                return new ValidationResult(ErrorMessageConst.ClientSecretFormatError);
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
                return new ValidationResult(ErrorMessageConst.ClientSecretFormatError);
            }

            return ValidationResult.Success;
        }
    }
}
