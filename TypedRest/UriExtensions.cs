using System;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="Uri"/>.
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
    }
}