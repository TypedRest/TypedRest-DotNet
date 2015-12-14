using System;

namespace TypedRest
{
    public static class UriExtensions
    {
        /// <summary>
        /// Adds a trailing slash to the URI if it does not already have one.
        /// </summary>
        public static Uri EnsureTrailingSlash(this Uri uri)
        {
            return uri.OriginalString.EndsWith("/")
                ? uri
                : new Uri(uri.OriginalString + "/", uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }
    }
}