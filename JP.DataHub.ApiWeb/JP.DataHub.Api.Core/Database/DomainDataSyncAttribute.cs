using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Repository;

namespace JP.DataHub.Api.Core.Database
{
    internal enum ParameterType
    {
        Argument,
        Result,
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    internal class DomainDataSyncAttribute : HandlerAttribute
    {
        private Dictionary<string, string> _dic = new Dictionary<string, string>();
        private ParameterType _parameterType;

        public DomainDataSyncAttribute(ParameterType type, params string[] param)
        {
            _parameterType = type;
            if ((param.Length % 2) != 0)
            {
                throw new ArgumentException("引数パラメータはテーブル名,オブジェクトパスの組み合わせです。数は必ず偶数になるはずです");
            }
            for (int i = 0; i < param.Length / 2; i++)
            {
                _dic.Add(param[i * 2 + 0], param[i * 2 + 1]);
            }
        }

        public DomainDataSyncAttribute(params string[] param)
        {
            _parameterType = ParameterType.Argument;
            if ((param.Length % 2) != 0)
            {
                throw new ArgumentException("引数パラメータはテーブル名,オブジェクトパスの組み合わせです。数は必ず偶数になるはずです");
            }
            for (int i = 0; i < param.Length / 2; i++)
            {
                _dic.Add(param[i * 2 + 0], param[i * 2 + 1]);
            }
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
            => new DomainDataSyncHandler(_parameterType, _dic);

        public class DomainDataSyncHandler : ICallHandler
        {
#if Oracle
            private IStreamingServiceEventRepository _streamingServiceEventRepository => _lazyStreamingServiceEventRepository.Value;
            private Lazy<IStreamingServiceEventRepository> _lazyStreamingServiceEventRepository = new(() => UnityCore.Resolve<IStreamingServiceEventRepository>("DomainDataSyncOci"));
#else
            private Lazy<IServiceBusEventRepository> _lazyServiceBusEventRepository = new(() => UnityCore.Resolve<IServiceBusEventRepository>("DomainDataSync"));
            private IServiceBusEventRepository _serviceBusEventRepository => _lazyServiceBusEventRepository.Value;
#endif

            private Dictionary<string, string> _dic = new Dictionary<string, string>();
            private ParameterType _type;

            public DomainDataSyncHandler(ParameterType type, Dictionary<string, string> dic)
            {
                _type = type;
                _dic = dic;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                // 仕様
                // 1. _typeはメソッドを呼び出したときの結果を使うか、呼び出す際の引数を使うか　※利用するオブジェクトを特定する
                //    ParameterType.Result : メソッド呼び出した結果を使う
                //    ParameterType.Argument : メソッドを呼び出す際の引数を使う
                // 2. オブジェクトから指定された変数パスを見つける
                //     その値を利用してDomainDataSync用のServiceBusに登録する
                // 3. オブジェクトが配列（やリスト）だった場合、すべての要素に対して処理をする
                // 4. ParameterType.Resultかつオブジェクトパスが"."の場合はメソッドリターン値そのものを使う
                if (result.Exception == null)
                {
                    var task = new List<Task>();
                    foreach (var param in _dic)
                    {
                        var vals = _type == ParameterType.Argument ? input.Arguments.MethodArgumentToValueList(param.Value) : result.ReturnValue.MethodResultToValueList(param.Value);
                        if (vals != null)
                        {
                            foreach (var val in vals)
                            {
#if Oracle
                                task.Add(_streamingServiceEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = $"{param.Key}Sync", PkValue = val }));
#else
                                task.Add(_serviceBusEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = $"{param.Key}Sync", PkValue = val }));
#endif
                            }
                        }
                    }
                    Task.WaitAll(task.ToArray());
                }
                return result;
            }

            public int Order
            {
                get => 1;
                set { }
            }
        }
    }
}
