using ProjectWorldCup.FifaLibrary;

namespace Tests.WorldCup;

public class FinalBettingPickTest
{
    FifaMatchTeam MakeFakeTeam(string teamId)
    {
        return new()
        {
            IdCountry = teamId,
            TeamName = new List<FifaIdName> { new() { Description = teamId } },
        };
    }
    FifaMatchData MakeFakeMatch(string stageId, string matchId, string home, string away)
    {
        return new FifaMatchData
        {
            IdStage = stageId,
            IdMatch = matchId,
            Home = MakeFakeTeam(home),
            Away = MakeFakeTeam(away),
        };
    }

    List<KnMatch> MakeMatches()
    {
        return new List<KnMatch>
        {
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.1", "Team1", "Team5")),
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.2", "Team2", "Team6")),
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.3", "Team3", "Team7")),
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.4", "Team4", "Team8")),
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round4StageId, "Match.4.1", string.Empty, string.Empty)),
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round4StageId, "Match.4.2", string.Empty, string.Empty)),
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.ThirdStageId, "Match.2.1", string.Empty, string.Empty)),
            KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.FinalStageId, "Match.1.1", string.Empty, string.Empty)),
        };
    }
    List<(string StageId, List<KnMatch> Matches)> MakeStageMatches()
    {
        return new List<(string StageId, List<KnMatch> Matches)>
        {
            (Fifa.Round8StageId, new List<KnMatch>()
            {
                KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.1", "Team1", "Team5")),
                KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.2", "Team2", "Team6")),
                KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.3", "Team3", "Team7")),
                KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round8StageId, "Match.8.4", "Team4", "Team8")),
            }),
            //(Fifa.Round4StageId, new List<KnMatch>()
            //{
            //    KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round4StageId, "Match.4.1", string.Empty, string.Empty)),
            //    KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.Round4StageId, "Match.4.2", string.Empty, string.Empty)),
            //}),
            //(Fifa.ThirdStageId, new List<KnMatch>()
            //{
            //    KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.ThirdStageId, "Match.2.1", string.Empty, string.Empty)),
            //}),
            //(Fifa.FinalStageId, new List<KnMatch>()
            //{
            //    KnMatch.CreateFromFifaMatchData(MakeFakeMatch(Fifa.ThirdStageId, "Match.1.1", string.Empty, string.Empty)),
            //}),
        };
    }

    [Fact]
    public void FinalBettingPickTeam_8강1경기()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        var stageMatches = MakeStageMatches();
        var matches = MakeMatches();
        var match = stageMatches[0].Matches[0];
        stageMatches = service.PickTeam(match.StageId, match.MatchId, match.HomeTeam, stageMatches, matches);

        Assert.Equal(match.HomeTeam.Id, stageMatches[1].Matches[0].AwayTeam.Id);
    }

    [Fact]
    public void FinalBettingPickTeam_8강2경기()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        var stageMatches = MakeStageMatches();
        var matches = MakeMatches();
        var match = stageMatches[0].Matches[1];
        stageMatches = service.PickTeam(match.StageId, match.MatchId, match.HomeTeam, stageMatches, matches);

        Assert.Equal(match.HomeTeam.Id, stageMatches[1].Matches[0].HomeTeam.Id);
    }

    [Fact]
    public void FinalBettingPickTeam_8강3경기()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        var stageMatches = MakeStageMatches();
        var matches = MakeMatches();
        var match = stageMatches[0].Matches[2];
        stageMatches = service.PickTeam(match.StageId, match.MatchId, match.HomeTeam, stageMatches, matches);

        Assert.Equal(match.HomeTeam.Id, stageMatches[1].Matches[1].AwayTeam.Id);
    }

    [Fact]
    public void FinalBettingPickTeam_8강4경기()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        var stageMatches = MakeStageMatches();
        var matches = MakeMatches();
        var match = stageMatches[0].Matches[3];
        stageMatches = service.PickTeam(match.StageId, match.MatchId, match.HomeTeam, stageMatches, matches);

        Assert.Equal(match.HomeTeam.Id, stageMatches[1].Matches[1].HomeTeam.Id);
    }

    [Fact]
    public void FinalBettingPickTeam_4강1경기()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        var stageMatches = MakeStageMatches();
        var matches = MakeMatches();
        foreach (var match in stageMatches[0].Matches)
        {
            // 8강경기
            stageMatches = service.PickTeam(match.StageId, match.MatchId, match.HomeTeam, stageMatches, matches);
        }

        var semiFinalMatch1 = stageMatches[1].Matches[0];
        stageMatches = service.PickTeam(semiFinalMatch1.StageId, semiFinalMatch1.MatchId, semiFinalMatch1.HomeTeam, stageMatches, matches);

        var thirdMatch = stageMatches[2].Matches[0];
        var finalMatch = stageMatches[3].Matches[0];
        Assert.Equal(thirdMatch.HomeTeam.Id, semiFinalMatch1.AwayTeam.Id); // loser
        Assert.Equal(finalMatch.HomeTeam.Id, semiFinalMatch1.HomeTeam.Id); // winner
    }

    [Fact]
    public void FinalBettingPickTeam_4강2경기()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        var stageMatches = MakeStageMatches();
        var matches = MakeMatches();
        foreach (var match in stageMatches[0].Matches)
        {
            // 8강경기
            stageMatches = service.PickTeam(match.StageId, match.MatchId, match.HomeTeam, stageMatches, matches);
        }

        var semiFinalMatch2 = stageMatches[1].Matches[1];
        stageMatches = service.PickTeam(semiFinalMatch2.StageId, semiFinalMatch2.MatchId, semiFinalMatch2.HomeTeam, stageMatches, matches);

        var thirdMatch = stageMatches[2].Matches[0];
        var finalMatch = stageMatches[3].Matches[0];
        Assert.Equal(thirdMatch.AwayTeam.Id, semiFinalMatch2.AwayTeam.Id); // loser
        Assert.Equal(finalMatch.AwayTeam.Id, semiFinalMatch2.HomeTeam.Id); // winner
    }

    [Fact]
    public void FinalBettingPickTeam_RandomPick()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        var stageMatches = MakeStageMatches();
        var matches = MakeMatches();
        (stageMatches, var pickTeams) = service.PickRandom(stageMatches, matches);

        var leftTeams = stageMatches[0].Matches.Take(2).SelectMany(match => match.Teams);
        var rightTeams = stageMatches[0].Matches.Skip(2).SelectMany(match => match.Teams);

        var winner = pickTeams[0];
        var second = pickTeams[1];
        var third = pickTeams[2];
        var fourth = pickTeams[3];

        if (leftTeams.Contains(winner))
        {
            // 우승팀이 왼쪽에서 나온 경우
            Assert.Contains(second, rightTeams);

            if (stageMatches[0].Matches[0].Teams.Contains(winner))
            {
                Assert.True(stageMatches[0].Matches[1].Teams.Contains(third)
                    || stageMatches[0].Matches[1].Teams.Contains(fourth));
            }
            else if (stageMatches[0].Matches[1].Teams.Contains(winner))
            {
                Assert.True(stageMatches[0].Matches[0].Teams.Contains(third)
                    || stageMatches[0].Matches[0].Teams.Contains(fourth));
            }

            if (stageMatches[0].Matches[2].Teams.Contains(second))
            {
                Assert.True(stageMatches[0].Matches[3].Teams.Contains(third)
                    || stageMatches[0].Matches[3].Teams.Contains(fourth));
            }
            else if (stageMatches[0].Matches[3].Teams.Contains(second))
            {
                Assert.True(stageMatches[0].Matches[2].Teams.Contains(third)
                    || stageMatches[0].Matches[2].Teams.Contains(fourth));
            }
        }
        else
        {
            // 우승팀이 오른쪽에서 나온 경우
            Assert.Contains(winner, rightTeams);
            Assert.Contains(second, leftTeams);

            if (stageMatches[0].Matches[2].Teams.Contains(winner))
            {
                Assert.True(stageMatches[0].Matches[3].Teams.Contains(third)
                    || stageMatches[0].Matches[3].Teams.Contains(fourth));
            }
            else if (stageMatches[0].Matches[3].Teams.Contains(winner))
            {
                Assert.True(stageMatches[0].Matches[2].Teams.Contains(third)
                    || stageMatches[0].Matches[2].Teams.Contains(fourth));
            }

            if (stageMatches[0].Matches[0].Teams.Contains(second))
            {
                Assert.True(stageMatches[0].Matches[1].Teams.Contains(third)
                    || stageMatches[0].Matches[1].Teams.Contains(fourth));
            }
            else if (stageMatches[0].Matches[1].Teams.Contains(second))
            {
                Assert.True(stageMatches[0].Matches[0].Teams.Contains(third)
                    || stageMatches[0].Matches[0].Teams.Contains(fourth));
            }
        }
    }

    [Fact]
    public void FinalBettingPickTeam_RandomPick1000()
    {
        IBettingFinalService service = new BettingFinalService(null, null, null, null, null);

        Dictionary<string, int> winCount = new();
        for (var i = 0; i < 1000; i++)
        {
            var stageMatches = MakeStageMatches();
            var matches = MakeMatches();
            (_, var pickTeams) = service.PickRandom(stageMatches, matches);
            if (winCount.ContainsKey(pickTeams[0].Id))
            {
                winCount[pickTeams[0].Id]++;
            }
            else
            {
                winCount[pickTeams[0].Id] = 1;
            }
        }

        Assert.Equal(8, winCount.Count);
        foreach (var winData in winCount)
        {
            Assert.InRange(winData.Value, 80, 170);
        }
    }

}
