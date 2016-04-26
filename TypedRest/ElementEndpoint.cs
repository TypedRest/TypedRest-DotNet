using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public ElementEndpoint(IEndpoint parent, Uri relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        /// <summary>
        /// Creates a new element endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public ElementEndpoint(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        /// <summary>
        /// The last entity tag returned by the server. Used for caching and to avoid lost updates.
        /// </summary>
        private EntityTagHeaderValue _etag;

        /// <summary>
        /// The last entity returned by the server. Used for caching.
        /// </summary>
        private TEntity _cachedResponse;

        public virtual async Task<TEntity> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uri);
            if (_etag != null) request.Headers.IfNoneMatch.Add(_etag);

            var response = await HttpClient.SendAsync(request, cancellationToken).NoContext();
            if (response.StatusCode == HttpStatusCode.NotModified && _cachedResponse != null)
                return _cachedResponse;

            await HandleResponseAsync(Task.FromResult(response)).NoContext();
            _etag = response.Headers.ETag;
            return _cachedResponse = await response.Content.ReadAsAsync<TEntity>(cancellationToken).NoContext();
        }

        public bool? UpdateAllowed => IsVerbAllowed(HttpMethod.Put.Method);

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var request = new HttpRequestMessage(HttpMethod.Put, Uri)
            {
                Content = new ObjectContent<TEntity>(entity, Serializer)
            };
            if (_etag != null) request.Headers.IfMatch.Add(_etag);
            var response = await HandleResponseAsync(HttpClient.SendAsync(request, cancellationToken)).NoContext();

            return response.Content == null
                ? default(TEntity)
                : await response.Content.ReadAsAsync<TEntity>(cancellationToken);
        }

        public bool? DeleteAllowed => IsVerbAllowed(HttpMethod.Delete.Method);

        public virtual Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HandleResponseAsync(HttpClient.DeleteAsync(Uri, cancellationToken));
        }
    }
}