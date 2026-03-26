using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public class WebRequestRestRequest : RestRequestBase
    {
        public WebRequestRestRequest(
            string path,
            string? baseUrl,
            Dictionary<string, string> headers) : base(path, baseUrl, headers)
        {
        }

        public override async Task<RestResponse> GetAsync()
        {
            var request = CreateRequest("GET");
            return await SendAsync(request);
        }

        public override async Task<RestResponse> PostAsync(string body)
        {
            var request = CreateRequest("POST");

            var requestBody = body ?? string.Empty;
            var bodyBytes = Encoding.UTF8.GetBytes(requestBody);

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

            return request;
        }

        private static void ApplyHeader(HttpWebRequest request, string key, string value)
        {
            switch (key.ToLowerInvariant())
            {
                case "content-type":
                    request.ContentType = value;
                    break;
                case "accept":
                    request.Accept = value;
                    break;
                case "user-agent":
                    request.UserAgent = value;
                    break;
                case "referer":
                    request.Referer = value;
                    break;
                case "host":
                    request.Host = value;
                    break;
                default:
                    request.Headers[key] = value;
                    break;
            }
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