using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;
using JP.DataHub.Com.Helper;

namespace JP.DataHub.Com.Extensions
{
    public static class NJsonSchemaPropertyExtensions
    {
        public static bool IsLength(this JsonSchemaProperty prop) => prop.MinLength != null || prop.MaxLength != null;

        public static string RequiredText(string itemName) => $"{itemName}は必須です";
        public static string RequiredText(this bool flag, string itemName) => flag == true ? $"{itemName}は必須です" : null;
        public static string RequiredText(this JsonSchemaProperty prop, string itemName) => prop?.IsRequired == true ? $"{itemName}は必須です" : null;

        public static string LengthText(this JsonSchemaProperty prop, string itemName) => ValidatorHelper.LengthText(itemName, prop?.MinLength, prop?.MaxLength);
        public static string LengthText(this bool flag, string itemName, int? min, int? max) => flag == true ? ValidatorHelper.LengthText(itemName, min, max) : null;

        public static string InvalidRegexText(this bool flag, string itemName) => flag == true ? $"{itemName}の形式が不正です" : null;
        public static string InvalidRegexText(this JsonSchemaProperty prop, string itemName) => $"{itemName}の形式が不正です";
    }
}
