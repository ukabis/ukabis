using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Helper
{
    public static class ValidatorHelper
    {
        public static string RequiredText(string itemName) => $"{itemName}は必須です";
        public static string LengthText(string itemName, int? minLength, int? maxLength)
        {
            if (minLength == null && maxLength == null)
            {
                return null;
            }
            else if (minLength == null) // maxLength
            {
                return $"{itemName}は{maxLength}文字以下です";
            }
            else if (maxLength == null) // minLength
            {
                return $"{itemName}は{minLength}文字以上です";
            }
            else
            {
                return $"{itemName}は{minLength}文字以上{maxLength}文字以下です";
            }
        }
    }
}
