using HelloJkwClient.Shared;
using HelloJkwService.Diary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Diary
{
    public partial class DiaryWrite : JkwPageBase
    {
        [Inject]
        DiaryService DiaryService { get; set; }

        [Parameter]
        public string DiaryName { get; set; }

        DateTime Date { get; set; } = DateTime.Today;
        string Content { get; set; }

        DiaryInfo DiaryInfo;
        string Password = string.Empty;

        protected override async Task OnPageInitializedAsync()
        {
            DiaryInfo = await DiaryService.GetDiaryInfoByUserIdAsync(User.Id);

            if (DiaryService.TryGetPassword(User.Id, out var pw))
            {
                Password = pw;
                StateHasChanged();
            }
        }

        async Task WriteDiary()
        {
            if (!IsAuthenticated)
                return;

            if (DiaryInfo?.Writers.Contains(User?.Email) ?? false)
            {
                var result = await DiaryService.WriteDiaryAsync(DiaryName, Date, Content, DiaryInfo.IsSecure, Password);

                if (result.IsSuccess)
                {
                    NavigationManager.GotoDiary(DiaryName, Date);
                }
            }
        }
    }
}
