using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <see cref="ElementEndpoint{TEntity}"/>s with bulk create and replace support.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class BulkCollectionEndpoint<TEntity> : BulkCollectionEndpointBase<TEntity, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new bulk collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        public BulkCollectionEndpoint(IEndpoint parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new bulk collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        public BulkCollectionEndpoint(IEndpoint parent, string relativeUri) : base(parent, relativeUri)
        {
        }

        public override ElementEndpoint<TEntity> this[Uri relativeUri] => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}