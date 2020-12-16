using Common.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public interface IDiaryService
    {
        #region DiaryInfo
        Task<DiaryInfo> CreateDiaryInfoAsync(AppUser user, string diaryName, bool isSecret);
        Task<DiaryInfo> GetUserDiaryInfoAsync(AppUser user);
        Task<DiaryInfo> GetDiaryInfoAsync(AppUser user, string diaryName);
        Task<List<DiaryInfo>> GetWritableDiaryInfoAsync(AppUser user);
        Task<List<DiaryInfo>> GetViewableDiaryInfoAsync(AppUser user);
        #endregion

        #region GetDiaryView
        Task<DiaryView> GetLastDiaryViewAsync(AppUser user, DiaryInfo diary);
        Task<DiaryView> GetDiaryViewAsync(AppUser user, DiaryInfo diary, DateTime date);
        #endregion

        #region Write modify update diary
        Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text);
        Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, string password);
        Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents);
        Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents, string password);
        #endregion
    }
}
