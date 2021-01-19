using Common;
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
    public partial class DiaryAdminTool : JkwPageBase
    {
        [Inject]
        IDiaryService DiaryService { get; set; }
        [Inject]
        IDiarySearchService DiarySearchService { get; set; }

        IEnumerable<DiaryData> DiaryDataList { get; set; }

        Dictionary<string, (bool ProgressOn, int ProgressTotal, int ProgressValue)> ProgressDic = new();

        protected override async Task OnPageInitializedAsync()
        {
            if (IsAuthenticated && User.HasRole(UserRole.Admin))
            {
                var list = await DiaryService.GetAllDiaryListAsync(User);

                DiaryDataList = await list
                    .Select(async x => await CreateDiaryData(x))
                    .WhenAll();
            }
        }

        private async Task<DiaryData> CreateDiaryData(DiaryInfo diaryInfo)
        {
            var diaryData = new DiaryData(diaryInfo);

            var files = await DiaryService.GetDiaryFileAllAsync(User, diaryInfo);

            diaryData.DiaryFileList = files;

            return diaryData;
        }

        async Task CreateTrie(DiaryData diaryData)
        {
            if (diaryData.IsSecret)
            {
                return;
            }

            diaryData.Progress = (true, diaryData.DiaryFileList.Count, 0);
            StateHasChanged();

            await DiarySearchService.ClearTrie(diaryData.DiaryName);

            var progressValue = 0;
            foreach (var fileName in diaryData.DiaryFileList)
            {
                progressValue++;
                var content = await DiaryService.GetDiaryContentAsync(User, diaryData, fileName);
                await DiarySearchService.AppendDiaryTextAsync(diaryData.DiaryName, fileName, content.Text);

                if (diaryData.Progress.Total > 100)
                {
                    if (progressValue % 10 == 0)
                    {
                        diaryData.Progress = (true, diaryData.DiaryFileList.Count, progressValue);
                        StateHasChanged();
                    }
                }
                else
                {
                    diaryData.Progress = (true, diaryData.DiaryFileList.Count, progressValue);
                    StateHasChanged();
                }
            }
            diaryData.Progress = (true, diaryData.DiaryFileList.Count, diaryData.DiaryFileList.Count);
            StateHasChanged();

            await DiarySearchService.SaveDiaryTrie(diaryData.DiaryName);
        }
    }

    class DiaryData : DiaryInfo
    {
        public List<DiaryFileName> DiaryFileList { get; set; }
        public (bool On, int Total, int Value) Progress { get; set; }

        public DiaryData(DiaryInfo info)
            :base(info)
        {
            Progress = (false, 0, 0);
        }
    }
}
