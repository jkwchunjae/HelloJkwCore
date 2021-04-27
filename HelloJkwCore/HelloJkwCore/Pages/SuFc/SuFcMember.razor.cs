using Common;
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
        private List<Member> Members { get; set; } = new();
        private bool IsUserSomeoneConnected = false;

        private async Task LoadAsync()
        {
            Members = await SuFcService.GetAllMember();
            if (User != null)
            {
                IsUserSomeoneConnected = Members.Any(m => m.ConnectIdList.Contains(User.Id));
            }
        }

        protected override async Task OnPageInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task ConnectId(Member member, AppUser user)
        {
            member.ConnectIdList.Add(user.Id);
            var result = await SuFcService.SaveMember(member);
            if (result)
            {
                await LoadAsync();
            }
        }

        private async Task DisconnectId(Member member, AppUser user)
        {
            member.ConnectIdList.Remove(user.Id);
            var result = await SuFcService.SaveMember(member);
            if (result)
            {
                await LoadAsync();
            }
        }
    }
}
