using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.TimeZone;

namespace UnitTest.JP.DataHub.Com.TimeZone
{
    [TestClass]
    public class UnitTest_DateTimeExtensions
    {
        [TestMethod]
        public void DateTime_ConvertToJst_Utc()
        {
            var now = DateTime.UtcNow;
            var jst = now.ConvertToJst();
            Assert.AreEqual(9, (jst - now).Hours);
            if (now.Kind != DateTimeKind.Utc)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DateTime_ConvertToJst_Local()
        {
            var now = DateTime.Now;
            var jst = now.ConvertToJst();
            if (now == now.ToUniversalTime())
            {
                Assert.AreEqual(9, (jst - now).Hours);
            } 
            else
            {
                Assert.AreNotEqual(9, (jst - now).Hours);
            }
            if (now.Kind == DateTimeKind.Utc)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DateTime_TruncateHour()
        {
            var now = DateTime.UtcNow;
            var truncated = now.TruncateHour();
            var expected = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            Assert.AreEqual(expected, truncated);
        }

        [TestMethod]
        public void DateTime_TruncateMinute()
        {
            var now = DateTime.UtcNow;
            var truncated = now.TruncateMinute();
            var expected = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            Assert.AreEqual(expected, truncated);
        }

        [TestMethod]
        public void DateTime_TruncateSecond()
        {
            var now = DateTime.UtcNow;
            var truncated = now.TruncateSecond();
            var expected = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            Assert.AreEqual(expected, truncated);
        }
    }
}