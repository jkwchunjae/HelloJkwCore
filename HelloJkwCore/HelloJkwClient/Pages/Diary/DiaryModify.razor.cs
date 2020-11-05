using HelloJkwClient.Shared;
using HelloJkwService.Diary;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Diary
{
    public partial class DiaryModify : JkwPageBase
    {
        [Inject]
        DiaryService DiaryService { get; set; }

        [Parameter]
        public string DiaryName { get; set; }
        [Parameter]
        public string DateStr { get; set; }

        DateTime DiaryDate { get; set; }
        List<DiaryData> DiaryList { get; set; } = new List<DiaryData>();

        protected override async Task OnPageInitializedAsync()
        {
            if (!IsAuthenticated)
                return;

            var diaryInfo = await DiaryService.GetDiaryInfoByDiaryNameAsync(DiaryName);

            if (diaryInfo?.Writers.Contains(User?.Email) ?? false)
            {
                if (DateStr.TryToDate(out var date))
                    DiaryDate = date;
                else
                    DiaryDate = DateTime.MinValue;

                var all = DiaryService.GetDiaryDataListFromCache(DiaryName);
                DiaryList = all.Where(x => x.Date == DiaryDate).ToList();
            }
        }

        async Task UpdateDiary()
        {
            if (!IsAuthenticated)
                return;

            var diaryInfo = await DiaryService.GetDiaryInfoByDiaryNameAsync(DiaryName);

            if (diaryInfo?.Writers.Contains(User?.Email) ?? false)
            {
                var result = await DiaryService.UpdateDiaryAsync(DiaryName, DiaryList);
                if (result.IsSuccess)
                {
                    NavigationManager.NavigateTo($"diary/{DiaryName}/{DiaryDate:yyyyMMdd}");
                }
            }
        }
    }
}
