namespace ProjectDiary;

public interface IDiarySearchService
{
    Task AppendDiaryTextAsync(DiaryName diaryName, DiaryFileName fileName, string diaryText);
    Task<IEnumerable<DiaryFileName>> SearchAsync(DiaryName diaryName, DiarySearchData searchData);
    Task ClearTrie(DiaryName diaryName);
    Task<bool> SaveDiaryTrie(DiaryName diaryName);
    void RefreshCache(DiaryName diaryName);
    void RefreshCacheAll();
}