using HelloJkwCore.Shared;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcActiveSchedule : JkwPageBase
    {
        [Inject]
        private ISuFcService SuFcService { get; set; }

        [Parameter]
        public ScheduleData Schedule { get; set; }
        [Parameter]
        public EventCallback<ScheduleData> ScheduleChanged { get; set; }

        private MemberName MyName;
        private ScheduleMemberStatus MyStatus = ScheduleMemberStatus.None;
        private MemberName SelectedName;
        private List<Member> Members = new();
        private List<Member> NotConnectedMembers = new();
        private List<TeamResult> TeamResultList = new();

        protected override async Task OnPageInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            TeamResultList = await SuFcService.GetAllTeamResult();
            Members = await SuFcService.GetAllMember();
            NotConnectedMembers = Members.Where(x => x.ConnectIdList.Empty()).ToList();
            await LoadMyInfoAsync();
        }

        private async Task LoadMyInfoAsync()
        {
            if (!IsAuthenticated)
                return;

            var me = await FindMe();
            if (me != null)
            {
                MyName = me.Name;
                var myVote = Schedule.Members.FirstOrDefault(x => x.Name == MyName);
                if (myVote != null)
                {
                    MyStatus = myVote.Status;
                }
            }
        }

        private async Task<Member> FindMe()
        {
            var members = await SuFcService.GetAllMember();
            var found = members.Find(x => x.ConnectIdList.Contains(User.Id));

            if (found == null)
            {
                Navi.NavigateTo("/sufc/member");
            }

            return found;
        }

        private async Task VoteAsync(ScheduleMemberStatus memberStatus)
        {
            var name = IsAuthenticated ? MyName : SelectedName;
            var (success, result) = await SuFcService.Vote(Schedule, name, memberStatus);
            if (success)
            {
                await ScheduleChanged.InvokeAsync(result);
                Schedule = result;
                await LoadAsync();
                StateHasChanged();
            }
        }
    }
}
