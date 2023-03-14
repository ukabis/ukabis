
namespace JP.DataHub.Infrastructure.Database.Data
{
    public class JPDataHubHttpClientFactory : IJPDataHubHttpClientFactory
    {
        public HttpClient CreateClient()
        {
            return new HttpClient();
        }
    }
}
