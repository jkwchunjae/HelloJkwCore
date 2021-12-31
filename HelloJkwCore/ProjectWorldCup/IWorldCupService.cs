using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public interface IWorldCupService
    {
        Task<List<Team>> Get2022QualifiedTeamsAsync();
        Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender);
    }
}
