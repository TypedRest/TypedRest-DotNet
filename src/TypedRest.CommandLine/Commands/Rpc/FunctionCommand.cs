using TypedRest.Endpoints.Rpc;

namespace TypedRest.CommandLine.Commands.Rpc;

/// <summary>
/// Command operating on an <see cref="IFunctionEndpoint{TEntity,TResult}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TEntity">The type of entity the <see cref="IFunctionEndpoint{TEntity,TResult}"/> takes as input.</typeparam>
/// <typeparam name="TResult">The type of entity the <see cref="IFunctionEndpoint{TEntity,TResult}"/> returns as a result.</typeparam>
public class FunctionCommand<TEntity, TResult>(IFunctionEndpoint<TEntity, TResult> endpoint) : EndpointCommand<IFunctionEndpoint<TEntity, TResult>>(endpoint)
{
    protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
        => OutputEntity(await Endpoint.InvokeAsync(InputEntity(args.Skip(1).ToList()), cancellationToken));

    /// <summary>
    /// Acquires a <typeparamref name="TEntity"/> from the user, e.g. by parsing the <paramref name="args"/> or via JSON on the console.
    /// </summary>
    protected virtual TEntity InputEntity(IReadOnlyList<string> args)
        => Input<TEntity>(args);

    /// <summary>
    /// Outputs a <typeparamref name="TResult"/> to the user via the console.
    /// </summary>
    protected virtual void OutputEntity(TResult entity)
        => Console.Write(entity);
}
