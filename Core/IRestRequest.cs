using System.Threading.Tasks;

namespace FluentRestClient.Core
{
    public interface IRestRequest
    {
        IRestRequest WithBasicAuth(string username, string password);
        IRestRequest WithHeader(string key, string value);
        Task<RestResponse> GetAsync();
        Task<RestResponse> PostAsync(string body);
    }
}