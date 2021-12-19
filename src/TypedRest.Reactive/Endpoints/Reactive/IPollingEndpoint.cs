using TypedRest.Endpoints.Generic;

namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a resource that can be polled for state changes.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public interface IPollingEndpoint<
#if DOXYGEN
out
#endif
    TEntity> : IElementEndpoint<TEntity>
    where TEntity : class
{
    /// <summary>
    /// The interval in which to send requests to the server.
    /// The server can modify this value using the "Retry-After" header.
    /// </summary>
    TimeSpan PollingInterval { get; set; }

    /// <summary>
    /// Provides an observable stream of entity states. Compares entities using <see cref="object.Equals(object)"/> to detect changes.
    /// </summary>
    IObservable<TEntity> GetObservable();
}