using TypedRest.Endpoints.Generic;

namespace TypedRest.CommandLine.Commands.Generic;

public class ElementCommandTest : CommandTestBase<ElementCommand<MockEntity>, IElementEndpoint<MockEntity>>
{
    [Fact]
    public async Task TestRead()
    {
        var entity = new MockEntity(5, "test");

        ConsoleMock.Setup(x => x.Write(entity));
        EndpointMock.Setup(x => x.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        await ExecuteAsync();
    }

    [Fact]
    public async Task TestSet()
    {
        var entity = new MockEntity(5, "test");

        ConsoleMock.Setup(x => x.Read<MockEntity>()).Returns(entity);
        EndpointMock.Setup(x => x.SetAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        ConsoleMock.Setup(x => x.Write(entity));

        await ExecuteAsync("set");
    }

    [Fact]
    public async Task TestMerge()
    {
        var entity = new MockEntity(5, "test");

        ConsoleMock.Setup(x => x.Read<MockEntity>()).Returns(entity);
        EndpointMock.Setup(x => x.MergeAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        ConsoleMock.Setup(x => x.Write(entity));

        await ExecuteAsync("merge");
    }

    [Fact]
    public async Task TestDelete()
    {
        EndpointMock.Setup(x => x.DeleteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await ExecuteAsync("delete");
    }
}
