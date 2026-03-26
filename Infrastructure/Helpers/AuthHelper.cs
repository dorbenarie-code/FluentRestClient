using System;
using System.Text;

namespace FluentRestClient.Infrastructure.Helpers
{
    public static class AuthHelper
    {
        public static string CreateBasicAuthHeader(string username, string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            var rawCredentials = $"{username}:{password}";
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCredentials));

            return $"Basic {encodedCredentials}";
        }
    }
}