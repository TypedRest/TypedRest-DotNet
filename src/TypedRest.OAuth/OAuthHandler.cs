using System.Diagnostics;
using System.Runtime.CompilerServices;
using IdentityModel;
using IdentityModel.Client;

namespace TypedRest.OAuth;

/// <summary>
/// HTTP message delegating handler that transparently performs OAuth 2.0 authentication with a client secret. Performs OpenID Connect discovery to find the token endpoint.
/// </summary>
public class OAuthHandler : DelegatingHandler
{
    private readonly OAuthOptions _oAuthOptions;
    private readonly Lazy<HttpClient> _httpClient;

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

        if (innerHandler != null) InnerHandler = innerHandler;
        _httpClient = new(() => new HttpClient(InnerHandler ?? new HttpClientHandler()));
    }

    private AccessToken? _accessToken;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_accessToken == null || _accessToken.IsExpired)
            await RequestAccessTokenAsync(cancellationToken);

        var response = await SendAuthenticatedAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Retry with new access token
            await RequestAccessTokenAsync(cancellationToken);
            return await SendAuthenticatedAsync(request, cancellationToken);
        }
        else return response;
    }

    private async Task RequestAccessTokenAsync(CancellationToken cancellationToken)
    {
        var request = new ClientCredentialsTokenRequest
        {
            Address = await DiscoverTokenEndpointAsync(cancellationToken),
            ClientId = _oAuthOptions.ClientId,
            ClientSecret = _oAuthOptions.ClientSecret,
            Scope = _oAuthOptions.Scope
        };
        request.Parameters.AddOptional(OidcConstants.TokenRequest.Audience, _oAuthOptions.Audience);

        var response = await HandleAsync(() => _httpClient.Value.RequestClientCredentialsTokenAsync(request, cancellationToken)).ConfigureAwait(false);

        if (response.Exception != null) throw response.Exception;
        if (response.IsError) throw new AuthenticationException(response.Error);
        _accessToken = new(
            response.AccessToken,
            DateTime.Now + TimeSpan.FromSeconds(response.ExpiresIn) - _oAuthOptions.TokenLifetimeBuffer);
    }

    private async Task<string> DiscoverTokenEndpointAsync(CancellationToken cancellationToken)
    {
        var response = await HandleAsync(() => _httpClient.Value.GetDiscoveryDocumentAsync(_oAuthOptions.Uri.OriginalString, cancellationToken)).ConfigureAwait(false);
        return response.TokenEndpoint;
    }

    private static readonly ActivitySource _activitySource = new("TypedRest.OAuth");

    private static async Task<TResponse> HandleAsync<TResponse>(Func<Task<TResponse>> request, [CallerMemberName] string caller = "unknown")
        where TResponse : ProtocolResponse
    {
        using var activity = _activitySource.StartActivity(nameof(OAuthHandler) + "." + TrimEnding(caller, "Async"))
                                           ?.AddTag("component", "TypedRest.OAuth")
                                            .AddTag("span.kind", "client");

        try
        {
            var response = await request().ConfigureAwait(false);
            activity?.AddTag("http.url", response.HttpResponse!.RequestMessage!.RequestUri!.ToString())
                     .AddTag("http.method", response.HttpResponse.RequestMessage.Method.Method)
                     .AddTag("http.status_code", ((int)response.HttpResponse.StatusCode).ToString());

            if (response.Exception != null) throw response.Exception;
            if (response.IsError) throw new AuthenticationException(response.Error);
            return response;
        }
        catch (Exception ex)
        {
            activity?.AddTag("error", "true")
                     .AddTag("error.type", ex.GetType().Name)
                     .AddTag("error.message", ex.Message);
            throw;
        }
    }

    private static string TrimEnding(string value, string ending)
        => value.EndsWith(ending)
            ? value.Substring(0, value.Length - ending.Length)
            : value;

    private async Task<HttpResponseMessage> SendAuthenticatedAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new("Bearer", _accessToken!.Value);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}