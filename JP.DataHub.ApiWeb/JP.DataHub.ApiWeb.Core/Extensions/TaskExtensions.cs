using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Core.Extensions
{
    public static class TaskExtension
    {
        private static readonly IMapper Mapper = new MapperConfiguration(cfg =>
        {
            // マッピング時にソース側のRequestHeadersが空になる事象が発生することがあるため、
            // RequestHeadersを明示的にディープコピーすることにより事象を回避している。
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>()
                .ForMember(dst => dst.RequestHeaders, opt => opt.MapFrom(src => src.RequestHeaders.ToDictionary(x => x.Key, x => new List<string>(x.Value))));
        }).CreateMapper();

        /// <summary>
        /// HttpContext, PerRequestDataContainerを継承しつつバックグラウンド処理を実行する
        /// PerRequestLifeTimeな依存関係を持ったクラスが処理の経路上にある場合に使用する
        /// (このような場合に何もせずバックグラウンド処理を実行すると HttpContextが引き継がれず依存性の解決でほとんど失敗する)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="srcContext"></param>
        /// <param name="srcDataContainer"></param>
        /// <returns></returns>
        public static Task<TResult> RunWithContext<TResult>(Func<TResult> func, HttpContext srcContext, IPerRequestDataContainer srcDataContainer)
        {

            return Task.Run(() =>
            {
                SwitchHttpContext(srcContext);
                SwitchDataContainer(srcDataContainer);
                return func.Invoke();
            });
        }


        public static void SwitchHttpContext(HttpContext src)
        {
            if (src is null)
            {
                return;
            }
        }

        public static void SwitchDataContainer(IPerRequestDataContainer src)
        {
            if (src is null)
            {
                return;
            }
            lock (src)
            {
                var localContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                Mapper.Map(src, localContainer);

                if (!string.IsNullOrEmpty(src.InternalCallKeyword))
                {
                    localContainer.InternalCallKeyword = string.Copy(src.InternalCallKeyword);
                }
            }
        }
    }
}
