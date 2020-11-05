﻿using HelloJkwClient.Shared;
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

        async Task WriteDiary()
        {
            if (!IsAuthenticated)
                return;

            var diaryInfo = await DiaryService.GetDiaryInfoByUserIdAsync(User.Id);
            if (diaryInfo?.Writers.Contains(User?.Email) ?? false)
            {
                var result = await DiaryService.WriteDiaryAsync(DiaryName, Date, Content);

                if (result.IsSuccess)
                {
                    NavigationManager.NavigateTo($"diary/{DiaryName}/{Date:yyyyMMdd}");
                }
            }
        }
    }
}
