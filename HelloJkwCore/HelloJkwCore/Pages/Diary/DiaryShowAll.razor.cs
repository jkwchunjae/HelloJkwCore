using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Diary
{
    public partial class DiaryShowAll : JkwPageBase
    {
        [Parameter]
        public string DiaryName { get; set; }

        [Inject]
        public IDiaryService DiaryService { get; set; }

        DiaryInfo DiaryInfo { get; set; }
        List<DateTime> DateList { get; set; } = new();

        protected override async Task OnPageInitializedAsync()
        {
            if (!IsAuthenticated)
            {
                Navi.NavigateTo("/login");
                return;
            }

            DiaryInfo = await DiaryService.GetDiaryInfoAsync(User, DiaryName);

            if (DiaryInfo != null)
            {
                var diaryFileAll = await DiaryService.GetDiaryFileAllAsync(User, DiaryInfo);

                DateList = diaryFileAll.Select(x => x.Date).Distinct().ToList();
            }
        }
    }
}
