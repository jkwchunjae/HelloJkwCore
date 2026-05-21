using Microsoft.Extensions.DependencyInjection;

namespace ProjectBob;

public class BobService : IBobService
{
    private readonly IFileSystem _fileSystem;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly BobMenuAvailabilityAnalyzer _availabilityAnalyzer = new();
    private readonly BobWeeklyMealPlanner _weeklyMealPlanner = new();

    public BobService([FromKeyedServices(nameof(BobService))] IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

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

    public async Task<BobMenu> SaveMenuAsync(BobMenu menu, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            NormalizeMenu(menu);

            var index = store.Menus.FindIndex(x => x.Id == menu.Id);
            if (index >= 0)
            {
                menu.CreatedAt = store.Menus[index].CreatedAt;
                menu.UpdatedAt = DateTime.Now;
                store.Menus[index] = menu;
            }
            else
            {
                menu.Id = menu.Id == Guid.Empty ? Guid.NewGuid() : menu.Id;
                menu.CreatedAt = DateTime.Now;
                menu.UpdatedAt = DateTime.Now;
                store.Menus.Add(menu);
            }

            await SaveStoreUnlockedAsync(store, ct);
            return menu;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task DeleteMenuAsync(Guid menuId, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);
            store.Menus.RemoveAll(menu => menu.Id == menuId);
            foreach (var entry in store.CalendarEntries)
            {
                entry.MenuIds.RemoveAll(id => id == menuId);
            }

            await SaveStoreUnlockedAsync(store, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }

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

    public async Task<List<BobMenuAvailability>> FindMenusFromFridgeAsync(CancellationToken ct = default)
    {
        var store = await GetStoreAsync(ct);
        return _availabilityAnalyzer.Analyze(store);
    }

    public async Task<BobWeeklyPlan> CreateWeeklyPlanAsync(DateOnly weekStart, CancellationToken ct = default)
    {
        var store = await GetStoreAsync(ct);
        return _weeklyMealPlanner.CreatePlan(store, weekStart);
    }

    public async Task ConfirmWeeklyPlanAsync(BobWeeklyPlan plan, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var store = await LoadStoreUnlockedAsync(ct);

            foreach (var meal in plan.Meals.Where(x => x.MenuIds.Count > 0))
            {
                var existing = store.CalendarEntries
                    .FirstOrDefault(entry => entry.Date == meal.Date && entry.MealType == meal.MealType);

                if (existing == null)
                {
                    store.CalendarEntries.Add(new BobCalendarEntry
                    {
                        Date = meal.Date,
                        MealType = meal.MealType,
                        MenuIds = meal.MenuIds.Distinct().ToList(),
                        Memo = "주간 추천 확정",
                        UpdatedAt = DateTime.Now,
                    });
                }
                else
                {
                    // 추천 확정은 해당 날짜/끼니의 실제 기록 후보로 덮어쓴다.
                    existing.MenuIds = meal.MenuIds.Distinct().ToList();
                    existing.Memo = string.IsNullOrWhiteSpace(existing.Memo) ? "주간 추천 확정" : existing.Memo;
                    existing.UpdatedAt = DateTime.Now;
                }
            }

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

    private static void NormalizeStore(BobDataStore store)
    {
        store.Menus ??= [];
        store.FridgeItems ??= [];
        store.PurchaseUnits ??= [];
        store.ChildProfile ??= new BobChildProfile();
        store.CalendarEntries ??= [];

        foreach (var menu in store.Menus)
        {
            NormalizeMenu(menu);
        }

        store.FridgeItems = store.FridgeItems.Select(NormalizeFridgeItem).ToList();
        foreach (var purchaseUnit in store.PurchaseUnits)
        {
            NormalizePurchaseUnit(purchaseUnit);
        }

        store.ChildProfile = NormalizeProfile(store.ChildProfile);
    }

    private static void NormalizeMenu(BobMenu menu)
    {
        menu.Id = menu.Id == Guid.Empty ? Guid.NewGuid() : menu.Id;
        menu.Name = menu.Name.Trim();
        menu.Memo = menu.Memo?.Trim() ?? string.Empty;
        menu.Ingredients ??= [];

        foreach (var ingredient in menu.Ingredients)
        {
            ingredient.Id = ingredient.Id == Guid.Empty ? Guid.NewGuid() : ingredient.Id;
            ingredient.Name = ingredient.Name.Trim();
            ingredient.Unit = ingredient.Unit.Trim();
            ingredient.SubstituteNames = ingredient.SubstituteNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        menu.Ingredients = menu.Ingredients
            .Where(ingredient => !string.IsNullOrWhiteSpace(ingredient.Name) && ingredient.Quantity > 0)
            .ToList();
    }

    private static BobFridgeItem NormalizeFridgeItem(BobFridgeItem item)
    {
        item.Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;
        item.Name = item.Name.Trim();
        item.Unit = item.Unit.Trim();
        item.Category = item.Category == BobIngredientCategory.Other
            ? BobIngredientRules.GuessCategory(item.Name)
            : item.Category;
        item.UpdatedAt = DateTime.Now;
        return item;
    }

    private static void NormalizePurchaseUnit(BobPurchaseUnit purchaseUnit)
    {
        purchaseUnit.Id = purchaseUnit.Id == Guid.Empty ? Guid.NewGuid() : purchaseUnit.Id;
        purchaseUnit.IngredientName = purchaseUnit.IngredientName.Trim();
        purchaseUnit.Unit = purchaseUnit.Unit.Trim();
        purchaseUnit.Category = purchaseUnit.Category == BobIngredientCategory.Other
            ? BobIngredientRules.GuessCategory(purchaseUnit.IngredientName)
            : purchaseUnit.Category;
    }

    private static BobChildProfile NormalizeProfile(BobChildProfile profile)
    {
        profile.AllergyIngredients = NormalizeStringList(profile.AllergyIngredients);
        profile.DislikedIngredients = NormalizeStringList(profile.DislikedIngredients);
        profile.BlockedIngredients = NormalizeStringList(profile.BlockedIngredients);
        profile.FavoriteMenuNames = NormalizeStringList(profile.FavoriteMenuNames);
        return profile;
    }

    private static List<string> NormalizeStringList(IEnumerable<string>? values)
    {
        return values?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];
    }
}
