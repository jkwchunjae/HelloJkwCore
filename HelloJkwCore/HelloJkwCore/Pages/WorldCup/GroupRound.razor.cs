using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectWorldCup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.WorldCup
{
    public partial class GroupRound : JkwPageBase
    {
        [Inject]
        private IWorldCupService Service { get; set; }

        private List<Group> Groups { get; set; } = new();

        protected override async Task OnPageInitializedAsync()
        {
            Groups = await Service.GetGroupsAsync();
        }
    }
}
