using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TypedRest.Endpoints;
using TypedRest.Http;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="EntryEndpoint"/> for dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically.</param>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the underlying <see cref="HttpClient"/>.</returns>
        public static IHttpClientBuilder AddTypedRest(this IServiceCollection services, Uri uri)
            => services.AddTypedRest<EntryEndpoint>(uri);

        /// <summary>
        /// Registers a type derived from <see cref="EntryEndpoint"/> for dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically.</param>
        /// <typeparam name="TEndpoint">The type of the <see cref="EntryEndpoint"/>. Must provide a constructor that accepts an <see cref="HttpClient"/>.</typeparam>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the underlying <see cref="HttpClient"/>.</returns>
        public static IHttpClientBuilder AddTypedRest<TEndpoint>(this IServiceCollection services, Uri uri)
            where TEndpoint : EntryEndpoint
            => services.AddHttpClient<TEndpoint>(client => client.BaseAddress = uri);

        /// <summary>
        /// Registers a type derived from <see cref="EntryEndpoint"/> for dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically.</param>
        /// <typeparam name="TInterface">The interface to register.</typeparam>
        /// <typeparam name="TEndpoint">The type of the <see cref="EntryEndpoint"/>. Must provide a constructor that accepts an <see cref="HttpClient"/>.</typeparam>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the underlying <see cref="HttpClient"/>.</returns>
        public static IHttpClientBuilder AddTypedRest<TInterface, TEndpoint>(this IServiceCollection services, Uri uri)
            where TInterface : class
            where TEndpoint : EntryEndpoint, TInterface
            => services.AddHttpClient<TInterface, TEndpoint>(client => client.BaseAddress = uri);

        /// <summary>
        /// Adds HTTP Basic authentication.
        /// </summary>
        /// <param name="builder">The builder to apply the configuration to.</param>
        /// <param name="configureCredentials">A delegate that is used to configure <see cref="NetworkCredential"/>.</param>
        public static IHttpClientBuilder AddBasicAuth(this IHttpClientBuilder builder, Action<NetworkCredential> configureCredentials)
            => builder.ConfigureHttpClient(httpClient =>
            {
                var credentials = new NetworkCredential();
                configureCredentials(credentials);
                httpClient.AddBasicAuth(credentials);
            });

        /// <summary>
        /// Adds HTTP Basic authentication.
        /// </summary>
        /// <param name="builder">The builder to apply the configuration to.</param>
        /// <param name="credentials">A credential provider. Will be queried using <see cref="HttpClient.BaseAddress"/> as the uri and "Basic" as the authType.</param>
        public static IHttpClientBuilder AddBasicAuth(this IHttpClientBuilder builder, ICredentials credentials)
            => builder.ConfigureHttpClient(httpClient =>
            {
                var networkCredential = credentials.GetCredential(httpClient.BaseAddress ?? throw new InvalidOperationException("HttpClient.BaseAddress must be set."), "Basic");
                if (networkCredential != null)
                    httpClient.AddBasicAuth(networkCredential);
            });
    }
}
