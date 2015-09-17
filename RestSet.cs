using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a set of <typeparamref name="TEntity"/>s providing <see cref="RestItem{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class RestSet<TEntity> : RestSetBase<TEntity, RestItem<TEntity>>
    {
        public RestSet(IRestEndpoint parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        public RestSet(IRestEndpoint parent, string relativeUri) : base(parent, relativeUri)
        {
        }

        protected override RestItem<TEntity> GetItem(Uri relativeUri)
        {
            return new RestItem<TEntity>(this, relativeUri);
        }
    }
}