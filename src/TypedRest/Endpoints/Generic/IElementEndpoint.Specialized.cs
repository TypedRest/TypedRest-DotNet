namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for an individual resource. Usually you will want to use the typed-variant of this interface: <see cref="IElementEndpoint{TEntity}"/>
    /// </summary>
    public interface IElementEndpoint : IEndpoint
    {
        /// <summary>
        /// Determines whether the element currently exists.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows whether the server has indicated that <see cref="DeleteAsync"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
        bool? DeleteAllowed { get; }

        /// <summary>
        /// Deletes the element.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidOperationException">The entity has changed since it was last retrieved with <see cref="IElementEndpoint{T}.ReadAsync"/>. Your delete call was rejected to prevent a lost update.</exception>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task DeleteAsync(CancellationToken cancellationToken = default);
    }
}
