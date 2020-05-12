using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.Http;

#if NETSTANDARD
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
#endif

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
                await HandleAsync(() => HttpClient.HeadAsync(Uri, cancellationToken)).NoContext();
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

            var response = await HandleAsync(() => HttpClient.PatchAsync(Uri, entity, Serializer, cancellationToken));
            return response.Content == null
                ? default
                : await response.Content.ReadAsAsync<TEntity>(Serializer, cancellationToken);
        }

        public bool? DeleteAllowed => IsMethodAllowed(HttpMethod.Delete);

        public virtual async Task DeleteAsync(CancellationToken cancellationToken = default)
            => await DeleteContentAsync(cancellationToken);

        public Task<TEntity?> UpdateAsync(Action<TEntity> updateAction, int maxRetries = 3, CancellationToken cancellationToken = default)
            => TracedAsync(async _ =>
            {
                int retryCounter = 0;
                while (true)
                {
                    var entity = await ReadAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    updateAction(entity);
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        return await SetAsync(entity, cancellationToken);
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (retryCounter++ >= maxRetries) throw;
                        await ex.HttpRetryDelayAsync(cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            });

#if NETSTANDARD
        public async Task<TEntity?> UpdateAsync(Action<JsonPatchDocument<TEntity>> patchAction, int maxRetries = 3, CancellationToken cancellationToken = default)
        {
            if (!(Serializer is JsonMediaTypeFormatter serializer))
                throw new NotSupportedException($"JSON Patch can only be used if the endpoint's serializer is a {nameof(JsonMediaTypeFormatter)}.");

            var patch = new JsonPatchDocument<TEntity>(new List<Operation<TEntity>>(), serializer.SerializerSettings.ContractResolver);
            patchAction(patch);

            var response = await TracedAsync(_ => HttpClient.SendAsync(new HttpRequestMessage(HttpMethods.Patch, Uri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(patch))
                {
                    Headers = {ContentType = new MediaTypeHeaderValue("application/json-patch+json")}
                }
            }, cancellationToken)).NoContext();

            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.MethodNotAllowed)
                return await UpdateAsync(patch.ApplyTo, maxRetries, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await ErrorHandler.HandleAsync(response).NoContext();

            return response.Content == null
                ? default
                : await response.Content.ReadAsAsync<TEntity>(Serializer, cancellationToken);
        }
#endif
    }
}
