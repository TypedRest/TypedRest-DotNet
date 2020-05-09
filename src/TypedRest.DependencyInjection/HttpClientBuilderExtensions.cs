using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TypedRest.OAuth;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <see cref="IHttpClientBuilder"/>.
    /// </summary>
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// Adds an HTTP message delegating handler that transparently performs OAuth 2.0 authentication with a client secret.
        /// </summary>
        /// <param name="builder">The builder to apply the configuration to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        public static IHttpClientBuilder AddOAuthHandler(this IHttpClientBuilder builder, Action<OAuthOptions> configureOptions)
        {
            builder.Services.Configure(builder.Name, configureOptions);
            return builder.AddHttpMessageHandler(provider => new OAuthHandler(provider.GetRequiredService<IOptionsSnapshot<OAuthOptions>>().Get(builder.Name)));
        }

        /// <summary>
        /// Adds an HTTP message delegating handler that transparently performs OAuth 2.0 authentication with a client secret.
        /// </summary>
        /// <param name="builder">The builder to apply the configuration to.</param>
        /// <param name="uri">The URI of the identity server to request an authentication token from.</param>
        /// <param name="clientId">The client identifier to present to the identity server.</param>
        /// <param name="clientSecret">The client secret to present to the identity server.</param>
        public static IHttpClientBuilder AddOAuthHandler(this IHttpClientBuilder builder, Uri uri, string clientId, string clientSecret)
            => builder.AddOAuthHandler(options =>
            {
                options.Uri = uri;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });
    }
}
