using GameLibra;
using GameLibra.Service;

namespace Test.GameLibra;

public class LibraAssistorTest
{
    [Fact]
    public void TestAssistor_ABC_1_5()
    {
        var state = new LibraGameState
        {
            CubeInfo = new List<Cube>
            {
                new Cube { Name = "a" },
                new Cube { Name = "b" },
                new Cube { Name = "c" },
            },
            Rule = new LibraGameRule
            {
                CubeMinValue = 1,
                CubeMaxValue = 5,
            },
            HintInfos = new HintInfo[]
            {
                new HintInfo { CubeName = "a", Value = 4, Order = 1 },
            },
        };

        var libraAssistor = new LibraAssistor();
        libraAssistor.LessThan(new [] { "b" }, new [] { "c" });
        libraAssistor.SetValue("a", 4);
        libraAssistor.Init(state);

        Assert.True(libraAssistor.Sets.All(set => set["a"] == 4));
        Assert.True(libraAssistor.Sets.All(set => set["a"] > set["b"]));
        Assert.True(libraAssistor.Sets.All(set => set["a"] > set["c"]));
        Assert.True(libraAssistor.Sets.All(set => set["b"] < set["c"]));
        Assert.Equal(3, libraAssistor.Sets.Count());
    }
    [Fact]
    public void TestAssistor_ABC_1_4_second()
    {
        var state = new LibraGameState
        {
            CubeInfo = new List<Cube>
            {
                new Cube { Name = "a" },
                new Cube { Name = "b" },
                new Cube { Name = "c" },
            },
            Rule = new LibraGameRule
            {
                CubeMinValue = 1,
                CubeMaxValue = 4,
            },
            HintInfos = new HintInfo[]
            {
            },
        };

        var libraAssistor = new LibraAssistor();
        libraAssistor.LessThan(new [] { "b" }, new [] { "c" });
        libraAssistor.GreaterThan(new [] { "c" }, new [] { "a" });
        libraAssistor.Init(state);

        // a b c
        // 1 2 3
        // 1 2 4
        // 1 3 4
        // 2 1 3
        // 2 1 4
        // 2 3 4
        // 3 1 4
        // 3 2 4

        Assert.True(libraAssistor.Sets.All(set => set["a"] < set["c"]));
        Assert.True(libraAssistor.Sets.All(set => set["b"] < set["c"]));
        Assert.Equal(8, libraAssistor.Sets.Count());
        Assert.Equal(4, libraAssistor.Sets.Count(set => set["a"] < set["b"]));
        Assert.Equal(4, libraAssistor.Sets.Count(set => set["a"] > set["b"]));
    }
}