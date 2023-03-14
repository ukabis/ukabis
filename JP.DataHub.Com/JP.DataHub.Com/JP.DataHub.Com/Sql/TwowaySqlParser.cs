using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.SQL
{
    public static class TwowaySqlParserExtensions
    {
        public static string TwowaySqlParser(this string sql, IDictionary<string, object> param) => new TwowaySqlParser(sql, param).Sql;
        public static string TwowaySqlParser(this string sql, TwowaySqlParser.DatabaseType db, IDictionary<string, object> param) => new TwowaySqlParser(db, sql, param).Sql;
        public static string TwowaySqlParser(this string sql, object param) => new TwowaySqlParser(sql, param).Sql;
        public static string TwowaySqlParser(this string sql, TwowaySqlParser.DatabaseType db, object param) => new TwowaySqlParser(db, sql, param).Sql;
    }

    /// <summary>
    /// 2waySQLパーサー
    /// </summary>
    /// <remarks>http://mk3008net.hatenablog.com/entry/2014/10/12/004638</remarks>
    public class TwowaySqlParser
    {
        public enum DatabaseType
        {
            SqlServer,
            Oracle,
            Unknown,
        }

        public static DatabaseType DefaultDatabaseType { get; set; }

        private string prefixHostVal
        {
            get
            {
                if (Database == DatabaseType.Unknown) Database = DefaultDatabaseType;
                if (Database == DatabaseType.Unknown) Database = DatabaseType.SqlServer;
                return Database == DatabaseType.SqlServer ? prefixHostValSqlServer : prefixHostValOracle;
            }
        }
        private string prefixHostValSqlServer = "@";
        private string prefixHostValOracle = ":";
        public DatabaseType Database { get; set; } = DatabaseType.Unknown;

        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        public string Sql { get; private set; }

        public static string ToParse(string sql, IDictionary<string, object> param) => new TwowaySqlParser(sql, param).Sql;
        public static string ToParse(string sql, object param) => new TwowaySqlParser(sql, param).Sql;

        public TwowaySqlParser(string sql, IDictionary<string, object> param)
        {
            Sql = Parse(sql, param);
        }

        public TwowaySqlParser(DatabaseType db, string sql, IDictionary<string, object> param)
        {
            Database = db;
            Sql = Parse(sql, param);
        }

        public TwowaySqlParser(string sql, object param)
        {
            Sql = Parse(sql, param.ObjectToDictionary());
        }

        public TwowaySqlParser(DatabaseType db, string sql, object param)
        {
            Database = db;
            Sql = Parse(sql, param.ObjectToDictionary());
        }

        private string Parse(string sql, IDictionary<string, object> param)
        {
            string WHERE_BLOCK = @"/\*ds where\*/.*?(?<command>(where|order\sby))(?<block>.*?)/\*ds end where\*/[ ]?";

            StringBuilder s = new StringBuilder();
            int pos = 0;

            // dsqlを解析
            Match m = Regex.Match(sql, WHERE_BLOCK, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success == false)
            {
                return ReadAsIfBlock(sql, param, false);
            }

            while (m.Success == true)
            {
                Group g = m.Groups["command"];
                Group block = m.Groups["block"];
                string parts = ReadAsIfBlock(block.Value, param, true);

                // コマンドテキスト
                if (String.IsNullOrEmpty(parts) == true)
                {
                    // ブロックが空の場合、コマンド句なしとする
                    s.AppendFormat("{0} ", sql.Substring(pos, m.Index - pos));
                }
                else
                {
                    // ブロックが空でない場合、コマンド句を記述する
                    s.AppendFormat("{0}{1}{2} ", sql.Substring(pos, m.Index - pos), g.Value, parts.ToString());
                }

                // 事後処理
                pos = m.Index + m.Length;
                m = m.NextMatch();
            }

            // 残った文字列はそのまま付け足す
            s.Append(sql.Substring(pos, sql.Length - pos));

            return s.ToString().TrimEnd();
        }

        public class IfBlock
        {
            public Match IfStatement { get; set; }
            public Match EndStatement { get; set; }
            public List<IfBlock> Children { get; set; } = new List<IfBlock>();
        }

        /// <summary>
        /// IFブロック読み込み
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="isWhereIn"></param>
        /// <returns></returns>
        /// <remarks>
        /// NULLの場合条件式自体を消す
        /// </remarks>
        private string ReadAsIfBlock(string sql, IDictionary<string, object> param, bool isWhereIn)
        {
            // 書式
            string IF_BLOCK = @"/\*ds if (?<name>.*?)[ ]?(\!|\=)\=[ ]?(null|true|false)\*/[ ]?(\r\n|\r|\n)";
            string ENDIF_BLOCK = @"/\*ds end if\*/[ ]?";

            bool hasCommand = false;
            StringBuilder s = new StringBuilder();
            int pos = 0;

            // 構造解析
            var matches = new List<(Match Match, bool IsOpen)>();
            var m = Regex.Match(sql, IF_BLOCK, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            while (m.Success == true)
            {
                matches.Add((m, true));
                m = m.NextMatch();
            }
            m = Regex.Match(sql, ENDIF_BLOCK, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            while (m.Success == true)
            {
                matches.Add((m, false));
                m = m.NextMatch();
            }

            var stack = new Stack<IfBlock>();
            var ifblocks = new List<IfBlock>();
            foreach (var match in matches.OrderBy(x => x.Match.Index))
            {
                if (match.IsOpen)
                {
                    var block = new IfBlock() { IfStatement = match.Match };
                    if (stack.Any())
                    {
                        var parent = stack.Peek();
                        parent.Children.Add(block);
                    }
                    else
                    {
                        ifblocks.Add(block);
                    }
                    stack.Push(block);
                }
                else
                {
                    if (stack.Any())
                    {
                        stack.Pop().EndStatement = match.Match;
                    }
                    else
                    {
                        throw new Exception("ds end ifの方が多いです");
                    }
                }
            }
            if (!ValidateHierarchy(ifblocks))
            {
                throw new Exception("ds ifの方が多いです");
            }

            // dsqlを解析
            if (ifblocks.Any())
            {
                foreach (var ifblock in ifblocks)
                {
                    ParseCommand(sql, param, isWhereIn, ifblock, ref s, ref pos, ref hasCommand);
                }
            }
            else
            {
                sql = ReadAsParamCode(sql, param);
            }
            if (isWhereIn == false)
            {
                var last = sql.Substring(pos);
                s.Append(last);
            }

            // 演算子（接尾書式）の処理
            string result = null;
            if (isWhereIn == true)
            {
                if (hasCommand)
                {
                    string SUFFIX_OPERATOR_CODE = @"(?<code>.*)(?<op>(and|or))\s*$";
                    Match opSuffix = Regex.Match(s.ToString(), SUFFIX_OPERATOR_CODE, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (opSuffix.Success == true)
                    {
                        result = opSuffix.Groups["code"].Value.TrimEnd();
                    }
                    else
                    {
                        result = s.ToString().TrimEnd();
                    }
                }
            }
            else
            {
                result = ReadAsParamCode(s.ToString().TrimEnd(), param);
            }
            return result;
        }

        private void ParseCommand(string sql, IDictionary<string, object> param, bool isWhereIn, IfBlock ifblock, ref StringBuilder s, ref int pos, ref bool hasCommand)
        {
            var g = ifblock.IfStatement.Groups["name"];
            var val = param.Keys.Contains(g.Value) == false ? null : param[g.Value];
            var output = new StringBuilder();

            // IF開始までの文字列を出力
            var tmp = sql.Substring(pos, ifblock.IfStatement.Index - pos);
            output.Append(ReadAsParamCode(tmp, param));

            // IF内部の文字列を出力
            if (val != null && DBNull.Value.Equals(val) == false)
            {
                pos = ifblock.IfStatement.Index + ifblock.IfStatement.Length;

                foreach (var childBlock in ifblock.Children)
                {
                    ParseCommand(sql, param, isWhereIn, childBlock, ref output, ref pos, ref hasCommand);
                }

                tmp = sql.Substring(pos, ifblock.EndStatement.Index - pos);
                output.Append(ReadAsParamCode(tmp, param));
            }

            pos = ifblock.EndStatement.Index + ifblock.EndStatement.Length;

            // コマンドテキスト
            var outputStr = output.ToString();
            if (!string.IsNullOrWhiteSpace(outputStr))
            {
                // 演算子（接頭書式）の処理
                string PREFIX_OPERATOR_CODE = @"^(?<space>\s*)(?<operator>(and|or)\s*)(?<code>[\s\S]*)";

                var mPrefix = Regex.Match(outputStr, PREFIX_OPERATOR_CODE, RegexOptions.IgnoreCase);
                if (s.Length == 0 && !string.IsNullOrEmpty(mPrefix.Groups["operator"].Value))
                {
                    s.AppendFormat("{0}{1}", mPrefix.Groups["space"].Value, mPrefix.Groups["code"].Value);
                }
                else
                {
                    s.Append(outputStr);
                }
                hasCommand = true;
                pos = ifblock.EndStatement.Index + ifblock.EndStatement.Length;
            }

            pos = ifblock.EndStatement.Index + ifblock.EndStatement.Length;
        }

        private string ReadAsParamCode(string sql, IDictionary<string, object> param)
        {
            // パラメータ書式
            string PARAM_CODE = @"/\*ds (?<hard>\$?)(?<name>[^ ]+)\*/(?<dummy>\S+)(?<space> ?)";

            StringBuilder s = new StringBuilder();
            int pos = 0;

            // sqlを解析
            Match m = Regex.Match(sql, PARAM_CODE, RegexOptions.IgnoreCase);
            if (m.Success == false)
            {
                return sql;
            }

            while (m.Success == true)
            {
                Group g = m.Groups["name"];
                Group sp = m.Groups["space"];

                if (param.ContainsKey(g.Value) == true)
                {
                    object val = param[g.Value];

                    String parts = string.Empty;
                    if (string.IsNullOrEmpty(m.Groups["hard"].Value))
                    {
                        // $で始まらない場合、パラメータ
                        parts = CreateParameterCode(g.Value, val);
                    }
                    else
                    {
                        // $で始まる場合、埋め込み
                        parts = CreateHardCode(g.Value, val);
                    }

                    // コマンドテキスト
                    string pre = sql.Substring(pos, m.Index - pos);
                    //if (Regex.IsMatch(pre, @"\sin\s.*$", RegexOptions.IgnoreCase))
                    //{
                    //    // IN句による括弧書き追加
                    //    parts = string.Format("({0})", parts);
                    //}
                    s.AppendFormat("{0}{1}{2}", pre, parts, sp.Value);

                    // 事後処理
                    pos = m.Index + m.Length;
                }

                m = m.NextMatch();
            }

            // 残った文字列はそのまま付け足す
            var tmp = sql.Substring(pos, sql.Length - pos);
            s.Append(tmp);

            return s.ToString();
        }

        private string CreateParameterCode(string name, object val)
        {
            IEnumerable<object> vals = val as IEnumerable<object>;
            StringBuilder parts = new StringBuilder();

            //if (vals == null || val is string)
            {
                // 標準
                if (Parameters.Keys.Contains(prefixHostVal + name) == false)
                {
                    Parameters.Add(prefixHostVal + name, val);
                }
                parts.AppendFormat("{0}{1}", prefixHostVal, name);
            }
            //else
            //{
            //    // 配列
            //    int idx = 0;
            //    foreach (var item in vals)
            //    {
            //        string s = string.Format("{0}_{1}", name, idx);
            //        if (Parameters.Keys.Contains(prefixHostVal + s) == false)
            //        {
            //            Parameters.Add(prefixHostVal + s, item);
            //            idx++;
            //        }
            //        parts.AppendDelimiter(", ").AppendFormat("{0}{1}", prefixHostVal, s);
            //    }
            //}

            return parts.ToString();
        }

        private string CreateHardCode(string name, object val)
        {
            ICollection<object> vals = val as ICollection<object>;
            StringBuilder parts = new StringBuilder();

            if (vals == null || val is string)
            {
                // 標準
                parts.AppendDelimiter(val.ToString());
            }
            else
            {
                // 配列
                foreach (var item in vals)
                {
                    parts.AppendDelimiter(", ").Append(item.ToString());
                }
            }

            string s = parts.ToString();
            if (s.Contains(";"))
            {
                throw new ArgumentException(";を埋め込むことはできません。");
            }
            return s;
        }

        private bool ValidateHierarchy(List<IfBlock> blocks)
        {
            if (blocks == null)
            {
                return true;
            }

            foreach (var block in blocks)
            {
                if (block.EndStatement == null)
                {
                    return false;
                }

                if (!ValidateHierarchy(block.Children))
                {
                    return false;
                }
            }

            return true;
        }
    }


    static class StringBuilderExtension
    {
        public static StringBuilder AppendDelimiter(this StringBuilder source, string delimiter)
        {
            if (source.Length > 0)
            {
                source.Append(delimiter);
            }
            return source;
        }
    }
}