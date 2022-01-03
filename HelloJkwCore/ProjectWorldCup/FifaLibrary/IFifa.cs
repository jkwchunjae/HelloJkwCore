using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public interface IFifa
    {
        Task<List<QualifiedTeam>> GetQualifiedTeamsAsync();
        Task<List<RankingTeamData>> GetLastRankingAsync(Gender gender);
        Task<List<FifaMatchData>> GetGroupStageMatchesAsync();
        Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync();
    }
}
