namespace ProjectBob;

public partial class BobService
{
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
}
