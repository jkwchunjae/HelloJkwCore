using Common;
using HelloJkwService.Diary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HelloJkwTests
{
    public class DiaryService_Test
    {
        private readonly DiaryOption _option;
        private readonly DiaryService _service;

        public DiaryService_Test()
        {
            _option = GetDiaryOption();
            _service = new DiaryService(_option);
        }

        DiaryOption GetDiaryOption()
        {
            var configuration = UnitTest_Base.GetIConfiguration();

            var diaryOption = new DiaryOption
            {
                RootPath = configuration.GetPath(PathOf.DiaryRootPath),
                DiaryListPath = configuration.GetPath(PathOf.DiaryListFile),
            };

            return diaryOption;
        }

        [Fact]
        public async Task GetDiaryInfo_jkw()
        {
            // Arrange

            // Action
            var diaryInfo = await _service.GetDiaryInfoByDiaryNameAsync("jkw", CancellationToken.None);

            // Assert
            Assert.Equal("jkw", diaryInfo.DiaryName);
        }

        [Fact]
        public async Task GetDiaryDataList_jkw()
        {
            // Arrange

            // Action
            var diaryDataList = await _service.GetDiaryDataListAsync("jkw", CancellationToken.None);

            // Assert
            Assert.True(diaryDataList.Any());
        }
    }
}
