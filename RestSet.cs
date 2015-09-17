using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a set of items.
    /// </summary>
    /// <typeparam name="TItem">The type of item the endpoint represents.</typeparam>
    public class RestSet<TItem> : RestEndpoint
    {
        /// <summary>
        /// Creates a new item set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        public RestSet(RestEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri.EnsureTrailingSlash())
        {
        }

        /// <summary>
        /// Creates a new item set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        public RestSet(RestEndpoint parent, string relativeUri)
            : this(parent, new Uri(relativeUri, UriKind.Relative))
        {
        }

        /// <summary>
        /// Returns a <see cref="RestItem{TItem}"/> for a specific item of this set. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="id">The ID used to identify the item within the set.</param>
        public virtual RestItem<TItem> this[object id] => new RestItem<TItem>(this, relativeUri: id.ToString());

        /// <summary>
        /// Returns all <typeparamref name="TItem"/>s.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public virtual async Task<IEnumerable<TItem>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrors(response);

            return await response.Content.ReadAsAsync<List<TItem>>(cancellationToken);
        }

        /// <summary>
        /// Creates a new <typeparamref name="TItem"/>.
        /// </summary>
        /// <param name="item">The new <typeparamref name="TItem"/>.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The newly created <see cref="RestItem{TItem}"/>; may be <see langword="null"/> if the server deferred creating the resource.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public virtual async Task<RestItem<TItem>> CreateAsync(TItem item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.PostAsJsonAsync(Uri, item, cancellationToken);
            await HandleErrors(response);

            return (response.StatusCode == HttpStatusCode.Created)
                ? new RestItem<TItem>(this, response.Headers.Location)
                : null;
        }
    }
}