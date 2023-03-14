using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.OData.CosmosDb.ODataToSqlTranslator;

namespace UnitTest.JP.DataHub.OData.CosmosDb
{
    [TestClass]
    public class UnitTest_SQLQueryFormatter
    {
        [TestInitialize()]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void TranslateFieldName_Test()
        {
            var fieldNames = new List<string> { "test1", "abc", "hoge" };
            var formatter = new SQLQueryFormatter();

            fieldNames.ForEach(m =>
            {
                Assert.AreEqual(formatter.TranslateFieldName(m), $"c.{m}");
            });
        }

        [TestMethod]
        public void TranslateFieldName_EscapeKeyword_Test()
        {
            var selectEscapeKeywords = new List<string> { "ASC", "AS", "AND", "BY", "BETWEEN", "CASE", "CAST", "CONVERT", "CROSS", "DESC", "DISTINCT", "ELSE", "END", "EXISTS", "FOR", "FROM", "GROUP", "HAVING", "IN", "INNER", "INSERT", "INTO", "IS", "JOIN", "LEFT", "LIKE", "NOT", "ON", "OR", "ORDER", "OUTER", "RIGHT", "SELECT", "SET", "THEN", "TOP", "UPDATE", "VALUE", "WHEN", "WHERE", "WITH" };
            var formatter = new SQLQueryFormatter();

            selectEscapeKeywords.ForEach(m =>
            {
                Assert.AreEqual(formatter.TranslateFieldName(m), $"c[\"{m}\"]");
            });
        }
    }
}
