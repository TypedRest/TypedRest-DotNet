using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using TypedRest.Endpoints;

namespace TypedRest
{
    /// <summary>
    /// Builds <see cref="EntryEndpoint"/>s using config files, interactive authentication, OAuth tokens, etc.
    /// </summary>
    /// <typeparam name="T">The type of entry endpoint created. Must have a constructors with the following signatures: (<see cref="Uri"/>, <see cref="ICredentials"/>) for HTTP Basic Auth and (<see cref="Uri"/>, <see cref="string"/>) for OAuth token.</typeparam>
    public abstract class EndpointProviderBase<T> : IEndpointProvider<T>
        where T : EntryEndpoint
    {
        private string ConfigDir => Path.Combine(
#if NET45
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
#else
            Environment.ExpandEnvironmentVariables("%appdata%"),
#endif
            Assembly.GetEntryAssembly().GetName().Name);

        private string UriFile => Path.Combine(ConfigDir, "uri");

        /// <summary>
        /// Gets an URI and stores it.
        /// </summary>
        private Uri GetUri()
        {
            var uri = GetLocalUri() ?? GetStoredUri();
            if (uri != null) return uri;

            uri = RequestUri();
            if (uri == null) throw new InvalidOperationException("Unable to request endpoint URI.");

            if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
            File.WriteAllText(UriFile, uri.AbsoluteUri, Encoding.UTF8);
            return uri;
        }

        /// <summary>
        /// Gets an endpoint URI placed in a file next to the executable.
        /// </summary>
        /// <returns>The stored URI or <c>null</c> if none exists.</returns>
        private Uri GetLocalUri()
        {
            try
            {
                string localUriFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? Directory.GetCurrentDirectory(), "uri");
                return File.Exists(localUriFile)
                    ? new Uri(File.ReadAllText(localUriFile, Encoding.UTF8), UriKind.Absolute)
                    : null;
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a previously stored endpoint URI.
        /// </summary>
        /// <returns>The stored URI or <c>null</c> if none exists.</returns>
        private Uri GetStoredUri()
        {
            try
            {
                return File.Exists(UriFile)
                    ? new Uri(File.ReadAllText(UriFile, Encoding.UTF8), UriKind.Absolute)
                    : null;
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Asks the user or a service for the base URI of the REST API.
        /// </summary>
        /// <returns>The endpoint URI or <c>null</c> if it cannot be requested.</returns>
        protected abstract Uri RequestUri();

        public void ResetUri()
        {
            if (File.Exists(UriFile)) File.Delete(UriFile);
        }

        private string TokenCacheFile => Path.Combine(ConfigDir, "token");

        /// <summary>
        /// Gets an OAuth token and caches it.
        /// </summary>
        private string GetToken(Uri uri)
        {
            var token = GetCachedToken();
            if (token != null) return token;

            token = RequestToken(uri);
            if (token == null) throw new InvalidOperationException("Unable to request OAuth token.");

            if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
            File.WriteAllText(TokenCacheFile, token, Encoding.UTF8);

            return token;
        }

        /// <summary>
        /// Gets a previously cached OAuth token.
        /// </summary>
        /// <returns>The cached token or <c>null</c> if none exists.</returns>
        private string GetCachedToken()
            => File.Exists(TokenCacheFile)
                ? File.ReadAllText(TokenCacheFile, Encoding.UTF8)
                : null;

        /// <summary>
        /// Asks the user or a service for the OAuth token to present as a "Bearer" to the REST API.
        /// </summary>
        /// <returns>The OAuth token or <c>null</c> if it cannot be requested.</returns>
        protected abstract string RequestToken(Uri uri);

        public void ResetAuthentication()
        {
            if (File.Exists(TokenCacheFile)) File.Delete(TokenCacheFile);
            if (!string.IsNullOrEmpty(GetStoredUri()?.UserInfo)) File.Delete(UriFile);
        }

        public T Build()
        {
            var uri = GetUri();
            var credentials = ExtractCredentials(ref uri);

            return (credentials == null)
                ? NewEndpoint(uri, GetToken(uri))
                : NewEndpoint(uri, credentials);
        }

        private static ICredentials ExtractCredentials(ref Uri uri)
        {
            var builder = new UriBuilder(uri);
            if (string.IsNullOrEmpty(builder.UserName)) return null;

            var credentials = new NetworkCredential(builder.UserName, builder.Password);
            builder.UserName = "";
            builder.Password = "";
            uri = builder.Uri;
            return credentials;
        }

        /// <summary>
        /// Instantiates a <typeparamref name="T"/> with an <see cref="Uri"/> and <see cref="ICredentials"/>.
        /// </summary>
        /// <param name="uri">The base URI of the REST API.</param>
        /// <param name="credentials">Optional HTTP Basic Auth credentials used to authenticate against the REST API.</param>
        protected virtual T NewEndpoint(Uri uri, ICredentials credentials)
            => (T)Activator.CreateInstance(typeof(T), uri, credentials);

        /// <summary>
        /// Instantiates a <typeparamref name="T"/> with an <see cref="Uri"/> and a token.
        /// </summary>
        /// <param name="uri">The base URI of the REST API.</param>
        /// <param name="token">The OAuth token to present as a "Bearer" to the REST API.</param>
        protected virtual T NewEndpoint(Uri uri, string token)
            => (T)Activator.CreateInstance(typeof(T), uri, token);
    }
}
