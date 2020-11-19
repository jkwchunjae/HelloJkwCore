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
        public List<DiaryData> DiaryList { get; set; } = new List<DiaryData>();
        public List<DiaryData> CurrentDiary { get; set; } = new List<DiaryData>();
        public DateTime DiaryDate => CurrentDiary.First().Date;

        public bool IsMyDiary => (User?.Email ?? "") == (DiaryInfo?.Owner ?? "");
        public bool HasDiary => DiaryList != null && DiaryList.Any();

        public string Password = string.Empty;

        protected override async Task OnPageInitializedAsync()
        {
            if (!IsAuthenticated)
                return;

            if (DiaryService.TryGetPassword(User.Id, out var pw))
            {
                Password = pw;
                StateHasChanged();
            }

            DiaryInfo = await LoadDiaryInfoAsync();
            if (DiaryInfo?.Viewers.Contains(User?.Email) ?? false)
            {
                await LoadDiaryListAsync(DiaryInfo);
            }
        }

        protected override async Task HandleLocationChanged(LocationChangedEventArgs e)
        {
            if (!IsAuthenticated)
                return;

            DiaryInfo = await LoadDiaryInfoAsync();
            if (DiaryInfo?.Viewers.Contains(User?.Email) ?? false)
            {
                await LoadDiaryListAsync(DiaryInfo);
            }
        }

        public void GotoDiary(string diaryName, DateTime date)
        {
            NavigationManager.GotoDiary(diaryName, date);

            CurrentDiary = DiaryList.WhereAndDecrypt(date, Password).ToList();
        }

        public void WriteDiary()
        {
            NavigationManager.GotoWriteDiary(DiaryInfo.DiaryName);
        }

        public void EditDiary()
        {
            NavigationManager.GotoEditDiary(DiaryInfo.DiaryName, DiaryDate);
        }

        public void ShowDiaryList()
        {
            NavigationManager.GotoShowDiaryList(DiaryInfo.DiaryName);
        }

        public void DiarySearch()
        {
            NavigationManager.GotoSearchDiary(DiaryInfo.DiaryName);
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
                        CurrentDiary = DiaryList.WhereAndDecrypt(date, Password).ToList();
                    }
                    else
                    {
                        // 날짜가 파싱되지 않으면 마지막 일기
                        CurrentDiary = DiaryList.WhereAndDecrypt(DiaryList.Last().Date, Password).ToList();
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

        private void SetPassword()
        {
            if (Password != string.Empty)
            {
                DiaryService.CachePassword(User.Id, Password);
                CurrentDiary = DiaryList.WhereAndDecrypt(DiaryList.Last().Date, Password).ToList();
                StateHasChanged();
            }
        }

        private void ResetPassword()
        {
            Password = string.Empty;
        }
    }
}
