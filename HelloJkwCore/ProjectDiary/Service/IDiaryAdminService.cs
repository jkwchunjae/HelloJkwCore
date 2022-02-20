namespace ProjectDiary;

public interface IDiaryAdminService
{
    Task<List<DiaryInfo>> GetAllDiaryListAsync(AppUser user);
}
