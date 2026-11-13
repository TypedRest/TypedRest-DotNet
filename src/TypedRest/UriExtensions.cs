namespace TypedRest;

/// <summary>
/// Provides extension methods for <see cref="Uri"/>.
/// </summary>
public static class UriExtensions
{
    /// <param name="uri">The base URI to resolve from.</param>
    extension(Uri uri)
    {
        /// <summary>
        /// Adds a trailing slash to the URI if it does not already have one.
        /// </summary>
        public Uri EnsureTrailingSlash()
        {
            if (!uri.IsAbsoluteUri)
            {
                // Temporarily turn relative URIs into absolute ones for appending the slash
                const string prefix = "http://fake:80/";
                string resolvedWithFakeBase = new Uri(new Uri(prefix), uri).EnsureTrailingSlash().OriginalString;
                return new Uri(resolvedWithFakeBase.Substring(uri.OriginalString.StartsWith("/")
                    ? prefix.Length - 1
                    : prefix.Length), UriKind.Relative);
            }

            var builder = new UriBuilder
            {
                Scheme = uri.Scheme,
                Host = uri.Host,
                Port = uri.Port,
                // Add trailing slash to path while preserving query parameters
                Path = uri.AbsolutePath.EndsWith("/") ? uri.AbsolutePath : uri.AbsolutePath + "/",
                Query = uri.Query.TrimStart('?')
            };
            return new Uri(builder.ToString(), UriKind.Absolute);
        }

        /// <summary>
        /// Resolves a relative URI using this URI as the base.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve. Add a <c>./</c> prefix here to imply a trailing slash in <paramref name="uri"/> even if it is missing there.</param>
        /// <example><code>
        /// Debug.Assert(new Uri("http://myhost/path").Join("./subpath") == new Uri("http://myhost/path/subpath"));
        /// </code></example>
        public Uri Join(string relativeUri)
            => new(relativeUri.StartsWith("./") ? uri.EnsureTrailingSlash() : uri, relativeUri);

        /// <summary>
        /// Resolves a relative URI using this URI as the base.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve. Add a <c>./</c> prefix here to imply a trailing slash in <paramref name="uri"/> even if it is missing there.</param>
        /// <example><code>
        /// Debug.Assert(new Uri("http://myhost/path").Join(new Uri("./subpath", UriKind.Relative)) == new Uri("http://myhost/path/subpath"));
        /// </code></example>
        public Uri Join(Uri relativeUri)
            => uri.Join(relativeUri.OriginalString);

        /// <summary>
        /// Extracts credentials from user info in URI if set.
        /// </summary>
        public NetworkCredential? ExtractCredentials()
        {
            var builder = new UriBuilder(uri);
            return string.IsNullOrEmpty(builder.UserName)
                ? null
                : new NetworkCredential(builder.UserName, builder.Password);
        }
    }
}
