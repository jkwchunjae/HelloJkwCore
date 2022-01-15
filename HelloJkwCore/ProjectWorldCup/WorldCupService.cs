using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup;

public class WorldCupService : IWorldCupService
{
    private IFifa _fifa;

    public WorldCupService(IFifa fifa)
    {
        _fifa = fifa;
    }

    public async Task<List<Team>> Get2022QualifiedTeamsAsync()
    {
        var qualifiedTeams = await _fifa.GetQualifiedTeamsAsync();
        var rankings = await _fifa.GetLastRankingAsync(Gender.Men);

        var teams = qualifiedTeams
            .Select(team => new { Team = team, Ranking = rankings.FirstOrDefault(x => x.RankingItem.Name == team.Name) })
            .Select(x => new Team
            {
                Id = x.Team.Id,
                Name = x.Team.Name,
                Flag = x.Team.Flag?.Src,
                FifaRank = x.Ranking?.RankingItem.Rank ?? 0,
                Region = x.Ranking?.Region.Text,
            })
            .ToList();

        return teams;
    }

    public async Task<List<League>> GetGroupsAsync()
    {
        return await CreateDummyGroupsAsync();
    }

    private async Task<List<League>> CreateDummyGroupsAsync()
    {
        var groupNames = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        var qTeams = await Get2022QualifiedTeamsAsync();
        var Qatar = qTeams.First(x => x.Name == "Qatar");
        var result = Enumerable.Range(0, 32).Select(x => x % 4 + 1)
            .Chunk(4)
            .Zip(groupNames, (teamNumbers, groupName) =>
            {
                var group = new League { Name = groupName };
                var teams = teamNumbers.Select(index => new Team { Id = $"{groupName}{index}", Name = $"{groupName}{index}" }).ToList();
                if (groupName == "A")
                    teams[0] = Qatar;
                foreach (var team in teams)
                    group.AddTeam(team);
                return group;
            })
            .ToList();

        return result;
    }

    public Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender)
    {
        return _fifa.GetLastRankingAsync(gender);
    }

    public async Task<KnockoutStageData> GetKnockoutStageDataAsync()
    {
        var knockoutMatches = await _fifa.GetKnockoutStageMatchesAsync();
        return new KnockoutStageData
        {
            Round16 = knockoutMatches.Where(x => x.StageName == "Round of 16").Select(x => Match.CreateFromFifaMatchData(x)).ToList(),
            QuarterFinals = knockoutMatches.Where(x => x.StageName == "Quarter-finals").Select(x => Match.CreateFromFifaMatchData(x)).ToList(),
            SemiFinals = knockoutMatches.Where(x => x.StageName == "Semi-finals").Select(x => Match.CreateFromFifaMatchData(x)).ToList(),
            ThirdPlacePlayOff = knockoutMatches.Where(x => x.StageName == "Play-off for third place").Select(x => Match.CreateFromFifaMatchData(x)).First(),
            Final = knockoutMatches.Where(x => x.StageName == "Final").Select(x => Match.CreateFromFifaMatchData(x)).First(),
        };
    }

    private async Task<KnockoutStageData> CreateDummyKnockoutDataAsync()
    {
        var teams = await Get2022QualifiedTeamsAsync();

        teams = teams.Concat(teams).Concat(teams).ToList();
        var index = 0;
        Func<Match> createMatch = () =>
        {
            return new Match
            {
                //HomeTeam = teams[index++],
                //AwayTeam = teams[index++],
                HomeTeam = null,
                AwayTeam = null,
            };
        };
        var data = new KnockoutStageData
        {
            Final = createMatch(),
            ThirdPlacePlayOff = createMatch(),
            SemiFinals = new List<Match> { createMatch(), createMatch() },
            QuarterFinals = new List<Match> { createMatch(), createMatch(), createMatch(), createMatch() },
            Round16 = new List<Match> { createMatch(), createMatch(), createMatch(), createMatch(), createMatch(), createMatch(), createMatch(), createMatch() },
        };

        return data;
    }

    public async Task<List<Match>> GetGroupStageMatchesAsync()
    {
        var matches = await _fifa.GetGroupStageMatchesAsync();

        return matches.Select(x => Match.CreateFromFifaMatchData(x)).ToList();
    }

    public async Task<List<Match>> GetKnockOutStageMatchesAsync()
    {
        var matches = await _fifa.GetKnockoutStageMatchesAsync();

        return matches.Select(x => Match.CreateFromFifaMatchData(x)).ToList();
    }
}