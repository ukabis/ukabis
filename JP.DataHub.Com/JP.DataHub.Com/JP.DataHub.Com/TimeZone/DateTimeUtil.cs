using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using TimeZoneConverter;

namespace JP.DataHub.Com.TimeZone
{
    public class DateTimeUtil
    {
        private readonly IConfiguration Configuration = UnityCore.ResolveOrDefault<IConfiguration>();

        /// <summary>
        /// 年月日を文字列化するためのフォーマット
        /// YORK では "yyyy/MM/dd"
        /// </summary>
        public string DateFormat { get; }

        /// <summary>
        /// 年月日時分秒を文字列化するためのフォーマット
        /// YORK では "yyyy/MM/dd hh:mm:ss tt"
        /// </summary>
        public string[] DateTimeFormat { get; }

        /// <summary>
        /// 年月日文字列をParseするための文字列
        /// YORK では "yyyy/M/d"
        /// </summary>
        public string DateParseFormat { get; }

        public DateTime LocalNow => this.GetLocalTime(DateTime.UtcNow);
        public DateTime UtcNow => GetUtc(LocalNow);
        public TimeZoneInfo TimeZoneInfo { get; }
        public CultureInfo CurrentCultureInfo { get; }

        public DateTimeUtil(string dateFormat, string[] dateTimeFormat, string dateParseFormat, TimeZoneInfo timeZone = null, CultureInfo culture = null)
        {
            DateFormat = dateFormat;
            DateTimeFormat = dateTimeFormat;
            DateParseFormat = dateParseFormat;

            if (timeZone == null)
            {
                var timeZoneId = Configuration?.GetValue<string>("TimeZoneId");
                if (!string.IsNullOrEmpty(timeZoneId))
                {
                    TimeZoneInfo = TZConvert.GetTimeZoneInfo(timeZoneId);
                }
                else
                {
                    TimeZoneInfo = TimeZoneInfo.Local;
                }
            }
            else
            {
                TimeZoneInfo = timeZone;
            }

            if (culture == null)
            {
                CultureInfo ci;
                var cultureName = Configuration?.GetValue<string>("CultureName");
                if (!string.IsNullOrEmpty(cultureName))
                {
                    ci = CultureInfo.GetCultureInfo(cultureName);
                }
                else
                {
                    ci = CultureInfo.CurrentCulture;
                }
                var ciClone = (CultureInfo)ci.Clone();
                ciClone.DateTimeFormat.DateSeparator = "/";
                CurrentCultureInfo = ciClone;
            }
            else
            {
                CurrentCultureInfo = culture;
            }
        }

        public DateTimeUtil(string dateFormat, string dateTimeFormat, string dateParseFormat, TimeZoneInfo timeZone = null, CultureInfo culture = null)
            : this(dateFormat, new string[] { dateTimeFormat }, dateParseFormat, timeZone, culture)
        {
        }

        public DateTime GetUtc(DateTime localDateTime) => TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo);

        public DateTime GetLocalTime(DateTime utcDateTime) => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo);

        public DateTime? GetUtc(DateTime? localDateTime) => localDateTime.HasValue ? (DateTime?)TimeZoneInfo.ConvertTimeToUtc(System.DateTime.ParseExact(localDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss"), "yyyy/MM/dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault), TimeZoneInfo) : null;

        public DateTime? GetUtc(string ymd, string hour = null, string min = null, string ampm = null) => GetUtc(GetDateTime(ymd, hour, min, ampm));

        public DateTime? GetLocalTime(DateTime? utcDateTime) => utcDateTime.HasValue ? (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, TimeZoneInfo) : null;

        public bool TryParse(string source, out DateTime dateTime)
        {
            DateTime d;
            var res = DateTime.TryParseExact(source, this.DateParseFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out d);

            dateTime = d;
            return res;
        }

        public bool TryParseDateTime(string source, out DateTime dateTime)
        {
            bool res;
            DateTime d;
            foreach (var fmt in DateTimeFormat)
            {
                res = DateTime.TryParseExact(source, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                if (res)
                {
                    dateTime = d;
                    return true;
                }
            }
            res = DateTime.TryParseExact(source, DateParseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
            if (res)
            {
                dateTime = d;
                return true;
            }
            res = DateTime.TryParseExact(source, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
            if (res)
            {
                dateTime = d;
                return true;
            }
            dateTime = d;
            return false;
        }

        public DateTime Parse(string source) =>
            DateTime.ParseExact(source, this.DateParseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);

        public DateTime? ParseDateTimeNull(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }
            DateTime dt;
            if (TryParseDateTime(source, out dt))
            {
                return dt;
            }
            return null;
        }

        private DateTime? GetDateTime(string ymd, string hour = null, string min = null, string ampm = null)
        {
            if (string.IsNullOrWhiteSpace(ymd))
            {
                return null;
            }

            var dateTime = Parse(ymd);

            int h;
            if (int.TryParse(hour, out h))
            {
                dateTime = dateTime.AddHours(h);
            }

            int m;
            if (int.TryParse(min, out m))
            {
                dateTime = dateTime.AddMinutes(m);
            }

            if (ampm?.ToUpper() == this.CurrentCultureInfo.DateTimeFormat.AMDesignator.ToUpper())
            {
                // do nothing.
            }
            else if (ampm?.ToUpper() == this.CurrentCultureInfo.DateTimeFormat.PMDesignator.ToUpper())
            {
                dateTime = dateTime.AddHours(12);
            }

            return dateTime;
        }
    }

    public static class DateTimeUtilExtension
    {
        public static DateTime? ToDateTime(this string str) => string.IsNullOrEmpty(str) == true ? null : DateTime.Parse(str);
    }
}
