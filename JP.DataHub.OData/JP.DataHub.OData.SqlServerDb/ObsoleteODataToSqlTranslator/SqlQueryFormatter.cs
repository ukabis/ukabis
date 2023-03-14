using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace JP.DataHub.OData.SqlServerDb.ObsoleteODataToSqlTranslator
{
    /// <summary>
    /// string formmater for OData to Sql converter
    /// </summary>
    [Obsolete("互換性のために残している旧版です。")]
    public class SQLQueryFormatter : IQueryFormatter
    {
        /// <summary>
        /// constructor
        /// </summary>
        public SQLQueryFormatter
            ()
        {
            startLetter = 'a';
            startLetter--;
        }

        /// <summary>
        /// fieldName => c.fieldName
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string TranslateFieldName(string fieldName)
        {
            return string.Concat(SqlKeywords.FieldName, Constants.Dot, $"[{fieldName.Trim()}]");
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
            return string.Concat(Constants.SingleQuote, enuMType.AsEnum().ToStringLiteral(result), Constants.SingleQuote);
        }

        /// <summary>
        /// Convert fieldname (parent and child) to SQL format: "class/field" => "c.class.field'"
        /// </summary>
        /// <param name="source">the parent field</param>
        /// <param name="edmProperty">the child field</param>
        /// <returns>The translated source</returns>
        public string TranslateSource(string source, string edmProperty)
        {
            var str = string.Concat(source.Trim(), Constants.Dot, $"[{edmProperty.Trim()}]");
            return str.StartsWith(SqlKeywords.FieldName + Constants.Dot) ? str : string.Concat(SqlKeywords.FieldName, Constants.Dot, str);
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
                case ODataKeywords.ToUpper:
                    return SqlKeywords.Upper;

                case ODataKeywords.ToLower:
                    return SqlKeywords.Lower;

                case ODataKeywords.Length:
                    return SqlKeywords.Len;

                case ODataKeywords.IndexOf:
                    return SqlKeywords.IndexOf;

                case ODataKeywords.GeoDistance:
                    return SqlKeywords.StDistance;

                case ODataKeywords.GeoIntersects:
                    return SqlKeywords.StIntersects;

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
            return string.Concat(SqlKeywords.Join,
                Constants.Space, startLetter,
                Constants.Space,
                SqlKeywords.In,
                Constants.Space,
                SqlKeywords.FieldName,
                Constants.Dot, joinCollection);

        }
        /// <summary>
        /// translate any expression to a where clause
        /// </summary>
        /// <param name="source"></param>
        /// <param name="edmProperty"></param>
        public string TranslateJoinClause(string source, string edmProperty)
        {
            return string.Concat(source.StartsWith(SqlKeywords.Join) ? source : startLetter.ToString(),
                Constants.Dot, edmProperty);
        }
    }
}
