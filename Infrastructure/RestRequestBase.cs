using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public abstract class RestRequestBase : FluentHeadersBase<IRestRequest>, IRestRequest
    {
        protected readonly string _path;
        protected readonly string? _baseUrl;

        protected RestRequestBase(
            string path,
            string? baseUrl,
            Dictionary<string, string> headers)
            : base(headers)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            _path = path;
            _baseUrl = baseUrl;
        }

        protected override IRestRequest Self => this;

        public abstract Task<RestResponse> GetAsync();

        public abstract Task<RestResponse> PostAsync(string body);
    }
}