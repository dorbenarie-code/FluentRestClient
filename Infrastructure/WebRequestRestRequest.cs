using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public class WebRequestRestRequest : IRestRequest
    {
        private readonly string _path;
        private readonly string? _baseUrl;
        private string? _username;
        private string? _password;
        private readonly Dictionary<string, string> _headers;

        public WebRequestRestRequest(
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
            var request = CreateRequest("GET");
            return await SendAsync(request);
        }

        public async Task<RestResponse> PostAsync(string body)
        {
            var request = CreateRequest("POST");

            var requestBody = body ?? string.Empty;
            var bodyBytes = Encoding.UTF8.GetBytes(requestBody);

            request.ContentType = GetContentTypeOrDefault();
            request.ContentLength = bodyBytes.Length;

            await using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
            }

            return await SendAsync(request);
        }

        private HttpWebRequest CreateRequest(string method)
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
                throw new InvalidOperationException("Base URL was not configured.");

            var fullUrl = $"{_baseUrl.TrimEnd('/')}/{_path.TrimStart('/')}";
            var request = (HttpWebRequest)WebRequest.Create(fullUrl);

            request.Method = method;

            foreach (var header in _headers)
            {
                ApplyHeader(request, header.Key, header.Value);
            }

            if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_password))
            {
                var rawCredentials = $"{_username}:{_password}";
                var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCredentials));
                request.Headers[HttpRequestHeader.Authorization] = $"Basic {encodedCredentials}";
            }

            return request;
        }

        private static void ApplyHeader(HttpWebRequest request, string key, string value)
        {
            if (string.Equals(key, "Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                request.ContentType = value;
                return;
            }

            if (string.Equals(key, "Accept", StringComparison.OrdinalIgnoreCase))
            {
                request.Accept = value;
                return;
            }

            if (string.Equals(key, "User-Agent", StringComparison.OrdinalIgnoreCase))
            {
                request.UserAgent = value;
                return;
            }

            if (string.Equals(key, "Referer", StringComparison.OrdinalIgnoreCase))
            {
                request.Referer = value;
                return;
            }

            if (string.Equals(key, "Host", StringComparison.OrdinalIgnoreCase))
            {
                request.Host = value;
                return;
            }

            request.Headers[key] = value;
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

        private static async Task<RestResponse> SendAsync(HttpWebRequest request)
        {
            try
            {
                using var response = (HttpWebResponse)await request.GetResponseAsync();
                var body = await ReadResponseBodyAsync(response);

                return new RestResponse((int)response.StatusCode, body);
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse errorResponse)
            {
                using (errorResponse)
                {
                    var body = await ReadResponseBodyAsync(errorResponse);
                    return new RestResponse((int)errorResponse.StatusCode, body);
                }
            }
        }

        private static async Task<string> ReadResponseBodyAsync(HttpWebResponse response)
        {
            using var responseStream = response.GetResponseStream();

            if (responseStream is null)
                return string.Empty;

            using var reader = new StreamReader(responseStream);
            return await reader.ReadToEndAsync();
        }
    }
}