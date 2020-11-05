using HelloJkwClient.Shared;
using HelloJkwService.Diary;
using HelloJkwService.User;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Diary
{
    public partial class DiaryHome : JkwPageBase
    {
        [Inject]
        DiaryService DiaryService { get; set; }

        [Parameter]
        public string DiaryName { get; set; }
        [Parameter]
        public string DateStr { get; set; }

        public DiaryInfo DiaryInfo { get; set; }
        public List<DiaryData> DiaryList;
        public DiaryData Diary;
        public DateTime DiaryDate => Diary.Date;

        public bool IsMyDiary => (User?.Email ?? "") == (DiaryInfo?.Owner ?? "");
        public bool HasDiary => DiaryList != null && DiaryList.Any();

        protected override async Task OnPageInitializedAsync()
        {
            if (IsAuthenticated)
                return;

            DiaryInfo = await LoadDiaryInfoAsync();
            if (DiaryInfo?.Viewers.Contains(User?.Email) ?? false)
            {
                await LoadDiaryListAsync(DiaryInfo);
            }
        }

        protected override async Task HandleLocationChanged(LocationChangedEventArgs e)
        {
            if (IsAuthenticated)
                return;

            DiaryInfo = await LoadDiaryInfoAsync();
            if (DiaryInfo?.Viewers.Contains(User?.Email) ?? false)
            {
                await LoadDiaryListAsync(DiaryInfo);
            }
        }

        public void GotoDiary(string diaryName, DateTime date)
        {
            NavigationManager.NavigateTo($"diary/{diaryName}/{date.ToString("yyyyMMdd")}");

            Diary = DiaryList.First(x => x.Date == date);
        }

        public void WriteDiary()
        {
            NavigationManager.NavigateTo($"diary/write/{DiaryInfo.DiaryName}");
        }

        public void EditDiary()
        {
            NavigationManager.NavigateTo($"diary/modify/{DiaryInfo.DiaryName}/{Diary.Date.ToString("yyyyMMdd")}");
        }

        public void ShowDiaryList()
        {
            NavigationManager.NavigateTo($"diary/showdates/{DiaryInfo.DiaryName}");
        }

        public void DiarySearch()
        {
            NavigationManager.NavigateTo($"diary/search/{DiaryInfo.DiaryName}");
        }

        private async Task<DiaryInfo> LoadDiaryInfoAsync()
        {
            if (DiaryName == null)
            {
                // Load my diary
                return await DiaryService.GetDiaryInfoByUserIdAsync(User.Id, CancellationToken.None);
            }
            else
            {
                return await DiaryService.GetDiaryInfoByDiaryNameAsync(DiaryName, CancellationToken.None);
            }
        }

        private async Task LoadDiaryListAsync(DiaryInfo diaryInfo)
        {
            if (diaryInfo != null)
            {
                DiaryList = await DiaryService.GetDiaryDataListAsync(diaryInfo, CancellationToken.None);

                if (DiaryList.Any())
                {
                    if (DateStr != null && DateStr.TryToDate(out var date))
                    {
                        // Normalize date
                        date = NormalizeDate(date, DiaryList);

                        // 항상 참
                        Diary = DiaryList.First(x => x.Date == date);
                    }
                    else
                    {
                        // 날짜가 파싱되지 않으면 마지막 일기
                        Diary = DiaryList.Last();
                    }
                }
                else
                {
                    // 일기가 아무것도 없는 경우
                }
            }
        }

        private DateTime NormalizeDate(DateTime date, List<DiaryData> diaryList)
        {
            if (diaryList.Any(x => x.Date >= date))
            {
                return diaryList.First(x => x.Date >= date).Date;
            }
            else if (diaryList.Any(x => x.Date < date))
            {
                return diaryList.Last(x => x.Date < date).Date;
            }
            return DateTime.MinValue;
        }
    }
}
