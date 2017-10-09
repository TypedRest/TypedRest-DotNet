using System;

namespace TypedRest
{
    [Obsolete("Use ICollectionEndpoint instead")]
    public class PagedCollectionEndpoint<TEntity, TElementEndpoint> : CollectionEndpoint<TEntity, TElementEndpoint>, IPagedCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        public PagedCollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        public PagedCollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}
    }

    [Obsolete("Use ICollectionEndpoint instead")]
    public class PagedCollectionEndpoint<TEntity> : PagedCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>, IPagedCollectionEndpoint<TEntity>
    {
        public PagedCollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        public PagedCollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        protected override IElementEndpoint<TEntity> BuildElementEndpoint(Uri relativeUri) => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}