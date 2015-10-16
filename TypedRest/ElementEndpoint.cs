using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class ElementEndpoint<TEntity> : EndpointBase, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new element endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public ElementEndpoint(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new element endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public ElementEndpoint(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        public virtual async Task<TEntity> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrorsAsync(response);

            return await response.Content.ReadAsAsync<TEntity>(cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.PutAsync(Uri, entity, Serializer, cancellationToken);
            await HandleErrorsAsync(response);
        }

        public virtual async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.DeleteAsync(Uri, cancellationToken);
            await HandleErrorsAsync(response);
        }
    }
}
