using System;
using System.Threading;
using MorseCode.ITask;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for a collection of <typeparamref name="TEntity"/>s addressable as <see cref="ElementEndpoint{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
    public class CollectionEndpoint<TEntity> : CollectionEndpoint<TEntity, ElementEndpoint<TEntity>>, ICollectionEndpoint<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Creates a new collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public CollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        public CollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        IElementEndpoint<TEntity> IIndexerEndpoint<IElementEndpoint<TEntity>>.this[string id]
            => this[id];

        IElementEndpoint<TEntity> ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>>.this[TEntity entity]
            => this[entity];

        ITask<IElementEndpoint<TEntity>> ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>>.CreateAsync(TEntity entity, CancellationToken cancellationToken)
            => CreateAsync(entity, cancellationToken);
    }
}
