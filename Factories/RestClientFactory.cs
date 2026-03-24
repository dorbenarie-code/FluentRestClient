using FluentRestClient.Core;
using FluentRestClient.Infrastructure;

namespace FluentRestClient.Factories
{
    public static class RestClientFactory
    {
        public static IRestClient CreateDefault()
        {
            return new HttpClientRestClient();
        }

        public static IRestClient CreateHttpClient()
        {
            return new HttpClientRestClient();
        }

        public static IRestClient CreateWebRequest()
        {
            return new WebRequestRestClient();
        }
    }
}