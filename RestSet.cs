using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a set of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class RestSet<TEntity> : RestEndpoint, IRestSet<TEntity>
    {
        /// <summary>
        /// Creates a new item set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        public RestSet(IRestEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri.EnsureTrailingSlash())
        {
        }

        /// <summary>
        /// Creates a new item set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        public RestSet(IRestEndpoint parent, string relativeUri)
            : this(parent, new Uri(relativeUri, UriKind.Relative))
        {
        }

        public IRestItem<TEntity> this[object id] => GetItem(new Uri(id.ToString(), UriKind.Relative));

        public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrors(response);

            return await response.Content.ReadAsAsync<List<TEntity>>(cancellationToken);
        }

        public virtual async Task<IRestItem<TEntity>> CreateAsync(TEntity item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.PostAsJsonAsync(Uri, item, cancellationToken);
            await HandleErrors(response);

            return (response.StatusCode == HttpStatusCode.Created)
                ? GetItem(response.Headers.Location)
                : null;
        }

        /// <summary>
        /// Instantiates a child <typeparamref name="TEntity"/> for an item in this set.
        /// </summary>
        protected virtual IRestItem<TEntity> GetItem(Uri realtiveUri)
        {
            return new RestItem<TEntity>(this, realtiveUri);
        }
    }
}