using HelloJkwCore.Shared;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;

namespace HelloJkwCore.Pages.Diary
{
    public partial class DiaryHome : JkwPageBase
    {
        [Inject]
        private IDiaryService DiaryService { get; set; }
        [Inject]
        private UserInstantData UserData { get; set; }
        [Inject]
        private ILogger<DiaryHome> Logger { get; set; }

        [Parameter]
        public string DiaryName { get; set; }
        [Parameter]
        public string Date { get; set; }

        private bool Loading = true;

        private DiaryInfo DiaryInfo { get; set; }
        //private List<DiaryInfo> WritableDiary { get; set; }
        //private List<DiaryInfo> ViewableDiary { get; set; }

        private DiaryView View { get; set; }

        private bool HasDiaryInfo => DiaryInfo != null;
        private bool HasDiaryContent => HasDiaryInfo && (View?.DiaryContents?.Any() ?? false);

        public DiaryHome()
        {
        }

        protected override async Task OnPageInitializedAsync()
        {
            if (!IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            Loading = true;
            await InitDiary();
            Loading = false;

            StateHasChanged();
        }

        protected override async Task HandleLocationChanged(LocationChangedEventArgs e)
        {
            Loading = true;
            await InitDiary();
            Loading = false;

            StateHasChanged();
        }

        private async Task InitDiary()
        {
            Logger?.LogDebug("InitDiary");

            DiaryInfo = null;
            View = null;

            if (!IsAuthenticated)
            {
                Logger?.LogDebug("Not Authenticated");
                return;
            }

            if (!string.IsNullOrWhiteSpace(DiaryName))
            {
                DiaryInfo = await DiaryService.GetDiaryInfoAsync(User, DiaryName);
            }
            else
            {
                var userDiaryInfo = await DiaryService.GetUserDiaryInfoAsync(User);
                if (userDiaryInfo?.MyDiaryList?.Empty() ?? true)
                    return;

                var diaryName = userDiaryInfo.MyDiaryList.First();
                DiaryInfo = await DiaryService.GetDiaryInfoAsync(User, diaryName);
            }

            if (DiaryInfo == null)
            {
                Logger?.LogDebug("DiaryInfo is null");
                return;
            }

            Logger?.LogDebug("Set DiaryInfo: {diaryInfo}", Json.Serialize(DiaryInfo));

            if (DiaryInfo.IsSecret && string.IsNullOrWhiteSpace(UserData.Password))
            {
                Navi.NavigateTo(DiaryUrl.SetPassword());
                return;
            }
            //WritableDiary = await DiaryService.GetWritableDiaryInfoAsync(User);
            //ViewableDiary = await DiaryService.GetViewableDiaryInfoAsync(User);

            if (HasDiaryInfo)
            {
                if (string.IsNullOrWhiteSpace(Date))
                {
                    View = await DiaryService.GetLastDiaryViewAsync(User, DiaryInfo);
                }
                else if (Date.TryToDate(out var parsedDate))
                {
                    View = await DiaryService.GetDiaryViewAsync(User, DiaryInfo, parsedDate);
                }

                if (DiaryInfo.IsSecret && HasDiaryContent)
                {
                    foreach (var content in View?.DiaryContents)
                    {
                        try
                        {
                            content.Text = content.Text.Decrypt(UserData.Password);
                        }
                        catch
                        {
                            content.Text = "임호화된 일기를 해석하지 못했습니다. 비밀번호를 다시 확인해주세요.";
                        }
                    }
                }
            }
        }

        private bool IsMyText(DiaryView view)
        {
            if (view.DiaryInfo.Owner == User.Email)
                return true;

            if (view.DiaryInfo.Writers?.Contains(User.Email) ?? false)
                return true;

            return false;
        }
    }
}
