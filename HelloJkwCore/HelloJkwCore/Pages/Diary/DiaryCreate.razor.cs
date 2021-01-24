using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Diary
{
    public partial class DiaryCreate : JkwPageBase
    {
        [Inject]
        IDiaryService DiaryService { get; set; }

        DiaryCreateModel _createModel = new DiaryCreateModel();

        private async Task CreateDiaryAsync(DiaryCreateModel model)
        {
            try
            {
                var diary = await DiaryService.CreateDiaryInfoAsync(User, model.DiaryName, model.IsSecret);
                Navi.NavigateTo(DiaryUrl.Home(diary.DiaryName));
            }
            catch
            {
            }
        }
    }

    public class DiaryCreateModel
    {
        public string DiaryName { get; set; }
        public bool IsSecret { get; set; }
    }
}
