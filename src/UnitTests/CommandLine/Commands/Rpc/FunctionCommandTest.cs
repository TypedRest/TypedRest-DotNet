using System.Threading.Tasks;
using Moq;
using TypedRest.Endpoints.Rpc;
using Xunit;

namespace TypedRest.CommandLine.Commands.Rpc
{
    public class FunctionCommandTest : CommandTestBase<FunctionCommand<MockEntity, MockEntity>, IFunctionEndpoint<MockEntity, MockEntity>>
    {
        [Fact]
        public async Task TestInvoke()
        {
            var input = new MockEntity(1, "a");
            var output = new MockEntity(2, "b");

            ConsoleMock.Setup(x => x.Read<MockEntity>()).Returns(input);
            EndpointMock.Setup(x => x.InvokeAsync(input, default)).ReturnsAsync(output);
            ConsoleMock.Setup(x => x.Write(output));

            await ExecuteAsync();
        }
    }
}
