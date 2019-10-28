using System;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for a collection of <typeparamref name="TEntity"/>s addressable as <see cref="ElementEndpoint{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
    public class CollectionEndpoint<TEntity> : CollectionEndpoint<TEntity, IElementEndpoint<TEntity>>, ICollectionEndpoint<TEntity>
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
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public CollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        protected override IElementEndpoint<TEntity> BuildElementEndpoint(Uri relativeUri)
            => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}
