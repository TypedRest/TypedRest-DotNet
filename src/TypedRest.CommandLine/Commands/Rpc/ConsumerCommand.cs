using TypedRest.Endpoints.Rpc;

namespace TypedRest.CommandLine.Commands.Rpc;

/// <summary>
/// Command operating on an <see cref="IConsumerEndpoint{TEntity}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TEntity">The type of entity the <see cref="IConsumerEndpoint{TEntity}"/> takes as input.</typeparam>
public class ConsumerCommand<TEntity>(IConsumerEndpoint<TEntity> endpoint) : EndpointCommand<IConsumerEndpoint<TEntity>>(endpoint)
{
    protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
        => await Endpoint.InvokeAsync(InputEntity(args.Skip(1).ToList()), cancellationToken);

    /// <summary>
    /// Acquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the console.
    /// </summary>
    protected virtual TEntity InputEntity(IReadOnlyList<string> args)
        => Input<TEntity>(args);
}
