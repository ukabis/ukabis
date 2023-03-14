using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using JP.DataHub.Com.Extensions.Attributes;

namespace JP.DataHub.Com.Extensions
{
    public static class StringExtensions
    {
        public static string NoCRLF(this string str)
            => str?.Replace("\r\n", " ")?.Replace("\r", " ")?.Replace("\n", " ");

        public static bool IsSnakeOrChainCase(this string str) => string.IsNullOrEmpty(str) == false && (str.IndexOf('-') != -1 || str.IndexOf('_') != -1);

        public static string[] SplitSnakeOrChainCase(this string str) => str?.Split("-_".ToCharArray());

        public static string[] SplitPascalOrCamel(this string str)
        {
            var r = new Regex("[a-z0-9][A-Z]");
            var ms = r.Matches(str);
            if (ms.Count == 0)
            {
                return new string[] { str };
            }
            else
            {
                List<string> result = new List<string>();
                int pos = 0;
                for (int i = 0; i < ms.Count; i++)
                {
                    var m = ms[i];
                    result.Add(str.Substring(pos, m.Index + 1 - pos));
                    if (i == ms.Count - 1)
                    {
                        result.Add(str.Substring(m.Index + 1));
                    }
                    pos = m.Index + 1;
                }
                return result.ToArray();
            }
        }

        public static string[] SplitWord(this string str) => IsSnakeOrChainCase(str) ? SplitSnakeOrChainCase(str) : SplitPascalOrCamel(str);

        public static string OneWordToPascal(this string str)
            => str?.Substring(0, 1).ToUpper() + str?.Substring(1).ToLower();
        public static string OneWordToCamel(this string str)
            => str?.Substring(0, 1).ToLower() + str?.Substring(1);

        public static string ToCamel(this string str)
            => string.Join("", SplitWord(str).ToList().Select(x => OneWordToPascal(x)).ToArray()).OneWordToCamel();

        public static string ToPascal(this string str)
            => string.Join("", SplitWord(str).ToList().Select(x => OneWordToPascal(x)).ToArray());

        public static string ToSnake(this string str)
            => string.Join("_", SplitWord(str).ToList().Select(x => x.ToLower()).ToArray());

        public static string ToChain(this string str)
            => string.Join("-", SplitWord(str).ToList().Select(x => x.ToLower()).ToArray());

        public static string ToConstant(this string str)
            => string.Join("_", SplitWord(str).ToList().Select(x => x.ToUpper()).ToArray());

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
        /// Valueの最後の文字が「/」なら、その最後の「/」を削除した文字列を返す
        /// </summary>
        /// <returns></returns>
        public static string NormalizeUrlRelative(this string str)
        {
            if (str != null && str.Length > 0 && str[str.Length - 1] == '/')
            {
                return str.Substring(0, str.Length - 1);
            }
            return str;
        }

        public static T Convert<T>(this string obj)
        {
            try
            {
                var type = typeof(T);
                if (type == typeof(Guid))
                {
                    return (T)(object)Guid.Parse(obj?.ToString());
                }
                return (T)System.Convert.ChangeType(obj, type);
            }
            catch
            {
                return default(T);
            }
        }

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsHttpPrefix(this string str) => str?.StartsWith("http://") == true || str?.StartsWith("https://") == true;

        public static Uri ToUri(this string str) => new Uri(str);

        /// <summary>
        /// 対象の文字列から文字列検索し、ヒットした文字位置を返す。
        /// </summary>
        /// <param name="target">検索対象</param>
        /// <param name="searchWord">検索文字列</param>
        /// <param name="startIdx">検索開始位置</param>
        /// <returns>ヒットした場合は文字位置、ヒットしない場合は-1を返す。</returns>
        public static int FindItemIndex(this string target, string searchWord, int startIdx = 0)
        {
            if (target.Length <= searchWord.Length)
            {
                return -1;
            }

            var idx = target.IndexOf(searchWord, startIdx, StringComparison.OrdinalIgnoreCase);

            if (idx == -1)
            {
                return -1;
            }

            // 対象の前後を取得
            var before = target.Substring(startIdx, idx);
            var after = target.Substring(before.Length);

            // 前後が空白か
            if (after != searchWord &&
                string.IsNullOrEmpty(before.Substring(before.Length - 1).Trim()) &&
                string.IsNullOrEmpty(after.Substring(searchWord.Length, 1).Trim()))
            {
                return idx;
            }
            // 前後空白でない場合/after=searchWordの場合は次を探す
            else
            {
                idx = FindItemIndex(after, searchWord, searchWord.Length + 1);
                if (idx == -1)
                {
                    return -1;
                }
                return idx;
            }
        }

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

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

        public static string FormatWith(this string target, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, target, args);
        }

        /// <summary>
        /// 住所を都道府県と市区町村以降に分割します。
        /// </summary>
        /// <param name="address">住所</param>
        /// <returns>都道府県、市区町村以降</returns>
        public static (string prefecture, string otherAddress) PrefectureSplit(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return (null, null);
            }

            var list = new string[]
            {
            "北海道", "青森県", "岩手県", "宮城県", "秋田県", "山形県", "福島県", "茨城県", "栃木県",
            "群馬県", "埼玉県", "千葉県", "東京都", "神奈川県", "新潟県", "富山県", "石川県", "福井県",
            "山梨県", "長野県", "岐阜県", "静岡県", "愛知県", "三重県", "滋賀県", "京都府", "大阪府",
            "兵庫県", "奈良県", "和歌山県", "鳥取県", "島根県", "岡山県", "広島県", "山口県", "徳島県",
            "香川県", "愛媛県", "高知県", "福岡県", "佐賀県", "長崎県", "熊本県", "大分県", "宮崎県","鹿児島県", "沖縄県"
            };

            var prefecture = list.FirstOrDefault(x => address.StartsWith(x));

            if (string.IsNullOrEmpty(prefecture))
            {
                return (null, address);
            }
            else
            {
                return (prefecture, address.Remove(0, prefecture.Length));
            }
        }

        /// <summary>
        /// 文字列のパターンチェックを行います。
        /// </summary>
        /// <param name="value">文字列</param>
        /// <param name="pattern">パータン</param>
        /// <returns>パータンに一致しない場合はNULLが返されます</returns>
        public static string PatternCheck(this string value, string pattern)
        {
            try
            {
                if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
