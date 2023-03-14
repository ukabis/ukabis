using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Sql
{
    public class ParseSql
    {
        public string Select { get; set; }
        public string From { get; set; }
        public string Condition { get; set; }
        public string Order { get; set; }

        public ParseSql(string sql)
        {
            string select, from, condition, order;
            SplitQuery(sql, out select, out from, out condition, out order);
            Select = select;
            From = from;
            Condition = condition;
            Order = order;
        }

        private void SplitQuery(string query, out string select, out string from, out string condition, out string order)
        {
            //Whereで分割して検索条件のみにする。
            var findwhereIndex = FindItemIndex(query, "where");
            order = "";
            condition = "";
            select = null;
            from = null;

            // 検索条件なし
            if (findwhereIndex == -1)
            {
                var findorderIndex = FindItemIndex(query, "order");
                if (findorderIndex == -1)
                {
                    // SelectとFromのみ
                    select = query;
                    condition = string.Empty;
                }
                else
                {
                    // SelectとFromとOrderBy
                    select = query.Substring(0, findorderIndex);
                    order = query.Substring(findorderIndex);
                }
            }
            else
            {
                // SelectとFromとWhere(とあればOrderBy)
                select = query.Substring(0, findwhereIndex);
                condition = query.Substring(findwhereIndex);
                var findorderIndex = FindItemIndex(condition, "order");
                if (findorderIndex != -1)
                {
                    order = condition.Substring(findorderIndex);
                    condition = condition.Substring(0, findorderIndex);
                }
            }

            var findFromIndex = FindItemIndex(select, "from");
            if (findFromIndex == -1)
            {
                // ODataのクエリで渡された場合
                if (select.StartsWith("$"))
                {
                    select = "";
                    from = "";
                }
                else
                {
                    from = "from c";
                }
            }
            else
            {
                from = select.Substring(findFromIndex);
                select = select.Substring(0, findFromIndex);
            }
        }

        /// <summary>
        /// 対象の文字列から文字列検索し、ヒットした文字位置を返す。
        /// </summary>
        /// <param name="target">検索対象</param>
        /// <param name="searchWord">検索文字列</param>
        /// <param name="startIdx">検索開始位置</param>
        /// <returns>ヒットした場合は文字位置、ヒットしない場合は-1を返す。</returns>
        private int FindItemIndex(string target, string searchWord, int startIdx = 0)
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
                var idx2 = FindItemIndex(after, searchWord, searchWord.Length + 1);
                if (idx2 == -1)
                {
                    return -1;
                }
                return idx + idx2;
            }
        }
    }
}
