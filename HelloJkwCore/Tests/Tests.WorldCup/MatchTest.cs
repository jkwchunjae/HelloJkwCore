﻿namespace Tests.WorldCup;

public class MatchTest
{
    private Team Team1 { get; set; }
    private Team Team2 { get; set; }

    public MatchTest()
    {
        Team1 = new Team { Id = "A", Name = "A", FifaTeamId = "A" };
        Team2 = new Team { Id = "B", Name = "B", FifaTeamId = "B" };
    }

    [Fact]
    public void Match_Drawn_Test()
    {
        var match = new Match<Team>
        {
            HomeTeam = Team1,
            AwayTeam = Team2,
            HomeScore = 0,
            AwayScore = 0,
            HomePenaltyScore = 0,
            AwayPenaltyScore = 0,
        };

        Assert.True(match.IsDraw);
    }

    [Fact]
    public void Match_Winner_Test1()
    {
        var match = new Match<Team>
        {
            HomeTeam = Team1,
            AwayTeam = Team2,
            HomeScore = 1,
            AwayScore = 0,
            HomePenaltyScore = 0,
            AwayPenaltyScore = 0,
            WinnerId = Team1.FifaTeamId,
        };

        Assert.Equal(Team1.Id, match.Winner.Team.Id);
        Assert.Equal(Team2.Id, match.Looser.Team.Id);
    }

    [Fact]
    public void Match_Winner_Test2()
    {
        var match = new Match<Team>
        {
            HomeTeam = Team1,
            AwayTeam = Team2,
            HomeScore = 0,
            AwayScore = 1,
            HomePenaltyScore = 0,
            AwayPenaltyScore = 0,
            WinnerId = Team2.FifaTeamId,
        };

        Assert.Equal(Team2.Id, match.Winner.Team.Id);
        Assert.Equal(Team1.Id, match.Looser.Team.Id);
    }

    [Fact]
    public void Match_Winner_Test3()
    {
        var match = new Match<Team>
        {
            HomeTeam = Team1,
            AwayTeam = Team2,
            HomeScore = 0,
            AwayScore = 0,
            HomePenaltyScore = 1,
            AwayPenaltyScore = 0,
            WinnerId = Team1.FifaTeamId,
        };

        Assert.Equal(Team1.Id, match.Winner.Team.Id);
        Assert.Equal(Team2.Id, match.Looser.Team.Id);
    }

    [Fact]
    public void Match_Winner_Test4()
    {
        var match = new Match<Team>
        {
            HomeTeam = Team1,
            AwayTeam = Team2,
            HomeScore = 0,
            AwayScore = 0,
            HomePenaltyScore = 0,
            AwayPenaltyScore = 1,
            WinnerId = Team2.FifaTeamId,
        };

        Assert.Equal(Team2.Id, match.Winner.Team.Id);
        Assert.Equal(Team1.Id, match.Looser.Team.Id);
    }
}