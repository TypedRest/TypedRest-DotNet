using Microsoft.AspNetCore.JsonPatch;

namespace TypedRest.Endpoints.Generic;

/// <summary>
/// Endpoint for an individual resource.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public interface IElementEndpoint<TEntity> : IElementEndpoint
    where TEntity : class
{
    /// <summary>
    /// A cached copy of the entity as received from the server.
    /// </summary>
    TEntity? Response { get; }

    /// <summary>
    /// Returns the entitiy.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<TEntity> ReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows whether the server has indicated that <see cref="SetAsync"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? SetAllowed { get; }

    /// <summary>
    /// Sets/replaces the entity.
    /// </summary>
    /// <param name="entity">The new entities.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>The entity as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
    /// <exception cref="InvalidOperationException">The entity has changed since it was last retrieved with <see cref="ReadAsync"/>. Your changes were rejected to prevent a lost update.</exception>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<TEntity?> SetAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows whether the server has indicated that <see cref="MergeAsync"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? MergeAllowed { get; }

    /// <summary>
    /// Modifies an existing entity by merging changes on the server-side.
    /// </summary>
    /// <param name="entity">The entity data to merge with the existing one.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>The modified entity as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
    /// <exception cref="InvalidOperationException">The entity has changed since it was last retrieved with <see cref="ReadAsync"/>. Your changes were rejected to prevent a lost update.</exception>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<TEntity?> MergeAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the current state of the entity, applies a change to it and stores the result. Applies optimistic concurrency using automatic retries.
    /// </summary>
    /// <param name="updateAction">A callback that takes the current state of the entity and applies the desired modifications.</param>
    /// <param name="maxRetries">The maximum number of retries to perform for optimistic concurrency before giving up.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>The entity as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
    /// <exception cref="InvalidOperationException">The number of retries performed for optimistic concurrency exceeded <paramref name="maxRetries"/>.</exception>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<TEntity?> UpdateAsync(Action<TEntity> updateAction, int maxRetries = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a JSON Patch to the entity. Sends the patch instructions to the server for processing; falls back to local processing with optimistic concurrency if that fails.
    /// </summary>
    /// <param name="patchAction">Callback for building a patch document describing the desired modifications.</param>
    /// <param name="maxRetries">The maximum number of retries to perform for optimistic concurrency before giving up.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>The entity as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
    /// <exception cref="InvalidOperationException">The number of retries performed for optimistic concurrency exceeded <paramref name="maxRetries"/>.</exception>
    /// <exception cref="NotSupportedException"><see cref="IEndpoint.Serializer"/> is not a <see cref="JsonMediaTypeFormatter"/>.</exception>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<TEntity?> UpdateAsync(Action<JsonPatchDocument<TEntity>> patchAction, int maxRetries = 3, CancellationToken cancellationToken = default);
}
