using System;
using TypedRest.Endpoints.Generic;

namespace TypedRest.Endpoints.Reactive
{
    /// <summary>
    /// Endpoint for a collection of <typeparamref name="TEntity"/>s observable as an append-only stream using long-polling.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class StreamingCollectionEndpoint<TEntity> : StreamingCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>, IStreamingCollectionEndpoint<TEntity>
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
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public StreamingCollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        protected override IElementEndpoint<TEntity> BuildElementEndpoint(Uri relativeUri)
            => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}
