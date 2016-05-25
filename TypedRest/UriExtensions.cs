using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

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
            return uri.OriginalString.EndsWith("/")
                ? uri
                : new Uri(uri.OriginalString + "/", uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        /// <summary>
        /// Adds or sets query parameters to the URI.
        /// </summary>
        public static Uri WithQueryParameters(this Uri uri, object queryParams)
        {
            if (queryParams == null)
                return uri;

            IDictionary<string, string> queryParamsDict = ResolveQueryParameters(queryParams);
            return AddQueryParametersToUri(uri, queryParamsDict);
        }

        private static IDictionary<string, string> ResolveQueryParameters(object queryValues)
        {
            if (queryValues == null)
                return new Dictionary<string, string>();

            IDictionary<string, string> dictionary = queryValues as IDictionary<string, string>;

            if (dictionary != null)
                return dictionary;

            dictionary = new Dictionary<string, string>();
            foreach (PropertyHelper property in PropertyHelper.GetProperties(queryValues))
                dictionary.Add(property.Name, property.GetValueAsString(queryValues));

            return dictionary;
        }

        private static Uri AddQueryParametersToUri(Uri uri, IDictionary<string, string> queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
                return uri;

            var uriBuilder = new UriBuilder(uri);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (string key in queryParams.Keys)
            {
                query.Set(key, queryParams[key]);
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }
    }
}