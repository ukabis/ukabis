using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;

namespace JP.DataHub.ManageApi.Service.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    internal class AuthenticationAttribute : HandlerAttribute
    {
        private AuthenticationType _authenticationType = AuthenticationType.All;

        public AuthenticationAttribute(AuthenticationType authenticationType = AuthenticationType.All)
        {
            _authenticationType = authenticationType;
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
            => new AuthenticationHandler(_authenticationType);

        public class AuthenticationHandler : ICallHandler
        {
            private AuthenticationType _authenticationType = AuthenticationType.All;

            public AuthenticationHandler(AuthenticationType authenticationType)
            {
                _authenticationType = authenticationType;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var dataContainer = DataContainerUtil.ResolveDataContainer();
                if (_authenticationType.IsCheck(AuthenticationType.OpenId) && dataContainer.OpenId == null)
                {
                    var error = Newtonsoft.Json.JsonConvert.DeserializeObject<RFC7807ProblemDetailExtendErrors>(dataContainer.AuthorizationError);
                    throw new Rfc7807Exception(error);
                }
                if (_authenticationType.IsCheck(AuthenticationType.Vendor) && (dataContainer.VendorId == null || dataContainer.SystemId == null))
                {
                    throw new Rfc7807Exception(ErrorCodeMessage.Code.E02408.GetRFC7807());
                }
                return getNext()(input, getNext);
            }

            private IMethodReturn NonLogExecution(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                return getNext()(input, getNext);
            }

            public int Order { get => 1; set { } }
        }
    }
}
