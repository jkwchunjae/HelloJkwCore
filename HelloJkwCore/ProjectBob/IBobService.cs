namespace ProjectBob;

public interface IBobService
{
    Task<BobDataStore> GetStoreAsync(CancellationToken ct = default);
    Task SaveStoreAsync(BobDataStore store, CancellationToken ct = default);
    Task<BobMenu> SaveMenuAsync(BobMenu menu, CancellationToken ct = default);
    Task DeleteMenuAsync(Guid menuId, CancellationToken ct = default);
    Task SaveFridgeItemsAsync(List<BobFridgeItem> fridgeItems, CancellationToken ct = default);
    Task<BobPurchaseUnit> SavePurchaseUnitAsync(BobPurchaseUnit purchaseUnit, CancellationToken ct = default);
    Task DeletePurchaseUnitAsync(Guid purchaseUnitId, CancellationToken ct = default);
    Task SaveChildProfileAsync(BobChildProfile profile, CancellationToken ct = default);
    Task<BobCalendarEntry> SaveCalendarEntryAsync(BobCalendarEntry entry, CancellationToken ct = default);
    Task DeleteCalendarEntryAsync(Guid entryId, CancellationToken ct = default);
    Task<List<BobMenuAvailability>> FindMenusFromFridgeAsync(CancellationToken ct = default);
    Task<BobWeeklyPlan> CreateWeeklyPlanAsync(DateOnly weekStart, CancellationToken ct = default);
    Task ConfirmWeeklyPlanAsync(BobWeeklyPlan plan, CancellationToken ct = default);
}
