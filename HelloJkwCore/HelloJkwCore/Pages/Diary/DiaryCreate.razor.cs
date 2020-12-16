using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Diary
{
    public partial class DiaryCreate : JkwPageBase
    {
        [Inject]
        IDiaryService DiaryService { get; set; }

        private string DiaryName { get; set; }
        private bool IsSecret { get; set; }

        private async Task CreateDiaryAsync()
        {
            try
            {
                var diary = await DiaryService.CreateDiaryInfoAsync(User, DiaryName, IsSecret);
                Navi.NavigateTo(DiaryUrl.Home(diary.DiaryName));
            }
            catch
            {
            }
        }
    }
}
