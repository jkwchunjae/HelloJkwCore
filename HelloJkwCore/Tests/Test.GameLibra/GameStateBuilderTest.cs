using GameLibra;

namespace Test.GameLibra;

public class GameStateBuilderTest
{
    [Fact]
    public void DevilsPlanRule_should_be_work()
    {
        var gameState = new GameStateBuilder()
            .UseDevilsPlanRule()
            .Build();

        var cubeValues = gameState.CubeInfo
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToArray();

        Assert.Equal(5, cubeValues.Distinct().Count()); // ��ġ�� �ʾƾ� ��
        Assert.Contains(cubeValues, x => x == 10); // 10�� �־�� ��
        Assert.True(gameState.Rule.CubeCount == 5);
        Assert.True(gameState.Rule.PlayerCount == 7);
        Assert.True(gameState.Rule.CubeMinValue == 1);
        Assert.True(gameState.Rule.CubeMaxValue == 20);
        Assert.True(gameState.Rule.CubePerPlayer == 2);
    }
}