using TypedRest.Endpoints.Generic;

namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a collection of <typeparamref name="TEntity"/>s observable as an append-only stream using long-polling.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public class StreamingCollectionEndpoint<TEntity> : StreamingCollectionEndpoint<TEntity, ElementEndpoint<TEntity>>, IStreamingCollectionEndpoint<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Creates a new streaming collection endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
    public StreamingCollectionEndpoint(IEndpoint referrer, Uri relativeUri)
        : base(referrer, relativeUri)
    {}

    /// <summary>
    /// Creates a new streaming collection endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    public StreamingCollectionEndpoint(IEndpoint referrer, string relativeUri)
        : base(referrer, relativeUri)
    {}

    IElementEndpoint<TEntity> IIndexerEndpoint<IElementEndpoint<TEntity>>.this[string id]
        => this[id];

    IElementEndpoint<TEntity> ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>>.this[TEntity entity]
        => this[entity];

    ITask<IElementEndpoint<TEntity>?> ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>>.CreateAsync(TEntity entity, CancellationToken cancellationToken)
        => CreateAsync(entity, cancellationToken);
}