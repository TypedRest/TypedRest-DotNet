using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;

namespace TypedRest.CommandLine.Commands.Generic;

/// <summary>
/// Command operating on a <see cref="IIndexerEndpoint{TElementEndpoint}"/>.
/// </summary>
/// <typeparam name="TEndpoint">The specific type of <see cref="IIndexerEndpoint{TElementEndpoint}"/> to operate on.</typeparam>
/// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual elements.</typeparam>
/// <typeparam name="TElementCommand">The specific type of <see cref="IEndpointCommand"/> is used to handle <typeparamref name="TElementEndpoint"/>s. This must be a non-abstract class with a constructor that takes a <typeparamref name="TElementEndpoint"/>, unless you override <see cref="GetElementCommand"/>.</typeparam>
public abstract class IndexerCommand<TEndpoint, TElementEndpoint, TElementCommand> : EndpointCommand<TEndpoint>
    where TEndpoint : IIndexerEndpoint<TElementEndpoint>
    where TElementEndpoint : IEndpoint
    where TElementCommand : IEndpointCommand
{
    /// <summary>
    /// Creates a new REST Indexer command.
    /// </summary>
    /// <param name="endpoint">The endpoint this command operates on.</param>
    protected IndexerCommand(TEndpoint endpoint)
        : base(endpoint)
    {}

    protected override IEndpointCommand GetSubCommand(string name) => GetElementCommand(Endpoint[name]);

    /// <summary>
    /// Gets an <see cref="IEndpointCommand"/> for the given <paramref name="elementEndpoint"/>.
    /// </summary>
    protected TElementCommand GetElementCommand(TElementEndpoint elementEndpoint)
        => (TElementCommand)Activator.CreateInstance(typeof(TElementCommand), elementEndpoint)!;
}
