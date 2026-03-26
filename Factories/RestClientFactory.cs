using FluentRestClient.Core;
using FluentRestClient.Infrastructure;

namespace FluentRestClient.Factories
{
    public static class RestClientFactory
    {
        public static IRestClient CreateDefault() => new HttpClientRestClient();

        public static IRestClient CreateHttpClient() => new HttpClientRestClient();

        public static IRestClient CreateWebRequest() => new WebRequestRestClient();
    }
}