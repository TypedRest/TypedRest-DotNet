using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for connection factories with Token-based authentication.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of entry endpoint created by the factory. Must have a constructor that takes only an <see cref="Uri"/> and a constructor that takes an <see cref="Uri"/> and an authentication token string.</typeparam>
    public abstract class ConnectionFactoryBase<TEndpoint>
        where TEndpoint : EntryEndpoint
    {
        private readonly Uri _uri;

        /// <summary>
        /// Creates a connection factory.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. If it contains credentials HTTP Basic Auth is used. If it does not contain credentials Token-based authentication is used.</param>
        protected ConnectionFactoryBase(Uri uri)
        {
            _uri = uri;
        }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns>The entry endpoint for the connection.</returns>
        public async Task<TEndpoint> ConnectAsync()
        {
            return string.IsNullOrEmpty(_uri.UserInfo)
                ? await ConnectTokenAsync()
                : NewEndpoint(_uri);
        }

        /// <summary>
        /// Creates a new connection using Token-based authentication.
        /// </summary>
        private async Task<TEndpoint> ConnectTokenAsync() =>
            await ConnectCachedTokenAsync()
            ?? NewEndpoint(_uri, GetToken());

        /// <summary>
        /// Creates a new connection using a cached Token.
        /// </summary>
        /// <returns>The entry endpoint for the connection or <c>null</c> if no valid Token is cached.</returns>
        private async Task<TEndpoint> ConnectCachedTokenAsync()
        {
            string token = GetCachedToken();
            if (token == null) return null;

            var endpoint = NewEndpoint(_uri, token);

            try
            {
                await CheckConnectionAsync(endpoint);
                return endpoint;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>
        /// Instantiates a <typeparamref name="TEndpoint"/> with an <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface.</param>
        protected virtual TEndpoint NewEndpoint(Uri uri) => (TEndpoint)Activator.CreateInstance(typeof(TEndpoint), uri);

        /// <summary>
        /// Instantiates a <typeparamref name="TEndpoint"/> with an <see cref="Uri"/> and a token.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface.</param>
        /// <param name="token"></param>
        protected virtual TEndpoint NewEndpoint(Uri uri, string token) => (TEndpoint)Activator.CreateInstance(typeof(TEndpoint), uri, token);

        /// <summary>
        /// Performs a read on <paramref name="endpoint"/> to check whether the current token is valid.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">The current token is invalid.</exception>
        protected virtual Task CheckConnectionAsync(TEndpoint endpoint) => endpoint.ReadMetaAsync();

        private string ConfigDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Assembly.GetEntryAssembly().GetName().Name);

        private string TokenCacheFile => Path.Combine(ConfigDir, "token");

        /// <summary>
        /// Gets a previously cached token.
        /// </summary>
        /// <returns>The cached token or <c>null</c> if none exists.</returns>
        private string GetCachedToken() => File.Exists(TokenCacheFile)
            ? File.ReadAllText(TokenCacheFile, Encoding.UTF8)
            : null;

        /// <summary>
        /// Gets a new token and caches it.
        /// </summary>
        private string GetToken()
        {
            string token = RequestToken();

            if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
            File.WriteAllText(TokenCacheFile, token, Encoding.UTF8);

            return token;
        }

        /// <summary>
        /// Asks the user or a service for a token.
        /// </summary>
        protected abstract string RequestToken();
    }
}