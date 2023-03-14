using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace JP.DataHub.OData.CosmosDb.ODataToSqlTranslator
{
    /// <summary>
    /// string formmater for OData to Sql converter
    /// </summary>
    public class SQLQueryFormatter : IQueryFormatter
    {
        /// <summary>
        /// constructor
        /// </summary>
        public SQLQueryFormatter
            ()
        {
            startLetter = 'd';
            startLetter--;
        }
        /// <summary>
        /// fieldName => c.fieldName
        /// value => c["value"]
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string TranslateFieldName(string fieldName)
        {
            return GetFieldName(Constants.SQLFieldNameSymbol, fieldName.Trim());
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

                case Constants.KeywordToUpper:
                    return Constants.SQLUpperSymbol;

                case Constants.KeywordToLower:
                    return Constants.SQLLowerSymbol;

                case Constants.KeywordIndexOf:
                    return Constants.SQLIndexOfSymbol;

                case Constants.KeywordTrim:
                    return $"{Constants.SQLLtrimSymbol}{Constants.SymbolOpenParen}{Constants.SQLRtrimSymbol}";

                case Constants.KeywordGeoDistance:
                    return Constants.SQLStDistanceSymbol;

                case Constants.KeywordGeoIntersects:
                    return Constants.SQLStIntersectsSymbol;

                default:
                    return functionName.ToUpper();
            }
        }
        private char startLetter;

        /// <summary>
        /// returns e.g. JOIN a IN c.companies
        /// </summary>
        /// <param name="joinCollection"></param>
        public string TranslateJoinClause(string joinCollection)
        {
            startLetter++;
            //startLetter becomes 'b', 'c' etc

            return string.Concat(Constants.SQLJoinSymbol, Constants.SymbolSpace, startLetter, Constants.SymbolSpace, Constants.SQLInKeyword, Constants.SymbolSpace, GetFieldName(Constants.SQLFieldNameSymbol, joinCollection));
        }

        /// <summary>
        /// translate any expression to a where clause
        /// </summary>
        /// <param name="source"></param>
        /// <param name="edmProperty"></param>
        public string TranslateJoinClause(string source, string edmProperty)
        {
            return GetFieldName(source.StartsWith(Constants.SQLJoinSymbol) ? source : startLetter.ToString(), edmProperty);
        }

        private string GetFieldName(string letterString, string fieldName)
        {
            // 予約語の場合はエスケープする
            if (Constants.SelectEscapeKeywords.Contains(fieldName.ToUpper()))
            {
                return string.Concat(letterString, Constants.SymbolBeginEscapeKeyword, fieldName, Constants.SymbolEndEscapeKeyword);
            }
            else
            {
                return string.Concat(letterString, Constants.SymbolDot, fieldName);
            }
        }
    }
}
