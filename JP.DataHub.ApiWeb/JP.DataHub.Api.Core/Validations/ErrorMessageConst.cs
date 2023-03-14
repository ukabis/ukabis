using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Validations
{
    internal class ErrorMessageConst
    {
        internal const string Required = "必須項目です。";
        internal const string AccessTokenExpirationRangeOutside = "00:01～24:00の範囲で入力してください。";
        internal const string ClientSecretLengthError = "8文字以上64文字以内で入力してください。";
        internal const string ClientSecretFormatError = "大文字、小文字、数字、記号のいずれか3つを含めた半角で入力してください。";
    }
}
