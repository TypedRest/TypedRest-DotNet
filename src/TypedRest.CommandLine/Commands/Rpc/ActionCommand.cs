using TypedRest.Endpoints.Rpc;

namespace TypedRest.CommandLine.Commands.Rpc;

/// <summary>
/// Command operating on an <see cref="IActionEndpoint"/>.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
public class ActionCommand(IActionEndpoint endpoint) : EndpointCommand<IActionEndpoint>(endpoint)
{
    protected override async Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
        => await Endpoint.InvokeAsync(cancellationToken);
}
