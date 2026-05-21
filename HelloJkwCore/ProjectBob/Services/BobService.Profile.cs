namespace ProjectBob;

public partial class BobService
{
    public async Task SaveChildProfileAsync(BobChildProfile profile, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            store.ChildProfile = NormalizeProfile(profile);
            await SaveStoreUnlockedAsync(store, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
