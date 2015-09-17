using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class RestItem<TEntity> : RestEndpoint, IRestItem<TEntity>
    {
        /// <summary>
        /// Creates a new item endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public RestItem(IRestEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        public virtual async Task<TEntity> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrors(response);

            return await response.Content.ReadAsAsync<TEntity>(cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.PutAsJsonAsync(Uri, item, cancellationToken);
            await HandleErrors(response);
        }

        public virtual async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.DeleteAsync(Uri, cancellationToken);
            await HandleErrors(response);
        }
    }
}
