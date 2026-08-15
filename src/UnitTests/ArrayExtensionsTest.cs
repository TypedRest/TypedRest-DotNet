namespace TypedRest;

public class ArrayExtensionsTest
{
    [Fact]
    public void TestIndexOfPattern()
    {
        var array = new[] {0, 0, 0, 1, 1, 0, 0, 0};
        array.IndexOfPattern([0, 1], startIndex: 1, count: 5).Should().Be(2);
        array.IndexOfPattern([0, 1], startIndex: 3, count: 3).Should().Be(-1);
        array.IndexOfPattern([0, 1], startIndex: 1, count: 1).Should().Be(-1);

        // Pattern ending exactly at the end of the searched range
        array.IndexOfPattern([0, 1], startIndex: 1, count: 3).Should().Be(2);
        array.IndexOfPattern([1, 0], startIndex: 0, count: 6).Should().Be(4);
    }
}
