namespace TypedRest.Http;

/// <summary>
/// Provides extension methods for <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <param name="httpClient">The HTTP client to configure.</param>
    extension(HttpClient httpClient)
    {
        /// <summary>
        /// Adds HTTP Basic authentication.
        /// </summary>
        /// <param name="credentials">The credentials to use.</param>
        public void AddBasicAuth(NetworkCredential credentials)
        {
            httpClient.DefaultRequestHeaders.Authorization = new("Basic",
                Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(credentials.UserName + ":" + credentials.Password)));
        }

        /// <summary>
        /// Send a HEAD request to the specified URI.
        /// </summary>
        public Task<HttpResponseMessage> HeadAsync(Uri uri, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new(HttpMethod.Head, uri), cancellationToken);

        /// <summary>
        /// Send an OPTIONS request to the specified URI.
        /// </summary>
        public Task<HttpResponseMessage> OptionsAsync(Uri uri, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new(HttpMethod.Options, uri), cancellationToken);

        public Task<HttpResponseMessage> PatchAsync<T>(Uri uri, T value, MediaTypeFormatter formatter, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new(HttpMethods.Patch, uri)
            {
                Content = new ObjectContent<T>(value, formatter)
            }, cancellationToken);
    }
}
