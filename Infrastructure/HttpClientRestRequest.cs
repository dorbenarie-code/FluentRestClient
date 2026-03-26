using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public class HttpClientRestRequest : RestRequestBase
    {
        private static readonly HttpClient SharedHttpClient = new HttpClient();

        public HttpClientRestRequest(
            string path,
            string? baseUrl,
            Dictionary<string, string> headers) : base(path, baseUrl, headers)
        {
        }

        public override async Task<RestResponse> GetAsync()
        {
            using var requestMessage = CreateRequestMessage(HttpMethod.Get);
            return await SendAsync(requestMessage);
        }

        public override async Task<RestResponse> PostAsync(string body)
        {
            using var requestMessage = CreateRequestMessage(HttpMethod.Post);

            var requestBody = body ?? string.Empty;

            if (_headers.TryGetValue("Content-Type", out var contentType) && !string.IsNullOrWhiteSpace(contentType))
            {
                requestMessage.Content = new StringContent(requestBody, Encoding.UTF8, contentType);
            }
            else
            {
                requestMessage.Content = new StringContent(requestBody, Encoding.UTF8);
                requestMessage.Content.Headers.ContentType = null;
            }

            return await SendAsync(requestMessage);
        }

        private HttpRequestMessage CreateRequestMessage(HttpMethod method)
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
                throw new InvalidOperationException("Base URL was not configured.");

            var fullUrl = $"{_baseUrl.TrimEnd('/')}/{_path.TrimStart('/')}";
            var requestMessage = new HttpRequestMessage(method, fullUrl);

            foreach (var header in _headers)
            {
                if (string.Equals(header.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
                    continue;

                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return requestMessage;
        }

        private static async Task<RestResponse> SendAsync(HttpRequestMessage requestMessage)
        {
            using var response = await SharedHttpClient.SendAsync(requestMessage);
            var body = await response.Content.ReadAsStringAsync();

            return new RestResponse((int)response.StatusCode, body);
        }
    }
}