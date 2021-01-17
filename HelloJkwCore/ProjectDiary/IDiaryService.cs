using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public interface IDiaryService
    {
        #region DiaryInfo
        Task<DiaryInfo> CreateDiaryInfoAsync(AppUser user, string diaryName, bool isSecret);
        Task<UserDiaryInfo> GetUserDiaryInfoAsync(AppUser user);
        Task<DiaryInfo> GetDiaryInfoAsync(AppUser user, string diaryName);
        Task<List<DiaryInfo>> GetWritableDiaryInfoAsync(AppUser user);
        Task<List<DiaryInfo>> GetViewableDiaryInfoAsync(AppUser user);
        #endregion

        #region GetDiaryView
        Task<DiaryContent> GetDiaryContentAsync(AppUser user, DiaryInfo diary, DiaryFileName diaryFileName);
        Task<DiaryView> GetLastDiaryViewAsync(AppUser user, DiaryInfo diary);
        Task<DiaryView> GetDiaryViewAsync(AppUser user, DiaryInfo diary, DateTime date);
        Task<List<DiaryFileName>> GetDiaryFileAllAsync(AppUser user, DiaryInfo diary);
        #endregion

        #region Write modify update diary
        Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text);
        Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, string password);
        Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents);
        Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents, string password);
        #endregion
    }
}
