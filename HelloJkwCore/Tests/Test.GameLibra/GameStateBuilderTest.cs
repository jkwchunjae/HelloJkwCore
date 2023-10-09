using GameLibra;

namespace Test.GameLibra;

public class GameStateBuilderTest
{
    [Fact]
    public void DevilsPlan����_��_����Ǿ���()
    {
        var gameState = new GameStateBuilder()
            .UseDevilsPlanRule()
            .Build();

        var cubeValues = gameState.CubeInfo
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToArray();

        Assert.Equal(5, cubeValues.Distinct().Count()); // ��ġ�� �ʾƾ� ��
        Assert.Equal(10, cubeValues[2]); // 3��° ���� 10�̾�� ��
        Assert.True(gameState.Rule.CubeCount == 5);
        Assert.True(gameState.Rule.PlayerCount == 7);
        Assert.True(gameState.Rule.CubeMinValue == 1);
        Assert.True(gameState.Rule.CubeMaxValue == 20);
        Assert.True(gameState.Rule.CubePerPlayer == 2);
    }
}