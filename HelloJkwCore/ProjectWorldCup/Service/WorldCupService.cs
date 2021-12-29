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

        public async Task<List<QualifiedTeam>> Get2022QualifiedTeamsAsync()
        {
            var teams = await _fifa.GetQualifiedTeamsAsync();
            var rankings = await _fifa.GetLastRankingAsync(Gender.Men);

            foreach (var team in teams)
            {
                var r = rankings.FirstOrDefault(x => x.RankingItem.Name == team.Name);

                if (r != null)
                {
                    team.Ranking = r;
                }
            }

            return teams;
        }

        public Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender)
        {
            return _fifa.GetLastRankingAsync(gender);
        }
    }
}
