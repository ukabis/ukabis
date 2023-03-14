using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    [Log]
    internal interface IClientCertificateRepository
    {
        ClientCertificate GetClientCertificateByThumbPrint(string thumbprint);
    }
}
