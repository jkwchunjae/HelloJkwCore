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
        private ScheduleData schedule => Schedule;

        private MemberName MyName;
        private bool IsVoted = false;
        private ScheduleMemberStatus MyStatus = ScheduleMemberStatus.None;
        private MemberName SelectedName;
        private List<Member> Members;
        private List<Member> NotConnectedMembers;

        private List<(ScheduleMemberStatus MemberStatus, string Text)> ScheduleTypes = new List<(ScheduleMemberStatus MemberStatus, string Text)>
        {
            (ScheduleMemberStatus.Yes, "참석"),
            (ScheduleMemberStatus.No, "불참"),
            (ScheduleMemberStatus.NotYet, "미정"),
            (ScheduleMemberStatus.None, "투표안함"),
        };

        protected override async Task OnPageInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
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
                    IsVoted = myVote?.Status != ScheduleMemberStatus.None;
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

        private async Task<List<ScheduleData>> GetActiveSchedulesAsync()
        {
            var schedules = await SuFcService.GetAllSchedule();
            var activeList = schedules.Where(x => x.Status == ScheduleStatus.Active).ToList();

            return activeList;
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
