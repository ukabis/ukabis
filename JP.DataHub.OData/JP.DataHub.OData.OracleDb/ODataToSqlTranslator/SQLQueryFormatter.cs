using JP.DataHub.OData.Interface;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.OData.OracleDb.ODataToSqlTranslator
{
    /// <summary>
    /// string formmater for OData to Sql converter
    /// </summary>
    public class SQLQueryFormatter : IQueryFormatter
    {
        private IODataFilterValidator ODataFilterValidator { get; }

        public bool IsFilterValueUnescapeEnabled => ODataFilterValidator?.IsFilterValueUnescapeEnabled ?? false;

        /// <summary>
        /// constructor
        /// </summary>
        public SQLQueryFormatter(IODataFilterValidator oDataFilterValidator = null)
        {
            ODataFilterValidator = oDataFilterValidator;
        }

        /// <summary>
        /// Trim fieldName
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string TranslateFieldName(string fieldName)
        {
            // 理由は不明だが、ODataの構文解析をすると、「$filter=id eq A」の「id」が「Id」になってしまう。（１文字目が大文字）
            // その対処のため、Nameを変更する
            if (fieldName == "Id")
            {
                return $"\"id\"";
            }
            else
            {
                return $"\"{fieldName.Trim()}\"";
            }
        }

        /// <summary>
        /// Convert value to SQL format: Namespace'enumVal' => c.enumVal
        /// </summary>
        /// <param name="enuMType">the enum value</param>
        /// <param name="enuMValue">Namespace of the enum type</param>
        /// <returns>enumValue without the namespace</returns>
        public string TranslateEnumValue(IEdmTypeReference enuMType, string enuMValue)
        {
            long result;
            var success = long.TryParse(enuMValue, out result);
            if (!success)
            {

                return enuMValue;
            }
            return string.Concat(Constants.SymbolSingleQuote, enuMType.AsEnum().ToStringLiteral(result), Constants.SymbolSingleQuote);
        }

        /// <summary>
        /// Convert fieldname (parent and child) to SQL format: "class/field" => "c.class.field'"
        /// </summary>
        /// <param name="source">the parent field</param>
        /// <param name="edmProperty">the child field</param>
        /// <returns>The translated source</returns>
        public string TranslateSource(string source, string edmProperty)
        {
            var str = string.Concat(source.Trim(), Constants.SymbolDot, edmProperty.Trim());
            return str.StartsWith(Constants.SQLFieldNameSymbol + Constants.SymbolDot) ? str : string.Concat(Constants.SQLFieldNameSymbol, Constants.SymbolDot, str);
        }

        /// <summary>
        /// Convert functionName to SQL format: funtionName => FUNCTIONNAME
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public string TranslateFunctionName(string functionName)
        {
            switch (functionName)
            {
                case Constants.KeywordLength:
                    return Constants.SQLLengthSymbol;

                case Constants.KeywordToUpper:
                    return Constants.SQLUpperSymbol;

                case Constants.KeywordToLower:
                    return Constants.SQLLowerSymbol;

                case Constants.KeywordIndexOf:
                    return Constants.SQLIndexOfSymbol;

                case Constants.KeywordTrim:
                    return Constants.SQLTrimSymbol;

                case Constants.KeywordSubstring:
                    return Constants.SQLSubstringSymbol;

                case Constants.KeywordCeiling:
                    return Constants.SQLCeilingSymbol;

                case Constants.KeywordGeoDistance:
                    return Constants.SQLStDistanceSymbol;

                case Constants.KeywordGeoIntersects:
                    return Constants.SQLStIntersectsSymbol;

                default:
                    return functionName.ToUpper();
            }
        }


        /// <summary>
        /// 値の検証とフォーマットを行う
        /// </summary>
        public object ValidateAndFormatValue(string propertyName, object value)
        {
            return ODataFilterValidator == null ? value : ODataFilterValidator.ValidateAndFormat(propertyName, value);
        }

        /// <summary>
        /// Boolean型の項目かどうか
        /// </summary>
        public bool IsBooleanProperty(string propertyName)
        {
            return ODataFilterValidator?.IsBooleanProperty(propertyName) ?? false;
        }
    }
}
