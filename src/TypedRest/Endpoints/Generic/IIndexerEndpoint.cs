namespace TypedRest.Endpoints.Generic;

/// <summary>
/// Endpoint that addresses child <typeparamref name="TElementEndpoint"/>s by ID.
/// </summary>
/// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual elements.</typeparam>
public interface IIndexerEndpoint<out TElementEndpoint> : IEndpoint
    where TElementEndpoint : IEndpoint
{
    /// <summary>
    /// Returns a <typeparamref name="TElementEndpoint"/> for a specific child element.
    /// </summary>
    /// <param name="id">The ID identifying the entity.</param>
    TElementEndpoint this[string id] { get; }
}