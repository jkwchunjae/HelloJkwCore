using System.Runtime.Caching;

namespace ProjectDiary;

public class DiaryTemporaryService : IDiaryTemporaryService
{
    class DiaryTempData
    {
        public DateTime Date { get; set; }
        public string Content { get; set; }
    }

    private MemoryCache _cache = new("diary-temporary");

    private string GetKey(AppUser user, DiaryInfo diary)
        => $"{user.Id}::{diary.DiaryName}";

    public Task<(bool Found, DateTime Date, string Content)> GetTemporaryDiary(AppUser user, DiaryInfo diary)
    {
        var key = GetKey(user, diary);
        if (_cache.Contains(key))
        {
            var data = (DiaryTempData)_cache[key];
            if (data != null)
                return Task.FromResult((true, data.Date, data.Content));
        }

        return Task.FromResult((false, DateTime.MinValue, string.Empty));
    }

    public Task SaveTemporaryDiary(AppUser user, DiaryInfo diary, DateTime date, string content)
    {
        var key = GetKey(user, diary);
        var data = new DiaryTempData { Date = date, Content = content };
        _cache[key] = data;

        return Task.CompletedTask;
    }

    public Task RemoveTemporaryDiary(AppUser user, DiaryInfo diary)
    {
        var key = GetKey(user, diary);
        _cache.Remove(key);

        return Task.CompletedTask;
    }
}
