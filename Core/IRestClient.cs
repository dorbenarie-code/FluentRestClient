namespace FluentRestClient.Core
{
    public interface IRestClient
    {
        IRestClient WithBaseUrl(string baseUrl);
        IRestClient WithBasicAuth(string username, string password);
        IRestClient WithHeader(string key, string value);
        IRestRequest CreateRequest(string path);
    }
}