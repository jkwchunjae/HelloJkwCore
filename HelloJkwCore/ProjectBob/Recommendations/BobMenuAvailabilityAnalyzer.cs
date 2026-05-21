namespace ProjectBob;

public class BobMenuAvailabilityAnalyzer
{
    public List<BobMenuAvailability> Analyze(BobDataStore store, IEnumerable<BobFridgeItem>? fridgeOverride = null)
    {
        var fridgeItems = fridgeOverride?.ToList() ?? store.FridgeItems;
        var fridgeBook = BobIngredientAmountBook.FromFridge(fridgeItems);
        var ingredientReuseMap = BuildIngredientReuseMap(store.Menus);

        return store.Menus
            .Where(menu => menu.HasName)
            .Select(menu => AnalyzeMenu(menu, store, fridgeBook, ingredientReuseMap))
            .OrderBy(x => x.HasRestrictedIngredient)
            .ThenBy(x => x.MissingIngredients.Count)
            .ThenByDescending(x => x.ReusableIngredientScore)
            .ThenByDescending(x => x.Menu.TasteLevel)
            .ThenBy(x => x.Menu.Name)
            .ToList();
    }

    private static BobMenuAvailability AnalyzeMenu(
        BobMenu menu,
        BobDataStore store,
        BobIngredientAmountBook fridgeBook,
        IReadOnlyDictionary<BobIngredientKey, int> ingredientReuseMap)
    {
        var missing = new List<BobMissingIngredient>();

        foreach (var ingredient in menu.Ingredients.Where(x => x.IsRequired && !x.IsBasic))
        {
            var available = fridgeBook.GetQuantity(ingredient.Name, ingredient.Unit);
            if (available >= ingredient.Quantity)
                continue;

            var substitute = ingredient.SubstituteNames
                .Where(name => !IsRestricted(name, store.ChildProfile))
                .Select(name => new
                {
                    Name = name,
                    Quantity = fridgeBook.GetQuantity(name, ingredient.Unit),
                })
                .FirstOrDefault(x => x.Quantity >= ingredient.Quantity);

            if (substitute != null)
                continue;

            missing.Add(new BobMissingIngredient
            {
                Name = ingredient.Name,
                RequiredQuantity = ingredient.Quantity,
                AvailableQuantity = available,
                Unit = ingredient.Unit,
                SubstituteName = substitute?.Name,
            });
        }

        return new BobMenuAvailability
        {
            Menu = menu,
            MissingIngredients = missing,
            HasRestrictedIngredient = HasRestrictedIngredient(menu, store.ChildProfile),
            HasDislikedIngredient = HasDislikedIngredient(menu, store.ChildProfile),
            ReusableIngredientScore = GetReusableIngredientScore(menu, ingredientReuseMap),
            LastServedDate = GetLastServedDate(menu.Id, store.CalendarEntries),
        };
    }

    private static Dictionary<BobIngredientKey, int> BuildIngredientReuseMap(IEnumerable<BobMenu> menus)
    {
        return menus
            .SelectMany(menu => menu.Ingredients.Where(x => !x.IsBasic))
            .GroupBy(ingredient => BobIngredientKey.From(ingredient.Name, ingredient.Unit))
            .ToDictionary(group => group.Key, group => group.Select(x => x.Name).Count());
    }

    private static int GetReusableIngredientScore(BobMenu menu, IReadOnlyDictionary<BobIngredientKey, int> ingredientReuseMap)
    {
        return menu.Ingredients
            .Where(x => !x.IsBasic)
            .Sum(ingredient =>
            {
                var key = BobIngredientKey.From(ingredient.Name, ingredient.Unit);
                return ingredientReuseMap.TryGetValue(key, out var count) ? count : 0;
            });
    }

    private static bool HasRestrictedIngredient(BobMenu menu, BobChildProfile profile)
    {
        return menu.Ingredients.Any(ingredient => IsRestricted(ingredient.Name, profile));
    }

    private static bool HasDislikedIngredient(BobMenu menu, BobChildProfile profile)
    {
        return menu.Ingredients.Any(ingredient =>
            profile.DislikedIngredients.Any(disliked => BobIngredientRules.IsSameIngredient(disliked, ingredient.Name)));
    }

    private static bool IsRestricted(string ingredientName, BobChildProfile profile)
    {
        return profile.AllergyIngredients.Any(allergy => BobIngredientRules.IsSameIngredient(allergy, ingredientName))
            || profile.BlockedIngredients.Any(blocked => BobIngredientRules.IsSameIngredient(blocked, ingredientName));
    }

    private static DateOnly? GetLastServedDate(Guid menuId, IEnumerable<BobCalendarEntry> entries)
    {
        var dates = entries
            .Where(entry => entry.MenuIds.Contains(menuId))
            .Select(entry => entry.Date)
            .ToList();

        return dates.Count == 0 ? null : dates.Max();
    }
}
