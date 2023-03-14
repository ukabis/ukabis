using System.Security.Cryptography.X509Certificates;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Com.Validations.Attributes;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    [Log]
    [TransactionScope]
    [ArgumentValidator]
    public interface IVendorSystemClientCertificateAuthenticationApplicationService
    {
        VendorSystemAuthenticationResult AuthenticateCetificate(X509Certificate2 clientCertificate);
    }
}
