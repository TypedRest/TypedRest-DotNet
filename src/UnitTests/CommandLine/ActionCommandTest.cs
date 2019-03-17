using System.Threading.Tasks;
using Xunit;

namespace TypedRest.CommandLine
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
