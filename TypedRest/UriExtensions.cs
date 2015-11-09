using System;

namespace TypedRest
{
    public static class UriExtensions
    {
        /// <summary>
        /// An alternate version of <see cref="Uri.ToString"/> that produces results escaped according to RFC 2396.
        /// </summary>
        public static string ToStringRfc(this Uri uri)
        {
            return (uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString);
        }

        /// <summary>
        /// Adds a trailing slash to the URI if it does not already have one.
        /// </summary>
        public static Uri EnsureTrailingSlash(this Uri uri)
        {
            string escapedString = uri.ToStringRfc();
            return escapedString.EndsWith("/")
                ? uri
                : new Uri(escapedString + "/", uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }
    }
}