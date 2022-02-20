namespace ProjectDiary;

public interface IDiaryTemporaryService
{
    Task SaveTemporaryDiary(AppUser user, DiaryInfo diary, DateTime date, string content);
    Task RemoveTemporaryDiary(AppUser user, DiaryInfo diary);
    Task<(bool Found, DateTime Date, string Content)> GetTemporaryDiary(AppUser user, DiaryInfo diary);
}
