namespace ProjectBob;

public interface IBobService
{
    Task<BobDataStore> GetStoreAsync(CancellationToken ct = default);
    Task SaveStoreAsync(BobDataStore store, CancellationToken ct = default);
}
