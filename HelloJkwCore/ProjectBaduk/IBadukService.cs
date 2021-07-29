using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    public interface IBadukService
    {
        Task<List<BadukGameData>> GetBadukSummaryList(DiaryName diaryName);
        Task<BadukGameData> GetBadukGameData(DiaryName diaryName, string subject);
        Task SaveBadukGameData(DiaryName diaryName, BadukGameData badukGameData);
        Task DeleteBadukGameData(DiaryName diaryName, string subject);

        Task<List<BadukDiary>> GetBadukDiaryList(AppUser user);
        Task CreateBadukDiary(AppUser user, DiaryName diaryName);
        Task DeleteBadukDiary(AppUser user, DiaryName diaryName);
    }
}
