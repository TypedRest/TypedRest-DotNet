using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TypedRest.CommandLine
{
    public class ActionCommandTest : CommandTestBase<ActionCommand, IActionEndpoint>
    {
        [Fact]
        public async Task TestTrigger()
        {
            EndpointMock.Setup(x => x.TriggerAsync(CancellationToken.None)).Returns(Task.CompletedTask);

            await ExecuteAsync();
        }
    }

    public class ActionCommandWithInputTest : CommandTestBase<ActionCommand<MockEntity>, IActionEndpoint<MockEntity>>
    {
        [Fact]
        public async Task TestTrigger()
        {
            var input = new MockEntity(1, "a");

            ConsoleMock.Setup(x => x.Read<MockEntity>()).Returns(input);
            EndpointMock.Setup(x => x.TriggerAsync(input, CancellationToken.None)).Returns(Task.CompletedTask);

            await ExecuteAsync();
        }
    }
}
