using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public class HttpClientRestRequest : IRestRequest
    {
        private static readonly HttpClient SharedHttpClient = new HttpClient();
        private readonly string _path;
        private readonly string? _baseUrl;
        private string? _username;
        private string? _password;
        private readonly Dictionary<string, string> _headers;

        public HttpClientRestRequest(
            string path,
            string? baseUrl,
            string? username,
            string? password,
            Dictionary<string, string> headers)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            _path = path;
            _baseUrl = baseUrl;
            _username = username;
            _password = password;
            _headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        public IRestRequest WithBasicAuth(string username, string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            _username = username;
            _password = password;
            return this;
        }

        public IRestRequest WithHeader(string key, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);

            _headers[key] = value;
            return this;
        }

        public async Task<RestResponse> GetAsync()
        {
            using var requestMessage = CreateRequestMessage(HttpMethod.Get);
            return await SendAsync(requestMessage);
        }

        public async Task<RestResponse> PostAsync(string body)
        {
            using var requestMessage = CreateRequestMessage(HttpMethod.Post);

            var contentType = GetContentTypeOrDefault();
            requestMessage.Content = new StringContent(body ?? string.Empty, Encoding.UTF8, contentType);

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

            if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_password))
            {
                var encodedCredentials = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{_username}:{_password}"));

                requestMessage.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encodedCredentials);
            }

            return requestMessage;
        }

        private string GetContentTypeOrDefault()
        {
            if (_headers.TryGetValue("Content-Type", out var contentType) &&
                !string.IsNullOrWhiteSpace(contentType))
            {
                return contentType;
            }

            return "application/json";
        }

        private static async Task<RestResponse> SendAsync(HttpRequestMessage requestMessage)
        {
            using var response = await SharedHttpClient.SendAsync(requestMessage);
            var body = await response.Content.ReadAsStringAsync();

            return new RestResponse((int)response.StatusCode, body);
        }
    }
}