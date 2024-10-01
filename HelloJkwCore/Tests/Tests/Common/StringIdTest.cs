using FluentAssertions;

namespace Tests.Common;

public class StringIdTest
{
    [Fact]
    public void StringIdTest1()
    {
        var id1 = new StringId("test");
        var id2 = new StringId("test");
        var id3 = new StringId("test2");

        id1.Id.Should().Be("test");
        id2.Id.Should().Be("test");
        id3.Id.Should().Be("test2");

        id1.Should().Be(id2);
        id1.Should().NotBe(id3);
        id2.Should().NotBe(id3);
    }
}