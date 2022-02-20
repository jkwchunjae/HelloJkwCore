namespace ProjectDiary;

public interface IDiaryTemporaryService
{
    Task SaveTemproryDiary(AppUser user, DiaryInfo diary, DateTime date, string content);
    Task<(bool Found, DateTime Date, string Content)> GetTemproryDiary(AppUser user, DiaryInfo diary);
}
