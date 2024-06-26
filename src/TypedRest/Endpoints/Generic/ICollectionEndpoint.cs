namespace TypedRest.Endpoints.Generic;

/// <summary>
/// Endpoint for a collection of <typeparamref name="TEntity"/>s addressable as <typeparamref name="TElementEndpoint"/>s.
/// </summary>
/// <remarks>Use the more constrained <see cref="ICollectionEndpoint{TEntity}"/> when possible.</remarks>
/// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
/// <typeparam name="TElementEndpoint">The type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
public interface ICollectionEndpoint<TEntity, out TElementEndpoint> : IIndexerEndpoint<TElementEndpoint>
    where TEntity : class
    where TElementEndpoint : IElementEndpoint<TEntity>
{
    /// <summary>
    /// Returns an <see cref="ElementEndpoint{TEntity}"/> for a specific child element.
    /// </summary>
    /// <param name="entity">An existing entity to extract the ID from.</param>
    TElementEndpoint this[TEntity entity] { get; }

    /// <summary>
    /// Indicates whether the server has specified <see cref="ReadAllAsync"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? ReadAllAllowed { get; }

    /// <summary>
    /// Returns all entities in the collection.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<List<TEntity>> ReadAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified <see cref="ReadRangeAsync"/> is allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns>An indicator whether the method is allowed. If no request has been sent yet.</returns>
    bool? ReadRangeAllowed { get; }

    /// <summary>
    /// Returns all entities within a specific range of the collection.
    /// </summary>
    /// <param name="range">The range of entities to retrieve.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>A subset of the entities and the range they come from. May not exactly match the request <paramref name="range"/>.</returns>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="InvalidOperationException">The requested range is not satisfiable.</exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<PartialResponse<TEntity>> ReadRangeAsync(RangeItemHeaderValue range, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified <see cref="CreateAsync(TEntity,CancellationToken)"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? CreateAllowed { get; }

    /// <summary>
    /// Adds a entity as a new element to the collection.
    /// </summary>
    /// <param name="entity">The new entity.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>An endpoint for the newly created entity; <c>null</c> if the server returned neither a "Location" header nor an entity with an ID in the response body.</returns>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    ITask<TElementEndpoint?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified <see cref="CreateAllAllowed"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns>An indicator whether the verb is allowed. If no request has been sent yet or the server did not specify allowed verbs <c>null</c> is returned.</returns>
    bool? CreateAllAllowed { get; }

    /// <summary>
    /// Adds (or updates) multiple entities as elements in the collection.
    /// </summary>
    /// <param name="entities">The entities to create or modify.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    /// <remarks>Uses a link with the relation type <c>bulk</c> to determine the URI to POST to. Defaults to the relative URI <c>bulk</c>.</remarks>
    Task CreateAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified <see cref="SetAllAllowed"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns>An indicator whether the verb is allowed. If no request has been sent yet or the server did not specify allowed verbs <c>null</c> is returned.</returns>
    bool? SetAllAllowed { get; }

    /// <summary>
    /// Replaces the entire content of the collection with new entities.
    /// </summary>
    /// <param name="entities">The new set of entities the collection shall contain.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="InvalidOperationException">The entities have changed since they were last retrieved with <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.ReadAllAsync"/>. Your changes were rejected to prevent a lost update.</exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task SetAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
