using Microsoft.Extensions.DependencyInjection;
using TypedRest.Endpoints;

namespace TypedRest;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers <see cref="EntryEndpoint"/> for dependency injection.
        /// </summary>
        /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically.</param>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the underlying <see cref="HttpClient"/>.</returns>
        public IHttpClientBuilder AddTypedRest(Uri uri)
            => services.AddTypedRest<EntryEndpoint>(uri);

        /// <summary>
        /// Registers a type derived from <see cref="EntryEndpoint"/> for dependency injection.
        /// </summary>
        /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically.</param>
        /// <typeparam name="TEndpoint">The type of the <see cref="EntryEndpoint"/>. Must provide a constructor that accepts an <see cref="HttpClient"/>.</typeparam>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the underlying <see cref="HttpClient"/>.</returns>
        public IHttpClientBuilder AddTypedRest<TEndpoint>(Uri uri)
            where TEndpoint : EntryEndpoint
            => services.AddHttpClient<TEndpoint>(client => client.BaseAddress = uri);

        /// <summary>
        /// Registers a type derived from <see cref="EntryEndpoint"/> for dependency injection.
        /// </summary>
        /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically.</param>
        /// <typeparam name="TInterface">The interface to register.</typeparam>
        /// <typeparam name="TEndpoint">The type of the <see cref="EntryEndpoint"/>. Must provide a constructor that accepts an <see cref="HttpClient"/>.</typeparam>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the underlying <see cref="HttpClient"/>.</returns>
        public IHttpClientBuilder AddTypedRest<TInterface, TEndpoint>(Uri uri)
            where TInterface : class
            where TEndpoint : EntryEndpoint, TInterface
            => services.AddHttpClient<TInterface, TEndpoint>(client => client.BaseAddress = uri);
    }

    /// <param name="builder">The builder to apply the configuration to.</param>
    extension(IHttpClientBuilder builder)
    {
        /// <summary>
        /// Adds HTTP Basic authentication.
        /// </summary>
        /// <param name="configureCredentials">A delegate that is used to configure <see cref="NetworkCredential"/>.</param>
        public IHttpClientBuilder AddBasicAuth(Action<NetworkCredential> configureCredentials)
            => builder.ConfigureHttpClient(httpClient =>
            {
                var credentials = new NetworkCredential();
                configureCredentials(credentials);
                httpClient.AddBasicAuth(credentials);
            });

        /// <summary>
        /// Adds HTTP Basic authentication.
        /// </summary>
        /// <param name="credentials">A credential provider. Will be queried using <see cref="HttpClient.BaseAddress"/> as the uri and "Basic" as the authType.</param>
        public IHttpClientBuilder AddBasicAuth(ICredentials credentials)
            => builder.ConfigureHttpClient(httpClient =>
            {
                if (credentials.GetCredential(httpClient.BaseAddress ?? throw new InvalidOperationException("HttpClient.BaseAddress must be set."), "Basic") is {} networkCredential)
                    httpClient.AddBasicAuth(networkCredential);
            });
    }
}
