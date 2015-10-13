using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a set of elements that can be retrieved partially (pagination).
    /// </summary>
    /// <typeparam name="TElement">The type of element the endpoint represents.</typeparam>
    public interface IPaginationEndpoint<TElement> : IEndpoint
    {
        /// <summary>
        /// Returns all <typeparamref name="TElement"/>s currently in the set.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<ICollection<TElement>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns all <typeparamref name="TElement"/>s within a specific range of the set.
        /// </summary>
        /// <param name="range">The range of elements to retrieve.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>A subset of the <typeparamref name="TElement"/>s and the range they come from. May not exactly match the request <paramref name="range"/>.</returns>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="IndexOutOfRangeException"><see cref="HttpStatusCode.RequestedRangeNotSatisfiable"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<PartialResponse<TElement>> ReadPartialAsync(RangeItemHeaderValue range,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}