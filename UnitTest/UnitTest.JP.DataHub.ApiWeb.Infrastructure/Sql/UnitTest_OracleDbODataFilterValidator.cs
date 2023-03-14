using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Json.Schema;
using Unity;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.UnitTest.Com;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    [TestClass]
    public class UnitTest_OracleDbODataFilterValidator : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private JSchema Schema = JSchema.Parse($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"" }},
        ""NUM_VALUE"": {{ ""type"": ""number"" }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }}
    }},
    ""additionalProperties"": false
}}");


        [TestInitialize]
        public void TestInitialize()
        {
            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterInstance(Configuration);

            var dataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(dataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);
        }

        #region string

        [TestMethod]
        public void ValidateAndFormat_StringIntoString()
        {
            var propertyName = "STR_VALUE";
            var value = Guid.NewGuid().ToString();

            var target = new OracleDbODataFilterValidator(Schema);
            var result = target.ValidateAndFormat(propertyName, value);
            (result is string).IsTrue();
            ((string)result).Is(value);
        }

        [TestMethod]
        public void ValidateAndFormat_NumberIntoString() => ValidateError("STR_VALUE", 123m);

        #endregion

        #region number

        [TestMethod]
        public void ValidateAndFormat_NumberIntoNumber()
        {
            var propertyName = "NUM_VALUE";
            var value = 123m;

            var target = new OracleDbODataFilterValidator(Schema);
            var result = target.ValidateAndFormat(propertyName, value);
            (result is decimal).IsTrue();
            ((decimal)result).Is(value);
        }

        [TestMethod]
        public void ValidateAndFormat_StringIntoNumber() => ValidateError("NUM_VALUE", Guid.NewGuid().ToString());

        #endregion

        #region date-time

        [TestMethod]
        [TestCase("2020-11-12", "2020-11-12 00:00:00.000000")]
        [TestCase("2020/11/12", "2020-11-12 00:00:00.000000")]
        [TestCase("2020-1-2", "2020-01-02 00:00:00.000000")]
        [TestCase("2020/1/2", "2020-01-02 00:00:00.000000")]
        [TestCase("2020-11-12 14:15:16", "2020-11-12 14:15:16.000000")]
        [TestCase("2020-11-12 14:15:16.12345", "2020-11-12 14:15:16.123450")]
        [TestCase("2020-11-12 14:15:16.123456", "2020-11-12 14:15:16.123456")]
        [TestCase("2020-11-12 14:15:16+09:00", "2020-11-12 05:15:16.000000")]
        [TestCase("2020-11-12T14:15:16", "2020-11-12 14:15:16.000000")]
        [TestCase("2020-11-12T14:15:16.12345", "2020-11-12 14:15:16.123450")]
        [TestCase("2020-11-12T14:15:16.123456", "2020-11-12 14:15:16.123456")]
        [TestCase("2020-11-12T14:15:16+09:00", "2020-11-12 05:15:16.000000")]
        [TestCase("2020-11-12T14:15:16Z", "2020-11-12 14:15:16.000000")]
        public void ValidateAndFormat_DateTimeIntoDateTime()
        {
            TestContext.Run<string, string>((input, expected) =>
            {
                var propertyName = "DAT_VALUE";

                var target = new OracleDbODataFilterValidator(Schema);
                var result = target.ValidateAndFormat(propertyName, input);
                (result is DateTime).IsTrue();
                ((DateTime)result).Is(DateTime.Parse(expected));
            });
        }

        [TestMethod]
        public void ValidateAndFormat_StringIntoDateTime() => ValidateError("DAT_VALUE", Guid.NewGuid().ToString());

        [TestMethod]
        public void ValidateAndFormat_NumberIntoDateTime() => ValidateError("DAT_VALUE", 123m);

        #endregion

        #region boolean

        [TestMethod]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateAndFormat_BooleanIntoBoolean()
        {
            TestContext.Run<bool>((input) =>
            {
                var propertyName = "BOL_VALUE";

                var target = new OracleDbODataFilterValidator(Schema);
                var result = target.ValidateAndFormat(propertyName, input);
                (result is bool).IsTrue();
                ((bool)result).Is(input);
            });
        }

        [TestMethod]
        public void ValidateAndFormat_StringIntoBoolean() => ValidateError("BOL_VALUE", Guid.NewGuid().ToString());

        [TestMethod]
        public void ValidateAndFormat_NumberIntoBoolean() => ValidateError("BOL_VALUE", 123m);

        #endregion

        #region object/array

        [TestMethod]
        public void ValidateAndFormat_Object() => ValidateError("OBJ_VALUE", Guid.NewGuid().ToString());

        [TestMethod]
        public void ValidateAndFormat_Array() => ValidateError("ARY_VALUE", Guid.NewGuid().ToString());

        #endregion

        #region AdditionalProperties

        [TestMethod]
        public void ValidateAndFormat_AdditionalProperties() => ValidateError(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "E10422", false);

        #endregion


        private void ValidateError(string propertyName, object value, string errorCode = "E10426", bool hasErros = true)
        {
            var target = new OracleDbODataFilterValidator(Schema);
            var ex = AssertEx.Catch<Rfc7807Exception>(() => target.ValidateAndFormat(propertyName, value));
            var detail = ex.Rfc7807 as RFC7807ProblemDetailExtendErrors;
            detail.ErrorCode.Is(errorCode);
            if (hasErros)
            {
                detail.Errors.ContainsKey(propertyName).IsTrue();
            }
            else
            {
                detail.Errors.IsNull();
            }
        }
    }
}
