using System.Security.Cryptography.X509Certificates;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    class VendorSystemClientCertificateAuthenticationApplicationService : IVendorSystemClientCertificateAuthenticationApplicationService
    {
        protected IClientCertificateRepository ClientCertificateRepository => _lazyClientCertificateRepository.Value;
        private Lazy<IClientCertificateRepository> _lazyClientCertificateRepository = new Lazy<IClientCertificateRepository>(() => UnityCore.Resolve<IClientCertificateRepository>());

        protected IPerRequestDataContainer PerRequestDataContainer => _lazyPerRequestDataContainer.Value;
        private Lazy<IPerRequestDataContainer> _lazyPerRequestDataContainer = new Lazy<IPerRequestDataContainer>(() => UnityCore.Resolve<IPerRequestDataContainer>());


        public VendorSystemAuthenticationResult AuthenticateCetificate(X509Certificate2 clientCertificate)
        {
            var dbcert = GetSubscribedClientCertificates(clientCertificate?.Thumbprint);
            if (dbcert == null || clientCertificate == null)
            {
                return new VendorSystemAuthenticationResult(ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E02409, null));
            }

            return VerifyCertificate(clientCertificate, dbcert);
        }

        private VendorSystemAuthenticationResult VerifyCertificate(X509Certificate2 receivedCert, ClientCertificate dbCert)
        {
            var now = PerRequestDataContainer.GetDateTimeUtil().GetUtc(PerRequestDataContainer.GetDateTimeUtil().LocalNow);

            if (receivedCert.Subject != dbCert.ClientCertificateObject.ValueX509.Subject || receivedCert.Issuer != dbCert.ClientCertificateObject.ValueX509.Issuer)
            {
                return new VendorSystemAuthenticationResult(ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E02409, null));
            }

            // 送られてきたものとサーバー保存のものどちらも日付チェック
            if (DateTime.Compare(now, receivedCert.NotBefore) < 0 || DateTime.Compare(now, receivedCert.NotAfter) > 0 ||
                DateTime.Compare(now, dbCert.ClientCertificateObject.ValueX509.NotBefore) < 0 || DateTime.Compare(now, dbCert.ClientCertificateObject.ValueX509.NotAfter) > 0)
            {
                return new VendorSystemAuthenticationResult(ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E02410, null));
            }
            return new VendorSystemAuthenticationResult(dbCert.VendorId, dbCert.SystemId);
        }

        private ClientCertificate GetSubscribedClientCertificates(string thumbprint)
        {
            return ClientCertificateRepository.GetClientCertificateByThumbPrint(thumbprint);
        }
    }
}
