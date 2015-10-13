using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a set of elements that can be retrieved partially (pagination). Uses the HTTP Range header.
    /// </summary>
    /// <typeparam name="TElement">The type of element the endpoint represents.</typeparam>
    public class PaginationEndpoint<TElement> : EndpointBase, IPaginationEndpoint<TElement>
    {
        /// <summary>
        /// Creates a new pagination endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public PaginationEndpoint(IEndpoint parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new pagination endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public PaginationEndpoint(IEndpoint parent, string relativeUri) : base(parent, relativeUri)
        {
        }

        public virtual async Task<ICollection<TElement>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrorsAsync(response);

            return await response.Content.ReadAsAsync<List<TElement>>(cancellationToken);
        }

        /// <summary>
        /// The value used for <see cref="RangeHeaderValue.Unit"/>.
        /// </summary>
        public const string RangeUnit = "elements";

        public async Task<PartialResponse<TElement>> ReadPartialAsync(RangeItemHeaderValue range,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uri)
            {
                Headers = {Range = new RangeHeaderValue {Ranges = {range}, Unit = RangeUnit}}
            };

            var response = await HttpClient.SendAsync(request, cancellationToken);
            await HandleErrorsAsync(response);

            return new PartialResponse<TElement>(
                elements: await response.Content.ReadAsAsync<List<TElement>>(cancellationToken),
                range: response.Content.Headers.ContentRange);
        }
    }
}