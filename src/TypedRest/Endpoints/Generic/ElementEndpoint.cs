using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.Http;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for an individual resource.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class ElementEndpoint<TEntity> : ETagEndpointBase, IElementEndpoint<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Creates a new element endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public ElementEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new element endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public ElementEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public TEntity? Response
            => ResponseCache?.GetContent().ReadAsAsync<TEntity>(Serializer).Result;

        public virtual async Task<TEntity> ReadAsync(CancellationToken cancellationToken = default)
        {
            var content = await GetContentAsync(cancellationToken);
            return await content.ReadAsAsync<TEntity>(Serializer, cancellationToken).NoContext();
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await HandleResponseAsync(HttpClient.HeadAsync(Uri, cancellationToken)).NoContext();
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            return true;
        }

        public bool? SetAllowed => IsMethodAllowed(HttpMethod.Put);

        public virtual async Task<TEntity?> SetAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var content = new ObjectContent<TEntity>(entity, Serializer);
            var response = await PutContentAsync(content, cancellationToken);
            return response.Content == null
                ? default
                : await response.Content.ReadAsAsync<TEntity>(Serializer, cancellationToken);
        }

        public bool? MergeAllowed => IsMethodAllowed(HttpMethods.Patch);

        public async Task<TEntity?> MergeAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var response = await HandleResponseAsync(HttpClient.PatchAsync(Uri, entity, Serializer, cancellationToken));
            return response.Content == null
                ? default
                : await response.Content.ReadAsAsync<TEntity>(Serializer, cancellationToken);
        }

        public bool? DeleteAllowed => IsMethodAllowed(HttpMethod.Delete);

        public virtual async Task DeleteAsync(CancellationToken cancellationToken = default)
            => await DeleteContentAsync(cancellationToken);
    }
}
