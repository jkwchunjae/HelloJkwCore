namespace ProjectBob;

public partial class BobService
{
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
}
