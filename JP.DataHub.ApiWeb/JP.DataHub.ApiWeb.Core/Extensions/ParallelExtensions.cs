using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Core.Extensions
{
    public static class ParallelExtension
    {
        /// <summary>
        /// HttpContext, PerRequestDataContainerを継承しつつコレクションに対する並列処理を実行する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="body"></param>
        /// <param name="srcContext"></param>
        /// <param name="srcDataContainer"></param>
        /// <returns></returns>
        public static ParallelLoopResult ForEachWithContext(Partitioner<Tuple<int, int>> source, Action<int> body, HttpContext srcContext, IPerRequestDataContainer srcDataContainer)
        {
            var exs = new ConcurrentBag<Exception>();

            var result = Parallel.ForEach(source, (range, loop) =>
            {
                for (var i = range.Item1; i < range.Item2; i++)
                {
                    try
                    {
                        TaskExtension.SwitchHttpContext(srcContext);
                        TaskExtension.SwitchDataContainer(srcDataContainer);
                        body(i);
                    }
                    catch (Exception e)
                    {
                        exs.Add(e);
                    }
                }
            });
            if (!exs.Any()) return result;
            if (exs.Count == 1)
            {
                throw exs.Single();
            }
            throw new AggregateException(exs);
        }

        /// <summary>
        /// HttpContext, PerRequestDataContainerを継承しつつコレクションに対する並列処理を実行する
        /// 同時にParitionerを作成しながら並列処理を行う
        /// </summary>
        /// <remarks>
        /// 並列で実行する処理が短時間で終わる処理ならPartitionerを利用しつつ処理したほうがパフォーマンスが出る
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        /// <param name="srcContext"></param>
        /// <param name="srcDataContainer"></param>
        /// <returns></returns>
        public static ParallelLoopResult ForEachWithContextAndPartitiner<T>(IEnumerable<T> source, Action<int> body,
            HttpContext srcContext, IPerRequestDataContainer srcDataContainer)
        {
            return ForEachWithContext(Partitioner.Create(0, source.Count()), body, srcContext, srcDataContainer);
        }
    }
}
