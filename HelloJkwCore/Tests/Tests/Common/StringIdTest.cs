using Shouldly;

namespace Tests.Common;

public class StringIdTest
{
    [Fact]
    public void StringIdTest1()
    {
        var id1 = new StringId("test");
        var id2 = new StringId("test");
        var id3 = new StringId("test2");

        id1.Id.ShouldBe("test");
        id2.Id.ShouldBe("test");
        id3.Id.ShouldBe("test2");

        id1.ShouldBe(id2);
        id1.ShouldNotBe(id3);
        id2.ShouldNotBe(id3);
    }
}