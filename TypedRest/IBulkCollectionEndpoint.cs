using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    [Obsolete("Use ICollectionEndpoint instead")]
    public interface IBulkCollectionEndpoint<TEntity, TElementEndpoint> : ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : IEndpoint
    {
        [Obsolete("Use CreateAllAsync() instead")]
        Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
    }

    [Obsolete("Use ICollectionEndpoint instead")]
    public interface IBulkCollectionEndpoint<TEntity> : IBulkCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>,
        ICollectionEndpoint<TEntity>
    {}
}
