using TypedRest.CommandLine.Commands.Generic;
using TypedRest.Endpoints.Generic;
using TypedRest.Endpoints.Reactive;

namespace TypedRest.CommandLine.Commands.Reactive;

/// <summary>
/// Command operating on a <see cref="IStreamingCollectionEndpoint{TEntity,TElementEndpoint}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
/// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the endpoint provides for individual <typeparamref name="TEntity"/>s.</typeparam>
/// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. Must have a public constructor with a <typeparamref name="TElementEndpoint"/> parameter.</typeparam>
public class StreamingCollectionCommand<TEntity, TElementEndpoint, TElementCommand>(IStreamingCollectionEndpoint<TEntity, TElementEndpoint> endpoint) : StreamingCollectionCommand<TEntity, IStreamingCollectionEndpoint<TEntity, TElementEndpoint>, TElementEndpoint, TElementCommand>(endpoint)
    where TEntity : class
    where TElementEndpoint : IElementEndpoint<TEntity>
    where TElementCommand : IEndpointCommand;

/// <summary>
/// Command operating on a <see cref="IStreamingCollectionEndpoint{TEntity}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public class StreamingCollectionCommand<TEntity>(IStreamingCollectionEndpoint<TEntity> endpoint) : StreamingCollectionCommand<TEntity, IElementEndpoint<TEntity>, ElementCommand<TEntity>>(endpoint)
    where TEntity : class;
