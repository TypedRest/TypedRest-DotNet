using System;

namespace TypedRest
{
    [Obsolete("Use ICollectionEndpoint instead")]
    public interface IPagedCollectionEndpoint<TEntity, TElementEndpoint> :
        ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {

    }

    [Obsolete("Use ICollectionEndpoint instead")]
    public interface IPagedCollectionEndpoint<TEntity> : IPagedCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>,
        ICollectionEndpoint<TEntity>
    {
    }
}