using Common;
using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Diary
{
    public partial class DiaryMigrate : JkwPageBase
    {
        [Inject]
        private IFileSystemService FsService { get; set; }

        IFileSystem _fsLocal;
        IFileSystem _fsAzure;

        Dictionary<string, List<string>> diaryList = new();
        Dictionary<string, int> copyCount = new();

        List<string> DiaryNameList = new List<string>
        {
            "devbv",
            "hibbah",
            "jkw",
            "lala-land",
            "lucia",
        };

        protected override void OnPageInitialized()
        {
            _fsLocal = FsService.GetFileSystem(FileSystemType.Local);
            _fsAzure = FsService.GetFileSystem(FileSystemType.Azure);

            diaryList = DiaryNameList
                .ToDictionary(x => x, x => new List<string>());

            copyCount = DiaryNameList
                .ToDictionary(x => x, x => 0);
        }

        public async Task CheckDiary(string diaryName)
        {
            var list = await _fsLocal.GetFilesAsync(path => path.Diary(diaryName), ".diary");

            diaryList[diaryName] = list;
        }

        public async Task Copy(string diaryName)
        {
            var list = await _fsLocal.GetFilesAsync(path => path.Diary(diaryName), ".diary");
            diaryList[diaryName] = list;

            var count = 0;
            foreach (var fileName in list)
            {
                var diary = await _fsLocal.ReadJsonAsync<DiaryContent>(path => path.Content(diaryName, fileName));

                await _fsAzure.WriteJsonAsync(path => path.Content(diaryName, fileName), diary);
                count++;
                copyCount[diaryName] = count;

                StateHasChanged();
            }
        }
    }
}
