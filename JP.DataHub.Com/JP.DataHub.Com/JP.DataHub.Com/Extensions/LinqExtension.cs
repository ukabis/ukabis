using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class LinqExtension
    {
        /// <summary>
        /// 指定したキーでコレクションから重複を排除する キー同士の比較はEquals()
        /// ※使用メモリに注意 コレクションのユニークな要素の分だけ一時的にメモリに乗る
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var set = new HashSet<TSource>();
            foreach (var item in source)
            {
                if (!set.Any(x => keySelector(x).Equals(keySelector(item))))
                {
                    set.Add(item);
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 指定したキーでコレクションから重複を排除する キー同士の比較は指定したComparerer
        /// ※使用メモリに注意 コレクションのユニークな要素の分だけ一時的にメモリに乗る
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer)
        {
            var set = new HashSet<TSource>();
            foreach (var item in source)
            {
                if (!set.Any(x => comparer(keySelector(x), (keySelector(item)))))
                {
                    set.Add(item);
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 指定された要素を重複しない場合のみAddする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        public static void Merge<T>(this ICollection<T> collection, T value)
        {
            if (!collection.Any(x => x?.Equals(value) == true))
            {
                collection.Add(value);
            }

        }

        /// <summary>
        /// コレクションの要素を削除し新しいコレクションを入れなおす
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        public static void ClearAndAdd<T>(this ICollection<T> collection, ICollection<T> value)
        {
            collection.Clear();
            value.ToList().ForEach(x => collection.Add(x));
        }


        /// <summary>
        /// コレクションの要素を削除し新しいコレクションを入れなおす
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        public static void ClearAndAdd<T>(this ICollection<T> collection, T value)
        {
            collection.Clear();
            collection.Add(value);
        }

    }
}
