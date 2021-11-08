using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
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

        bool success;

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

        private IEnumerable<string> DiaryNameValidator(string diaryName)
        {
            if (string.IsNullOrWhiteSpace(diaryName))
                yield return "일기장 이름을 입력해주세요.";

            diaryName = diaryName.Trim();
            var minLength = 3;
            var maxLength = 30;
            if (diaryName.Length < minLength || diaryName.Length > maxLength)
                yield return "일기장은 3 - 30자 사이를 입력해주세요.";

            var pattern = @"^[a-z]+$";
            if (!Regex.IsMatch(diaryName, pattern))
                yield return "일기장은 알파뱃 소문자만 입력해주세요.";
        }
    }

    public class DiaryCreateModel
    {
        public string DiaryName { get; set; }
        public bool IsSecret { get; set; }
    }
}
