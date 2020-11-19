using HelloJkwClient.Shared;
using HelloJkwService.Diary;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Diary
{
    public partial class DiarySpreadList : JkwPageBase
    {
        [Inject]
        DiaryService DiaryService { get; set; }

        [Parameter]
        public string DiaryName { get; set; }

        List<(int Year, List<(int Month, List<DateTime> DateList)> MonthList)> DateGroup
            = new List<(int Year, List<(int Month, List<DateTime> DateList)> MonthList)>();

        protected override async Task OnPageInitializedAsync()
        {
            if (!IsAuthenticated)
                return;

            var diaryInfo = await DiaryService.GetDiaryInfoByDiaryNameAsync(DiaryName);
            if (diaryInfo?.Writers.Contains(User?.Email) ?? false)
            {
                var diaries = DiaryService.GetDiaryDataListFromCache(DiaryName);
                DateGroup = diaries.Select(x => x.Date)
                    .Distinct()
                    .GroupBy(x => x.Year)
                    .Select(x => 
                    (
                        Year: x.Key,
                        MonthList: x.GroupBy(e => e.Month)
                            .Select(e => (Month: e.Key, DateList: e.ToList()))
                            .OrderByDescending(e => e.Month)
                            .ToList()
                    ))
                    .OrderByDescending(x => x.Year)
                    .ToList();
            }
        }
    }
}
