namespace ProjectDiary;

public interface IDiarySearchService
{
    Task AppendDiaryTextAsync(DiaryName diaryName, DiaryFileName fileName, string diaryText);
    Task<IEnumerable<DiaryFileName>> SearchAsync(DiaryName diaryName, DiarySearchData searchData);
    Task ClearTrie(DiaryName diaryName);
    Task ClearTrieYear(DiaryName diaryName, int year);
    Task<bool> SaveDiaryTrie(DiaryName diaryName, int year);
    void RefreshCache(DiaryName diaryName);
    void RefreshCacheAll();
}