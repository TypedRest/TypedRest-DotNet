using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints that represents a set of <typeparamref name="TEntity"/>s as <typeparamref name="TElement"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElement">The specific type of <see cref="IRestElement{TEntity}"/>s to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class RestSetBase<TEntity, TElement> : RestEndpointBase, IRestSet<TEntity, TElement>
        where TElement : class, IRestElement<TEntity>
    {
        /// <summary>
        /// Creates a new element set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected RestSetBase(IRestEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri.EnsureTrailingSlash())
        {
        }

        /// <summary>
        /// Creates a new element set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected RestSetBase(IRestEndpoint parent, string relativeUri)
            // Use this instead of base to ensure trailing slash gets appended for REST Set URIs
            : this(parent, new Uri(relativeUri, UriKind.Relative))
        {
        }

        public TElement this[object id]
        {
            get { return GetElement(new Uri(id.ToString(), UriKind.Relative)); }
        }

        public virtual async Task<ICollection<TEntity>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrorsAsync(response);

            return await response.Content.ReadAsAsync<List<TEntity>>(cancellationToken);
        }

        public virtual async Task<TElement> CreateAsync(TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.PostAsJsonAsync(Uri, entity, cancellationToken);
            await HandleErrorsAsync(response);

            return (response.StatusCode == HttpStatusCode.Created)
                ? GetElement(response.Headers.Location)
                : null;
        }

        /// <summary>
        /// Instantiates a <typeparamref name="TElement"/> for an element in this set.
        /// </summary>
        protected abstract TElement GetElement(Uri relativeUri);
    }
}