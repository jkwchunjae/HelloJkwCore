namespace ProjectBob;

public partial class BobService
{
    public async Task<BobDataStore> GetStoreAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            return await LoadStoreUnlockedAsync(ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveStoreAsync(BobDataStore store, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            NormalizeStore(store);
            await SaveStoreUnlockedAsync(store, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<BobDataStore> LoadStoreUnlockedAsync(CancellationToken ct)
    {
        if (!await _fileSystem.FileExistsAsync(path => path.StoreFile(), ct))
        {
            return new BobDataStore();
        }

        var store = await _fileSystem.ReadJsonAsync<BobDataStore>(path => path.StoreFile(), ct);
        NormalizeStore(store);
        return store;
    }

    private async Task SaveStoreUnlockedAsync(BobDataStore store, CancellationToken ct)
    {
        NormalizeStore(store);
        await _fileSystem.WriteJsonAsync(path => path.StoreFile(), store, ct);
    }
}
