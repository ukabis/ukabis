using System.Security.Cryptography.X509Certificates;
using JP.DataHub.Com.DDD;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    [MessagePackObject]
    class ClientCertificateVO : IValueObject
    {
        [Key(0)]
        public string Value { get; }

        [IgnoreMember]
        public X509Certificate2 ValueX509 
        {
            get
            {
                if (_cert == null)
                {
                    ConvertCertificate();
                }
                return _cert;
            }
        }

        [IgnoreMember]
        private X509Certificate2 _cert = null;


        public ClientCertificateVO(string value)
        {
            this.Value = value;
            ConvertCertificate();
        }

        private void ConvertCertificate()
        {
            if (string.IsNullOrEmpty(this.Value)) return;

            var bytes = System.Text.Encoding.UTF8.GetBytes(this.Value);
            _cert = new X509Certificate2(bytes);
        }
    }
}
