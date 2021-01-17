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
    public partial class DiarySearch : JkwPageBase
    {
        [Parameter]
        public string DiaryName { get; set; }

        [Inject]
        public IDiaryService DiaryService { get; set; }
        [Inject]
        public IDiarySearchService DiarySearchService { get; set; }

        DiaryInfo DiaryInfo { get; set; }
        List<DiaryContent> DiaryContentList { get; set; } = null;

        DiarySearchData searchData = null;

        protected override async Task OnPageInitializedAsync()
        {
            if (!IsAuthenticated)
            {
                Navi.NavigateTo("/login");
                return;
            }

            DiaryInfo = await DiaryService.GetDiaryInfoAsync(User, DiaryName);
            var list = await DiaryService.GetDiaryFileAllAsync(User, DiaryInfo);

            if (list?.Empty() ?? true)
            {
                searchData = null;
                return;
            }

            searchData = new DiarySearchData();
            searchData.BeginDate = list.First().Date;
            searchData.EndDate = list.Last().Date;
        }

        private async Task Search(DiarySearchData searchData)
        {
            var files = await DiarySearchService.SearchAsync(DiaryInfo.DiaryName, searchData) ?? new List<DiaryFileName>();

            var contents = await files
                .Select(async filename => await DiaryService.GetDiaryContentAsync(User, DiaryInfo, filename))
                .WhenAll();

            DiaryContentList = contents.Where(x => x != null).ToList();
        }

        private async Task OnSubmitAsync()
        {
            await Search(searchData);
        }
    }
}
