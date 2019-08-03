using System.Threading.Tasks;
using TypedRest.Endpoints.Rpc;
using Xunit;

namespace TypedRest.CommandLine.Commands.Rpc
{
    public class ActionCommandTest : CommandTestBase<ActionCommand, IActionEndpoint>
    {
        [Fact]
        public async Task TestInvoke()
        {
            EndpointMock.Setup(x => x.InvokeAsync(default)).Returns(Task.CompletedTask);

            await ExecuteAsync();
        }
    }
}
