using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Validations.Annotations.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class NumberAttribute : ValidationAttribute
    {
        private int MaxLength { get; set; }
        private int? Precision { get; set; }

        private readonly string DefaultErrorMessage = "{0} is invalid type.The number of digits is {1}{2}.";

        public NumberAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }

        public NumberAttribute(int maxLength, int precision)
        {
            MaxLength = maxLength;
            Precision = precision;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var str = value?.ToString();
            decimal dec;
            if (decimal.TryParse(str, out dec) == false)
            {
                return new ValidationResult(string.Format(DefaultErrorMessage, validationContext.DisplayName, MaxLength, Precision == null ? "" : $".{Precision}"));
            }
            string[] vals = str.Split('.');
            if (vals.Length != 0 && vals[0].StartsWith("-") == true)
            {
                vals[0] = vals[0].Substring(1);
            }
            bool isError = false;
            if ((vals.Length == 1 && vals[0].Length > MaxLength))
            {
                isError = true;
            }
            else if (vals.Length == 2)
            {
                if (Precision == null)
                {
                    if (vals[0].Length + vals[1].Length > MaxLength)
                    {
                        isError = true;
                    }
                }
                else
                {
                    if (vals[0].Length + vals[1].Length > MaxLength || vals[1].Length > Precision)
                    {
                        isError = true;
                    }
                }
            }
            else if (vals.Length >= 3)
            {
                isError = true;
            }
            if (isError)
            {
                return new ValidationResult(string.Format(DefaultErrorMessage, validationContext.DisplayName, MaxLength, Precision == null ? "" : $".{Precision}"));
            }
            return null;
        }
    }

    public class NumberAttributeEx : NumberAttribute, IValidationAttributeEx
    {
        public Type ExceptionType { get; }

        public NumberAttributeEx(int maxLength, Type exceptionType)
            : base(maxLength)
        {
            ExceptionType = exceptionType;
        }

        public NumberAttributeEx(int maxLength, int precision, Type exceptionType)
            : base(maxLength, precision)
        {
            ExceptionType = exceptionType;
        }
    }
}
