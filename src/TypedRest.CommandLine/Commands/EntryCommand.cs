using System.Collections;
using TypedRest.Endpoints;

namespace TypedRest.CommandLine.Commands;

/// <summary>
/// Command providing an entry point to a hierarchy of named <see cref="IEndpointCommand"/>s.
/// </summary>
/// <param name="endpoint">The endpoint this command operates on.</param>
/// <typeparamref name="TEndpoint">The specific type of <see cref="IEndpoint"/> the command starts with.</typeparamref>
public class EntryCommand<TEndpoint>(TEndpoint endpoint) : EndpointCommand<TEndpoint>(endpoint), IEnumerable<KeyValuePair<string, Func<TEndpoint, IEndpointCommand>>>
    where TEndpoint : IEndpoint
{
    private readonly Dictionary<string, Func<TEndpoint, IEndpointCommand>> _commandProviders =
        new(StringComparer.OrdinalIgnoreCase);

    #region Enumerable
    public IEnumerator<KeyValuePair<string, Func<TEndpoint, IEndpointCommand>>> GetEnumerator() => _commandProviders.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _commandProviders.GetEnumerator();
    #endregion

    public void Add(string name, Func<TEndpoint, IEndpointCommand> commandProvider)
        => _commandProviders.Add(name, commandProvider);

    protected override IEndpointCommand? GetSubCommand(string name)
        => _commandProviders.TryGetValue(name, out var commandProvider) ? commandProvider(Endpoint) : null;

    protected override Task ExecuteInnerAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
    {
        Console.WriteError("Known commands:" + Environment.NewLine + string.Join(Environment.NewLine, _commandProviders.Keys));

        return base.ExecuteInnerAsync(args, cancellationToken);
    }
}
