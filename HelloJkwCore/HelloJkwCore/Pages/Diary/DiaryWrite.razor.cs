using HelloJkwCore.Shared;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Diary
{
    public partial class DiaryWrite : JkwPageBase
    {
        [Inject]
        private IDiaryService DiaryService { get; set; }
        [Inject]
        private UserInstantData UserData { get; set; }

        [Parameter]
        public string DiaryName { get; set; }
        [Parameter]
        public string DiaryDate { get; set; }

        private DiaryInfo DiaryInfo { get; set; }
        private DateTime Date { get; set; }
        private string Content { get; set; }

        protected override async Task OnPageInitializedAsync()
        {
            if (!IsAuthenticated)
            {
                Navi.NavigateTo("/login");
                return;
            }

            var list = await DiaryService.GetWritableDiaryInfoAsync(User);
            DiaryInfo = list.FirstOrDefault(x => x.DiaryName == DiaryName);

            if (DiaryInfo == null)
            {
                Navi.NavigateTo(DiaryUrl.Home(DiaryName));
            }

            if (DiaryInfo.IsSecret && string.IsNullOrWhiteSpace(UserData.Password))
            {
                Navi.NavigateTo(DiaryUrl.SetPassword());
                return;
            }

            Date = DateTime.Today;

            if (!string.IsNullOrWhiteSpace(DiaryDate))
            {
                if (DiaryDate.TryToDate(out var parsedDate))
                {
                    Date = parsedDate;
                }
            }
        }

        async Task WriteDiaryAsync()
        {
            if (!IsAuthenticated)
                return;

            if (DiaryInfo == null)
                return;

            DiaryContent content;
            if (DiaryInfo.IsSecret)
            {
                content = await DiaryService.WriteDiaryAsync(User, DiaryInfo, Date, Content, UserData.Password);
            }
            else
            {
                content = await DiaryService.WriteDiaryAsync(User, DiaryInfo, Date, Content);
            }

            if (content != null)
            {
                Navi.NavigateTo(DiaryUrl.DiaryContent(DiaryInfo.DiaryName, Date));
            }
        }
    }
}
