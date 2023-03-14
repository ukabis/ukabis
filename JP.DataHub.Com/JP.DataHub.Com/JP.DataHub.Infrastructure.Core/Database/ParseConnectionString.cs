using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JP.DataHub.Infrastructure.Core.Database
{
    public class ParseConnectionString
    {
        public const string CONNECTIONSTRING_DEFAULT_SEPARATOR_STRING = ";";

        public string ConnectionString { get; }

        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public string this[string key]
        {
            get
            {
                return Parameters.ContainsKey(key) ? Parameters[key] : null;
            }
        }

        public ParseConnectionString(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            ConnectionString = connectionString;
            Parameters.Clear();

            if (string.IsNullOrEmpty(connectionString) == false)
            {
                string SENTENCE = @"(?<key>[^=]*)=(?<val>[^;]*);*";
                Match m = Regex.Match(connectionString, SENTENCE, RegexOptions.IgnoreCase);
                while (m.Success == true)
                {
                    Group key = m.Groups["key"];
                    Group val = m.Groups["val"];
                    Parameters.Add(key.Value, val.Value);
                    m = m.NextMatch();
                }
            }
        }

        public string CreateConnectionString(string[] keys)
        {
            var x = CreateConnectionString(CONNECTIONSTRING_DEFAULT_SEPARATOR_STRING, keys);
            return x;
        }

        public string CreateConnectionString(string separator, string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < keys.Length; i++)
            {
                if (i != 0)
                {
                    sb.Append(separator);
                }
                sb.AppendFormat("{0}={1}", keys[i], this[keys[i]]);
            }
            return sb.ToString();
        }
    }

}
