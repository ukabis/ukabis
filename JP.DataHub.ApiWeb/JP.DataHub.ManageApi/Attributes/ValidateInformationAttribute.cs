using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateInformationAttribute : ValidationAttribute
    {
        private class ViewModelErrorMessage
        {
            internal const string InformationDateError = "2020/01/01または2020/01/01 10:11のような形式で入力してください。";
        }

        /// <summary>
        /// お知らせ日時(優先度)の検証
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class IsValidInformationDate : ValidateInformationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var result = new DateTime();
                return DateTime.TryParse((string)value, out result) ? ValidationResult.Success : new ValidationResult(ViewModelErrorMessage.InformationDateError);
            }
        }
    }
}
