using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentRestClient.Core;
using FluentRestClient.Infrastructure.Helpers;

namespace FluentRestClient.Infrastructure
{
    public abstract class RestRequestBase : IRestRequest
    {
        protected readonly string _path;
        protected readonly string? _baseUrl;
        protected readonly Dictionary<string, string> _headers;

        protected RestRequestBase(
            string path, 
            string? baseUrl, 
            Dictionary<string, string> headers)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            _path = path;
            _baseUrl = baseUrl;
            _headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        public IRestRequest WithBasicAuth(string username, string password)
        {
            _headers["Authorization"] = AuthHelper.CreateBasicAuthHeader(username, password);
            return this;
        }

        public IRestRequest WithHeader(string key, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);

            _headers[key] = value;
            return this;
        }

        public abstract Task<RestResponse> GetAsync();

        public abstract Task<RestResponse> PostAsync(string body);
    }
}