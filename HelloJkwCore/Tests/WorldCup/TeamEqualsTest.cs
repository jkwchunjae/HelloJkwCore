namespace Tests.WorldCup;

public class TeamEqualsTest
{
    [Fact]
    public void TeamId만_같으면_같은팀이다()
    {
        var team1 = new Team
        {
            Id = "T1",
            Name = "Name1",
        };

        var team2 = new Team
        {
            Id = "T1",
            Name = "Name2",
        };

        Assert.Equal(team1, team2);
    }

    [Fact]
    public void TeamId가_다르면_다른팀이다()
    {
        var team1 = new Team
        {
            Id = "T1",
            Name = "Name1",
        };

        var team2 = new Team
        {
            Id = "T2",
            Name = "Name2",
        };

        Assert.NotEqual(team1, team2);
    }
}
