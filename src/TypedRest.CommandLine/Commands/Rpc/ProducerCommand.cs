using TypedRest.Endpoints.Rpc;

namespace TypedRest.CommandLine.Commands.Rpc;

/// <summary>
/// Command operating on an <see cref="IProducerEndpoint{TResult}"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparam name="TResult">The type of entity the <see cref="IProducerEndpoint{TResult}"/> returns as a result.</typeparam>
public class ProducerCommand<TResult>(IProducerEndpoint<TResult> endpoint) : EndpointCommand<IProducerEndpoint<TResult>>(endpoint)
{
    protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
        => OutputEntity(await Endpoint.InvokeAsync(cancellationToken));

    /// <summary>
    /// Outputs a <typeparamref name="TResult"/> to the user via the console.
    /// </summary>
    protected virtual void OutputEntity(TResult entity)
        => Console.Write(entity);
}
