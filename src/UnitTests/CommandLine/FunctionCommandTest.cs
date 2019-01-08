using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace TypedRest.CommandLine
{
    public class FunctionCommandTest : CommandTestBase<FunctionCommand<MockEntity>, IFunctionEndpoint<MockEntity>>
    {
        [Fact]
        public async Task TestTrigger()
        {
            var output = new MockEntity(2, "b");

            EndpointMock.Setup(x => x.TriggerAsync(default)).ReturnsAsync(output);
            ConsoleMock.Setup(x => x.Write(output));

            await ExecuteAsync();
        }
    }

    public class FunctionCommandWithInputTest : CommandTestBase<FunctionCommand<MockEntity, MockEntity>, IFunctionEndpoint<MockEntity, MockEntity>>
    {
        [Fact]
        public async Task TestTrigger()
        {
            var input = new MockEntity(1, "a");
            var output = new MockEntity(2, "b");

            ConsoleMock.Setup(x => x.Read<MockEntity>()).Returns(input);
            EndpointMock.Setup(x => x.TriggerAsync(input, default)).ReturnsAsync(output);
            ConsoleMock.Setup(x => x.Write(output));

            await ExecuteAsync();
        }
    }
}
