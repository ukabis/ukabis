using Microsoft.OData.Edm;


namespace JP.DataHub.OData.SqlServerDb.ODataToSqlTranslator
{
    /// <summary>
    /// abstract class for query formatter used in <see cref="ODataNodeToStringBuilder"/>
    /// </summary>
    public interface IQueryFormatter
    {
        bool IsFilterValueUnescapeEnabled { get; }

        /// <summary>
        /// method to translate fieldName
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>returns translated field</returns>
        string TranslateFieldName(string fieldName);

        /// <summary>
        /// method to translate enum values
        /// </summary>
        /// <param name="typeName">the Odata enum type</param>
        /// <param name="value">string value of the type a number or literal</param>
        /// <returns>returns an enumValue without the namespace</returns>
        string TranslateEnumValue(IEdmTypeReference typeName, string value);

        /// <summary>
        /// method to convert parent/child field
        /// </summary>
        /// <param name="source">the parent field</param>
        /// <param name="edmProperty">the child field</param>
        /// <returns>returns translated parent and child</returns>
        string TranslateSource(string source, string edmProperty);

        /// <summary>
        /// method to convert function name
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns>returns a translated function name</returns>
        string TranslateFunctionName(string functionName);


        /// <summary>
        /// 値の検証とフォーマットを行う
        /// </summary>
        object ValidateAndFormatValue(string propertyName, object value);

        /// <summary>
        /// Boolean型の項目かどうか
        /// </summary>
        bool IsBooleanProperty(string propertyName);
    }
}
