using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Sql
{
    public static class UnionSplitter
    {
        private static string UNION_PATTERN = "[  \t^]UNION[ \t$]";
        private static string UNIONALL_PATTERN = "[  \t^]UNION ALL[ \t$]";


        private static Match ValidMatch(Match mUnion, Match mUnionAll)
        {
            if (mUnionAll.Success == true && mUnion.Success == true)
            {
                if (mUnionAll.Index == mUnion.Index)
                {
                    return mUnionAll;
                }
                else if (mUnion.Index < mUnionAll.Index)
                {
                    return mUnion;
                }
                return mUnionAll;
            }
            else if (mUnionAll.Success == true)
            {
                return mUnionAll;
            }
            else if (mUnion.Success == true)
            {
                return mUnion;
            }
            return null;
        }

        private static string CollectSql(string sql)
        {
            sql = sql.Replace("\r\n", "\n");  // 正規表現マッチングで^(行頭)や$(行末)を使う場合\r\nが利用できない。\nなら動作する
            sql = sql.Replace("\t", " ");
            for (; ; )
            {
                Match m = Regex.Match(sql, "^UNION", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (m.Success == true)
                {
                    sql = sql.Substring(0, m.Index - 1) + " UNION" + sql.Substring(m.Index + m.Length);
                    continue;
                }
                m = Regex.Match(sql, "UNION$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (m.Success == true)
                {
                    sql = sql.Substring(0, m.Index) + "UNION " + sql.Substring(m.Index + m.Length + 1);
                    continue;
                }
                m = Regex.Match(sql, "UNION ALL$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (m.Success == true)
                {
                    sql = sql.Substring(0, m.Index) + "UNION ALL " + sql.Substring(m.Index + m.Length + 1);
                    continue;
                }
                break;
            }
            return sql;
        }

        public static bool SplitUnion(string sql, List<SqlUnion> union)
        {
            sql = CollectSql(sql);
            Match mUnion = Regex.Match(sql, UNION_PATTERN, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match mUnionAll = Regex.Match(sql, UNIONALL_PATTERN, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (mUnion.Success == false && mUnionAll.Success == false)
            {
                return false;
            }
            bool prev = true;
            bool isUnionAll = mUnionAll.Success;
            for (; ; )
            {
                Match m = ValidMatch(mUnion, mUnionAll);
                union.Add(new SqlUnion() { IsUnionAll = prev, Query = m != null ? sql.Substring(0, m.Index) : sql });
                if (m == null)
                {
                    break;
                }
                sql = sql.Substring(m.Index + m.Length);
                mUnion = Regex.Match(sql, UNION_PATTERN, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                mUnionAll = Regex.Match(sql, UNIONALL_PATTERN, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                prev = isUnionAll;
                isUnionAll = ValidMatch(mUnion, mUnionAll) == mUnion ? false : mUnionAll.Success;
            }
            return true;
        }
    }
}
