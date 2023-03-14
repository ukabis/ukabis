using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace JP.DataHub.Com.StringExtensions
{
    public static class StringExtension
    {
        public static T Substring<T>(this string str, int start, int length = 0)
        {
            var tmp = str.Substring(start, length);
            var conv = TypeDescriptor.GetConverter(typeof(T));
            if (conv == null)
            {
                return default(T);
            }
            return (T)conv.ConvertFromString(tmp);
        }
        /// <summary>
        /// 指定した部分文字列を始端及び終端から検索し 最初に発見された対象を削除します
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trimInitStr">始端から削除する文字列 nullなら処理せずreturnします</param>
        /// <param name="trimTermStr">終端から削除する文字列 nullならtrimInitStrが使用されます</param>
        /// <returns></returns>
        public static string TrimString(this string str, string trimInitStr, string trimTermStr = null)
        {
            if (string.IsNullOrEmpty(trimInitStr)) return str;
            int index = str.IndexOf(trimInitStr);
            string result = index > -1 ? str.Substring(index + trimInitStr.Length) : str;
            int lastIndex = result.LastIndexOf(trimTermStr ?? trimInitStr);
            result = lastIndex > -1 ? result.Substring(0, lastIndex) : result;
            return result;

        }

        public static string RemoveEmptyLine(this string str) => Regex.Replace(str, $"^[{Environment.NewLine}]+$", "", RegexOptions.Multiline);

        public static string RemoveHeadLine(this string str, int skipCount) => string.Join(Environment.NewLine, str.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(skipCount).ToArray());

        public static Stream ToStream(this string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return new MemoryStream(Encoding.UTF8.GetBytes(str));
            }
            else
            {
                return new MemoryStream(encoding.GetBytes(str));
            }

        }

        public static string UrlCombine(this string str, string add)
        {
            string result = null;
            if (str == null)
            {
                result = add;
            }
            else if (add == null)
            {
                result = str;
            }
            else if (str != string.Empty && add != string.Empty)
            {
                if (str.LastOrDefault() != '/' && add.FirstOrDefault() != '/')
                {
                    str += "/";
                }
                if (str.LastOrDefault() == '/' && add.FirstOrDefault() == '/')
                {
                    return str.Substring(0, str.Length - 1) + add;
                }
                result = str + add;
            }
            else
            {
                result = string.Empty;
            }
            if (result?.EndsWith("/") == true)
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static string NoCRLF(this string str) => str?.Replace("\r\n", " ")?.Replace("\r", " ")?.Replace("\n", " ");

        public static string ToCamel(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1);
            }
        }

        public static string ToPascal(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1);
            }
        }
    }
}
