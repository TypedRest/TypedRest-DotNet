using TypedRest.CommandLine.IO;
using TypedRest.Endpoints;

namespace TypedRest.CommandLine.Commands;

public abstract class CommandTestBase<TCommand, TEndpoint> : IDisposable
    where TCommand : EndpointCommand<TEndpoint>
    where TEndpoint : class, IEndpoint
{
    protected readonly Mock<TEndpoint> EndpointMock = new();
    protected readonly Mock<IConsole> ConsoleMock = new();

    private readonly TCommand _command;

    protected CommandTestBase()
    {
        _command = (TCommand)Activator.CreateInstance(typeof(TCommand), EndpointMock.Object)!;
        _command.Console = ConsoleMock.Object;
    }

    protected Task ExecuteAsync(params string[] args)
        => _command.ExecuteAsync(args);

    public void Dispose()
    {
        EndpointMock.VerifyAll();
        ConsoleMock.VerifyAll();
    }
}