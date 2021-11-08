using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcMain : JkwPageBase
    {
        [Inject]
        private ISuFcService SuFcService { get; set; }

        private List<ScheduleData> ActiveSchedules = new();

        protected override async Task OnPageInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            ActiveSchedules = await GetActiveSchedulesAsync();
        }

        private async Task<List<ScheduleData>> GetActiveSchedulesAsync()
        {
            var schedules = await SuFcService.GetAllSchedule();
            var activeList = schedules.Where(x => x.Status == ScheduleStatus.Active).ToList();

            return activeList;
        }

        private async Task OnScheduleChanged(ScheduleData s)
        {
            await LoadAsync();
            StateHasChanged();
        }
    }
}
