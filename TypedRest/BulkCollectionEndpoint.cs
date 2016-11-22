using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    [Obsolete("Use CollectionEndpoint instead")]
    public class BulkCollectionEndpoint<TEntity, TElementEndpoint> : CollectionEndpoint<TEntity, TElementEndpoint>, IBulkCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        public BulkCollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
        }

        public BulkCollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
        }

        [Obsolete("Use CreateAllAsync() instead")]
        public Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken)) =>
            CreateAllAsync(entities, cancellationToken);
    }

    [Obsolete("Use CollectionEndpoint instead")]
    public class BulkCollectionEndpoint<TEntity> : BulkCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>, IBulkCollectionEndpoint<TEntity>
    {
        public BulkCollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
        }

        public BulkCollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
        }

        protected override IElementEndpoint<TEntity> BuildElementEndpoint(Uri relativeUri) => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}