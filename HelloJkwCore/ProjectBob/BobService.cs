using Microsoft.Extensions.DependencyInjection;

namespace ProjectBob;

public class BobService : IBobService
{
    private readonly IFileSystem _fileSystem;

    public BobService([FromKeyedServices(nameof(BobService))] IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<BobDataStore> GetStoreAsync(CancellationToken ct = default)
    {
        if (!await _fileSystem.FileExistsAsync(path => path.StoreFile(), ct))
        {
            return new BobDataStore();
        }

        return await _fileSystem.ReadJsonAsync<BobDataStore>(path => path.StoreFile(), ct);
    }

    public async Task SaveStoreAsync(BobDataStore store, CancellationToken ct = default)
    {
        await _fileSystem.WriteJsonAsync(path => path.StoreFile(), store, ct);
    }
}
