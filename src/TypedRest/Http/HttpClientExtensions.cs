using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.Http
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Adds HTTP Basic authentication.
        /// </summary>
        /// <param name="httpClient">The HTTP client to configure.</param>
        /// <param name="credentials">The credentials to use.</param>
        public static void AddBasicAuth(this HttpClient httpClient, NetworkCredential credentials)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(credentials.UserName + ":" + credentials.Password)));
        }

        /// <summary>
        /// Send a HEAD request to the specified URI.
        /// </summary>
        public static Task<HttpResponseMessage> HeadAsync(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri), cancellationToken);

        /// <summary>
        /// Send an OPTIONS request to the specified URI.
        /// </summary>
        public static Task<HttpResponseMessage> OptionsAsync(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Options, uri), cancellationToken);

        public static Task<HttpResponseMessage> PatchAsync<T>(this HttpClient httpClient, Uri uri, T value, MediaTypeFormatter formatter, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethods.Patch, uri)
            {
                Content = new ObjectContent<T>(value, formatter)
            }, cancellationToken);
    }
}
