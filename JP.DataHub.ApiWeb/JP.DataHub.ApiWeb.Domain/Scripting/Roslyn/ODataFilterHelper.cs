using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// ODataの$filterの構文を作成するためのヘルパークラス
    /// </summary>
    [RoslynScriptHelp]
    public static class ODataFilterHelper
    {
        /// <summary>
        /// OData $filter用のパラメータ構文を作る
        /// </summary>
        /// <param name="obj">
        /// オブジェクト
        /// </param>
        /// <returns>
        /// $filterに合った文字列を作成
        /// </returns>
        public static string ToFilter(this object obj) => obj.ToFilter<string>();

        /// <summary>
        /// OData $filter用のパラメータ構文を作る
        /// </summary>
        /// <typeparam name="T">
        /// 型T
        /// </typeparam>
        /// <param name="obj">
        /// オブジェクト
        /// </param>
        /// <returns>
        /// $filterに合った文字列を作成
        /// </returns>
        public static string ToFilter<T>(this object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            else if (typeof(T).IsDecimal() && (obj.IsDecimal() || obj.IsValid<T>()))
            {
                return obj.ToString();
            }
            var val = obj.Convert(typeof(T));
            val = val?.ToString()?.Replace("'", "''");
            return $"'{val}'";
        }

        /// <summary>
        /// KeyValueResult（QueryStringからキーを探した時の結果）から$filter用構文（値用）の文字列を作成する
        /// </summary>
        /// <param name="key">
        /// QueryStringからキーを取得した情報
        /// </param>
        /// <returns>
        /// $filterの右辺時の文字列
        /// </returns>
        public static string ToFilter(this IKeyValueResult key)
        {
            if (key.HasKey == false)
            {
                return "null";
            }
            else if (key.Object.IsDecimal() && key.Type.IsDecimal() == true)
            {
                return key.Object.ToString();
            }
            var val = key.Object.Convert(key.Type);
            val = val?.ToString()?.Replace("'", "''");
            return $"'{val}'";
        }

        private static bool IsDecimal(this Type type)
        {
            return type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(decimal) ? true : false;
        }

        private static bool IsDecimal(this object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType().IsDecimal();
        }
    }
}
