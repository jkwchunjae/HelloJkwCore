using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Diary
{
    public static class DiaryNavigationExtension
    {
        public static void GotoHome(this NavigationManager navigationManager, string diaryName = default, bool forceLoad = default)
        {
            if (diaryName == default)
            {
                navigationManager.NavigateTo($"diary", forceLoad);
            }
            else
            {
                navigationManager.NavigateTo($"diary/{diaryName}", forceLoad);
            }
        }

        public static void GotoDiary(this NavigationManager navigationManager, string diaryName, DateTime date, bool forceLoad = default)
        {
            navigationManager.NavigateTo($"diary/{diaryName}/{date:yyyyMMdd}");
        }

        public static void GotoWriteDiary(this NavigationManager navigationManager, string diaryName, bool forceLoad = default)
        {
            navigationManager.NavigateTo($"diary/write/{diaryName}", forceLoad);
        }

        public static void GotoEditDiary(this NavigationManager navigationManager, string diaryName, DateTime date, bool forceLoad = default)
        {
            navigationManager.NavigateTo($"diary/modify/{diaryName}/{date:yyyyMMdd}", forceLoad);
        }

        public static void GotoShowDiaryList(this NavigationManager navigationManager, string diaryName, bool forceLoad = default)
        {
            navigationManager.NavigateTo($"diary/showdates/{diaryName}", forceLoad);
        }

        public static void GotoSearchDiary(this NavigationManager navigationManager, string diaryName, bool forceLoad = default)
        {
            navigationManager.NavigateTo($"diary/search/{diaryName}", forceLoad);
        }
    }
}
