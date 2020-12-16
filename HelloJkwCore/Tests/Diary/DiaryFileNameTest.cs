using Common;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Diary
{
    public class DiaryFileNameTest
    {
        [Fact]
        public void Constructor_DiaryFileName()
        {
            var fileName = "20200101_2.diary";
            var diaryFileName = new DiaryFileName(fileName);
            var diaryFileName2 = new DiaryFileName(fileName);

            Assert.Equal(fileName, diaryFileName.FileName);
            Assert.Equal(new DateTime(2020, 1, 1), diaryFileName.Date);
            Assert.Equal(2, diaryFileName.Index);
            Assert.True(diaryFileName.Equals(diaryFileName2));
        }

        [Fact]
        public void Sort_DiaryFileName()
        {
            var arr = new[]
            {
                "20201001_2.diary",
                "20201002_1.diary",
                "20200801_1.diary",
                "20201001_1.diary",
            };

            var fileNames = arr
                .Select(x => new DiaryFileName(x))
                .OrderBy(x => x)
                .ToList();

            var sorted = arr.OrderBy(x => x).ToList();

            Assert.Equal(sorted, fileNames.Select(x => x.FileName).ToList());
        }
    }
}
