using System.Threading;
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

    public class ActionCommandWithInputTest : CommandTestBase<ActionCommand<MockEntity>, IActionEndpoint<MockEntity>>
    {
        [Fact]
        public async Task TestInvoke()
        {
            var input = new MockEntity(1, "a");

            ConsoleMock.Setup(x => x.Read<MockEntity>()).Returns(input);
            EndpointMock.Setup(x => x.InvokeAsync(input, default)).Returns(Task.CompletedTask);

            await ExecuteAsync();
        }
    }
}
