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

        protected override async Task OnPageInitializedAsync()
        {
            ScheduleList = await SuFcService.GetAllSchedule();
            ScheduleList = ReorderingSchedlue(ScheduleList);
            TeamResultList = await SuFcService.GetAllTeamResult();
        }

        private List<ScheduleData> ReorderingSchedlue(List<ScheduleData> schedules)
        {
            schedules = schedules.OrderBy(x => x.Date).ToList();
            var active = schedules.Where(x => x.Status == ScheduleStatus.Active).ToList();
            var feature = schedules.Where(x => x.Status == ScheduleStatus.Feature).ToList();
            var done = schedules.Where(x => x.Status == ScheduleStatus.Done).ToList();

            schedules = active.Concat(feature).Concat(done).ToList();
            return schedules;
        }

        private async Task ChangeScheduleStatus(ScheduleData schedule, ScheduleStatus status)
        {
            schedule.Status = status;
            ScheduleList = ReorderingSchedlue(ScheduleList);
            await SuFcService.SaveSchedule(schedule);
            StateHasChanged();
        }

        public void UpdateSchedule(ScheduleData schedule)
        {
            Navi.NavigateTo($"/sufc/schedule/update/{schedule.Id}");
        }
    }
}
