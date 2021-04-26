using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcMember : JkwPageBase
    {
        [Inject]
        private ISuFcService SuFcService { get; set; }

        private List<Member> Players { get; set; } = new();

        private async Task LoadAsync()
        {
            Players = await SuFcService.GetMembers();
        }

        protected override async Task OnPageInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task DeleteMember(Member member)
        {
            await SuFcService.DeleteMember(member);
            await LoadAsync();
            StateHasChanged();
        }
    }
}
