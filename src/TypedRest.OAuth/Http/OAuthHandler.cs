using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace TypedRest.Http
{
    /// <summary>
    /// HTTP message delegating handler that transparently performs OAuth 2.0 authentication with a client secret. Performs OpenID Connect discovery to find the token endpoint.
    /// </summary>
    public class OAuthHandler : DelegatingHandler
    {
        private readonly OAuthOptions _oAuthOptions;

        private readonly HttpMessageHandler _innerHandler;

        /// <summary>
        /// Creates a new OAuth handler.
        /// </summary>
        /// <param name="oAuthOptions">Options for OAuth 2.0 / OpenID Connect authentication.</param>
        /// <param name="innerHandler">An optional inner HTTP message handler to delegate to.</param>
        public OAuthHandler(OAuthOptions oAuthOptions, HttpMessageHandler? innerHandler = null)
        {
            if (oAuthOptions == null) throw new ArgumentNullException(nameof(oAuthOptions));
            if (oAuthOptions.Uri == null) throw new ArgumentException($"{nameof(OAuthOptions)}.{nameof(OAuthOptions.Uri)} must not be null.", nameof(oAuthOptions));
            if (string.IsNullOrEmpty(oAuthOptions.ClientId)) throw new ArgumentException($"{nameof(OAuthOptions)}.{nameof(OAuthOptions.ClientId)} must not be null or empty.", nameof(oAuthOptions));
            if (string.IsNullOrEmpty(oAuthOptions.ClientSecret)) throw new ArgumentException($"{nameof(OAuthOptions)}.{nameof(OAuthOptions.ClientSecret)} must not be null or empty.", nameof(oAuthOptions));

            _oAuthOptions = oAuthOptions;
            _innerHandler = innerHandler ?? new HttpClientHandler();
        }

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Double-checked locking pattern
            if (InnerHandler == null)
            {
                await _lock.WaitAsync(cancellationToken).NoContext();
                try
                {
                    if (InnerHandler == null)
                        InnerHandler = await BuildTokenHandlerAsync(cancellationToken).NoContext();
                }
                finally
                {
                    _lock.Release();
                }
            }

            return await base.SendAsync(request, cancellationToken).NoContext();
        }

        private async Task<AccessTokenDelegatingHandler> BuildTokenHandlerAsync(CancellationToken cancellationToken)
        {
            var discovery = await new HttpClient(_innerHandler).GetDiscoveryDocumentAsync(_oAuthOptions.Uri.ToString(), cancellationToken).NoContext();

            return new AccessTokenDelegatingHandler(
                discovery.TokenEndpoint ?? throw new HttpRequestException($"Unable to discover token endpoint for {_oAuthOptions.Uri}."),
                _oAuthOptions.ClientId,
                _oAuthOptions.ClientSecret,
                _oAuthOptions.Scope,
                _innerHandler);
        }
    }
}
