namespace ProjectBob;

public partial class BobService
{
    public async Task<BobCalendarEntry> SaveCalendarEntryAsync(BobCalendarEntry entry, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            entry.MenuIds = entry.MenuIds.Distinct().ToList();
            entry.UpdatedAt = DateTime.Now;

            var index = store.CalendarEntries.FindIndex(x => x.Id == entry.Id);
            if (index >= 0)
            {
                store.CalendarEntries[index] = entry;
            }
            else
            {
                entry.Id = entry.Id == Guid.Empty ? Guid.NewGuid() : entry.Id;
                store.CalendarEntries.Add(entry);
            }

            await SaveStoreUnlockedAsync(store, ct);
            return entry;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task DeleteCalendarEntryAsync(Guid entryId, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            store.CalendarEntries.RemoveAll(entry => entry.Id == entryId);
            await SaveStoreUnlockedAsync(store, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
