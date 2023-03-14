using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    /// <summary>
    /// 日付時刻が指定された範囲か検証します。
    /// </summary>
    public class DateTimeRangeAttribute : ValidationAttribute
    {
        /// <summary>
        /// 現在時刻からの有効期間
        /// </summary>
        public string SpanFromNow { get; set; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="span">現在時刻からの有効期間</param>
        public DateTimeRangeAttribute(string span)
        {
            SpanFromNow = span;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var now = DateTime.UtcNow;

            return now.CompareTo(value) < 0 && now.Add(TimeSpan.Parse(SpanFromNow)).CompareTo(value) > 0 ?
                ValidationResult.Success : new ValidationResult("Out of range dateTime value.");
        }
    }
}