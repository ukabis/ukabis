using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Unity;
using TimeZoneConverter;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.TimeZone;

namespace UnitTest.JP.DataHub.Com.TimeZone
{
    [TestClass]
    public class UnitTest_DateTimeUtil
    {
        #region Mocking Configuration
        class ConfigurationProxy : IConfiguration, IConfigurationSection
        {
            private readonly string retTimeZoneId;
            private readonly string retCultureName;
            private string _key { get; set; }

            public ConfigurationProxy(string timeZoneId, string cultureName)
            {
                retTimeZoneId = timeZoneId;
                retCultureName = cultureName;
            }

            public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string Key => throw new NotImplementedException();
            public string Path => null;
            public string Value 
            {
                get => _key == "TimeZoneId" ? retTimeZoneId : retCultureName;
                set => throw new NotImplementedException();
            }
            public IEnumerable<IConfigurationSection> GetChildren() => throw new NotImplementedException();
            public IChangeToken GetReloadToken() => throw new NotImplementedException();
            public IConfigurationSection GetSection(string key)
            {
                _key = key;
                return this;
            }
        }
        #endregion

        [TestMethod]
        public void DateTimeUtil_Constructor_DateTimeFormatArray()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IConfiguration>(new ConfigurationProxy(null, null));
            UnityCore.UnityContainer = container;

            var verifyCi =  (CultureInfo)CultureInfo.CurrentCulture.Clone();
            verifyCi.DateTimeFormat.DateSeparator = "/";

            var dateFormat = "yyyy/MM/dd";
            var dateTimeFormat = new string[] { "yyyy/MM/dd hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd h:mm:ss" };
            var dateParseFormat = "yyyy/M/d";
            var dateTimeUtil = new DateTimeUtil(dateFormat, dateTimeFormat, dateParseFormat);
            dateTimeUtil.DateFormat.IsStructuralEqual(dateFormat);
            dateTimeUtil.DateTimeFormat.IsStructuralEqual(dateTimeFormat);
            dateTimeUtil.DateParseFormat.IsStructuralEqual(dateParseFormat);
            dateTimeUtil.TimeZoneInfo.IsStructuralEqual(TimeZoneInfo.Local);
            dateTimeUtil.CurrentCultureInfo.IsStructuralEqual(verifyCi);
        }

        [TestMethod]
        public void DateTimeUtil_Constructor_DateTimeFormatString()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IConfiguration>(new ConfigurationProxy(null, null));
            UnityCore.UnityContainer = container;

            var verifyCi = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            verifyCi.DateTimeFormat.DateSeparator = "/";

            var dateFormat = "yyyy/MM/dd";
            var dateTimeFormat = "yyyy/MM/dd hh:mm:ss tt";
            var dateParseFormat = "yyyy/M/d";
            var dateTimeUtil = new DateTimeUtil(dateFormat, dateTimeFormat, dateParseFormat);
            dateTimeUtil.DateFormat.IsStructuralEqual(dateFormat);
            dateTimeUtil.DateTimeFormat.IsStructuralEqual(new string[] { dateTimeFormat });
            dateTimeUtil.DateParseFormat.IsStructuralEqual(dateParseFormat);
            dateTimeUtil.TimeZoneInfo.IsStructuralEqual(TimeZoneInfo.Local);
            dateTimeUtil.CurrentCultureInfo.IsStructuralEqual(verifyCi);
        }

        [TestMethod]
        public void DateTimeUtil_Constructor_AllParameters()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IConfiguration>(new ConfigurationProxy(null, null));
            UnityCore.UnityContainer = container;

            var dateFormat = "yyyy/MM/dd";
            var dateTimeFormat = new string[] { "yyyy/MM/dd hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd h:mm:ss" };
            var dateParseFormat = "yyyy/M/d";
            var dateTimeUtil = new DateTimeUtil(dateFormat, dateTimeFormat, dateParseFormat, TimeZoneInfo.Utc, CultureInfo.CurrentCulture);
            dateTimeUtil.DateFormat.IsStructuralEqual(dateFormat);
            dateTimeUtil.DateTimeFormat.IsStructuralEqual(dateTimeFormat);
            dateTimeUtil.DateParseFormat.IsStructuralEqual(dateParseFormat);
            dateTimeUtil.TimeZoneInfo.IsStructuralEqual(TimeZoneInfo.Utc);
            dateTimeUtil.CurrentCultureInfo.IsStructuralEqual(CultureInfo.CurrentCulture);
        }

        [TestMethod]
        public void DateTimeUtil_Constructor_UseConfig()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IConfiguration>(new ConfigurationProxy("Tokyo Standard Time", "ja-JP"));
            UnityCore.UnityContainer = container;

            var verifyCi = (CultureInfo)CultureInfo.GetCultureInfo("ja-JP").Clone();
            verifyCi.DateTimeFormat.DateSeparator = "/";

            var dateFormat = "yyyy/MM/dd";
            var dateTimeFormat = new string[] { "yyyy/MM/dd hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd h:mm:ss" };
            var dateParseFormat = "yyyy/M/d";
            var dateTimeUtil = new DateTimeUtil(dateFormat, dateTimeFormat, dateParseFormat);
            dateTimeUtil.DateFormat.IsStructuralEqual(dateFormat);
            dateTimeUtil.DateTimeFormat.IsStructuralEqual(dateTimeFormat);
            dateTimeUtil.DateParseFormat.IsStructuralEqual(dateParseFormat);
            dateTimeUtil.TimeZoneInfo.IsStructuralEqual(TZConvert.GetTimeZoneInfo("Tokyo Standard Time"));
            dateTimeUtil.CurrentCultureInfo.IsStructuralEqual(verifyCi);
        }
    }
}