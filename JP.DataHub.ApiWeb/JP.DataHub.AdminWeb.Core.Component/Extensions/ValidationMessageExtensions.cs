using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Core.Component.Extensions
{
    public static class ValidationMessageExtensions
    {
        public const string RequiredValidationMessage = "必須項目です。";
        public static string ToMaxLengthValidationMessage(this int length) => $"文字数制限を超えています。最大{length}文字で入力して下さい。";
        public static string ToLengthRangeValidationMessage(this (int min, int max) range) => $"{range.min}以上{range.max}以内の文字数で入力してください。";

        public const string InvalidUrlMessage = "正しいURLではありません。";
    }
}
