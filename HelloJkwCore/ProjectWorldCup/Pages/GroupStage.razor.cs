using Common;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectWorldCup.Pages
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
