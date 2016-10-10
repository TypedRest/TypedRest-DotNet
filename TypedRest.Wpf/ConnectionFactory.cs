using System;
using System.Diagnostics;

namespace TypedRest.Wpf
{
    /// <summary>
    /// Connection factory with Token-based authentication.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of entry endpoint created by the factory. Must have a constructor that takes only an <see cref="Uri"/> and a constructor that takes an <see cref="Uri"/> and an authentication token string.</typeparam>
    public class ConnectionFactory<TEndpoint> : ConnectionFactoryBase<TEndpoint>
        where TEndpoint : EntryEndpoint
    {
        private readonly string _tokenProvider;

        /// <summary>
        /// Creates a connection factory.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. If it contains credentials HTTP Basic Auth is used. If it does not contain credentials Token-based authentication is used.</param>
        /// <param name="tokenProvider">An command to execute or URL to open in the browser when the user needs to fetch a new token.</param>
        public ConnectionFactory(Uri uri, string tokenProvider = null)
            : base(uri)
        {
            _tokenProvider = tokenProvider;
        }

        protected override string RequestToken()
        {
            if (_tokenProvider != null) Process.Start(_tokenProvider);

            // TODO: Ask user for token
            return null;
        }
    }
}