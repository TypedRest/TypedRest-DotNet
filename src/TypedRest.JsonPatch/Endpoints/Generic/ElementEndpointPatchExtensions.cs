using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using TypedRest.Http;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Provides extension methods for <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    public static class ElementEndpointPatchExtensions
    {
        /// <summary>
        /// Applies a JSON Patch to the entity. Sends the patch instructions to the server for processing; falls back to local processing with optimistic concurrency if that fails.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <param name="endpoint">The endpoint representing the entity.</param>
        /// <param name="patchAction">Callback for building a patch document describing the desired modifications.</param>
        /// <param name="maxRetries">The maximum number of retries to perform for optimistic concurrency before giving up.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The <typeparamref name="TEntity"/> as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
        /// <exception cref="NotSupportedException"><see cref="IEndpoint.Serializer"/> is not a <see cref="JsonMediaTypeFormatter"/>.</exception>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The number of retries performed for optimistic concurrency exceeded <paramref name="maxRetries"/>.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public static async Task<TEntity> UpdateAsync<TEntity>(this IElementEndpoint<TEntity> endpoint, Action<JsonPatchDocument<TEntity>> patchAction, int maxRetries = 3, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (!(endpoint.Serializer is JsonMediaTypeFormatter serializer))
                throw new NotSupportedException($"JSON Patch can only be used if the endpoint's serializer is a {nameof(JsonMediaTypeFormatter)}.");

            var patch = new JsonPatchDocument<TEntity>(new List<Operation<TEntity>>(), serializer.SerializerSettings.ContractResolver);
            patchAction(patch);

            var response = await endpoint.HttpClient.SendAsync(new HttpRequestMessage(HttpMethods.Patch, endpoint.Uri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(patch))
                {
                    Headers = {ContentType = new MediaTypeHeaderValue("application/json-patch+json")}
                }
            }, cancellationToken).NoContext();

            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.MethodNotAllowed)
                return await endpoint.UpdateAsync(patch.ApplyTo, maxRetries, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await endpoint.ErrorHandler.HandleAsync(response).NoContext();

            return response.Content == null
                ? default
                : await response.Content.ReadAsAsync<TEntity>(endpoint.Serializer, cancellationToken);
        }
    }
}
