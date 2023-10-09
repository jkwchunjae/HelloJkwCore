using GameLibra;

namespace Test.GameLibra;

public class GameStateBuilderTest
{
    [Fact]
    public void DevilsPlan룰이_잘_적용되야함()
    {
        var gameState = new GameStateBuilder()
            .UseDevilsPlanRule()
            .Build();

        var cubeValues = gameState.CubeInfo
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToArray();

        Assert.Equal(5, cubeValues.Distinct().Count()); // 겹치지 않아야 함
        Assert.Equal(10, cubeValues[2]); // 3번째 값은 10이어야 함
        Assert.True(gameState.Rule.CubeCount == 5);
        Assert.True(gameState.Rule.PlayerCount == 7);
        Assert.True(gameState.Rule.CubeMinValue == 1);
        Assert.True(gameState.Rule.CubeMaxValue == 20);
        Assert.True(gameState.Rule.CubePerPlayer == 2);
    }
}