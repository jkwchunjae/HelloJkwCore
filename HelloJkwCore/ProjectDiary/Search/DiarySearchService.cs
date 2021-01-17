using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public class DiarySearchService : IDiarySearchService
    {
        private readonly IDiaryService _diaryService;

        public DiarySearchService(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public void RefreshCache(string diaryName)
        {
            throw new NotImplementedException();
        }

        public void RefreshCacheAll()
        {
            throw new NotImplementedException();
        }

        public Task AppendDiaryText(string diaryName, DiaryFileName fileName, string diaryText)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DiaryFileName>> Search(string diaryName, string keyword)
        {
            return Task.FromResult(
                new List<DiaryFileName>
                {
                    new DiaryFileName("20210111_1.diary"),
                    new DiaryFileName("20210116_1.diary"),
                }.AsEnumerable());
        }
    }
}
