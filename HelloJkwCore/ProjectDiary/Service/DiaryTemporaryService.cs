using System.Runtime.Caching;

namespace ProjectDiary;

public class DiaryTemporaryService : IDiaryTemporaryService
{
    class DiaryTempData
    {
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }

    private MemoryCache _cache = new("diary-temporary");

    private string GetKey(AppUser user, DiaryInfo diary)
        => $"{user.Id}::{diary.DiaryName}";

    public Task<(bool Found, DateTime date, string text)> GetTemproryDiary(AppUser user, DiaryInfo diary)
    {
        var key = GetKey(user, diary);
        if (_cache.Contains(key))
        {
            var data = (DiaryTempData)_cache[key];
            if (data != null)
                return Task.FromResult((true, data.Date, data.Text));
        }

        return Task.FromResult((false, DateTime.MinValue, string.Empty));
    }

    public Task SaveTemproryDiary(AppUser user, DiaryInfo diary, DateTime date, string text)
    {
        var key = GetKey(user, diary);
        var data = new DiaryTempData { Date = date, Text = text };
        _cache[key] = data;
        return Task.CompletedTask;
    }
}
