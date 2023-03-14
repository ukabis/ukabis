using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class JsonPropertyFormatParser
    {
        public enum JsonFormatType
        {
            ForeignKey = 0x001,
            NumberDigit = 0x002,
            Reference = 0x100,
            Protect = 0x200,
            Notify = 0x400,
            Other = 0x8000,
            DollarReference,
            DollarValue,
            DollarNull,
        }

        public class JsonFormatInfo
        {
            public JsonFormatType FormatType { get; set; }

            public string FormatTypeName { get; set; }

            public Match Match { get; set; }

            public string KeyName1 { get; set; }

            public string KeyName2 { get; set; }

            public string KeyValue1 { get; set; }

            public string KeyValue2 { get; set; }
            public List<string> KeyValues2 { get; set; }
        }

        public class FormatterRegex
        {
            public JsonFormatType FormatType { get; set; }
            public Regex Regex { get; set; }
            public string KeyName1 { get; set; }
            public string KeyName2 { get; set; }
            public FormatterRegex(JsonFormatType formatType, Regex regex, string keyName1, string keyName2)
            {
                FormatType = formatType;
                Regex = regex;
                KeyName1 = keyName1;
                KeyName2 = keyName2;
            }
        }

        private static Regex regexForeignKey = new Regex("^\\s*ForeignKey\\s*\\(\\s*(?<url>.*?)\\s*\\)\\s*$|^\\s*ForeignKey\\s+(?<url>.*?)\\s*$", RegexOptions.Singleline);
        private static Regex regexNumberDigit = new Regex("^\\s*Number\\s*\\(\\s*(?<d1>\\d*?)\\,??(?<d2>\\d+?)\\s*\\)\\s*$|^\\s*Number\\s+(?<d1>\\d*?)\\,??(?<d2>\\d+?)\\s*$", RegexOptions.Singleline);
        private static Regex regexProtect = new Regex("^\\s*Protect\\s*$|^\\s*Protect\\s*\\(\\s*(?<syntax>.*?)\\s*\\)\\s*$|^\\s*Protect\\s+(?<syntax>.*?)\\s*$", RegexOptions.Singleline);
        private static Regex regexReference = new Regex("^\\s*Reference\\s*\\(\\s*\\\"(?<url>.*?)\\\"\\s*,\\s*(?<property>.*?)\\s*\\)\\s*$|^\\s*Reference\\s+\\\"(?<url>.*?)\\\"\\s*,\\s*(?<property>.*?)\\s*$|^\\s*Reference\\s*\\(\\s*(?<url>.*?)\\s*,\\s*(?<property>.*?)\\s*\\)\\s*$|^\\s*Reference\\s+(?<url>.*?)\\s*,\\s*(?<property>.*?)\\s*$", RegexOptions.Singleline);
        private static Regex regexNotify = new Regex("^\\s*Notify\\s*$", RegexOptions.Singleline);
        private static Regex regexDollarReference = new Regex("^\\s*\\$Reference\\s*\\(\\s*\\\"(?<url>.*?)\\\"\\s*,\\s*(?<property>.*?)\\s*\\)\\s*$|^\\s*\\$Reference\\s+\\\"(?<url>.*?)\\\"\\s*,\\s*(?<property>.*?)\\s*$|^\\s*\\$Reference\\s*\\(\\s*(?<url>.*?)\\s*,\\s*(?<property>.*?)\\s*\\)\\s*$|^\\s*\\$Reference\\s+(?<url>.*?)\\s*,\\s*(?<property>.*?)\\s*$", RegexOptions.Singleline);
        private static Regex regexDollarValue = new Regex("^\\s*\\$Value\\s*\\(\\s*(?<value>.*?)\\s*\\)\\s*$|^\\s*\\$Value\\s+(?<value>.*?)\\s*$", RegexOptions.Singleline);
        private static Regex regexDollarNull = new Regex("^\\s*\\$Null\\s*$", RegexOptions.Singleline);
        private static List<FormatterRegex> list = new List<FormatterRegex>(){
                new FormatterRegex(JsonFormatType.ForeignKey, regexForeignKey, "url", null),
                new FormatterRegex(JsonFormatType.NumberDigit, regexNumberDigit, "d1", "d2"),
                new FormatterRegex(JsonFormatType.Protect, regexProtect, "syntax", null),
                new FormatterRegex(JsonFormatType.Reference, regexReference, "url", "property"),
                new FormatterRegex(JsonFormatType.Notify, regexNotify, "url", "property"),
                new FormatterRegex(JsonFormatType.DollarReference, regexDollarReference, "url", "property"),
                new FormatterRegex(JsonFormatType.DollarValue, regexDollarValue, "value", null),
                new FormatterRegex(JsonFormatType.DollarNull, regexDollarNull, null, null),
            };

        public static List<JsonFormatInfo> ParseFormat(string formats)
        {
            var result = new List<JsonFormatInfo>();
            if (!string.IsNullOrEmpty(formats))
            {
                string[] splits = new Regex(";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Split(formats);
                foreach (var fmt in splits)
                {
                    var format = fmt?.Trim();
                    if (!string.IsNullOrEmpty(format))
                    {
                        bool hit = false;
                        foreach (var pattern in list)
                        {
                            Match m = pattern.Regex.Match(format);
                            if (m.Success)
                            {
                                var keyValue1 = string.IsNullOrEmpty(pattern.KeyName1) ? null : m?.Groups[pattern.KeyName1]?.Value;
                                var keyValue2 = string.IsNullOrEmpty(pattern.KeyName2) ? null : m?.Groups[pattern.KeyName2]?.Value;
                                List<string> keyValues2 = null;
                                if (keyValue2?.Contains(",") != null)
                                {
                                    keyValues2 = keyValue2.Split(',').ToList();
                                }
                                if (keyValue1 == string.Empty && keyValue2 != null && keyValue2 != string.Empty)
                                {
                                    var swap = keyValue1;
                                    keyValue1 = keyValue2;
                                    keyValue2 = swap;
                                }
                                result.Add(new JsonFormatInfo() { FormatType = pattern.FormatType, FormatTypeName = format, Match = m, KeyName1 = pattern.KeyName1, KeyName2 = pattern.KeyName2, KeyValue1 = keyValue1, KeyValue2 = keyValue2, KeyValues2 = keyValues2 });
                                hit = true;
                                break;
                            }
                        }
                        if (hit == false)
                        {
                            result.Add(new JsonFormatInfo() { FormatType = JsonFormatType.Other, FormatTypeName = format });
                        }
                    }
                }
            }
            return result;
        }
    }
}
