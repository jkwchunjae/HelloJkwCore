using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectWorldCup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.WorldCup
{
    public partial class QualifiedTeamsPage : JkwPageBase
    {
        [Inject]
        private IWorldCupService Service { get; set; }

        private List<QualifiedTeam> QualifiedTeams = new();
        private List<RankingTeamData> Rankings = new();
        private List<(TeamTag Region, List<QualifiedTeam> Teams)> Regions => QualifiedTeams
            .GroupBy(x => x.Ranking.Region.Id)
            .Select(x => new { Region = x.First().Ranking.Region, Teams = x.OrderBy(x => x.Rank).ToList() })
            .OrderByDescending(x => x.Teams.Count)
            .Select(x => (x.Region, x.Teams))
            .ToList();

        protected override async Task OnPageInitializedAsync()
        {
            QualifiedTeams = await Service.Get2022QualifiedTeamsAsync();
            Rankings = await Service.GetLastRankingTeamDataAsync(Gender.Men);
        }
    }
}
