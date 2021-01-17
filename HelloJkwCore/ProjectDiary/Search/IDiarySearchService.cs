using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public interface IDiarySearchService
    {
        Task AppendDiaryText(string diaryName, DiaryFileName fileName, string diaryText);
        Task<IEnumerable<DiaryFileName>> Search(string diaryName, string keyword);
        void RefreshCache(string diaryName);
        void RefreshCacheAll();
    }
}
