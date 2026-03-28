using System;
using System.Collections.Generic;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public abstract class RestClientBase : FluentHeadersBase<IRestClient>, IRestClient
    {
        protected string? _baseUrl;

        protected RestClientBase()
            : base(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
        {
        }

        protected override IRestClient Self => this;

        public IRestClient WithBaseUrl(string baseUrl)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

            _baseUrl = baseUrl.TrimEnd('/');
            return this;
        }

        public abstract IRestRequest CreateRequest(string path);
    }
}