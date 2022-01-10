using Common;
using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectWorldCup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.WorldCup
{
    public partial class GroupStage : JkwPageBase
    {
        [Inject]
        private IWorldCupService Service { get; set; }

        private List<League> Groups { get; set; } = new();

        protected override async Task OnPageInitializedAsync()
        {
            Groups = await Service.GetGroupsAsync();
        }
    }
}
