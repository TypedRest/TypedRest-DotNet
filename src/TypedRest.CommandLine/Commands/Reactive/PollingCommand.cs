using TypedRest.CommandLine.Commands.Generic;
using TypedRest.CommandLine.IO;
using TypedRest.Endpoints.Reactive;

namespace TypedRest.CommandLine.Commands.Reactive;

/// <summary>
/// Command operating on an <see cref="IPollingEndpoint{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
public class PollingCommand<TEntity> : ElementCommand<TEntity>
    where TEntity : class
{
    protected new readonly IPollingEndpoint<TEntity> Endpoint;

    /// <summary>
    /// Creates a new REST polling command.
    /// </summary>
    /// <param name="endpoint">The endpoint this command operates on.</param>
    public PollingCommand(IPollingEndpoint<TEntity> endpoint)
        : base(endpoint)
    {
        Endpoint = endpoint;
    }

    protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
    {
        if (args.Count >= 1 && args[0].ToLowerInvariant() == "poll")
        {
            await OutputEntitiesAsync(Endpoint.GetObservable(), cancellationToken);
            return;
        }

        await base.ExecuteInnerAsync(args, cancellationToken);
    }

    /// <summary>
    /// Outputs a stream of <typeparamref name="TEntity"/>s to the user, e.g., via <see cref="object.ToString"/> on the console.
    /// </summary>
    protected virtual async Task OutputEntitiesAsync(IObservable<TEntity> observable, CancellationToken cancellationToken = default)
    {
        var printer = new StreamPrinter<TEntity>(Console);
        await printer.PrintAsync(observable, cancellationToken);
    }
}
