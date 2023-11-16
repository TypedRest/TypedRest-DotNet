using TypedRest.Endpoints.Generic;

namespace TypedRest.CommandLine.Commands.Generic;

/// <summary>
/// Command operating on a <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
/// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the endpoint provides for individual <typeparamref name="TEntity"/>s.</typeparam>
/// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. Must have a public constructor with a <typeparamref name="TElementEndpoint"/> parameter.</typeparam>
public class CollectionCommand<TEntity, TElementEndpoint, TElementCommand>(ICollectionEndpoint<TEntity, TElementEndpoint> endpoint) : CollectionCommand<TEntity, ICollectionEndpoint<TEntity, TElementEndpoint>, TElementEndpoint, TElementCommand>(endpoint)
    where TEntity : class
    where TElementEndpoint : IElementEndpoint<TEntity>
    where TElementCommand : IEndpointCommand;

/// <summary>
/// Command operating on a <see cref="ICollectionEndpoint{TEntity}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public class CollectionCommand<TEntity>(ICollectionEndpoint<TEntity> endpoint) : CollectionCommand<TEntity, IElementEndpoint<TEntity>, ElementCommand<TEntity>>(endpoint)
    where TEntity : class;
