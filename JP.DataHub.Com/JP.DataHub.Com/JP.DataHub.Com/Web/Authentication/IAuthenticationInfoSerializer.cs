namespace JP.DataHub.Com.Web.Authentication
{
    public interface IAuthenticationInfoSerializer
    {
        void Set<T>(string key, T obj);
        void SetString(string key, string obj);
        void SetObject(string key, object obj);
        T Get<T>(string key) where T : new();
        string GetString(string key);
        object GetObject(string key);
    }
}
