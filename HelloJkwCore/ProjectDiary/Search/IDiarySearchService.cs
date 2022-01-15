namespace ProjectDiary;

public interface IDiarySearchService
{
    Task AppendDiaryTextAsync(string diaryName, DiaryFileName fileName, string diaryText);
    Task<IEnumerable<DiaryFileName>> SearchAsync(string diaryName, DiarySearchData searchData);
    Task ClearTrie(string diaryName);
    Task<bool> SaveDiaryTrie(string diaryName);
    void RefreshCache(string diaryName);
    void RefreshCacheAll();
}