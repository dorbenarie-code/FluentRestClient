using System;
using System.Collections.Generic;
using FluentRestClient.Core;

namespace FluentRestClient.Infrastructure
{
    public class WebRequestRestClient : RestClientBase
    {
        public override IRestRequest CreateRequest(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var headersCopy = new Dictionary<string, string>(_headers, StringComparer.OrdinalIgnoreCase);

            return new WebRequestRestRequest(
                path,
                _baseUrl,
                headersCopy);
        }
    }
}