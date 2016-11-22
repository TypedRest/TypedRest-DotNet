using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class ElementEndpoint<TEntity> : ETagEndpointBase, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new element endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public ElementEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new element endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public ElementEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
        }

        public virtual async Task<TEntity> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await GetContentAsync(cancellationToken);
            return await content.ReadAsAsync<TEntity>(new[] {Serializer}, cancellationToken).NoContext();
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await HandleResponseAsync(HttpClient.HeadAsync(Uri, CancellationToken.None)).NoContext();
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            return true;
        }

        public bool? SetAllowed => IsMethodAllowed(HttpMethod.Put);

        public virtual async Task<TEntity> SetAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var content = new ObjectContent<TEntity>(entity, Serializer);
            var response = await PutContentAsync(content, cancellationToken);
            return response.Content == null
                ? default(TEntity)
                : await response.Content.ReadAsAsync<TEntity>(new[] {Serializer}, cancellationToken);
        }

        [Obsolete("Use SetAsync() instead")]
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) => SetAsync(entity, cancellationToken);

        public bool? ModifyAllowed => IsMethodAllowed(HttpClientExtensions.Patch);

        public async Task<TEntity> ModifyAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var response = await HandleResponseAsync(HttpClient.PatchAsync(Uri, entity, Serializer, cancellationToken));
            return response.Content == null
                ? default(TEntity)
                : await response.Content.ReadAsAsync<TEntity>(new[] {Serializer}, cancellationToken);
        }

        public bool? DeleteAllowed => IsMethodAllowed(HttpMethod.Delete);

        public virtual async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await DeleteContentAsync(cancellationToken);
        }
    }
}