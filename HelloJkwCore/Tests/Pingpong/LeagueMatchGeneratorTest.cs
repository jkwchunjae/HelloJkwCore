using ProjectPingpong;
using ProjectPingpong.Utils;

namespace Tests.Pingpong;

public class LeagueMatchGeneratorTest
{
    [Fact]
    public void SingleMatchSetTest_6_0()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(6, 0);

        Assert.Equal(3, playerIndex.Count);
        Assert.Equal((0, 5), playerIndex[0]);
        Assert.Equal((1, 4), playerIndex[1]);
        Assert.Equal((2, 3), playerIndex[2]);
    }
    [Fact]
    public void SingleMatchSetTest_6_1()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(6, 1);

        Assert.Equal(3, playerIndex.Count);
        Assert.Equal((0, 1), playerIndex[0]);
        Assert.Equal((2, 5), playerIndex[1]);
        Assert.Equal((3, 4), playerIndex[2]);
    }
    [Fact]
    public void SingleMatchSetTest_6_2()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(6, 2);

        Assert.Equal(3, playerIndex.Count);
        Assert.Equal((0, 2), playerIndex[0]);
        Assert.Equal((3, 1), playerIndex[1]);
        Assert.Equal((4, 5), playerIndex[2]);
    }
    [Fact]
    public void SingleMatchSetTest_6_3()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(6, 3);

        Assert.Equal(3, playerIndex.Count);
        Assert.Equal((0, 3), playerIndex[0]);
        Assert.Equal((4, 2), playerIndex[1]);
        Assert.Equal((5, 1), playerIndex[2]);
    }
    [Fact]
    public void SingleMatchSetTest_6_4()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(6, 4);

        Assert.Equal(3, playerIndex.Count);
        Assert.Equal((0, 4), playerIndex[0]);
        Assert.Equal((5, 3), playerIndex[1]);
        Assert.Equal((1, 2), playerIndex[2]);
    }
    [Fact]
    public void SingleMatchSetTest_5_0()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(5, 0);

        Assert.Equal(2, playerIndex.Count);
        Assert.Equal((1, 4), playerIndex[0]);
        Assert.Equal((2, 3), playerIndex[1]);
    }
    [Fact]
    public void SingleMatchSetTest_5_1()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(5, 1);

        Assert.Equal(2, playerIndex.Count);
        Assert.Equal((0, 1), playerIndex[0]);
        Assert.Equal((3, 4), playerIndex[1]);
    }
    [Fact]
    public void SingleMatchSetTest_5_2()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(5, 2);

        Assert.Equal(2, playerIndex.Count);
        Assert.Equal((0, 2), playerIndex[0]);
        Assert.Equal((3, 1), playerIndex[1]);
    }
    [Fact]
    public void SingleMatchSetTest_5_3()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(5, 3);

        Assert.Equal(2, playerIndex.Count);
        Assert.Equal((0, 3), playerIndex[0]);
        Assert.Equal((4, 2), playerIndex[1]);
    }
    [Fact]
    public void SingleMatchSetTest_5_4()
    {
        var generator = new LeagueMatchGenerator();
        var playerIndex = generator.GetSingleMatchSet(5, 4);

        Assert.Equal(2, playerIndex.Count);
        Assert.Equal((0, 4), playerIndex[0]);
        Assert.Equal((1, 2), playerIndex[1]);
    }

    [Fact]
    public void CreateLeagueMatchTest_4()
    {
        var player1 = new Player { Name = new PlayerName("P1") };
        var player2 = new Player { Name = new PlayerName("P2") };
        var player3 = new Player { Name = new PlayerName("P3") };
        var player4 = new Player { Name = new PlayerName("P4") };
        var players = new List<Player> { player1, player2, player3, player4 };

        var generator = new LeagueMatchGenerator();
        var matches = generator.CreateLeagueMatch(players);

        Assert.Equal(6, matches.Count);
        Assert.Equal((player1, player4), matches[0]);
        Assert.Equal((player2, player3), matches[1]);
        Assert.Equal((player1, player2), matches[2]);
        Assert.Equal((player3, player4), matches[3]);
        Assert.Equal((player1, player3), matches[4]);
        Assert.Equal((player4, player2), matches[5]);
    }

    [Fact]
    public void CreateLeagueMatchTest_5()
    {
        var player1 = new Player { Name = new PlayerName("P1") };
        var player2 = new Player { Name = new PlayerName("P2") };
        var player3 = new Player { Name = new PlayerName("P3") };
        var player4 = new Player { Name = new PlayerName("P4") };
        var player5 = new Player { Name = new PlayerName("P5") };
        var players = new List<Player> { player1, player2, player3, player4, player5 };

        var generator = new LeagueMatchGenerator();
        var matches = generator.CreateLeagueMatch(players);

        Assert.Equal(10, matches.Count);
        Assert.Equal((player2, player5), matches[0]);
        Assert.Equal((player3, player4), matches[1]);
        Assert.Equal((player1, player2), matches[2]);
        Assert.Equal((player4, player5), matches[3]);
        Assert.Equal((player1, player3), matches[4]);
        Assert.Equal((player4, player2), matches[5]);
        Assert.Equal((player1, player4), matches[6]);
        Assert.Equal((player5, player3), matches[7]);
        Assert.Equal((player1, player5), matches[8]);
        Assert.Equal((player2, player3), matches[9]);
    }
}
