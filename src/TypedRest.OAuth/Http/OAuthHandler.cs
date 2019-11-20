using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Creates a new OAuth handler.
        /// </summary>
        /// <param name="oAuthOptions">Options for OAuth 2.0 / OpenID Connect authentication.</param>
        /// <param name="innerHandler">An optional inner HTTP message handler to delegate to.</param>
        public OAuthHandler(OAuthOptions oAuthOptions, HttpMessageHandler? innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            if (oAuthOptions == null) throw new ArgumentNullException(nameof(oAuthOptions));
            if (oAuthOptions.Uri == null) throw new ArgumentException($"{nameof(OAuthOptions)}.{nameof(OAuthOptions.Uri)} must not be null.", nameof(oAuthOptions));
            if (string.IsNullOrEmpty(oAuthOptions.ClientId)) throw new ArgumentException($"{nameof(OAuthOptions)}.{nameof(OAuthOptions.ClientId)} must not be null or empty.", nameof(oAuthOptions));
            if (string.IsNullOrEmpty(oAuthOptions.ClientSecret)) throw new ArgumentException($"{nameof(OAuthOptions)}.{nameof(OAuthOptions.ClientSecret)} must not be null or empty.", nameof(oAuthOptions));

            _oAuthOptions = oAuthOptions;
            _httpClient = new HttpClient(InnerHandler);
        }

        private AccessToken? _accessToken;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_accessToken == null || _accessToken.IsExpired)
                _accessToken = await RequestAccessTokenAsync(cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.Value);
            return await base.SendAsync(request, cancellationToken).NoContext();
        }

        private async Task<AccessToken> RequestAccessTokenAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = await DiscoverTokenEndpointAsync(cancellationToken),
                ClientId = _oAuthOptions.ClientId,
                ClientSecret = _oAuthOptions.ClientSecret,
                Scope = _oAuthOptions.Scope
            }, cancellationToken).NoContext();

            if (response.Exception != null) throw response.Exception;
            if (response.IsError) throw new AuthenticationException(response.Error);
            return new AccessToken(
                response.AccessToken,
                DateTime.Now.AddSeconds(response.ExpiresIn - 15 /* buffer time */));
        }

        private async Task<string> DiscoverTokenEndpointAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                RequestUri = _oAuthOptions.Uri
            }, cancellationToken);

            if (response.Exception != null) throw response.Exception;
            if (response.IsError) throw new AuthenticationException(response.Error);
            return response.TokenEndpoint;
        }
    }
}
