using System;
using System.Collections.Generic;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public abstract class RestClientBase : IRestClient
    {
        protected string? _baseUrl;
        protected string? _username;
        protected string? _password;
        protected readonly Dictionary<string, string> _headers;

        protected RestClientBase()
        {
            _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IRestClient WithBaseUrl(string baseUrl)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

            _baseUrl = baseUrl.TrimEnd('/');
            return this;
        }

        public IRestClient WithBasicAuth(string username, string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            _username = username;
            _password = password;
            return this;
        }

        public IRestClient WithHeader(string key, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);

            _headers[key] = value;
            return this;
        }

        public abstract IRestRequest CreateRequest(string path);
    }
}