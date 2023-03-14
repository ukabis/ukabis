using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service
{
    internal static class UrlUtil
    {
        public static Dictionary<string, string> IsMatchUrlWithoutQueryString(string[] splitRelative, string controller_relative_url, string method_name)
        {
            var keyValue = new Dictionary<string, string>();
            string[] splitUrl = GetUrlNonQuery(controller_relative_url + "/" + method_name).Split("/".ToCharArray());
            if (splitRelative.Length != splitUrl.Length)
            {
                return null;
            }

            // URL部分が合致しているか？
            for (int i = 0; i < splitUrl.Length; i++)
            {
                if (splitUrl[i].Length > 0 && splitUrl[i][0] == '{')
                {
                    if (splitUrl[i][^1] != '}')
                    {
                        throw new Exception("url error");
                    }
                    else if (splitRelative[i].Length > 0)
                    {
                        keyValue.Add(splitUrl[i][1..^1], splitRelative[i]);
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

        public static string GetUrlNonQuery(string url)
        {
            if (string.IsNullOrEmpty(url) == true)
            {
                return url;
            }
            string[] x = url.Split("?".ToCharArray());
            return x[0];
        }

        public static List<string> GetParameterKeyList(string parameter)
        {
            return ParseQuery(parameter).Keys.ToList();
        }

        public class QueryStringDefinition
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public bool IsRequired { get; set; }

            public string TempString { get; set; }
            public bool TmpFlag { get; set; }
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

        public static Dictionary<string, string> ParseQuery(string url)
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
            var query = url.Substring(index + 1);

            var splits = query.Split("&".ToCharArray());
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

            return result;
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

        public static List<string> GetParameterKeyListToQueryParameter(string parameter)
        {
            List<string> result = new();
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
            return urlRelative.Split("/".ToCharArray());
        }

        public static string NormalizeUrlRelative(string urlRelative)
        {
            if (urlRelative[^1] == '/')
            {
                urlRelative = urlRelative[..^1];
            }
            return urlRelative;
        }
    }
}
