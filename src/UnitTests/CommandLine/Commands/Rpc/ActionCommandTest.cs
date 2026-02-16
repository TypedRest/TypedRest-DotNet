using TypedRest.Endpoints.Rpc;

namespace TypedRest.CommandLine.Commands.Rpc;

public class ActionCommandTest : CommandTestBase<ActionCommand, IActionEndpoint>
{
    [Fact]
    public async Task TestInvoke()
    {
        EndpointMock.Setup(x => x.InvokeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await ExecuteAsync();
    }
}
