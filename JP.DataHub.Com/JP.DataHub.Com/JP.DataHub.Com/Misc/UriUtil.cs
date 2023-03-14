using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Misc
{
    public class QueryStringDefinition
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsRequired { get; set; }

        public string TempString { get; set; }
        public bool TmpFlag { get; set; }
    }

    public static class UriUtil
    {
        public static readonly char URI_SEPARATOR_CHAR = '/';
        public static readonly string URI_SEPARATOR_STRING = "/";
        public static readonly char QUERTSTRING_SEPARATOR_CHAR = '?';
        public static readonly string QUERTSTRING_SEPARATOR_STRING = "?";

        public static string[] SplitSeparator(string urlRelative)
        {
            return urlRelative?.Split(URI_SEPARATOR_CHAR);
        }

        public static string JoinPath(string str1, string str2)
        {
            if (str1 == null)
            {
                return str2;
            }
            if (str2 == null)
            {
                return str1;
            }
            str1 = str1.NormalizeUrlRelative();
            return $"{str1}{URI_SEPARATOR_STRING}{str2}".NormalizeUrlRelative();
        }

        public static string GetUrlWithoutQueryString(string url)
        {
            if (string.IsNullOrEmpty(url) == true)
            {
                return url;
            }
            string[] x = url.Split(QUERTSTRING_SEPARATOR_CHAR);
            return x[0];
        }

        public static Dictionary<string, string> IsMatchUrlWithoutQueryString(string url, string relative_url, string method_name, string alias_method_name) => IsMatchUrlWithoutQueryString(UriUtil.SplitSeparator(url), relative_url, method_name, alias_method_name);

        public static Dictionary<string, string> IsMatchUrlWithoutQueryString(string[] splitRelative, string relative_url, string method_name, string alias_method_name)
        {
            var keyValue = IsMatchUrlWithoutQueryString(splitRelative, relative_url, method_name);
            if (keyValue == null)
            {
                keyValue = UriUtil.IsMatchUrlWithoutQueryString(splitRelative, relative_url, alias_method_name);
            }
            if (keyValue == null)
            {
                keyValue = new Dictionary<string, string>();
            }
            return keyValue;
        }

        public static Dictionary<string, string> IsMatchUrlWithoutQueryString(string[] splitRelative, string relative_url, string method_name)
        {
            var keyValue = new Dictionary<string, string>();
            string[] splitUrl = GetUrlWithoutQueryString(relative_url + URI_SEPARATOR_STRING + method_name).Split(URI_SEPARATOR_CHAR);
            if (splitRelative.Length != splitUrl.Length)
            {
                return null;
            }

            // URL部分が合致しているか？
            for (int i = 0; i < splitUrl.Length; i++)
            {
                if (splitUrl[i].Length > 0 && splitUrl[i][0] == '{')
                {
                    if (splitUrl[i][splitUrl[i].Length - 1] != '}')
                    {
                        throw new Exception("url error");
                    }
                    else if (splitRelative[i].Length > 0)
                    {
                        keyValue.Add(splitUrl[i].Substring(1, splitUrl[i].Length - 2), splitRelative[i]);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (splitRelative[i] != splitUrl[i])
                {
                    return null;
                }
            }

            return keyValue;
        }

        public static List<string> QueryStringToKeyList(string parameter) => ParseQueryString(parameter)?.Keys.ToList();

        public static List<string> QueryStringToQueryParameter(string parameter)
        {
            List<string> result = new List<string>();
            foreach (var par in parameter.Split('&'))
            {
                if (!string.IsNullOrEmpty(par))
                {
                    result.Add(par.Split('=')[0].Trim());
                }
            }
            return result;
        }
        public static string[] SplitRelativeUrl(string urlRelative)
        {
            var tmp = urlRelative.Replace("%2F", "/");
            return tmp.Split("/".ToCharArray());
        }

        public static Dictionary<string, string> ParseQueryString(string query)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query))
            {
                return result;
            }
            if (string.IsNullOrEmpty(query) == false)
            {
                string[] ps = query.Split("?".ToCharArray());
                if (ps.Length == 2)
                {
                    string[] splits = ps[1].Split("&".ToCharArray());
                    foreach (var split in splits)
                    {
                        if (!string.IsNullOrEmpty(split))
                        {
                            string[] sentence = split.Split("=".ToCharArray());
                            if (!result.ContainsKey(sentence[0]))
                            {
                                if (sentence.Length == 1)
                                {
                                    result.Add(sentence[0], null);
                                }
                                else if (sentence.Length != 0)
                                {
                                    result.Add(sentence[0], sentence[1]);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static List<string> GetParameterKeyList(string parameter)
        {
            return ParseQueryString(parameter).Keys.ToList();
        }

        public static List<string> GetParameterKeyListToQueryParameter(string parameter)
        {
            List<string> result = new List<string>();
            foreach (var par in parameter.Split('&'))
            {
                if (!string.IsNullOrEmpty(par))
                {
                    result.Add(par.Split('=')[0].Trim());
                }
            }
            return result;
        }

        public static string GetUrlNonQuery(string url)
        {
            if (string.IsNullOrEmpty(url) == true)
            {
                return url;
            }
            string[] x = url.Split("?".ToCharArray());
            return x[0];
        }

        public static Dictionary<string, string> SplitODataQuery(string url)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(url))
            {
                return result;
            }

            var index = url.IndexOf("?");
            if (index < 0)
            {
                return result;
            }

            // '&'で分割(値に含まれる'&'を考慮して分割)
            var query = url.Substring(index + 1).ToCharArray();
            var clauses = new List<string>();
            var from = 0;
            var quoting = false;
            for (var i = 0; i < query.Length; i++)
            {
                if (query[i] == '\'')
                {
                    quoting ^= true;
                }
                else if (query[i] == '&' && !quoting)
                {
                    clauses.Add(new string(query.Skip(from).Take(i - from).ToArray()));
                    from = i + 1;
                }
            }
            if (from < query.Length)
            {
                clauses.Add(new string(query.Skip(from).ToArray()));
            }

            // '='で分割
            foreach (var clause in clauses)
            {
                if (string.IsNullOrEmpty(clause))
                {
                    continue;
                }

                index = clause.IndexOf("=");
                if (index < 0)
                {
                    result.Add(clause, null);
                    continue;
                }
                else
                {
                    result.Add(clause.Substring(0, index), clause.Substring(index + 1));
                }
            }

            return result;
        }

        public static List<QueryStringDefinition> ParseQueryStringDefinition(string url)
        {
            var result = new List<QueryStringDefinition>();
            if (string.IsNullOrEmpty(url))
            {
                return result;
            }

            var index = url.IndexOf("?");
            if (index < 0)
            {
                return result;
            }
            var query = url.Substring(index + 1);

            var splits = query.Split("&".ToCharArray());
            foreach (var split in splits)
            {
                if (!string.IsNullOrEmpty(split))
                {
                    string[] sentence = split.Split("=".ToCharArray());
                    if (!result.Any(x => x.Key == sentence[0]) == true)
                    {
                        if (sentence.Length == 1)
                        {
                            result.Add(new QueryStringDefinition() { Key = sentence[0], Value = null });
                        }
                        else if (sentence.Length != 0)
                        {
                            bool isRequired = !sentence[1].StartsWith("!");
                            result.Add(new QueryStringDefinition() { Key = sentence[0], Value = sentence[1], IsRequired = isRequired });
                        }
                    }
                }
            }

            return result;
        }
    }
}
