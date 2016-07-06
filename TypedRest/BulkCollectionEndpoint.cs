using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <see cref="IElementEndpoint{TEntity}"/>s with bulk create and replace support.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class BulkCollectionEndpoint<TEntity> : BulkCollectionEndpointBase<TEntity, IElementEndpoint<TEntity>>, IBulkCollectionEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new bulk collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        public BulkCollectionEndpoint(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new bulk collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="parent"/> URI if missing.</param>
        public BulkCollectionEndpoint(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        public override IElementEndpoint<TEntity> this[Uri relativeUri] => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}