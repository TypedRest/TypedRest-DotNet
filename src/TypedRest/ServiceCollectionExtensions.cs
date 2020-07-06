using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TypedRest.Endpoints;

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
    }
}
