using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcSchedule : JkwPageBase
    {
        [Inject]
        private ISuFcService SuFcService { get; set; }

        private List<ScheduleData> ScheduleList = new();
        private List<TeamResult> TeamResultList = new();

        private List<(ScheduleMemberStatus MemberStatus, string Text)> ScheduleTypes = new List<(ScheduleMemberStatus MemberStatus, string Text)>
        {
            (ScheduleMemberStatus.Yes, "참석"),
            (ScheduleMemberStatus.No, "불참"),
            (ScheduleMemberStatus.NotYet, "미정"),
            (ScheduleMemberStatus.None, "투표안함"),
        };

        protected override async Task OnPageInitializedAsync()
        {
            ScheduleList = await SuFcService.GetAllSchedule();
            TeamResultList = await SuFcService.GetAllTeamResult();
        }

        private async Task ChangeScheduleStatus(ScheduleData schedule, ScheduleStatus status)
        {
            schedule.Status = status;
            await SuFcService.SaveSchedule(schedule);
            StateHasChanged();
        }

        private bool TryGetTeamResult(string teamTitle, out TeamResult teamResult)
        {
            var found = TeamResultList.Find(x => x.Title == teamTitle);
            if (found != null)
            {
                teamResult = found;
                return true;
            }
            teamResult = null;
            return false;
        }
    }
}
