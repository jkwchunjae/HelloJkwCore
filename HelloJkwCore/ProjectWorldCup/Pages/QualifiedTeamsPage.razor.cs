using Common;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWorldCup.Pages;

public partial class QualifiedTeamsPage : JkwPageBase
{
    [Inject]
    private IWorldCupService Service { get; set; }

    private List<Team> QualifiedTeams = new();
    private List<(string Region, List<Team> Teams)> TeamByRegion => QualifiedTeams
        .GroupBy(x => x.Region)
        .Select(x => new { Region = x.Key, Teams = x.OrderBy(e => e.FifaRank).ToList() })
        .OrderByDescending(x => x.Teams.Count)
        .Select(x => (x.Region, x.Teams))
        .ToList();

    protected override async Task OnPageInitializedAsync()
    {
        QualifiedTeams = await Service.Get2022QualifiedTeamsAsync();
    }
}