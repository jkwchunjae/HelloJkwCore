namespace ProjectBob;

public partial class BobService
{
    public async Task SaveFridgeItemsAsync(List<BobFridgeItem> fridgeItems, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            store.FridgeItems = fridgeItems
                .Where(item => !string.IsNullOrWhiteSpace(item.Name) && item.Quantity > 0)
                .Select(NormalizeFridgeItem)
                .ToList();

            await SaveStoreUnlockedAsync(store, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<BobPurchaseUnit> SavePurchaseUnitAsync(BobPurchaseUnit purchaseUnit, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            NormalizePurchaseUnit(purchaseUnit);
            var index = store.PurchaseUnits.FindIndex(x => x.Id == purchaseUnit.Id);

            if (index >= 0)
            {
                purchaseUnit.UpdatedAt = DateTime.Now;
                store.PurchaseUnits[index] = purchaseUnit;
            }
            else
            {
                purchaseUnit.Id = purchaseUnit.Id == Guid.Empty ? Guid.NewGuid() : purchaseUnit.Id;
                purchaseUnit.UpdatedAt = DateTime.Now;
                store.PurchaseUnits.Add(purchaseUnit);
            }

            await SaveStoreUnlockedAsync(store, ct);
            return purchaseUnit;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task DeletePurchaseUnitAsync(Guid purchaseUnitId, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            store.PurchaseUnits.RemoveAll(unit => unit.Id == purchaseUnitId);
            await SaveStoreUnlockedAsync(store, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
