using System.Threading.Tasks;
using Moq;
using Xunit;

namespace TypedRest.CommandLine
{
    public class ProducerCommandTest : CommandTestBase<ProducerCommand<MockEntity>, IProducerEndpoint<MockEntity>>
    {
        [Fact]
        public async Task TestInvoke()
        {
            var output = new MockEntity(2, "b");

            EndpointMock.Setup(x => x.InvokeAsync(default)).ReturnsAsync(output);
            ConsoleMock.Setup(x => x.Write(output));

            await ExecuteAsync();
        }
    }
}
