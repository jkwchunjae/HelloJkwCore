namespace ProjectDiary;

public interface IDiaryTemporaryService
{
    Task SaveTemproryDiary(AppUser user, DiaryInfo diary, DateTime date, string text);
    Task<(bool Found, DateTime date, string text)> GetTemproryDiary(AppUser user, DiaryInfo diary);
}
