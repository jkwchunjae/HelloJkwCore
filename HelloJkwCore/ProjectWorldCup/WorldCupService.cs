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

        public async Task<List<Group>> GetGroupsAsync()
        {
            var dummy = await CreateDummyGroupsAsync();
            return dummy;
        }

        private async Task<List<Group>> CreateDummyGroupsAsync()
        {
            var groupNames = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
            var teams = await Get2022QualifiedTeamsAsync();
            return teams.Concat(teams).Concat(teams).Take(32).Chunk(4)
                .Zip(groupNames, (teams, groupName) => new Group
                {
                    Name = groupName,
                    Teams = teams.ToList(),
                })
                .ToList();
        }

        public Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender)
        {
            return _fifa.GetLastRankingAsync(gender);
        }
    }
}
