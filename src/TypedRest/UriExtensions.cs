namespace TypedRest;

/// <summary>
/// Provides extension methods for <see cref="Uri"/>.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Adds a trailing slash to the URI if it does not already have one.
    /// </summary>
    public static Uri EnsureTrailingSlash(this Uri uri)
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
    /// <param name="baseUri">The base URI to resolve from.</param>
    /// <param name="relativeUri">The relative URI to resolve. Prepend <c>./</c> to imply a trailing slash in <paramref name="baseUri"/> even if it is missing there.</param>
    /// <example><code>
    /// Debug.Assert(new Uri("http://myhost/path").Join("./subpath") == new Uri("http://myhost/path/subpath"));
    /// </code></example>
    public static Uri Join(this Uri baseUri, string relativeUri)
        => new(relativeUri.StartsWith("./") ? baseUri.EnsureTrailingSlash() : baseUri, relativeUri);

    /// <summary>
    /// Resolves a relative URI using this URI as the base.
    /// </summary>
    /// <param name="baseUri">The base URI to resolve from.</param>
    /// <param name="relativeUri">The relative URI to resolve. Prepend <c>./</c> to imply a trailing slash in <paramref name="baseUri"/> even if it is missing there.</param>
    /// <example><code>
    /// Debug.Assert(new Uri("http://myhost/path").Join(new Uri("./subpath", UriKind.Relative)) == new Uri("http://myhost/path/subpath"));
    /// </code></example>
    public static Uri Join(this Uri baseUri, Uri relativeUri)
        => baseUri.Join(relativeUri.OriginalString);

    /// <summary>
    /// Extracts credentials from user info in URI if set.
    /// </summary>
    public static NetworkCredential? ExtractCredentials(this Uri uri)
    {
        var builder = new UriBuilder(uri);
        return string.IsNullOrEmpty(builder.UserName)
            ? null
            : new NetworkCredential(builder.UserName, builder.Password);
    }
}
