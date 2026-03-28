using System;
using System.Collections.Generic;
using System.Text;

namespace FluentRestClient.Infrastructure
{
    public abstract class FluentHeadersBase<TSelf>
    {
        protected readonly Dictionary<string, string> _headers;

        protected FluentHeadersBase(Dictionary<string, string> headers)
        {
            _headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        protected abstract TSelf Self { get; }

        public TSelf WithBasicAuth(string username, string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            var rawCredentials = $"{username}:{password}";
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCredentials));

            _headers["Authorization"] = $"Basic {encodedCredentials}";
            return Self;
        }

        public TSelf WithHeader(string key, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);

            _headers[key] = value;
            return Self;
        }
    }
}