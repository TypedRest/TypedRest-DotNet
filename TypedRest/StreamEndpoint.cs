using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a stream of <typeparamref name="TEntity"/>s as <see cref="IElementEndpoint{TEntity}"/>s. Uses the HTTP Range header.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class StreamEndpoint<TEntity> : StreamEndpointBase<TEntity, IElementEndpoint<TEntity>>, IStreamEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new stream endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Missing trailing slash will be appended automatically.</param>
        public StreamEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new stream endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public StreamEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
        }

        public override IElementEndpoint<TEntity> this[Uri relativeUri] => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}