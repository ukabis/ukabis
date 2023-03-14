using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Web;
using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    class TokenExpirationRecoveryAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container) => new TokenExpirationRecoveryHandler();

        public TokenExpirationRecoveryAttribute()
        {
        }

        internal class TokenExpirationRecoveryHandler : ICallHandler
        {
            public int Order { get => 1; set { } }

            public TokenExpirationRecoveryHandler()
            {
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                IMethodReturn result = null;
                // ループは最大３回。なぜならTokenはOpenId認証とVendor認証があるので１回の呼び出しでそれぞれ一方のみExpirationが発生する。
                // タイミングによってはちょうど両方がExpirationする可能性があるため
                //for (int  i = 0; i < 3; i++)
                {
                    result = getNext()(input, getNext);
                    // token expireか？
                    if (result.ReturnValue is HttpResponseMessage message && message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        var str = message.Content.ReadAsStringAsync().Result;
                        var rfc7807 = str.ToRFC7807ProblemDetail();
                        if (rfc7807 != null)
                        {
                            // token 再取得（どっち？）
                        }
                    }
                    //else
                    //{
                    //    break;
                    //}
                }
                return result;
            }
        }
    }
}
