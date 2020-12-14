using Common.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public interface IDiaryService
    {
        #region DiaryInfo
        Task<DiaryInfo> CreateDiaryAsync(AppUser user, string diaryName, bool isSecret);
        Task<DiaryInfo> GetUserDiaryAsync(AppUser user);
        Task<List<DiaryInfo>> GetWritableDiaryAsync(AppUser user);
        Task<List<DiaryInfo>> GetViewableDiaryAsync(AppUser user);
        #endregion

        #region GetDiaryView
        Task<DiaryView> GetLastDiaryAsync(AppUser user, DiaryInfo diary);
        Task<DiaryView> GetDiaryAsync(AppUser user, DiaryInfo diary, DateTime date);
        #endregion

        #region Write modify update diary
        Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text);
        Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, string password);
        Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents);
        #endregion
    }
}
