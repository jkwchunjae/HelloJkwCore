using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectWorldCup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.WorldCup
{
    public partial class QualifiedTeamsPage : JkwPageBase
    {
        [Inject]
        private IWorldCupService Service { get; set; }

        private List<QualifiedTeam> QualifiedTeams = new();

        protected override async Task OnPageInitializedAsync()
        {
            QualifiedTeams = await Service.Get2022QualifiedTeamsAsync();
        }
    }
}
