using System;
using TimeZoneConverter;

namespace JP.DataHub.Com.TimeZone
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertToJst(this DateTime date)
        {
            var jstZoneInfo = TZConvert.GetTimeZoneInfo("Tokyo Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(date.ToUniversalTime(), jstZoneInfo);
        }

        public static DateTime TruncateHour(this DateTime date) => DateTime.Parse(date.ToString("yyyy/MM/dd 00:00:00"));

        public static DateTime TruncateMinute(this DateTime date) => DateTime.Parse(date.ToString("yyyy/MM/dd HH:00:00"));

        public static DateTime TruncateSecond(this DateTime date) => DateTime.Parse(date.ToString("yyyy/MM/dd HH:mm:00"));
    }
}
