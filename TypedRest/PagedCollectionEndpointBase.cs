using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s with pagination support using the HTTP Range header.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class PagedCollectionEndpointBase<TEntity, TElementEndpoint> : CollectionEndpointBase<TEntity, TElementEndpoint>, IPagedCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new paged collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected PagedCollectionEndpointBase(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new paged collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected PagedCollectionEndpointBase(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// The value used for <see cref="RangeHeaderValue.Unit"/>.
        /// </summary>
        public const string RangeUnit = "elements";

        public async Task<PartialResponse<TEntity>> ReadRangeAsync(RangeItemHeaderValue range,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uri)
            {
                Headers = {Range = new RangeHeaderValue {Ranges = {range}, Unit = RangeUnit}}
            };

            var response = await HttpClient.SendAsync(request, cancellationToken);
            await HandleErrorsAsync(response);

            return new PartialResponse<TEntity>(
                elements: await response.Content.ReadAsAsync<List<TEntity>>(cancellationToken),
                range: response.Content.Headers.ContentRange);
        }
    }
}