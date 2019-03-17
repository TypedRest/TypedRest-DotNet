using System.Threading.Tasks;
using Xunit;

namespace TypedRest.CommandLine
{
    public class ConsumerCommandTest : CommandTestBase<ConsumerCommand<MockEntity>, IConsumerEndpoint<MockEntity>>
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
