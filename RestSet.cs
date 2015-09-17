using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a set of <typeparamref name="TEntity"/>s as <see cref="RestElement{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class RestSet<TEntity> : RestSetBase<TEntity, RestElement<TEntity>>
    {
        public RestSet(IRestEndpoint parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        public RestSet(IRestEndpoint parent, string relativeUri) : base(parent, relativeUri)
        {
        }

        protected override RestElement<TEntity> GetElement(Uri relativeUri)
        {
            return new RestElement<TEntity>(this, relativeUri);
        }
    }
}