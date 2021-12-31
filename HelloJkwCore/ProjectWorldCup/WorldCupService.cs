using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
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
            var teams = await Get2022QualifiedTeamsAsync();
            var Qatar = teams.First(x => x.Name == "Qatar");
            var result = teams.Concat(teams).Concat(teams).Take(32).Chunk(4)
                .Zip(groupNames, (teams, groupName) => new League
                {
                    Name = groupName,
                    // Teams = teams.ToList(),
                    Teams = teams.Select(x => new Team()).ToList(),
                })
                .ToList();

            result[0].Teams[0] = Qatar;
            return result;
        }

        public Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender)
        {
            return _fifa.GetLastRankingAsync(gender);
        }

        public Task<KnockoutStageData> GetKnockoutStageDataAsync()
        {
            return CreateDummyKnockoutDataAsync();
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
    }
}
