namespace ProjectBaduk;

public interface IBadukService
{
    Task<BadukGameData> GetBadukGameData(BadukDiaryName diaryName, string subject);
    Task<BadukDiary> SaveBadukGameData(BadukDiaryName diaryName, BadukGameData badukGameData);
    Task<BadukDiary> DeleteBadukGameData(BadukDiaryName diaryName, string subject);

    Task<BadukDiary> GetBadukDiary(BadukDiaryName diaryName);
    Task<List<BadukDiary>> GetBadukDiaryList(AppUser user);
    Task CreateBadukDiary(AppUser user, BadukDiaryName diaryName);
    Task DeleteBadukDiary(AppUser user, BadukDiaryName diaryName);
}